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

using UnityEngine.UI;
using UnityEngine;
using umi3d.browserRuntime.NotificationKeys;
using inetum.unityUtils;

namespace umi3d.browserRuntime.ui.settings
{
    [RequireComponent(typeof(Button))]
    internal class KeyboardVersionSetting : MonoBehaviour
    {
        [SerializeField] KeyboardLocalisationVersion localisationVersion;

        Button button;

        KeyboardSettings KeyboardSettings;

        Notifier notifier;

        void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(Click);

            KeyboardSettings = GetComponentInParent<KeyboardSettings>();

            notifier = NotificationHub.Default
                .GetNotifier<KeyboardNotificationKeys.ChangeVersion>(this);
        }

        void Start()
        {
            if (localisationVersion == KeyboardSettings.model.localisationVersion)
            {
                button.onClick?.Invoke();
            }
        }

        void Click()
        {
            KeyboardSettings.model.localisationVersion = localisationVersion;

            notifier[KeyboardNotificationKeys.ChangeVersion.Version] = localisationVersion;
            notifier.Notify();
        }
    }
}