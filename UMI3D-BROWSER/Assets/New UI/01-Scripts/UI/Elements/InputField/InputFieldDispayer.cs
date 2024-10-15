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

using System.Collections.Generic;
using TMPro;
using umi3d.common.interaction.form;
using UnityEngine;

namespace umi3dBrowsers.displayer
{
    public class InputFieldDispayer : MonoBehaviour, IInputFieldDisplayer
    {
        [SerializeField] TMP_Text title;
        TMP_InputField inputField;

        TMP_Text placeholder => inputField.placeholder as TMP_Text;

        void Awake()
        {
            inputField = GetComponentInChildren<TMP_InputField>();
        }

        public object GetValue(bool trim)
        {
            return GetText(trim);
        }
        public string GetText(bool trim)
        {
            if (trim)
                return inputField.text.Trim();
            else
                return inputField.text;
        }

        public void SetTitle(string title)
        {
            this.title.text = title + " :";
        }

        public void SetPlaceHolder(List<string> placeHolder)
        {
            placeholder.text = placeHolder[0];
        }

        public void SetColor(Color color)
        {
        }

        public void SetResource(object resource)
        {
        }

        public void SetType(TextType type)
        {
            inputField.contentType = ToContentType(type);
            if (type == TextType.Phone)
                inputField.characterLimit = 15;

            inputField.characterValidation = ToCharaterValidation(type);
        }

        private TMP_InputField.ContentType ToContentType(TextType type)
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

        private TMP_InputField.CharacterValidation ToCharaterValidation(TextType type)
        {
            switch (type)
            {
                case TextType.Text:
                    return TMP_InputField.CharacterValidation.None;
                case TextType.Mail:
                    return TMP_InputField.CharacterValidation.EmailAddress;
                case TextType.Password:
                    return TMP_InputField.CharacterValidation.None;
                case TextType.Phone:
                    return TMP_InputField.CharacterValidation.Integer;
                case TextType.URL:
                    return TMP_InputField.CharacterValidation.None;
                case TextType.Number:
                    return TMP_InputField.CharacterValidation.Decimal;
            }
            return TMP_InputField.CharacterValidation.Name;
        }
    }
}
