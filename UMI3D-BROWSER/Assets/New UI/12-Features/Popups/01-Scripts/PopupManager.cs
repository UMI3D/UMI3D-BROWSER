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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace umi3d.browserRuntime.ui.popup
{
    public class PopupManager : MonoBehaviour
    {
        [SerializeField] GameObject popupPrefab;

        GameObject popup;
        List<Notification> popupInfo = new();

        void Awake()
        {
            NotificationHub.Default
                .Subscribe<PopupNotificationKeys.Show>(
                this,
                new FilterByRef(FilterType.AcceptAllExcept, this),
                NewPopupEnqueued
            );

            NotificationHub.Default
                .Subscribe<PopupNotificationKeys.PopupClosed>(this, PopupClosed);

            popup = Instantiate(popupPrefab);
            popup.transform.SetParent(transform, false);
            popup.SetActive(false);
        }

        void OnDestroy()
        {
            NotificationHub.Default
             .Unsubscribe<PopupNotificationKeys.Show>(this);

            NotificationHub.Default
            .Unsubscribe<PopupNotificationKeys.PopupClosed>(this);
        }

        void NewPopupEnqueued(Notification notification)
        {
            popupInfo.Add(notification);

            if (!popup.activeInHierarchy)
            {
                DisplayNextPopup();
            }
        }

        void PopupClosed()
        {
            popup.SetActive(false);
            DisplayNextPopup();
        }

        void DisplayNextPopup()
        {
            if (popupInfo.Count == 0)
            {
                return;
            }

            Notification notif = null;
            for (int i = 0; i < popupInfo.Count; i++)
            {
                Notification _notif = popupInfo[i];
                if (!_notif.TryGetInfoT(PopupNotificationKeys.Show.Type, out PopupType type))
                {
                    continue;
                }

                if (type == PopupType.Error)
                {
                    notif = _notif;
                    break;
                }

                if (type == PopupType.Warning && notif == null)
                {
                    notif = _notif;
                }
            }

            if (notif == null)
            {
                notif = popupInfo[0];
            }
            popupInfo.Remove(notif);

            NotificationHub.Default.Notify<PopupNotificationKeys.Show>(this, notif.Info);

            popup.SetActive(true);
        }

    }
}