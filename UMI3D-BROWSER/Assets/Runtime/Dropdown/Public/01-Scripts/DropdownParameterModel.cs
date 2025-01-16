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

using inetum.unityUtils.observation;
using umi3d.cdk;
using umi3d.common.interaction;

namespace umi3d.browserRuntime.ui.dropdown
{
    /// <summary>
    /// Model of an dropdown element for a <see cref="EnumParameterDto<string>"/>.
    /// </summary>
    /// <remarks>
    /// Need an <see cref="DropdownModel"/> to work. (set by <see cref="DropdownParameterModelContainer"/> placed on the same gameobject of <see cref="InputFieldModelContainer"/>)
    /// </remarks>

    public class DropdownParameterModel
    {
        public EnumParameterDto<string> dto;

        public DropdownModel model;

        public DropdownParameterModel(DropdownModel newModel)
        {
            model = newModel;

            NotificationHub.Default.Subscribe(this,
                ID.FromType<DropdownNotificationKeys.DropdownUpdated>(),
                (Callback)ValueUpdated,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == model));
        }

        ~DropdownParameterModel()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        /// <summary> 
        /// This method sets the DTO (Data Transfer Object) for the dropdown parameter model.<br/>
        /// It updates the model's label, value, and possible options based on the provided DTO.<br/>
        /// <br/>
        /// <example>
        /// Given a valid DTO when setting the DTO then the model is updated.
        /// <code>
        /// var newDto = new EnumParameterDto {
        ///     name = "NewName",
        ///     value = "NewValue",
        ///     possibleValues = new List { "Option1", "Option2" }
        /// };
        /// dropdownParameterModel.SetDto(newDto);
        /// </code>
        /// </example>
        /// </summary>
        public void SetDto(EnumParameterDto<string> newDto)
        {
            dto = newDto;
            model.SetLabel(dto.name);
            model.SetValue(dto.value);
            model.SetOptions(dto.possibleValues);
        }

        private void ValueUpdated(Notification notification)
        {
            if (!notification.TryGetInfoT(DropdownNotificationKeys.DropdownUpdated.Value, out string value))
                return;

            dto.value = value;

            UMI3DClientServer.SendRequest(new ParameterSettingRequestDto() {
                id = dto.id,
                parameter = dto,
            }, true);
        }
    }
}