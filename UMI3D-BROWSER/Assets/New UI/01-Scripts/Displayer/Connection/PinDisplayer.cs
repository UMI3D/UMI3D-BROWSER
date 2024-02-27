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
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace umi3dBrowsers.displayer
{
    public class PinDisplayer : MonoBehaviour
    {
        [Header("Form")]
        [SerializeField] private TMP_UMI3DUIInputField pinInputField;
        [Space]
        [SerializeField] private Button button;
        [SerializeField] private UnityEvent<PinData> OnPinButtonPressed;

        private void Awake()
        {
            button.onClick.AddListener(() =>
            {
                PinData id = new PinData(
                    pinInputField.text.Trim()
                );

                OnPinButtonPressed?.Invoke(id);
            });
        }
    }

    [Serializable]
    public class PinData
    {
        [SerializeField] private string pin;

        public string Pin => pin;

        public PinData(string pin)
        {
            this.pin = pin;
        }
    }
}
