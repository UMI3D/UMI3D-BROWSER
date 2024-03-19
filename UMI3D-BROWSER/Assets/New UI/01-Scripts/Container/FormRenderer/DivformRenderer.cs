/*
Copyright 2019 - 2024 Inetum

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using umi3d.cdk;
using umi3d.common.interaction.form;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace umi3dBrowsers.container.formrenderer
{
    public class DivformRenderer : MonoBehaviour
    {
        private GameObject _contentRoot;
        private FormAnswerDto _answer;
        public event Action<FormAnswerDto> OnFormAnswer;
        List<GameObject> allContainers = new();

        [Header("UI Containers")]
        [SerializeField] private GameObject vignetteContainerPrefab;
        [SerializeField] private GameObject groupContainerPrefab;

        [Space]
        [SerializeField] private TabManager tabManager;

        [Header("UI Displayers")]
        [SerializeField] private GameObject labelDisplayerPrefab;
        [SerializeField] private GameObject buttonDisplayerPrefab;
        [SerializeField] private GameObject imageDisplayerPrefab;

        FormDto _form;

        public void Init(GameObject contentRoot)
        {
            this._contentRoot = contentRoot;
        }

        /// <summary>
        /// Makes sure that there is no trace of anything left in the form
        /// </summary>
        /// <param name="id"></param>
        internal void CleanContent(ulong id)
        {
            IniFormAnswer(id);
            tabManager.Clear();

            float delay = 0;
            for (int i = 1; i < allContainers.Count; i++)
            {
                if (allContainers[i] == null) continue;
#if UNITY_EDITOR
                DestroyImmediate(allContainers[i]);
#else
                Destroy(_notInPageFormParts[i], delay)
                delay += 0.01f;
#endif
            }
            allContainers = new();
        }

        /// <summary>
        /// Reads and instantiate the connection form dto 
        /// </summary>
        /// <param name="connectionFormDto"></param>
        internal void Handle(ConnectionFormDto connectionFormDto)
        {
            allContainers.Add(_contentRoot);
            EvaluateDiv(connectionFormDto, 0);
        }

        /// <summary>
        /// Entry point to parse the connection form dto
        /// Evaluate each div, one after another
        /// </summary>
        /// <param name="divParent"></param>
        /// <param name="parentId"></param>
        private void EvaluateDiv(DivDto divParent, int parentId)
        {
            InstantiateDiv(divParent, parentId);
        }

        private void InstantiateDiv(DivDto divParent, int parentId)
        {
            switch (divParent)
            {
                case BaseInputDto inputDto:
                    HandleInputDto(inputDto, parentId); break;
                case FormDto inputDto:
                    HandleFormDto(inputDto); break;
                case PageDto pageDto:
                    HandlePageDto(pageDto); break;
                case LabelDto labelDto: 
                    HandleLabelDto(labelDto, parentId); break;
                case ImageDto imageDto:
                    HandleImageDto(imageDto, parentId); break;
            }
        }

        private void HandleInputDto(BaseInputDto inputDto, int parentId)
        {
            switch (inputDto)
            {
                case GroupDto groupDto:
                    HandleGroupDto(groupDto, parentId); break;
                case ButtonDto buttonDto:
                    HandleButtonDto(buttonDto, parentId); break;
            }
        }

        private void HandleGroupDto(GroupDto groupDto, int parentId)
        {
            GameObject group = Instantiate(groupContainerPrefab, allContainers[parentId].transform);
            allContainers.Add(group);
            int currentId = allContainers.Count - 1;
            foreach (var div in groupDto.FirstChildren)
            {
                EvaluateDiv(div, currentId);
            }
        }

        private void HandleButtonDto(ButtonDto buttonDto, int parentId)
        {

        }
        private void HandleFormDto(FormDto formDto)
        {
            _form = formDto;
            foreach(var div in formDto.FirstChildren)
            {
                EvaluateDiv(div, 0);
            }
        }
        private void HandlePageDto(PageDto pageDto)
        {
            allContainers.Add(tabManager.AddNewTab(pageDto.name));
            int currentId = allContainers.Count - 1;
            foreach (var div in pageDto.FirstChildren)
            {
                EvaluateDiv(div, currentId);
            }
        }
        private void HandleLabelDto(LabelDto labelDto, int parentId)
        {

        }
        /// <summary>
        /// Image and loads the resource
        /// </summary>
        /// <param name="imageDto"></param>
        /// <param name="parentId"></param>
        private async void HandleImageDto(ImageDto imageDto, int parentId)
        {
            if (imageDto.FirstChildren.Count == 0) // Simple image
            {
                GameObject imageGO = Instantiate(imageDisplayerPrefab, allContainers[parentId].transform);
                if (imageDto.resource == null) return;

                object spriteTask = await UMI3DResourcesManager.Instance._LoadFile(0, 
                    imageDto.resource.variants[0],
                    new ImageDtoLoader()
                );

                Texture2D texture = spriteTask as Texture2D;
                imageGO.GetComponent<Image>().sprite = Sprite.Create(texture,
                    new Rect(0, 0,texture.Size().x, texture.Size().y),
                    new Vector2());
            }
            else // Vignette
            {
                GameObject imageGo = Instantiate(vignetteContainerPrefab, allContainers[parentId].transform);
            }
        }

        private void IniFormAnswer(ulong id)
        {
            _answer = new FormAnswerDto()
            {
                formId = id,

                inputs = new()
            };
        }
    }
}
