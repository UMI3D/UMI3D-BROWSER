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
using umi3d.common.interaction.form;
using UnityEngine;

namespace umi3d.browserRuntime.forms
{
    public class FormSliderBuilder<T> : ISliderBuilder where T : IComparable
    {
        public SliderFactory factory { get; private set; }

        GameObject control;
        SliderModel model;

        RangeDto<T> dto;
        FormAnswerDto formAnswerDto;
        InputAnswerDto answerDto;

        public FormSliderBuilder(SliderFactory factory, RangeDto<T> dto, FormAnswerDto formAnswerDto)
        {
            this.factory = factory;
            this.dto = dto;
            this.formAnswerDto = formAnswerDto;

            answerDto = new InputAnswerDto() { inputId = dto.guid, value = dto.Value };
            formAnswerDto.inputs.Add(answerDto);
            model.valueChanged += value => answerDto.value = value;
        }

        public void Build(Transform parent)
        {
            factory.TryToGetOrCreate(out control, out model, parent);
        }

        public void BuildStyle()
        {
            var style = dto.GetStyle();

            var formItemController = control.GetComponent<FormItemModelContainer>();
            FormItemModel model = formItemController.Model;

            model.SetPosition(style.Position);
            model.SetSize(style.Size);
            model.SetAnchor(style.AnchorMin, style.AnchorMax, style.Pivot);
            model.SetTextStyle(style.FontSize, style.FontColor, style.FontStyles, style.FontAlignmentOptions);
        }

        public void BuildLabel()
        {
            model.SetLabel(dto.label);
        }

        public void BuildRange()
        {
            if (dto.Min is int minInt && dto.Max is int maxInt)
            {
                model.SetMinValue(minInt);
                model.SetMaxValue(maxInt);
            }
            else if (dto.Min is float minFloat && dto.Max is float maxFloat)
            {
                model.SetMinValue(minFloat);
                model.SetMaxValue(maxFloat);
            }
        }

        public void BuildValue()
        {
            if (dto.Value is int valueInt)
            {
                model.SetValue(valueInt);
                model.SetIsInteger(true);
            }
            else if (dto.Value is float valueFloat)
            {
                model.SetValue(valueFloat);
                model.SetIsInteger(false);
            }
        }

        public GameObject GetControl()
        {
            control.SetActive(true);
            return control;
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