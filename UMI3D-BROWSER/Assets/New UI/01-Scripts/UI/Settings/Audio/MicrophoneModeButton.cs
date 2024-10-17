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
using System;
using System.Collections.Generic;
using umi3d.cdk.collaboration;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.settings.audio
{
    [RequireComponent(typeof(Button))]
    public class MicrophoneModeButton : MonoBehaviour
    {
        [SerializeField] private MicrophoneMode mode;
        [SerializeField] private bool isDefault;
        [SerializeField] private List<GameObject> gameObjects;
        [SerializeField] private string toggleGroup;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(() => {
                MicrophoneListener.Instance.SetCurrentMicrophoneMode(mode);
                PlayerPrefs.SetString(SettingsPlayerPrefsKeys.AudioMode, mode.ToString());
            });

            NotificationHub.Default.Subscribe(this, SettingsNotificationKeys.NewToggleCustomSelected + toggleGroup, Selected);
        }

        private void Start()
        {
            var modeString = PlayerPrefs.GetString(SettingsPlayerPrefsKeys.AudioMode, "");
            if (modeString == mode.ToString() || (modeString == string.Empty && isDefault))
                button.onClick?.Invoke();
        }

        private void Selected(Notification notification)
        {
            var active = (notification.Publisher as MonoBehaviour).gameObject == gameObject;
            foreach (var item in gameObjects)
                item.SetActive(active);
        }
    }
}