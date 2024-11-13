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
using umi3d.browserRuntime.notificationKeys;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.popup
{
    internal class PopupBorder : MonoBehaviour
    {
        Image image;

        void Awake()
        {
            image = GetComponent<Image>();

            NotificationHub.Default
               .Subscribe<PopupNotificationKeys.DisplayPopup>(
               this,
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
            if (!notification.TryGetInfoT(PopupNotificationKeys.EnqueuePopup.PopupInfo, out PopupInfo popupInfo))
            {
                return;
            }

            switch (popupInfo.type)
            {
                case PopupType.Information:
                    image.color = PopupColor.HexToColor(PopupColor.InformationColor);
                    break;
                case PopupType.Warning:
                    image.color = PopupColor.HexToColor(PopupColor.WarningColor);
                    break;
                case PopupType.Error:
                    image.color = PopupColor.HexToColor(PopupColor.ErrorColor);
                    break;
                default:
                    break;
            }
        }
    }
}