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
using umi3d.common.interaction;
using UnityEngine;

namespace umi3dBrowsers.container.formrenderer 
{
    public class ParamformRenderer : MonoBehaviour
    {
        [Header("Displayers")]
        [SerializeField] private GameObject textFieldPrefab;
        [SerializeField] private GameObject vectorFieldPrefab;
        [SerializeField] private GameObject dropDownFieldPrefab;
        [SerializeField] private GameObject sliderPrefab;
        [SerializeField] private GameObject validationButtonPrefab;
        [SerializeField] private GameObject toggleSwitchPrefab;

        private GameObject _contentRoot;
        List<Action> formBinding = new();
        private FormAnswerDto _formAnswer;

        public event Action<GameObject> OnObjectAddedToRoot;
        public event Action<FormAnswerDto> OnFormAnwser;
        public void Init(GameObject contentRoot)
        {
            this._contentRoot = contentRoot;
        }

        internal void Handle(ConnectionFormDto connectionFormDto)
        {
            for (int i = 0; i < connectionFormDto.fields.Count; i++)
            {
                ParameterSettingRequestDto paramRequestDto = new ParameterSettingRequestDto()
                {
                    toolId = connectionFormDto.fields[i].id,
                    id = connectionFormDto.fields[i].id,
                    hoveredObjectId = 0
                };

                _formAnswer.answers.Add(paramRequestDto);

                switch (connectionFormDto.fields[i])
                {
                    case EnumParameterDto<string> paramEnum:
                        {
                            GameObject gameObject = Instantiate(dropDownFieldPrefab, _contentRoot.transform);
                            OnObjectAddedToRoot?.Invoke(gameObject);
                            IDisplayer displayer = gameObject.GetComponent<IDisplayer>();
                            formBinding.Add(() => paramRequestDto.parameter = displayer.GetValue(true));
                            displayer.SetTitle(paramEnum.name);
                            displayer.SetPlaceHolder(paramEnum.possibleValues);
                        }
                        break;
                    case BooleanParameterDto boolParam:
                        {
                            GameObject gameObject = Instantiate(toggleSwitchPrefab, _contentRoot.transform);
                            OnObjectAddedToRoot?.Invoke(gameObject);
                            IDisplayer displayer = gameObject.GetComponentInChildren<IDisplayer>();
                            formBinding.Add(() => paramRequestDto.parameter = displayer.GetValue(true));
                            displayer.SetTitle(boolParam.name);
                            displayer.SetPlaceHolder(new List<string>() { boolParam.value ? "1" : "0" });
                        }
                        break;
                    case ColorParameterDto:

                        break;
                    case DeviceIdParameterDto:

                        break;
                    case StringParameterDto stringParam:
                        {
                            GameObject gameObject = Instantiate(textFieldPrefab, _contentRoot.transform);
                            OnObjectAddedToRoot?.Invoke(gameObject);
                            IDisplayer displayer = gameObject.GetComponent<IDisplayer>();
                            formBinding.Add(() => paramRequestDto.parameter = displayer.GetValue(true));
                            displayer.SetTitle(stringParam.name);
                            displayer.SetPlaceHolder(new List<string>() { stringParam.description });
                        }
                        break;
                    default:
                        break;
                }
            }

            GameObject go = Instantiate(validationButtonPrefab, _contentRoot.transform);
            SimpleButton simpleButton = go.GetComponent<SimpleButton>();
            simpleButton.OnClick.AddListener(() => ValidateForm());
            OnObjectAddedToRoot?.Invoke(go);
        }

        [ContextMenu("Validate form ")]
        public void ValidateForm()
        {
            formBinding.ForEach(action => action?.Invoke());
            OnFormAnwser?.Invoke(_formAnswer);
        }

        private void IniFormAnswer(ulong id)
        {
            _formAnswer = new FormAnswerDto()
            {
                boneType = 0,
                hoveredObjectId = 0,
                id = id,
                toolId = 0,
                answers = new List<ParameterSettingRequestDto>()
            };
        }

        internal void CleanContent(ulong id)
        {
            formBinding = new();
            IniFormAnswer(id);
        }
    }
}

