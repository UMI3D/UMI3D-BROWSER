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

namespace umi3d.browserRuntime.ui.settings
{
    public class GraphicsQualityDependenciesSettings : MonoBehaviour
    {
        void Awake()
        {
            NotificationHub.Default.Subscribe<SettingsNotificationKeys.QualityChanged>(
                this,
                QualityChanged
            );
        }

        void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe<SettingsNotificationKeys.QualityChanged>(this);
        }

        void QualityChanged(Notification notification)
        {
            if (!notification.TryGetInfoT(SettingsNotificationKeys.QualityChanged.Quality, out BrowserQualitySettings quality))
            {
                return;
            }

            gameObject.SetActive(quality == BrowserQualitySettings.Custom);
        }
    }
}