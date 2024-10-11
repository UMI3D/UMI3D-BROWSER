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
using umi3d.cdk.collaboration;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.settings.audio
{
    [RequireComponent(typeof(Button))]
    public class NoiseReductionButton : MonoBehaviour
    {
        [SerializeField] private bool isOnButton;
        [SerializeField] private GameObject activeBackground;

        private Button button;
        private bool isActive;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(Click);

            NotificationHub.Default.Subscribe(this, SettingsNotificationKeys.NewUseNoiseReductionSelected, Deactivate);
        }

        private void Start()
        {
            if (PlayerPrefs.GetInt(SettingsPlayerPrefsKeys.UseNoiseReduction, 0) > 0 == isOnButton)
                button.onClick?.Invoke();
        }

        private void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        private void Click()
        {
            NotificationHub.Default.Notify(this, SettingsNotificationKeys.NewUseNoiseReductionSelected);
            isActive = true;
            activeBackground.SetActive(true);

            MicrophoneListener.Instance.UseNoiseReduction = isOnButton;
            PlayerPrefs.SetInt(SettingsPlayerPrefsKeys.UseNoiseReduction, isOnButton ? 1 : 0);
        }

        private void Deactivate(Notification notification)
        {
            isActive = false;
            activeBackground.SetActive(false);
        }
    }
}