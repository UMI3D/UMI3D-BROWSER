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
using MathNet.Numerics.Distributions;
using System.Collections;
using System.Collections.Generic;
using umi3d.browserRuntime.NotificationKeys;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.keyboard
{
    public class KeyboardAnimationTest : MonoBehaviour
    {
        Notifier animationNotifier;

        [SerializeField] Toggle openCloseToggle;
        [SerializeField] Toggle keyPressToggle;

        void Awake()
        {
            animationNotifier = NotificationHub.Default.GetNotifier(
                this,
                KeyboardNotificationKeys.AnimationSettings
            );
        }

        private void Start()
        {
            animationNotifier[KeyboardNotificationKeys.Info.AnimationType] = KeyboardAnimationType.OpenOrClose;
            animationNotifier[KeyboardNotificationKeys.Info.WithAnimation] = openCloseToggle.isOn;
            animationNotifier.Notify();

            animationNotifier[KeyboardNotificationKeys.Info.AnimationType] = KeyboardAnimationType.KeyPress;
            animationNotifier[KeyboardNotificationKeys.Info.WithAnimation] = keyPressToggle.isOn;
            animationNotifier.Notify();
        }

        public void EnableOpenOrCloseAnimation(bool enable)
        {
            animationNotifier[KeyboardNotificationKeys.Info.AnimationType] = KeyboardAnimationType.OpenOrClose;
            animationNotifier[KeyboardNotificationKeys.Info.WithAnimation] = enable;
            animationNotifier.Notify();
        }

        public void EnableKeyPressAnimation(bool enable)
        {
            animationNotifier[KeyboardNotificationKeys.Info.AnimationType] = KeyboardAnimationType.KeyPress;
            animationNotifier[KeyboardNotificationKeys.Info.WithAnimation] = enable;
            animationNotifier.Notify();
        }
    }
}