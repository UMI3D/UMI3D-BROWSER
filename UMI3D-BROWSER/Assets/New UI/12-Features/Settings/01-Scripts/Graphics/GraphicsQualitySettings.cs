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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using umi3d.browserRuntime.notificationKeys;

namespace umi3d.browserRuntime.ui.settings
{
    [RequireComponent(typeof(Button))]
    public class GraphicsQualitySettings : MonoBehaviour
    {
        [SerializeField] BrowserQualitySettings quality;

        Button button;

        GraphicsSettings graphicsSettings;

        Notifier notifier;

        void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(Click);

            graphicsSettings = GetComponentInParent<GraphicsSettings>();

            notifier = NotificationHub.Default
                .GetNotifier<SettingsNotificationKeys.QualityChanged>(this);
        }

        void Start()
        {
            if (quality == graphicsSettings.model.quality)
            {
                button.onClick?.Invoke();
            }
        }

        void Click()
        {
            QualitySettings.SetQualityLevel((int)quality, false);
            graphicsSettings.model.quality = quality;

            notifier[SettingsNotificationKeys.QualityChanged.Quality] = quality;
            notifier.Notify();
        }
    }
}