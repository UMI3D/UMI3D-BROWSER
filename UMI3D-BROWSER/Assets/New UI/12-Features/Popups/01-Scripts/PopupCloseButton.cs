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
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.popup
{
    internal class PopupCloseButton : MonoBehaviour
    {
        Button button;
        Action<int> action;

        Notifier closeNotifier;

        void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(Click);

            NotificationHub.Default
               .Subscribe<PopupNotificationKeys.EnqueuePopup>(
               this,
               new FilterByCondition(FilterType.AcceptOnly, publisher => publisher is PopupManager),
               NewPopup
           );

            closeNotifier = NotificationHub.Default
               .GetNotifier<PopupNotificationKeys.PopupClosed>(this);
        }

        void OnDestroy()
        {
            NotificationHub.Default
             .Unsubscribe<PopupNotificationKeys.EnqueuePopup>(this);
        }

        void NewPopup(Notification notification)
        {
            notification.TryGetInfoT(PopupNotificationKeys.EnqueuePopup.ButtonActions, out action, false);

            notification.TryGetInfoNullableT(PopupNotificationKeys.EnqueuePopup.ID, out System.Guid? id, false);
            closeNotifier[PopupNotificationKeys.PopupClosed.ID] = id;

            if (notification.TryGetInfoT(PopupNotificationKeys.EnqueuePopup.HideCloseButton, out bool hide, false))
            {
                gameObject.SetActive(!hide);
            }
            else
            {
                gameObject.SetActive(true);
            }
        }

        void Click()
        {
            if (action == null)
            {
                UnityEngine.Debug.LogError($"[Popup] Action null.");
            }
            action?.Invoke(-1);

            closeNotifier.Notify();
        }
    }
}