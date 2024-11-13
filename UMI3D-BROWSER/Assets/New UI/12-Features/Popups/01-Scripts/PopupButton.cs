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
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.popup
{
    internal class PopupButton : MonoBehaviour
    {
        public int index;
        public Action<int> action;

        Button button;
        TMPro.TMP_Text text;
        LocalizeStringEvent stringEvent;

        Notifier closeNotifier;

        void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(Click);

            text = GetComponentInChildren<TMPro.TMP_Text>();
            stringEvent = GetComponentInChildren<LocalizeStringEvent>();

            closeNotifier = NotificationHub.Default
                .GetNotifier<PopupNotificationKeys.PopupClosed>(this);
        }

        void Click()
        {
            if (action == null)
            {
                UnityEngine.Debug.LogError($"[Popup] Action is null.");
            }
            action?.Invoke(index);

            closeNotifier.Notify();
        }

        public void UpdateText(string text)
        {
            UpdateLocalizeText(null, null);
            this.text.text = text;
        }

        public void UpdateLocalizeText(string table, string entry)
        {
            stringEvent.SetTable(table);
            stringEvent.SetEntry(entry);
            stringEvent.RefreshString();
        }

        public void SetPopupID(System.Guid? id)
        {
            closeNotifier[PopupNotificationKeys.PopupClosed.ID] = id;
        }
    }
}