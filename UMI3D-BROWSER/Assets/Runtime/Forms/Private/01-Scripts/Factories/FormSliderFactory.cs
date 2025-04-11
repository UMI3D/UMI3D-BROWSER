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

using inetum.unityUtils.observation;
using System.Collections.Generic;
using umi3d.browserRuntime.ui.inputField;
using umi3d.browserRuntime.ui.slider;
using umi3d.common.interaction.form;
using UnityEngine;

namespace umi3d.browserRuntime.forms
{
    [RequireComponent(typeof(SliderFactory)), ExecuteAlways]
    internal class FormSliderFactory : MonoBehaviour
    {
        private SliderFactory _sliderFactory;

        private void Awake()
        {
            _sliderFactory = GetComponent<SliderFactory>();
        }

        public GameObject CreateSlider(RangeDto<int> rangeDto, Transform parent, FormAnswerDto formAnswerDto)
        {
            var style = rangeDto.GetStyle();

            var sliderGameObject = _sliderFactory.GetOrCreateSlider(parent, rangeDto.label, rangeDto.Value, rangeDto.Min, rangeDto.Max, true);

            var formItemModelContainer = sliderGameObject.GetComponent<FormItemModelContainer>();

            formItemModelContainer.Model.SetPosition(style.Position);
            formItemModelContainer.Model.SetSize(style.Size);
            formItemModelContainer.Model.SetAnchor(style.AnchorMin, style.AnchorMax, style.Pivot);
            formItemModelContainer.Model.SetTextStyle(style.FontSize, style.FontColor, style.FontStyles, style.FontAlignmentOptions);

            InputAnswerDto inputAnswerDto = new InputAnswerDto() { inputId = rangeDto.guid };
            if (formAnswerDto.inputs == null)
                formAnswerDto.inputs = new List<InputAnswerDto>();
            formAnswerDto.inputs.Add(inputAnswerDto);

            var sliderModelContainer = sliderGameObject.GetComponent<SliderModelContainer>();
            NotificationHub.Default.Subscribe(this,
                ID.FromType<InputFieldNotificationsKeys.InputFieldUpdated>(),
                (Callback)UpdateAnswer,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == sliderModelContainer.model));

            return sliderGameObject;

            void UpdateAnswer(Notification notification) 
            {
                if (notification.TryGetInfoT(SliderNotifiactionKeys.SliderUpdated.Value, out string value))
                    inputAnswerDto.value = value;
            }
        }

        public GameObject CreateSlider(RangeDto<float> rangeDto, Transform parent)
        {
            var style = rangeDto.GetStyle();

            var sliderGameObject = _sliderFactory.GetOrCreateSlider(parent, rangeDto.label, rangeDto.Value, rangeDto.Min, rangeDto.Max, false);

            var formItemModelContainer = sliderGameObject.GetComponent<FormItemModelContainer>();

            formItemModelContainer.Model.SetPosition(style.Position);
            formItemModelContainer.Model.SetSize(style.Size);
            formItemModelContainer.Model.SetAnchor(style.AnchorMin, style.AnchorMax, style.Pivot);
            formItemModelContainer.Model.SetTextStyle(style.FontSize, style.FontColor, style.FontStyles, style.FontAlignmentOptions);

            return sliderGameObject;
        }
    }
}