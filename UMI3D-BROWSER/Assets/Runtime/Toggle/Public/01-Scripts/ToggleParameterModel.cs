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

namespace umi3d.browserRuntime.ui.toggle
{
    /// <summary>
    /// Model of an input field element for a <see cref="BooleanParameterDto"/>.
    /// </summary>
    /// <remarks>
    /// Need an <see cref="ToggleModel"/> to work. (set by <see cref="ToggleParameterModelContainer"/> placed on the same gameobject of <see cref="InputFieldModelContainer"/>)
    /// </remarks>
    public class ToggleParameterModel 
    {
        public BooleanParameterDto dto;

        public ToggleModel model;

        public ToggleParameterModel(ToggleModel newModel)
        {
            model = newModel;

            NotificationHub.Default.Subscribe<ToggleNotificationKeys.ToggleUpdated>(this, new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == model), ValueUpdated);
        }

        ~ToggleParameterModel()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        /// <summary>
        /// This method sets the DTO for the toggle model and updates the model's label and value accordingly.<br/>
        /// <br/>
        /// <example>
        /// Given a BooleanParameterDto:
        /// <code>
        /// BooleanParameterDto dto = new BooleanParameterDto() {
        ///     name = "Test Dto",
        ///     value = true,
        /// };
        /// _model.SetDto(dto);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="newDto">The new DTO to be set.</param>
        public void SetDto(BooleanParameterDto newDto)
        {
            dto = newDto;
            model.SetLabel(dto.name);
            model.SetValue(dto.value);
        }

        private void ValueUpdated(Notification notification)
        {
            if (!notification.TryGetInfoT(ToggleNotificationKeys.ToggleUpdated.Value, out bool value))
                return;

            dto.value = value;

            UMI3DClientServer.SendRequest(new ParameterSettingRequestDto() {
                id = dto.id,
                parameter = dto,
            }, true);
        }
    }
}