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
using umi3d.browserRuntime.notificationKeys;
using UnityEngine;

namespace umi3d.browserRuntime.ui.popup
{
    internal class PopupManager : MonoBehaviour
    {
        [SerializeField] GameObject popupPrefab;

        GameObject popup;
        List<PopupInfo> popupsInfo = new();
        PopupInfo? currentPopupInfo;

        Notifier displayPopupNotifier;
        Notifier closeCurrentPopupNotifier;

        void Awake()
        {
            NotificationHub.Default
                .Subscribe<PopupNotificationKeys.EnqueuePopup>(
                this,
                new FilterByRef(FilterType.AcceptAllExcept, this),
                NewPopupEnqueued
            );

            NotificationHub.Default
                .Subscribe<PopupNotificationKeys.DequeuePopup>(this, DequeuePopup);

            NotificationHub.Default
                .Subscribe<PopupNotificationKeys.PopupClosed>(this, PopupClosed);

            NotificationHub.Default
                .Subscribe<PopupNotificationKeys.ReplaceCurrentOpenedPopup>(this, ReplaceCurrentOpenedPopup);

            popup = Instantiate(popupPrefab);
            popup.transform.SetParent(transform, false);
            popup.SetActive(false);

            displayPopupNotifier = NotificationHub.Default
                .GetNotifier<PopupNotificationKeys.DisplayPopup>(this);

            closeCurrentPopupNotifier = NotificationHub.Default
                .GetNotifier<PopupNotificationKeys.CloseCurrentOpenedPopup>(this);
        }

        void OnDestroy()
        {
            NotificationHub.Default
             .Unsubscribe<PopupNotificationKeys.EnqueuePopup>(this);

            NotificationHub.Default
             .Unsubscribe<PopupNotificationKeys.DequeuePopup>(this);

            NotificationHub.Default
            .Unsubscribe<PopupNotificationKeys.PopupClosed>(this);

            NotificationHub.Default
             .Unsubscribe<PopupNotificationKeys.ReplaceCurrentOpenedPopup>(this);
        }

        void NewPopupEnqueued(Notification notification)
        {
            if (!notification.TryGetInfoT(PopupNotificationKeys.EnqueuePopup.PopupInfo, out PopupInfo popupInfo))
            {
                return;
            }

            if (!popupInfo.id.HasValue)
            {
                // This popup has no identification so add to the queue.

                popupsInfo.Add(popupInfo);

                if (!popup.activeInHierarchy)
                {
                    DisplayNextPopup();
                }
                return;
            }
            
            //if (currentId.HasValue && currentId.Value == id.Value)
            //{
            //    NotificationHub.Default.Notify<PopupNotificationKeys.EnqueuePopup>(this, notification.Info);
            //    return;
            //}

            // Check if a popup with the same id has already be enqueued.
            int index = popupsInfo.FindIndex(info =>
            {
                return info.id.HasValue && info.id.Value == popupInfo.id.Value;
            });
            if (index >= 0)
            {
                // If a popup with the same id has already be enqueued.
                popupsInfo[index] = popupInfo;
            }
            else
            {
                // Else add to the queue.
                popupsInfo.Add(popupInfo);
            }

            if (!popup.activeInHierarchy)
            {
                DisplayNextPopup();
            }
        }

        void DequeuePopup(Notification notification)
        {
            if (!notification.TryGetInfoT(PopupNotificationKeys.DequeuePopup.ID, out System.Guid id))
            {
                return;
            }

            if (currentPopupInfo.HasValue && currentPopupInfo.Value.id.HasValue && currentPopupInfo.Value.id.Value == id)
            {
                notification.TryGetInfoNullableT(PopupNotificationKeys.DequeuePopup.ActionIndex, out int? index, false);
                closeCurrentPopupNotifier[PopupNotificationKeys.CloseCurrentOpenedPopup.ActionIndex] = index;
                closeCurrentPopupNotifier.Notify();
                return;
            }

            int idx = popupsInfo.FindIndex(info => info.id == id);
            if (idx < 0)
            {
                UnityEngine.Debug.LogError($"[Popup] Try to dequeue a popup that was not enqueue.");
                return;
            }
            popupsInfo.RemoveAt(idx);
        }

        void PopupClosed()
        {
            popup.SetActive(false);
            currentPopupInfo = null;
            DisplayNextPopup();
        }

        void ReplaceCurrentOpenedPopup(Notification notification)
        {
            //if (!notification.TryGetInfoT(PopupNotificationKeys.ReplaceCurrentOpenedPopup.ID, out System.Guid id))
            //{
            //    return;
            //}

            //int idx = popupInfo.FindIndex(info => info.Item1 == id);
            //if (idx < 0)
            //{
            //    UnityEngine.Debug.LogError($"[Popup] Try to replace current popup by a popup that was not enqueue.");
            //    return;
            //}

            //notification.TryGetInfoNullableT(PopupNotificationKeys.ReplaceCurrentOpenedPopup.ActionIndex, out int? index, false);
            //if (index.HasValue)
            //{
            //    currentAction?.Invoke(index.Value);
            //}
            //DisplayPopup(idx);
        }

        void DisplayNextPopup()
        {
            if (popupsInfo.Count == 0)
            {
                NotificationHub.Default.Notify<PopupNotificationKeys.AllPopupAreClosed>(this);
                return;
            }

            int index = -1;
            for (int i = 0; i < popupsInfo.Count; i++)
            {
                PopupInfo popupInfo = popupsInfo[i];

                if (popupInfo.type == PopupType.Error)
                {
                    index = i;
                    break;
                }

                if (popupInfo.type == PopupType.Warning && index < 0)
                {
                    index = i;
                }
            }

            if (index < 0)
            {
                index = 0;
            }

            DisplayPopup(index);
        }

        void DisplayPopup(int index)
        {
            currentPopupInfo = popupsInfo[index];

            popupsInfo.RemoveAt(index);

            displayPopupNotifier[PopupNotificationKeys.DisplayPopup.PopupInfo] = currentPopupInfo.Value;
            displayPopupNotifier.Notify();

            popup.SetActive(true);
        }
    }
}