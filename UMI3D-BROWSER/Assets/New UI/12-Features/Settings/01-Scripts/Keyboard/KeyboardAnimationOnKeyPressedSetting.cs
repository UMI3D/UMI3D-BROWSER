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
using umi3d.browserRuntime.ui.keyboard;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.settings
{
    [RequireComponent(typeof(Button))]
    internal class KeyboardAnimationOnKeyPressedSetting : MonoBehaviour
    {
        [SerializeField] bool isOn;

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
            notifier[KeyboardNotificationKeys.AnimationSettings.AnimationType] = KeyboardAnimationType.KeyPress;
        }

        void Start()
        {
            if (isOn == KeyboardSettings.model.AnimateOnKeyPressed)
            {
                button.onClick?.Invoke();
            }
        }

        void Click()
        {
            KeyboardSettings.model.AnimateOnKeyPressed = isOn;

            notifier[KeyboardNotificationKeys.AnimationSettings.WithAnimation] = isOn;
            notifier.Notify();
        }
    }
}