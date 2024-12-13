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
using umi3d.common.interaction;
using umi3d.cdk;

namespace umi3d.browserRuntime.inputField
{
    public class InputFieldParameterModel 
    {
        public StringParameterDto dto;

        public InputFieldModel model;

        public InputFieldParameterModel(InputFieldModel newModel)
        {
            model = newModel;

            NotificationHub.Default.Subscribe<InputFieldNotificationsKeys.InputFieldUpdated>(this, new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == model), ValueUpdated);
        }

        ~InputFieldParameterModel() 
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        public void SetDto(StringParameterDto newDto)
        {
            UnityEngine.Debug.Log("SetDto");
            dto = newDto;
            model.SetTitle(dto.name);
            model.SetValue(dto.value);
            model.SetNbrLines(dto.NbLine);
        }

        private void ValueUpdated(Notification notification)
        {
            if (!notification.TryGetInfoT(InputFieldNotificationsKeys.InputFieldUpdated.Value, out string value))
                return;

            dto.value = value;

            UMI3DClientServer.SendRequest(new ParameterSettingRequestDto() {
                id = dto.id,
                parameter = dto,
            }, true);
        }
    }
}