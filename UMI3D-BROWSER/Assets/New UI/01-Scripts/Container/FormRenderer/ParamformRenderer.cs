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
using umi3d;
using umi3d.common.interaction;
using umi3dBrowsers.linker;
using UnityEngine;
using UnityEngine.UI;

namespace umi3dBrowsers.container.formrenderer 
{
    public class ParamformRenderer : MonoBehaviour
    {
        [Header("Displayers")]
        [SerializeField] private GameObject textFieldPrefab;
        [SerializeField] private GameObject textPrefab;
        [SerializeField] private GameObject vectorFieldPrefab;
        [SerializeField] private GameObject dropDownFieldPrefab;
        [SerializeField] private GameObject sliderPrefab;
        [SerializeField] private GameObject toggleSwitchPrefab;

        [Header("Roots")]
        [SerializeField] private TabManager tabManager;
        [SerializeField] private SimpleButton validationButton;
        [SerializeField] private ConnectionServiceLinker connectionServiceLinker;

        private GameObject _contentRoot;

        /// <summary>
        /// List of callbacks for every parts of the form to build the answer dto
        /// </summary>
        List<Action> formBinding = new();
        private FormAnswerDto _formAnswer;

        public event Action<FormAnswerDto> OnFormAnswer;
        public void Init(GameObject contentRoot)
        {
            this._contentRoot = contentRoot;

            OnFormAnswer += connectionServiceLinker.SendFormAnswer;
        }


        internal void Handle(ConnectionFormDto connectionFormDto)
        {
            validationButton.gameObject.SetActive(true);
            validationButton.OnClick.RemoveAllListeners();
            validationButton.OnClick.AddListener(() => ValidateForm());

            GameObject container = tabManager.AddNewTabForParamForm(connectionFormDto.fields[0].name, false);

            for (int i = 0; i < connectionFormDto.fields.Count; i++)
            {
                IDisplayer displayer = null;

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
                            GameObject gameObject = Instantiate(dropDownFieldPrefab, container.transform);
                            displayer = gameObject.GetComponentInChildren<IDisplayer>();
                            formBinding.Add(() => paramRequestDto.parameter = displayer.GetValue(true));
                            displayer.SetTitle(paramEnum.name);
                            displayer.SetPlaceHolder(paramEnum.possibleValues);
                        }
                        break;
                    case BooleanParameterDto boolParam:
                        {
                            GameObject gameObject = Instantiate(toggleSwitchPrefab, container.transform);
                            displayer = gameObject.GetComponentInChildren<IDisplayer>();
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
                            GameObject gameObject = null;
                            if (connectionFormDto.fields[i].name == "OR" && connectionFormDto.fields[i].tag == null)
                                formBinding.Add(() => paramRequestDto.parameter = null);
                            else
                            {
                                gameObject = Instantiate(stringParam.isDisplayer ? textPrefab : textFieldPrefab);
                                gameObject.transform.SetParent(container.transform, false);

                                var inputFieldDisplayer = gameObject?.GetComponentInChildren<IInputFieldDisplayer>();
                                displayer = inputFieldDisplayer;
                                formBinding.Add(() => paramRequestDto.parameter = displayer?.GetValue(true));
                                displayer?.SetTitle(stringParam.name);
                                displayer?.SetPlaceHolder(new List<string>() { stringParam.description });
                                inputFieldDisplayer?.SetType(connectionFormDto.fields[i].privateParameter ? umi3d.common.interaction.form.TextType.Password : umi3d.common.interaction.form.TextType.Text);
                            }
                        }
                        break;
                    default:
                        break;
                }

                if (connectionFormDto.fields[i].name == "OR" && connectionFormDto.fields[i].tag == null)
                {
                    container = tabManager.AddNewTabForParamForm(connectionFormDto.fields[i + 1].name, false);
                }
            }

            tabManager.InitSelectedButtonById(0);
        }

        [ContextMenu("Validate form ")]
        public void ValidateForm()
        {
            formBinding.ForEach(action => action?.Invoke());
            OnFormAnswer?.Invoke(_formAnswer);
        }

        private void InitFormAnswer(ulong id)
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
            InitFormAnswer(id);

            tabManager.Clear();
        }
    }
}

