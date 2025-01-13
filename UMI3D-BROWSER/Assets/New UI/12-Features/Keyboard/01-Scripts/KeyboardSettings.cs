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
using UnityEngine;

namespace umi3d.browserRuntime.ui.keyboard
{
    public class KeyboardSettings : MonoBehaviour
    {
        [Header("Version")]
        public KeyboardLocalisationVersion version = KeyboardLocalisationVersion.QWERTY;
        Notifier localisationVersionNotifier;

        [Header("Opening Closing Animation")]
        public bool openingClosingWithAnimation = false;
        public float openingClosingAnimationTime = 1f;
        public float openingClosingPhaseOneStartTimePercentage = .5f;

        [Header("Depth Animation")]
        public bool keyPressWithAnimation = true;

        Notifier animationSettingsNotifier;

        void Awake()
        {
            localisationVersionNotifier = NotificationHub.Default.GetNotifier(
                this,
                ID.FromType<KeyboardNotificationKeys.ChangeVersion>()
            );

            animationSettingsNotifier = NotificationHub.Default.GetNotifier(
                this,
                ID.FromType<KeyboardNotificationKeys.AnimationSettings>()
            );
        }

        void Start()
        {
            UpdateVersion();
            UpdateOpeningClosingAnimationSettings();
            UpdateKeyPressAnimationSettings();
        }

        public void UpdateVersion()
        {
            localisationVersionNotifier[KeyboardNotificationKeys.ChangeVersion.Version] = KeyboardLocalisationVersion.AZERTY;
            localisationVersionNotifier.Notify();
        }

        public void UpdateOpeningClosingAnimationSettings()
        {
            animationSettingsNotifier[KeyboardNotificationKeys.AnimationSettings.AnimationType] = KeyboardAnimationType.OpenOrClose;
            animationSettingsNotifier[KeyboardNotificationKeys.AnimationSettings.WithAnimation] = openingClosingWithAnimation;
            //animationSettingsNotifier[KeyboardNotificationKeys.Info.AnimationTime] = openingClosingAnimationTime;
            animationSettingsNotifier.Notify();
        }

        public void UpdateKeyPressAnimationSettings()
        {
            animationSettingsNotifier[KeyboardNotificationKeys.AnimationSettings.AnimationType] = KeyboardAnimationType.KeyPress;
            animationSettingsNotifier[KeyboardNotificationKeys.AnimationSettings.WithAnimation] = keyPressWithAnimation;
            animationSettingsNotifier.Notify();
        }
    }
}