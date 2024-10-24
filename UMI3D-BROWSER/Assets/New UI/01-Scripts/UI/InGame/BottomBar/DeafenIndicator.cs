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
using umi3d.browserRuntime.ui.settings;
using UnityEngine;

namespace umi3d.browserRuntime.ui.inGame.bottomBar
{
    public class DeafenIndicator : MonoBehaviour
    {
        private bool internalState = false;
        private bool isEnable = false;

        private void Awake()
        {
            NotificationHub.Default.Subscribe(this, InGameNotificationKeys.DeafenChanged, DeafenChagned);
            NotificationHub.Default.Subscribe(this, SettingsNotificationKeys.SetDeafenIndicator, Set);
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        private void Set(Notification notification)
        {
            if (notification.TryGetInfoT(SettingsNotificationKeys.IsDeafenIndicatorEnable, out bool enable))
            {
                isEnable = enable;
                if (!isEnable)
                    gameObject.SetActive(false);
                else
                    gameObject.SetActive(internalState);
            }
        }

        private void DeafenChagned(Notification notification)
        {
            if (notification.TryGetInfoT(InGameNotificationKeys.IsDeafen, out bool isDeafen))
            {
                if (isEnable)
                    gameObject.SetActive(isDeafen);
                internalState = isDeafen;
            }
        }
    }
}