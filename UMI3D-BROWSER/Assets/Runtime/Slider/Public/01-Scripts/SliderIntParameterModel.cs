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

using inetum.unityUtils;
using inetum.unityUtils.observation;
using umi3d.cdk;
using umi3d.common.interaction;

namespace umi3d.browserRuntime.ui.slider
{
    public class SliderIntParameterModel
    {
        public IntegerRangeParameterDto dto;

        public SliderModel model;

        public SliderIntParameterModel(SliderModel newModel)
        {
            model = newModel;

            NotificationHub.Default.Subscribe(this,
                ID.FromType<SliderNotifiactionKeys.SliderUpdated>(), 
                (Callback)ValueUpdated,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == model));
        }

        ~SliderIntParameterModel()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        /// <summary>
        /// This method sets the DTO for the integer slider and updates the slider model with the new DTO values.<br/>
        /// <br/>
        /// <example>
        /// Given an IntegerRangeParameterDto, when calling SetDto, then the slider model is updated with the DTO values.
        /// <code>
        /// IntegerRangeParameterDto dto = new IntegerRangeParameterDto { name = "Volume", min = 0, max = 100, value = 50 };
        /// SetDto(dto);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="newDto">The new DTO to set.</param>
        public void SetDto(IntegerRangeParameterDto newDto)
        {
            dto = newDto;
            model.SetLabel(dto.name);
            model.SetMinValue(dto.min);
            model.SetMaxValue(dto.max);
            model.SetValue(dto.value);
            model.SetIsInteger(true);
        }

        private void ValueUpdated(Notification notification)
        {
            if (!notification.TryGetInfoT(SliderNotifiactionKeys.SliderUpdated.Value, out int value))
                return;

            dto.value = value;
        }

        public void Submit()
        {
            UMI3DClientServer.SendRequest(new ParameterSettingRequestDto() {
                id = dto.id,
                parameter = dto,
            }, true);
        }
    }
}