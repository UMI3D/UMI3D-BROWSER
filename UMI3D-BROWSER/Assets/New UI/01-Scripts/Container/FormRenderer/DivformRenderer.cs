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

using form_generator;
using System;
using System.Collections;
using System.Collections.Generic;
using umi3d.common.interaction.form;
using UnityEngine;

namespace umi3dBrowsers.container.formrenderer
{
    public class DivformRenderer : MonoBehaviour
    {
        private GameObject _contentRoot;
        public event Action<GameObject> OnObjectAddedToRoot;
        public event Action<FormAnswerDto> OnFormAnwser;

        [Header("UI Containers")]
        [SerializeField] private GameObject panelManagerContainerPrefab;

        [Header("UI Displayers")]
        [SerializeField] private GameObject labelDisplayerPrefab;
        //[SerializeField] private GameObject 
        public void Init(GameObject contentRoot)
        {
            this._contentRoot = contentRoot;
        }

        internal void CleanContent(ulong id)
        {

        }

        internal void Handle(ConnectionFormDto connectionFormDto)
        {
            EvaluateDiv(connectionFormDto);
        }

        private void EvaluateDiv(DivDto divParent)
        {
            InstantiateDiv(divParent);
            foreach (var div in divParent.FirstChildren)
            {
                EvaluateDiv(div);
            }
        }

        private void InstantiateDiv(DivDto divParent)
        {
            switch (divParent)
            {
                case BaseInputDto inputDto:
                    HandleInputDto(inputDto); break;
                case FormDto inputDto:
                    HandleFormDto(inputDto); break;
                case PageDto pageDto:
                    HandlePageDto(pageDto); break;
                case LabelDto labelDto: 
                    HandleLabelDto(labelDto); break;
                case ImageDto imageDto:
                    HandleImageDto(imageDto); break;
            }
        }

        private void HandleInputDto(BaseInputDto inputDto)
        {
            switch (inputDto)
            {
                case GroupDto groupDto:
                    HandleGroupDto(groupDto); break;
                case ButtonDto buttonDto:
                    HandleButtonDto(buttonDto); break;
            }
        }


        private void HandleGroupDto(GroupDto groupDto)
        {
            throw new NotImplementedException();
        }
        private void HandleButtonDto(ButtonDto buttonDto)
        {
            throw new NotImplementedException();
        }
        private void HandleFormDto(FormDto inputDto)
        {
            throw new NotImplementedException();
        }
        private void HandlePageDto(PageDto pageDto)
        {
            throw new NotImplementedException();
        }
        private void HandleLabelDto(LabelDto labelDto)
        {
            throw new NotImplementedException();
        }
        private void HandleImageDto(ImageDto imageDto)
        {
            throw new NotImplementedException();
        }
    }
}
