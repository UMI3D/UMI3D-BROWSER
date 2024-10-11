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
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.settings
{
    [RequireComponent(typeof(Button))]
    public class SettingsButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private Color textColor;
        [SerializeField] private Color textColorHover;
        [SerializeField] private Color textColorActive;
        [SerializeField] private GameObject activeBackground;

        private Button button;
        private bool isActive;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(Click);

            NotificationHub.Default.Subscribe(this, SettingsNotificationKeys.NewPanelSelected, Deactivate);
        }

        private void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        private void Click()
        {
            NotificationHub.Default.Notify(this, SettingsNotificationKeys.NewPanelSelected);
            isActive = true;
            text.color = textColorActive;
            activeBackground.SetActive(true);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            text.color = textColorHover;
            activeBackground.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isActive)
                return;

            text.color = textColor;
            activeBackground.SetActive(false);
        }

        private void Deactivate(Notification notification)
        {
            isActive = false;
            text.color = textColor;
            activeBackground.SetActive(false);
        }
    }
}