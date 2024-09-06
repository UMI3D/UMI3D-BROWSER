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
using System.Collections;
using System.Collections.Generic;
using umi3d.browserRuntime.NotificationKeys;
using UnityEngine;

namespace umi3d.browserRuntime.ui.keyboard
{
    public class KeyQuit : MonoBehaviour
    {
        PointerDownBehaviour pointerDown;

        Notifier closeNotifier;

        void Awake()
        {
            pointerDown = GetComponent<PointerDownBehaviour>();

            pointerDown.pointerClickedSimple += PointerDown;

            closeNotifier = NotificationHub.Default.GetNotifier(
                this,
                KeyboardNotificationKeys.OpenOrClose,
                null,
                new()
                {
                    { KeyboardNotificationKeys.Info.IsOpening, false },
                    { KeyboardNotificationKeys.Info.WithAnimation, true },
                    { KeyboardNotificationKeys.Info.AnimationTime, 1f },
                    { KeyboardNotificationKeys.Info.PhaseOneStartTimePercentage, .5f }
                }
            );

            NotificationHub.Default.Subscribe(
                this,
                KeyboardNotificationKeys.AnimationSettings,
                EnableOrDisableAnimation
            );
        }

        void EnableOrDisableAnimation(Notification notification)
        {
            if (!notification.TryGetInfoT(KeyboardNotificationKeys.Info.AnimationType, out KeyboardAnimationType animationType))
            {
                return;
            }

            if (animationType != KeyboardAnimationType.OpenOrClose)
            {
                return;
            }

            if (!notification.TryGetInfoT(KeyboardNotificationKeys.Info.WithAnimation, out bool withAnimation))
            {
                return;
            }

            closeNotifier[KeyboardNotificationKeys.Info.WithAnimation] = withAnimation;
        }

        void PointerDown()
        {
            closeNotifier.Notify();
        }
    }
}