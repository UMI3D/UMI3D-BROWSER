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
    public class KeyboardOpenClose : MonoBehaviour
    {
        [Tooltip("Whether the keyboard is open or close.")]
        public bool isOpen = true;

        Notifier openOrCloseNotifier;
        Notifier selectionNotifier;

        bool withAnimation = true;
        float animationTime = 1f;
        float phaseOneStartTimePercentage = .5f;

        void Awake()
        {
            openOrCloseNotifier = NotificationHub.Default.GetNotifier(
                this,
                KeyboardNotificationKeys.OpenOrClose,
                null,
                new()
                {
                    { KeyboardNotificationKeys.Info.WithAnimation, withAnimation },
                    { KeyboardNotificationKeys.Info.AnimationTime, animationTime },
                    { KeyboardNotificationKeys.Info.PhaseOneStartTimePercentage, phaseOneStartTimePercentage }
                }
            );

            selectionNotifier = NotificationHub.Default.GetNotifier(
                this,
                KeyboardNotificationKeys.TextFieldSelected,
                null,
                null
            );
        }

        void Start()
        {
            openOrCloseNotifier[KeyboardNotificationKeys.Info.IsOpening] = isOpen;
            openOrCloseNotifier[KeyboardNotificationKeys.Info.WithAnimation] = false;
            openOrCloseNotifier.Notify();
        }

        void OnEnable()
        {
            NotificationHub.Default.Subscribe(
                this,
                KeyboardNotificationKeys.AnimationSettings,
                EnableOrDisableAnimation
            );

            NotificationHub.Default.Subscribe(
                this,
                KeyboardNotificationKeys.SpecialKeyPressed,
                SpecialKeyPressed
            );

            NotificationHub.Default.Subscribe(
                this,
                KeyboardNotificationKeys.TextFieldSelected,
                null,
                TextFieldSelected
            );
        }

        void OnDisable()
        {
            NotificationHub.Default.Unsubscribe(this, KeyboardNotificationKeys.AnimationSettings);

            NotificationHub.Default.Unsubscribe(this, KeyboardNotificationKeys.SpecialKeyPressed);

            NotificationHub.Default.Unsubscribe(this, KeyboardNotificationKeys.TextFieldSelected);
        }

        void Close()
        {
            isOpen = false;

            openOrCloseNotifier[KeyboardNotificationKeys.Info.IsOpening] = isOpen;
            openOrCloseNotifier[KeyboardNotificationKeys.Info.WithAnimation] = withAnimation;
            openOrCloseNotifier.Notify();

            selectionNotifier[KeyboardNotificationKeys.Info.IsActivation] = false;
            selectionNotifier[KeyboardNotificationKeys.Info.SelectionPositions] = null;
            selectionNotifier[KeyboardNotificationKeys.Info.InputFieldText] = null;
            selectionNotifier.Notify();
        }

        void Open()
        {
            isOpen = true;

            openOrCloseNotifier[KeyboardNotificationKeys.Info.IsOpening] = isOpen;
            openOrCloseNotifier[KeyboardNotificationKeys.Info.WithAnimation] = withAnimation;
            openOrCloseNotifier.Notify();
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

            this.withAnimation = withAnimation;
        }

        void SpecialKeyPressed(Notification notification)
        {
            if (!notification.TryGetInfoT(KeyboardNotificationKeys.Info.SpecialKey, out SpecialKey key) || key != SpecialKey.Quit)
            {
                return;
            }

            Close();
        }

        void TextFieldSelected(Notification notification)
        {
            if (isOpen)
            {
                return;
            }

            if (!notification.TryGetInfoT(KeyboardNotificationKeys.Info.IsActivation, out bool isActivation))
            {
                return;
            }

            if (!isOpen && isActivation)
            {
                Open();
                return;
            }

            if (isOpen && !isActivation)
            {
                Close();
                return;
            }
        }
    }
}