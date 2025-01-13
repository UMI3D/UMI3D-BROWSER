/*
Copyright 2019 - 2025 Inetum

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
    public class KeyboardHotFix : MonoBehaviour
    {
        Notifier animationNotifier;
        Notifier versionNotifier;

        void Start()
        {
            UnityEngine.Debug.LogWarning($"Warning : HotFix for keyboard.");
            animationNotifier = NotificationHub.Default.GetNotifier(
                this,
                ID.FromType<KeyboardNotificationKeys.AnimationSettings>()
            );
            animationNotifier[KeyboardNotificationKeys.AnimationSettings.AnimationType] = KeyboardAnimationType.OpenOrClose;
            animationNotifier[KeyboardNotificationKeys.AnimationSettings.WithAnimation] = false;
            animationNotifier.Notify();

            versionNotifier = NotificationHub.Default.GetNotifier(
                this,
                ID.FromType<KeyboardNotificationKeys.ChangeVersion>()
            );
            versionNotifier[KeyboardNotificationKeys.ChangeVersion.Version] = KeyboardLocalisationVersion.AZERTY;
            versionNotifier.Notify();
        }
    }
}