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
    public class LegacyFormDropdownBuilder : IDropdownBuilder
    {
        public DropdownFactory factory {  get; private set; }

        GameObject control;
        DropdownModel model;

        EnumParameterDto<string> dto;
        FormAnswerDto answerDto;
        ParameterSettingRequestDto requestDto;

        public LegacyFormDropdownBuilder(DropdownFactory factory, EnumParameterDto<string> dto, FormAnswerDto answerDto)
        {
            this.factory = factory;
            this.dto = dto;
            this.answerDto = answerDto;

            requestDto = new() { id = dto.id, parameter = dto.value };
            answerDto.answers.Add(requestDto);
            model.valueChanged += value => requestDto.parameter = value;
        }

        public void Build(Transform parent)
        {
            factory.TryToGetOrCreate(out control, parent);
        }

        public void BuildLabel()
        {
            model.SetLabel(dto.name);
        }

        public void BuildOptions()
        {
            model.SetOptions(dto.possibleValues ?? new());
        }

        public void BuildValue()
        {
            model.SetValue(dto.value);
        }

        public void BuildValueChanged(Action<string> action)
        {
            model.valueChanged += action;
        }

        public void BuildSubmit(ISubmitSubject subject, Action onSubmit)
        {
            throw new NotSupportedException();
        }

        public GameObject GetControl()
        {
            control.SetActive(true);
            return control;
        }

        public void Clear(ISubmitSubject subject)
        {
            subject?.Unsubscribe(model);
            factory.Return(control);
            control = null;
        }
    }
}