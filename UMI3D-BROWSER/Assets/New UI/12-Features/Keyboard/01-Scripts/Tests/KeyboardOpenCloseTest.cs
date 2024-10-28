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
using TMPro;
using umi3d.browserRuntime.NotificationKeys;
using UnityEngine;

namespace umi3d.browserRuntime.ui.keyboard
{
    public class KeyboardOpenCloseTest : MonoBehaviour
    {
        Notifier openOrCloseNotifier;

        [Tooltip("Whether the keyboard is open or close.")]
        public bool isOpen = true;
        [Tooltip("Button's text")]
        public TMP_Text text;

        bool withAnimation;

        void Awake()
        {
            openOrCloseNotifier = NotificationHub.Default.GetNotifier<KeyboardNotificationKeys.OpenOrClose>(
                this,
                null,
                new()
                {
                    { KeyboardNotificationKeys.OpenOrClose.IsOpening, isOpen },
                    { KeyboardNotificationKeys.OpenOrClose.WithAnimation, false },
                    { KeyboardNotificationKeys.OpenOrClose.AnimationTime, 1f },
                    { KeyboardNotificationKeys.OpenOrClose.PhaseOneStartTimePercentage, .5f }
                }
            );

            NotificationHub.Default.Subscribe<KeyboardNotificationKeys.AnimationSettings>(
                this,
                EnableOrDisableAnimation
            );

            text.text = isOpen ? "Close" : "Open";
        }

        void Start()
        {
            openOrCloseNotifier[KeyboardNotificationKeys.OpenOrClose.IsOpening] = isOpen;
            openOrCloseNotifier[KeyboardNotificationKeys.OpenOrClose.WithAnimation] = false;
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

        public void OpenOrClose()
        {
            isOpen = !isOpen;
            text.text = isOpen ? "Close" : "Open";
            openOrCloseNotifier[KeyboardNotificationKeys.OpenOrClose.IsOpening] = isOpen;
            openOrCloseNotifier[KeyboardNotificationKeys.OpenOrClose.WithAnimation] = withAnimation;
            openOrCloseNotifier.Notify();
        }

#if UNITY_EDITOR
        [ContextMenu("Test Open")]
        void TestOpen()
        {
            UnityEngine.Debug.Log($"test open");
            openOrCloseNotifier[KeyboardNotificationKeys.OpenOrClose.IsOpening] = true;
            openOrCloseNotifier[KeyboardNotificationKeys.OpenOrClose.WithAnimation] = withAnimation;
            openOrCloseNotifier.Notify();
        }

        [ContextMenu("Test Close")]
        void TestClose()
        {
            UnityEngine.Debug.Log($"test close");
            openOrCloseNotifier[KeyboardNotificationKeys.OpenOrClose.IsOpening] = false;
            openOrCloseNotifier[KeyboardNotificationKeys.OpenOrClose.WithAnimation] = withAnimation;
            openOrCloseNotifier.Notify();
        }
#endif
    }
}