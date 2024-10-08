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
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.inGame.tablet.menu.principal
{
    public class MenuPrincipalButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private GameObject info;
        [SerializeField] private GameObject activeBackground;
        [SerializeField] private Image icon;
        [SerializeField] private Color iconColor;
        [SerializeField] private Color iconColorHover;
        [SerializeField] private Color iconColorActive;

        private bool isActive;

        private void Awake()
        {
            info.SetActive(false);
            activeBackground.SetActive(false);

            NotificationHub.Default.Subscribe(this, TabletNotificationKeys.NewScreenSelected, Deactivate);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            NotificationHub.Default.Notify(this, TabletNotificationKeys.NewScreenSelected);
            isActive = true;
            activeBackground.SetActive(true);
            icon.color = iconColorActive;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isActive)
                return;

            info.SetActive(true);
            icon.color = iconColorHover;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isActive)
                return;

            info.SetActive(false);
            icon.color = iconColor;
        }

        private void Deactivate()
        {
            info.SetActive(false);
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