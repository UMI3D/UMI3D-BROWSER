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
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.settings
{
    [RequireComponent(typeof(Button))]
    internal class SettingsTabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Flags]
        public enum EPlatform
        {
            Nothing,
            PC,
            XR
        }

        [SerializeField] Color textColor;
        [SerializeField] Color textColorHover;
        [SerializeField] Color textColorActive;
        [SerializeField] EPlatform platform;

        TMP_Text text;
        Button button;
        Image background;
        GameObject content;

        bool isActive;

        Notifier notifier;

        void Awake()
        {
#if UMI3D_PC
            if (!platform.HasFlag(EPlatform.PC))
            {
                gameObject.SetActive(false);
                return;
            }
#elif UMI3D_XR
            if (!platform.HasFlag(EPlatform.XR))
            {
                gameObject.SetActive(false);
                return;
            }    
#endif

            button = GetComponent<Button>();
            button.onClick.AddListener(Click);

            text = GetComponentInChildren<TMP_Text>();
            background = transform.GetChild(1).GetComponent<Image>();

            content = transform.parent.GetChild(1).gameObject;

            NotificationHub.Default.Subscribe(
                this,
                ID.FromType<SettingsNotificationKeys.NewPanelSelected>(),
                (Callback)Deactivate,
                new FilterByRef(FilterType.AcceptAllExcept, this)
            );

            notifier = NotificationHub.Default.GetNotifier(
                this,
                ID.FromType<SettingsNotificationKeys.NewPanelSelected>()
            );
        }

        void OnDestroy()
        {
#if UMI3D_PC
            if (!platform.HasFlag(EPlatform.PC))
                return;
#elif UMI3D_XR
            if (!platform.HasFlag(EPlatform.XR))
                return;
#endif
            NotificationHub.Default.Unsubscribe(this);
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
            notifier[SettingsNotificationKeys.NewPanelSelected.panel] = name;
            notifier.Notify();
            Activate();
        }

        void Activate()
        {
            isActive = true;
            text.color = textColorActive;
            background.gameObject.SetActive(true);
            content.SetActive(true);
        }

        void Deactivate(Notification notification)
        {
            if (!notification.TryGetInfoT(SettingsNotificationKeys.NewPanelSelected.panel, out string panel))
            {
                return;
            }

            if (panel == name)
            {
                return;
            }

            isActive = false;
            text.color = textColor;
            background.gameObject.SetActive(false);
            content.SetActive(false);
        }
    }
}