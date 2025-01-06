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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace umi3d.browserRuntime.ui.popup
{
    internal class PopupTitle : MonoBehaviour
    {
        TMPro.TMP_Text text;
        LocalizeStringEvent stringEvent;

        void Awake()
        {
            text = GetComponent<TMPro.TMP_Text>();
            stringEvent = GetComponent<LocalizeStringEvent>();

            NotificationHub.Default.Subscribe(
               this,
               ID.FromType<PopupNotificationKeys.DisplayPopup>(),
               NewPopup
           );
        }

        void OnDestroy()
        {
            NotificationHub.Default
                .Unsubscribe<PopupNotificationKeys.DisplayPopup>(this);
        }

        void NewPopup(Notification notification)
        {
            if (!notification.TryGetInfoT(PopupNotificationKeys.DisplayPopup.PopupInfo, out PopupInfo popupInfo))
            {
                return;
            }

            UpdateArguments(popupInfo.arguments);

            System.Object title = popupInfo.title;

            if (title is string text)
            {
                UpdateText(text);
            }
            else if (title is (string table, string entry))
            {
                UpdateLocalizeText(table, entry);
            }
            else if (title == null)
            {
                UpdateText(null);
            }
            else
            {
                UnityEngine.Debug.LogError($"The description is neither string or (string, string)");
            }
        }

        void UpdateArguments(Dictionary<string, System.Object> arguments)
        {
            stringEvent.StringReference.Arguments = new object[] { arguments };
        }

        void UpdateText(string text)
        {
            UpdateLocalizeText(null, null);
            this.text.text = text;
        }

        void UpdateLocalizeText(string table, string entry)
        {
            stringEvent.SetTable(table);
            stringEvent.SetEntry(entry);
            stringEvent.RefreshString();
        }
    }
}