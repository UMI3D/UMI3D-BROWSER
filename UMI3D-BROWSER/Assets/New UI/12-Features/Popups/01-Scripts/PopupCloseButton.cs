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
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.popup
{
    internal class PopupCloseButton : MonoBehaviour
    {
        Button button;
        PopupInfo popupInfo;

        Notifier closeNotifier;

        void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(Click);

            NotificationHub.Default
               .Subscribe<PopupNotificationKeys.DisplayPopup>(
               this,
               NewPopup
           );

            NotificationHub.Default
               .Subscribe<PopupNotificationKeys.CloseCurrentOpenedPopup>(
               this,
               CloseCurrentOpenedPopup
           );

            closeNotifier = NotificationHub.Default
               .GetNotifier<PopupNotificationKeys.PopupClosed>(this);
        }

        void OnDestroy()
        {
            NotificationHub.Default
                .Unsubscribe<PopupNotificationKeys.DisplayPopup>(this);

            NotificationHub.Default
                .Unsubscribe<PopupNotificationKeys.CloseCurrentOpenedPopup>(this);
        }

        void NewPopup(Notification notification)
        {
            if (!notification.TryGetInfoT(PopupNotificationKeys.DisplayPopup.PopupInfo, out popupInfo))
            {
                return;
            }

            closeNotifier[PopupNotificationKeys.PopupClosed.ID] = popupInfo.id;

            gameObject.SetActive(!popupInfo.hideCloseButton);
        }

        void CloseCurrentOpenedPopup(Notification notification)
        {
            notification.TryGetInfoNullableT(PopupNotificationKeys.CloseCurrentOpenedPopup.ActionIndex, out int? index, false);
            if (index.HasValue)
            {
                popupInfo.buttonActions?.Invoke(index.Value);
            }
            closeNotifier.Notify();
        }

        void Click()
        {
            popupInfo.buttonActions?.Invoke(-1);

            closeNotifier.Notify();
        }
    }
}