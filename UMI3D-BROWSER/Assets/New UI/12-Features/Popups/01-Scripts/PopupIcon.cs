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
using umi3d.browserRuntime.ui.popup;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.pu
{
    internal class PopupIcon : MonoBehaviour
    {
        Image image;

        void Awake()
        {
            image = GetComponent<Image>();

            NotificationHub.Default
               .Subscribe<PopupNotificationKeys.Show>(
               this,
               new FilterByCondition(FilterType.AcceptOnly, publisher => publisher is PopupManager),
               NewPopup
           );
        }

        void OnDestroy()
        {
            NotificationHub.Default
              .Unsubscribe<PopupNotificationKeys.Show>(this);
        }

        void NewPopup(Notification notification)
        {
            if (!notification.TryGetInfoT(PopupNotificationKeys.Show.Type, out PopupType type))
            {
                return;
            }

            switch (type)
            {
                case PopupType.Information:
                    image.gameObject.SetActive(false);
                    break;
                case PopupType.Warning:
                    image.gameObject.SetActive(true);
                    image.color = PopupColor.HexToColor(PopupColor.WarningColor);
                    break;
                case PopupType.Error:
                    image.gameObject.SetActive(true);
                    image.color = PopupColor.HexToColor(PopupColor.ErrorColor);
                    break;
                default:
                    break;
            }
        }
    }
}