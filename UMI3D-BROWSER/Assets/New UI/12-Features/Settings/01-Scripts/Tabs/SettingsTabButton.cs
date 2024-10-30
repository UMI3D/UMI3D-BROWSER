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
    public class SettingsTabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] Color textColor;
        [SerializeField] Color textColorHover;
        [SerializeField] Color textColorActive;

        TMP_Text text;
        Button button;
        Image background;
        GameObject content;

        bool isActive;

        void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(Click);

            text = GetComponentInChildren<TMP_Text>();
            background = GetComponentInChildren<Image>();

            content = transform.parent.GetChild(1).gameObject;

            NotificationHub.Default.Subscribe<SettingsNotificationKeys.NewPanelSelected>(
                this,
                new FilterByRef(FilterType.AcceptAllExcept, this),
                Deactivate
            );
        }

        void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe<SettingsNotificationKeys.NewPanelSelected>(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            text.color = textColorHover;
            background.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isActive)
            {
                return;
            }

            text.color = textColor;
            background.gameObject.SetActive(false);
        }

        void Click()
        {
            NotificationHub.Default.Notify<SettingsNotificationKeys.NewPanelSelected>(this);
            Activate();
        }

        void Activate()
        {
            isActive = true;
            text.color = textColorActive;
            background.gameObject.SetActive(true);
            content.SetActive(true);
        }

        void Deactivate()
        {
            isActive = false;
            text.color = textColor;
            background.gameObject.SetActive(false);
            content.SetActive(false);
        }
    }
}