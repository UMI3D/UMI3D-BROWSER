/*
Copyright 2019 - 2023 Inetum

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
using umi3dBrowsers.displayer;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace umi3dBrowsers
{
    public class URLDisplayer : MonoBehaviour
    {
        [Header("UrlForm")]
        [SerializeField] private TMP_UMI3DUIInputField urlField;
        [SerializeField] private Button submitButton;
        [Space]
        public UnityEvent<string> OnSubmit;

        private void Awake()
        {
            submitButton.onClick.AddListener(() =>
            {
                OnSubmit?.Invoke(urlField.text.Trim());
            });
        }
    }
}