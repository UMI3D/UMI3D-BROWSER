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
            NotificationHub.Default.Subscribe(
                this,
                ID.FromType<PopupNotificationKeys.EnqueuePopup>(),
                NewPopupEnqueued,
                new FilterByRef(FilterType.AcceptAllExcept, this)
            );

            NotificationHub.Default.Subscribe(
                this,
                ID.FromType<PopupNotificationKeys.DequeuePopup>(),
                DequeuePopup
            );

            NotificationHub.Default.Subscribe(
                this, 
                ID.FromType<PopupNotificationKeys.PopupClosed>(),
                PopupClosed
            );

            NotificationHub.Default.Subscribe(
                this,
                ID.FromType<PopupNotificationKeys.ReplaceCurrentOpenedPopup>(),
                ReplaceCurrentOpenedPopup
            );

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
            NotificationHub.Default.Unsubscribe(this);
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

            if (currentPopupInfo.HasValue && currentPopupInfo.Value.id.HasValue && currentPopupInfo.Value.id.Value == popupInfo.id.Value)
            {
                currentPopupInfo = popupInfo;
                displayPopupNotifier[PopupNotificationKeys.DisplayPopup.PopupInfo] = currentPopupInfo.Value;
                displayPopupNotifier.Notify();
                return;
            }

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
                notification.TryGetInfoT(PopupNotificationKeys.DequeuePopup.ActionIndex, out int? index, false);
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
            if (popupsInfo.Count == 0) 
            {
                // No popup to replace.
                UnityEngine.Debug.LogError($"[Popup] Try to replace current popup but no popup are enqueued.");
                return;
            }

            if (!notification.TryGetInfoT(PopupNotificationKeys.ReplaceCurrentOpenedPopup.ID, out System.Guid id))
            {
                return;
            }

            if (currentPopupInfo.HasValue && currentPopupInfo.Value.id.HasValue && currentPopupInfo.Value.id.Value == id)
            {
                return;
            }

            int idx = popupsInfo.FindIndex(info => info.id == id);
            if (idx < 0)
            {
                UnityEngine.Debug.LogError($"[Popup] Try to replace current popup by a popup that was not enqueue.");
                return;
            }

            notification.TryGetInfoT(PopupNotificationKeys.ReplaceCurrentOpenedPopup.ActionIndex, out int? index, false);
            if (index.HasValue)
            {
                currentPopupInfo.Value.buttonActions?.Invoke(index.Value);
            }
            DisplayPopup(idx);
        }

        void DisplayNextPopup()
        {
            if (popupsInfo.Count == 0)
            {
                NotificationHub.Default.Notify(this, ID.FromType<PopupNotificationKeys.AllPopupAreClosed>());
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