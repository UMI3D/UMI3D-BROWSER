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
using umi3d.common.interaction.form;
using Unity.VisualScripting;
using UnityEngine;

namespace umi3dBrowsers.container.formrenderer
{
    public class DivformRenderer : MonoBehaviour
    {
        private GameObject _contentRoot;
        private FormAnswerDto _answer;
        public event Action<FormAnswerDto> OnFormAnswer;
        List<GameObject> _notInPageFormParts = new();

        [Header("UI Containers")]
        [SerializeField] private GameObject tabManagerPrefab;
        [SerializeField] private GameObject imageContainerPrefab;
        [Space]
        [SerializeField] private TabManager tabManager;

        [Header("UI Displayers")]
        [SerializeField] private GameObject labelDisplayerPrefab;
        [SerializeField] private GameObject buttonDisplayerPrefab;

        FormDto _form;

        public const string VIGNETTE_NAME = "Vignette";
        public void Init(GameObject contentRoot)
        {
            this._contentRoot = contentRoot;
        }

        internal void CleanContent(ulong id)
        {
            IniFormAnswer(id);
            tabManager.Clear();

            float delay = 0;
            for (int i = 0; i < _notInPageFormParts.Count; i++)
            {
#if UNITY_EDITOR
                DestroyImmediate(_notInPageFormParts[i]);
#else
                Destroy(_notInPageFormParts[i], delay)
                delay += 0.01f;
#endif
            }
            _notInPageFormParts = new();
        }

        internal void Handle(ConnectionFormDto connectionFormDto)
        {
            EvaluateDiv(connectionFormDto, -1);
        }

        private void EvaluateDiv(DivDto divParent, int pageId)
        {
            InstantiateDiv(divParent, pageId);
        }

        private void InstantiateDiv(DivDto divParent, int pageId)
        {
            switch (divParent)
            {
                case BaseInputDto inputDto:
                    HandleInputDto(inputDto, pageId); break;
                case FormDto inputDto:
                    HandleFormDto(inputDto); break;
                case PageDto pageDto:
                    HandlePageDto(pageDto); break;
                case LabelDto labelDto: 
                    HandleLabelDto(labelDto, pageId); break;
                case ImageDto imageDto:
                    HandleImageDto(imageDto, pageId); break;
            }
        }

        private void HandleInputDto(BaseInputDto inputDto, int pageId)
        {
            switch (inputDto)
            {
                case GroupDto groupDto:
                    HandleGroupDto(groupDto, pageId); break;
                case ButtonDto buttonDto:
                    HandleButtonDto(buttonDto, pageId); break;
            }
        }

        private void HandleGroupDto(GroupDto groupDto, int pageId)
        {

        }
        private void HandleButtonDto(ButtonDto buttonDto, int pageId)
        {

        }
        private void HandleFormDto(FormDto formDto)
        {
            _form = formDto;
            foreach(var div in formDto.FirstChildren)
            {
                EvaluateDiv(div, -1);
            }
        }
        private void HandlePageDto(PageDto pageDto)
        {
            int tabContainerId = tabManager.AddNewTab(pageDto.name);
            foreach (var div in pageDto.FirstChildren)
            {
                EvaluateDiv(div, tabContainerId);
            }
        }
        private void HandleLabelDto(LabelDto labelDto, int pageId)
        {

        }
        private void HandleImageDto(ImageDto imageDto, int pageId)
        {

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
