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

using System;
using umi3d.browserRuntime.ui;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.browserRuntime.forms
{
    public class LegacyFormInputFieldBuilder : IInputFieldBuilder
    {
        public InputFieldFactory factory { get; private set; }

        GameObject control;
        InputFieldModel model;

        StringParameterDto dto;
        FormAnswerDto formAnswerDto;
        ParameterSettingRequestDto parameterSettingRequestDto;

        public LegacyFormInputFieldBuilder(InputFieldFactory factory, StringParameterDto dto, FormAnswerDto formAnswerDto)
        {
            this.factory = factory;
            this.dto = dto;
            this.formAnswerDto = formAnswerDto;

            if (formAnswerDto.answers == null)
            {
                formAnswerDto.answers = new();
            }
            parameterSettingRequestDto = new ParameterSettingRequestDto() { id = dto.id, parameter = dto.value };
            formAnswerDto.answers.Add(parameterSettingRequestDto);
        }

        public void Build(Transform parent)
        {
            if (dto.IsMultiLine)
            {
                factory.TryToGetOrCreateMultiLine(out control, out model, parent);
            }
            else
            {
                factory.TryToGetOrCreateSingleLine(out control, out model, parent);
            }

            model.valueChanged += value => parameterSettingRequestDto.parameter = value;
        }

        public void BuildContentType()
        {
            model.SetContentType(TMPro.TMP_InputField.ContentType.Standard);
            model.SetIsPin(false);
        }

        public void BuildLabel()
        {
            if (!string.IsNullOrEmpty(dto.name))
            {
                model.SetLabel(dto.name);
            }
        }

        public void BuildLine()
        {
            if (dto.NbLine < 1) { dto.NbLine = 1; }
            if (dto.NbLine != 1) { model.SetNbrLines(dto.IsMultiLine, dto.NbLine); }
        }

        public void BuildPlaceholder()
        {
            model.SetPlaceholder(null);
        }

        public void BuildValue()
        {
            if (!string.IsNullOrEmpty(dto.value))
            {
                model.SetValue(dto.value);
            }
        }

        public GameObject GetControl()
        {
            control.SetActive(true);
            return control;
        }

        public void BuildValueChange(Action<string> action)
        {
            model.valueChanged += action;
        }

        public void BuildSubmit(ISubmitSubject subject, Action onSubmit)
        {
            subject.Subscribe(model);
            model.submit += onSubmit;
        }

        public void Clear(ISubmitSubject subject)
        {
            subject?.Unsubscribe(model);
            factory.Return(control);
            control = null;
        }
    }
}