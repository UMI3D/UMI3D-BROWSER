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
using umi3d.baseBrowser.inputs.interactions;
using umi3d.browserRuntime.notificationKeys;
using umi3d.common.interaction;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.inGame.interactionMapping
{
    [RequireComponent(typeof(Image))]
    public class CursorDisplayer : MonoBehaviour
    {
        [SerializeField] private Sprite cursorIcon;
        [SerializeField] private Sprite cursorHoverIcon;

        private Image cursorImage;

        private void Awake()
        {
            NotificationHub.Default.Subscribe(
                this,
                ID.FromType<InteractionNotificationKeys.ParameterInputFound>(),
                (Callback)ParameterInputFound
            );

            NotificationHub.Default.Subscribe(
                this,
                ID.FromType<InteractionNotificationKeys.ToolReleased>(),
                (Callback)ToolReleased
            );

            cursorImage = GetComponent<Image>();

            KeyboardInteraction.Mapped += ShowHover;
            KeyboardInteraction.Unmapped += ShowNormal;
        }

        private void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);

            KeyboardInteraction.Mapped -= ShowHover;
            KeyboardInteraction.Unmapped -= ShowNormal;
        }

        private void ShowNormal()
        {
            cursorImage.sprite = cursorIcon;
        }
        private void ShowNormal(KeyboardInteraction interaction, string k) => ShowNormal();

        private void ShowHover()
        {
            cursorImage.sprite = cursorHoverIcon;
        }
        private void ShowHover(KeyboardInteraction interaction, string arg2, InputAction action) => ShowHover();
        private void ShowHover(AbstractParameterDto dto) => ShowHover();

        void ParameterInputFound(Notification notification)
        {
            if (!notification.TryGetInfoT(InteractionNotificationKeys.ParameterInputFound.parameterDto, out AbstractParameterDto dto))
            {
                return;
            }

            ShowHover(dto);
        }

        void ToolReleased()
        {
            ShowNormal();
        }
    }
}