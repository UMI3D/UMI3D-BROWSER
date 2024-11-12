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

        System.Guid? currentId;
        GameObject popup;
        List<(System.Guid?, Notification)> popupInfo = new();

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
            if (!notification.TryGetInfoT(PopupNotificationKeys.Show.ID, out System.Guid id))
            {
                popupInfo.Add((null, notification));

                if (!popup.activeInHierarchy)
                {
                    DisplayNextPopup();
                }
                return;
            }
            

            if (currentId.HasValue && currentId.Value == id)
            {
                NotificationHub.Default.Notify<PopupNotificationKeys.Show>(this, notification.Info);
                return;
            }

            int index = popupInfo.FindIndex(info =>
            {
                return info.Item1.HasValue && info.Item1.Value == id;
            });
            if (index >= 0)
            {
                popupInfo[index] = (id, notification);
            }
            else
            {
                popupInfo.Add((id, notification));
            }

            if (!popup.activeInHierarchy)
            {
                DisplayNextPopup();
            }
        }

        void PopupClosed()
        {
            popup.SetActive(false);
            currentId = null;
            DisplayNextPopup();
        }

        void DisplayNextPopup()
        {
            if (popupInfo.Count == 0)
            {
                return;
            }

            int index = -1;
            for (int i = 0; i < popupInfo.Count; i++)
            {
                Notification _notif = popupInfo[i].Item2;
                if (!_notif.TryGetInfoT(PopupNotificationKeys.Show.Type, out PopupType type))
                {
                    continue;
                }

                if (type == PopupType.Error)
                {
                    index = i;
                    break;
                }

                if (type == PopupType.Warning && index < 0)
                {
                    index = i;
                }
            }

            if (index < 0)
            {
                index = 0;
            }
            Notification notif = popupInfo[index].Item2;
            currentId = popupInfo[index].Item1;
            popupInfo.RemoveAt(index);

            NotificationHub.Default.Notify<PopupNotificationKeys.Show>(this, notif.Info);

            popup.SetActive(true);
        }

    }
}