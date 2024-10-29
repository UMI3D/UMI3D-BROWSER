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
using umi3d.browserRuntime.NotificationKeys;
using umi3d.browserRuntime.ui.settings;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.settings
{
    public class KeyboardAnimationOnKeyPressedSetting : MonoBehaviour
    {
        [SerializeField] bool isOn;

        Button button;

        Notifier notifier;

        void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(Click);

            notifier = NotificationHub.Default
                .GetNotifier<KeyboardNotificationKeys.ChangeVersion>(this);
            notifier[KeyboardNotificationKeys.AnimationSettings.AnimationType] = KeyboardAnimationType.KeyPress;
        }

        void Start()
        {
            bool savedValue = PlayerPrefs.GetInt(SettingsPlayerPrefsKeys.KeyboardAnimationOnKeyPressed, 1) == 1 ? true : false;

            if (isOn == savedValue)
            {
                Click();
            }
        }

        void Click()
        {
            PlayerPrefs.SetInt(SettingsPlayerPrefsKeys.KeyboardAnimationOnKeyPressed, isOn ? 1 : 0);

            notifier[KeyboardNotificationKeys.AnimationSettings.WithAnimation] = isOn;
            notifier.Notify();
        }
    }
}