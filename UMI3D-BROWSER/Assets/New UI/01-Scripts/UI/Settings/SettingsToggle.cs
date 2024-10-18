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
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.settings
{
    [RequireComponent(typeof(Button))]
    public class SettingsToggle : MonoBehaviour
    {
        [SerializeField] private GameObject activeBackground;
        [SerializeField] private string group;

        private Button button;
        private bool isActive;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(Click);

            NotificationHub.Default.Subscribe(this, SettingsNotificationKeys.NewToggleCustomSelected + group, Deactivate);
        }

        private void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        private void Click()
        {
            NotificationHub.Default.Notify(this, SettingsNotificationKeys.NewToggleCustomSelected + group);
            isActive = true;
            activeBackground.SetActive(true);
        }

        private void Deactivate(Notification notification)
        {
            isActive = false;
            activeBackground.SetActive(false);
        }
    }
}