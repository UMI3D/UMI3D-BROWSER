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

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using umi3d;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace umi3dBrowsers.displayer
{
    public class InputFieldDispayer : MonoBehaviour, IInputFieldDisplayer
    {
        [Header("Form")]
        [SerializeField] private TMP_UMI3DUIInputField textInputField;
        [SerializeField] private TextMeshProUGUI placeHolder;
        [SerializeField] private TextMeshProUGUI title;

        public object GetValue(bool trim)
        {
            return GetText(trim);
        }
        public string GetText(bool trim)
        {
            if (trim)
                return textInputField.text.Trim();
            else
                return textInputField.text;
        }

        public void SetTitle(string title)
        {
            this.title.text = title + " :";
        }

        public void SetPlaceHolder(List<string> placeHolder)
        {
            this.placeHolder.text = placeHolder[0];
        }

        public void SetColor(Color color)
        {
            throw new NotImplementedException();
        }

        public void SetResource(object resource)
        {
            throw new NotImplementedException();
        }

        public void SetPrivateParam(bool privateParameter)
        {
            if (privateParameter == true)
                textInputField.contentType = TMP_InputField.ContentType.Password;
        }
    }
}
