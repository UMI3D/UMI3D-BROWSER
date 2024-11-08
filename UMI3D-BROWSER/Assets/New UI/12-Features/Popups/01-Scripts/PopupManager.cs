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
        List<Dictionary<string, System.Object>> popupInfo = new();

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
            transform.SetParent(popup.transform, false);
            popup.SetActive(false);
        }

        void OnDestroy()
        {
            NotificationHub.Default
             .Unsubscribe<PopupNotificationKeys.Show>(this);
        }

        void NewPopupEnqueued(Notification notification)
        {
            Dictionary<string, System.Object> info = new(notification.Info);
            popupInfo.Add(info);

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

            Dictionary<string, System.Object> info = null;
            for (int i = 0; i < popupInfo.Count; i++)
            {
                Dictionary<string, object> _info = popupInfo[i];
                if (!_info.TryGetValue(PopupNotificationKeys.Show.Type, out System.Object type))
                {
                    continue;
                }

                if (type is not PopupType popupType)
                {
                    continue; 
                }

                if (popupType == PopupType.Error)
                {
                    info = _info;
                    break;
                }

                if (popupType == PopupType.Warning && info == null)
                {
                    info = _info;
                }
            }

            if (info == null)
            {
                info = popupInfo[0];
            }
            popupInfo.Remove(info);

            NotificationHub.Default.Notify<PopupNotificationKeys.Show>(this, info);

            popup.SetActive(true);
        }

    }
}