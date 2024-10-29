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
using System;
using System.Collections.Generic;
using umi3d.browserRuntime.ui.popup;
using umi3dBrowsers.data.ui;
using umi3dBrowsers.linker;
using umi3dBrowsers.linker.ui;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.inGame.tablet.menu.browser
{
    public class LeaveEnvironementButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private GameObject activeBackground;
        [SerializeField] private Image icon;
        [SerializeField] private Color iconColor;
        [SerializeField] private Color iconColorHover;
        [SerializeField] private Color iconColorActive;
        [SerializeField] private ConnectionToImmersiveLinker connectionToImmersiveLinker;

        private bool isActive;

        private Notifier popupLeaveNotifier;

        private void Awake()
        {
            SetupPopupLeaveNotifier();

            activeBackground.SetActive(false);
        }

        private void SetupPopupLeaveNotifier()
        {
            popupLeaveNotifier = NotificationHub.Default.GetNotifier<PopupNotificationKeys.Show>(this);
            popupLeaveNotifier[PopupNotificationKeys.Show.Type] = PopupType.Information;
            popupLeaveNotifier[PopupNotificationKeys.Show.Description] = "popup_leave";
            popupLeaveNotifier[PopupNotificationKeys.Show.Buttons] = new List<(string, Action)>() {
                ("leave", () => {
                    connectionToImmersiveLinker.Leave();
                    activeBackground.SetActive(false);
                    icon.color = iconColor;
                    NotificationHub.Default.Notify<PopupNotificationKeys.CloseAll>(this);
                    NotificationHub.Default.Notify(this, TabletNotificationKeys.Close);
                }),
                ("resume", () => {
                    activeBackground.SetActive(false);
                    icon.color = iconColor;
                    NotificationHub.Default.Notify<PopupNotificationKeys.CloseAll>(this);
                })
            };
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            isActive = true;
            activeBackground.SetActive(true);
            icon.color = iconColorActive;

            popupLeaveNotifier.Notify();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isActive)
                return;

            activeBackground.SetActive(true);
            icon.color = iconColorHover;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isActive)
                return;

            activeBackground.SetActive(false);
            icon.color = iconColor;
        }

        private void Deactivate()
        {
            activeBackground.SetActive(false);
            isActive = false;
            icon.color = iconColor;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            icon.color = iconColor;
        }
#endif
    }
}