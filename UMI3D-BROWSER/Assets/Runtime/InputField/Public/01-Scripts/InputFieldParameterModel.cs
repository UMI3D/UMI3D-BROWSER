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

using umi3d.common.interaction;
using umi3d.cdk;
using inetum.unityUtils.observation;

namespace umi3d.browserRuntime.ui.inputField
{
    /// <summary>
    /// Model of an input field element for a <see cref="StringParameterDto"/>.
    /// </summary>
    /// <remarks>
    /// Need an <see cref="InputFieldModel"/> to work. (set by <see cref="InputFieldParameterModelContainer"/> placed on the same gameobject of <see cref="InputFieldModelContainer"/>)
    /// </remarks>
    public class InputFieldParameterModel
    {
        public StringParameterDto dto;

        public InputFieldModel model;

        public InputFieldParameterModel(InputFieldModel newModel)
        {
            model = newModel;

            NotificationHub.Default.Subscribe(this,
                ID.FromType<InputFieldNotificationsKeys.InputFieldUpdated>(),
                (Callback)ValueUpdated,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == model));
        }

        ~InputFieldParameterModel()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        /// <summary>
        /// This method sets the DTO for the input field model and updates the model's label, value, and number of lines accordingly.<br/>
        /// <br/>
        /// <example>
        /// Given a StringParameterDto:
        /// <code>
        /// StringParameterDto dto = new StringParameterDto() {
        ///     name = "Test Dto",
        ///     value = "Test Value",
        ///     NbLine = 1
        /// };
        /// _model.SetDto(dto);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="newDto">The new DTO to be set.</param>
        public void SetDto(StringParameterDto newDto)
        {
            dto = newDto;
            model.SetLabel(dto.name);
            model.SetValue(dto.value);
            model.SetNbrLines(newDto.IsMultiLine, dto.NbLine);
            model.SetContentType(dto.privateParameter ? TMPro.TMP_InputField.ContentType.Password : TMPro.TMP_InputField.ContentType.Standard);
        }

        /// <summary>
        /// Called by <see cref="InputFieldNotificationsKeys.InputFieldUpdated"/>.
        /// Update the <see cref="StringParameterDto"/> with the value changed by the user.
        /// </summary>
        /// <param name="notification"></param>
        private void ValueUpdated(Notification notification)
        {
            if (!notification.TryGetInfoT(InputFieldNotificationsKeys.InputFieldUpdated.Value, out string value))
                return;

            if (dto != null)
                dto.value = value;
        }

        public void Submit() 
        {
            UMI3DClientServer.SendRequest(new ParameterSettingRequestDto()
                {
                    id = dto.id,
                    parameter = dto,
                }, true);
        }

        /// <summary>
        /// This method releases the DTO by setting it to null.<br/>
        /// <br/>
        /// <example>
        /// Given a model with a non-null DTO, when calling ReleaseDto, then the DTO should be null.
        /// <code>
        /// model.ReleaseDto();
        /// </code>
        /// </example>
        /// </summary>
        public void ReleaseDto()
        {
            dto = null;
        }
    }
}