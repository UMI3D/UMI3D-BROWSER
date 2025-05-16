/*
Copyright 2019 - 2025 Inetum

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

using System.Collections.Generic;
using umi3d.browserRuntime.forms;
using umi3d.common.interaction;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.formMenu
{
    public class LegacyFormMenuFactory : FormMenuFactory
    {
        [SerializeField] internal Button _submitButton;

        internal FormAnswerDto _formAnswerDto = new FormAnswerDto();
        protected override object formAnswerDtoObject => _formAnswerDto;

        protected override void Awake()
        {
            base.Awake();
            _submitButton.onClick.AddListener(SendAnswer);
        }

        void OnDestroy()
        {
            _submitButton.onClick.RemoveListener(SendAnswer);
        }

        public override void DisplayForm(common.interaction.form.FormDto formDto)
        {
        }

        public override void DisplayLegacyForm(ConnectionFormDto legacyFormDto)
        {
            if (!_content) { _content = transform; }

            if (_content.childCount > 0) { Clear(); }

            _formAnswerDto.id = legacyFormDto.id;

            if (legacyFormDto.fields != null)
            {
                AddFields(legacyFormDto.fields);
            }

            _submitButton.gameObject.SetActive(true);
        }

        internal void AddFields(List<AbstractParameterDto> parameterDtos)
        {
            var container = _tabManager.AddNewTabForParamForm(parameterDtos[0].name, false).transform;

            for (int i = 0; i < parameterDtos.Count; i++)
            {
                AddField(parameterDtos[i], container);

                if (parameterDtos[i].name == "OR" && parameterDtos[i].tag == null)
                {
                    container = _tabManager.AddNewTabForParamForm(parameterDtos[i + 1].name, false).transform;
                }
            }
        }

        void AddField(AbstractParameterDto parameterDto, Transform container)
        {
            switch (parameterDto)
            {
                case EnumParameterDto<string> enumDto:
                    {
                        LegacyFormDropdownBuilder builder = new(_dropdownFactory, enumDto, _formAnswerDto);
                        builder.Build(container);
                        builder.BuildLabel();
                        builder.BuildOptions();
                        builder.BuildValue();
                        builder.GetControl();
                        _dropdownBuilder.Add(builder);
                        break;
                    }

                case StringParameterDto stringDto:
                    if (parameterDto.name == "OR" && parameterDto.tag == null) { break; }
                    else if (parameterDto.isDisplayer)
                    {
                        _textFactory.CreateText(stringDto, container);
                    }
                    else
                    {
                        LegacyFormInputFieldBuilder builder = new(_inputFieldFactory, stringDto, _formAnswerDto);
                        builder.Build(container);
                        builder.BuildLabel();
                        builder.BuildPlaceholder();
                        builder.BuildContentType();
                        builder.BuildLine();
                        builder.BuildValue();
                        builder.GetControl();
                        _inputFieldBuilders.Add(builder);
                    }
                    break;

                default:
                    break;
            }
        }

        protected override void Clear()
        {
            base.Clear();
            _submitButton.gameObject.SetActive(false);
        }

        protected override void ResetFormAnser()
        {
            _formAnswerDto = null;
        }
    }
}