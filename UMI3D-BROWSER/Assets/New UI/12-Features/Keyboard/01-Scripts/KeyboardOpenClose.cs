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
    public class KeyboardOpenClose : MonoBehaviour
    {
        [Tooltip("Whether the keyboard is open or close.")]
        public bool isOpen = true;

        Notifier openOrCloseNotifier;
        Notifier deselectionNotifier;

        bool withAnimation = true;
        float animationTime = 1f;
        float phaseOneStartTimePercentage = .5f;

        void Awake()
        {
            openOrCloseNotifier = NotificationHub.Default.GetNotifier(
                this,
                ID.FromType<KeyboardNotificationKeys.OpenOrClose>(),
                new()
                {
                    { KeyboardNotificationKeys.OpenOrClose.WithAnimation, withAnimation },
                    { KeyboardNotificationKeys.OpenOrClose.AnimationTime, animationTime },
                    { KeyboardNotificationKeys.OpenOrClose.PhaseOneStartTimePercentage, phaseOneStartTimePercentage }
                }
            );

            deselectionNotifier = NotificationHub.Default.GetNotifier(
                this,
                ID.FromType<KeyboardNotificationKeys.TextFieldDeselected>()
            );
        }

        void Start()
        {
            openOrCloseNotifier[KeyboardNotificationKeys.OpenOrClose.IsOpening] = isOpen;
            openOrCloseNotifier[KeyboardNotificationKeys.OpenOrClose.WithAnimation] = false;
            openOrCloseNotifier.Notify();
        }

        void OnEnable()
        {
            NotificationHub.Default.Subscribe(
                this,
                ID.FromType<KeyboardNotificationKeys.AnimationSettings>(),
                (Callback)EnableOrDisableAnimation
            );

            NotificationHub.Default.Subscribe(
                this,
                KeyboardNotificationKeys.SpecialKeyPressed,
                (Callback)SpecialKeyPressed
            );

            NotificationHub.Default.Subscribe(
                this,
                ID.FromType < KeyboardNotificationKeys.TextFieldSelected >(),
                (Callback)TextFieldSelected,
                new FilterByRef(FilterType.AcceptAllExcept, this)
            );

            NotificationHub.Default.Subscribe(
                this,
                ID.FromType<KeyboardNotificationKeys.TextFieldDeselected>(),
                (Callback)TextFieldDeselected,
                new FilterByRef(FilterType.AcceptAllExcept, this)
            );
        }

        void OnDisable()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        void Close()
        {
            isOpen = false;

            openOrCloseNotifier[KeyboardNotificationKeys.OpenOrClose.IsOpening] = isOpen;
            openOrCloseNotifier[KeyboardNotificationKeys.OpenOrClose.WithAnimation] = withAnimation;
            openOrCloseNotifier.Notify();
        }

        void Open()
        {
            isOpen = true;

            openOrCloseNotifier[KeyboardNotificationKeys.OpenOrClose.IsOpening] = isOpen;
            openOrCloseNotifier[KeyboardNotificationKeys.OpenOrClose.WithAnimation] = withAnimation;
            openOrCloseNotifier.Notify();
        }

        void EnableOrDisableAnimation(Notification notification)
        {
            if (!notification.TryGetInfoT(KeyboardNotificationKeys.AnimationSettings.AnimationType, out KeyboardAnimationType animationType))
            {
                return;
            }

            if (animationType != KeyboardAnimationType.OpenOrClose)
            {
                return;
            }

            if (!notification.TryGetInfoT(KeyboardNotificationKeys.AnimationSettings.WithAnimation, out bool withAnimation))
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

            deselectionNotifier.Notify();
        }

        void TextFieldSelected(Notification notification)
        {
            if (!notification.TryGetInfoT(KeyboardNotificationKeys.TextFieldSelected.IsPreviewBar, out bool isPreviewBar) || isPreviewBar)
            {
                return;
            }

            if (!isOpen)
            {
                Open();
            }
        }

        void TextFieldDeselected(Notification notification)
        {
            if (isOpen)
            {
                Close();
            }
        }
    }
}