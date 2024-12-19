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

using inetum.unityUtils.observation;
using umi3d.cdk.collaboration;
using UnityEngine;

namespace umi3d.browserRuntime.ui.settings
{
    internal class AudioModeDependencySettings : MonoBehaviour
    {
        [SerializeField] MicrophoneMode mode;
        int instanceID;

        void Awake()
        {
            instanceID = GetComponentInParent<SettingsContent>().GetInstanceID();

            NotificationHub.Default.Subscribe<SettingsNotificationKeys.MicrophoneModeChanged>(
                this,
                ModeChanged
            );
        }

        void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe<SettingsNotificationKeys.MicrophoneModeChanged>(this);
        }

        void ModeChanged(Notification notification)
        {
            if (!notification.TryGetInfoT(SettingsNotificationKeys.MicrophoneModeChanged.Mode, out MicrophoneMode mode))
            {
                return;
            }

            gameObject.SetActive(mode == this.mode);
            NotificationHub.Default.Notify(this, SettingsNotificationKeys.UpdateChildVisibilitySelected + instanceID);

        }
    }
}