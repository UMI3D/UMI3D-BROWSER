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
    public class ConnectionDsplayer : MonoBehaviour
    {
        [Header("Form")]
        [SerializeField] private TMP_UMI3DUIInputField usernameInputField;
        [SerializeField] private TMP_UMI3DUIInputField passwordInputField;
        [Space]
        [SerializeField] private Button button;
        [SerializeField] private UnityEvent<ConnectionData> OnConnectionButtonPressed;
        [Space]
        [SerializeField] private ToggleSwitch toggleSwitch;

        private void Awake()
        {
            button.onClick.AddListener(() =>
            {
                ConnectionData id = new ConnectionData(
                    usernameInputField.text.Trim(),
                    passwordInputField.text.Trim(),
                    toggleSwitch.CurrentValue
                );

                OnConnectionButtonPressed?.Invoke( id );
            });
        }
    }

    [Serializable]
    public class ConnectionData
    {
        [SerializeField] private string username;
        [SerializeField] private string password;
        [SerializeField] private bool rememberMe;

        public string Username => username;
        public string Password => password;
        public bool RememberMe => rememberMe;

        public ConnectionData(string username, string password, bool rememberMe)
        {
            this.username = username;
            this.password = password;
            this.rememberMe = rememberMe;
        }
    }
}

