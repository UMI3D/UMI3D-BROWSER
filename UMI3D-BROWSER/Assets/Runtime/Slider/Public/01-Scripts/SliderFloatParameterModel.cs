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
using umi3d.cdk;
using umi3d.common.interaction;

namespace umi3d.browserRuntime.ui.slider
{
    public class SliderFloatParameterModel
    {
        public FloatRangeParameterDto dto;

        public SliderModel model;

        public SliderFloatParameterModel(SliderModel newModel)
        {
            model = newModel;

            NotificationHub.Default.Subscribe<SliderNotifiactionKeys.SliderUpdated>(this, new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == model), ValueUpdated);
        }

        ~SliderFloatParameterModel()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        public void SetDto(FloatRangeParameterDto newDto)
        {
            dto = newDto;
            model.SetLabel(dto.name);
            model.SetMinValue(dto.min);
            model.SetMaxValue(dto.max);
            model.SetValue(dto.value);
            model.SetIsInteger(false);
        }

        private void ValueUpdated(Notification notification)
        {
            if (!notification.TryGetInfoT(SliderNotifiactionKeys.SliderUpdated.Value, out int value))
                return;

            dto.value = value;

            UMI3DClientServer.SendRequest(new ParameterSettingRequestDto() {
                id = dto.id,
                parameter = dto,
            }, true);
        }
    }
}