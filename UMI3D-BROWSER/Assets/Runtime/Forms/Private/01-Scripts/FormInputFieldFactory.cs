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

using TMPro;
using umi3d.browserRuntime.ui.inputField;
using umi3d.common.interaction.form;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.forms
{
    internal class FormInputFieldFactory : MonoBehaviour
    {
        private InputFieldFactory _inputFieldFactory;

        private void Awake()
        {
            _inputFieldFactory = GetComponent<InputFieldFactory>();
        }

        public GameObject CreateInputField<T>(InputDto<T> inputDto, Transform parent)
        {
            var style = inputDto.GetStyle();

            var inputFieldGameObject = _inputFieldFactory.GetOrCreateInputField(parent, false, inputDto.Name, inputDto.Value?.ToString(), inputDto.PlaceHolder?.ToString(), 1, TmpContentTypeFrom(inputDto.TextType));

            var formItemModelContainer = inputFieldGameObject.GetComponent<FormItemModelContainer>();

            formItemModelContainer.Model.SetPosition(style.Position);
            formItemModelContainer.Model.SetSize(style.Size);
            formItemModelContainer.Model.SetAnchor(style.AnchorMin, style.AnchorMax, style.Pivot);
            formItemModelContainer.Model.SetTextStyle(style.FontSize, style.FontColor, style.FontStyles, style.FontAlignmentOptions);

            return inputFieldGameObject;
        }

        private static TMP_InputField.ContentType TmpContentTypeFrom(TextType type)
        {
            switch (type)
            {
                case TextType.Text:
                    return TMP_InputField.ContentType.Standard;
                case TextType.Mail:
                    return TMP_InputField.ContentType.EmailAddress;
                case TextType.Password:
                    return TMP_InputField.ContentType.Password;
                case TextType.Phone:
                    return TMP_InputField.ContentType.IntegerNumber;
                case TextType.URL:
                    return TMP_InputField.ContentType.Standard;
                case TextType.Number:
                    return TMP_InputField.ContentType.IntegerNumber;
            }
            return TMP_InputField.ContentType.Standard;
        }
    }
}