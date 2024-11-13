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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace umi3d.browserRuntime.ui.popup
{
    internal class PopupBodyText : MonoBehaviour
    {
        TMPro.TMP_Text text;
        LocalizeStringEvent stringEvent;

        void Awake()
        {
            text = GetComponent<TMPro.TMP_Text>();
            stringEvent = GetComponent<LocalizeStringEvent>();

            NotificationHub.Default
               .Subscribe<PopupNotificationKeys.EnqueuePopup>(
               this,
               new FilterByCondition(FilterType.AcceptOnly, publisher => publisher is PopupManager),
               NewPopup
           );
        }

        void OnDestroy()
        {
            NotificationHub.Default
                .Unsubscribe<PopupNotificationKeys.EnqueuePopup>(this);
        }

        void NewPopup(Notification notification)
        {
            if (notification.TryGetInfoT(PopupNotificationKeys.EnqueuePopup.Arguments, out Dictionary<string, System.Object> arguments, false))
            {
                UpdateArguments(arguments);
            }
            else
            {
                UpdateArguments(null);
            }

            if (notification.TryGetInfoT(PopupNotificationKeys.EnqueuePopup.Description, out string text, false))
            {
                UpdateText(text);
            }
            else if (notification.TryGetInfoT(PopupNotificationKeys.EnqueuePopup.Description, out (string, string) tableAndEntry, false))
            {
                UpdateLocalizeText(tableAndEntry.Item1, tableAndEntry.Item2);
            }
            else
            {
                notification.LogError(this.GetType().FullName, PopupNotificationKeys.EnqueuePopup.Description, "The description is neither string or (string, string)");
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