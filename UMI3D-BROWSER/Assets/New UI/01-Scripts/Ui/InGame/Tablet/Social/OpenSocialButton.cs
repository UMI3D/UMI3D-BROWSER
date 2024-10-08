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
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.inGame.tablet.social
{
    [RequireComponent(typeof(Button))]
    public class OpenSocialButton : MonoBehaviour
    {
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OpenSocial);

            NotificationHub.Default.Subscribe(this, TabletNotificationKeys.ClickButtonSocial, Click);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(OpenSocial);
            NotificationHub.Default.Unsubscribe(this);
        }

        private void OpenSocial()
        {
            NotificationHub.Default.Notify(this, TabletNotificationKeys.CloseScreens);
            NotificationHub.Default.Notify(this, TabletNotificationKeys.OpenSocial);
        }

        private void Click(Notification notification)
        {
            button.onClick?.Invoke();
        }
    }
}