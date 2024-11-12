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
using UnityEngine;

namespace umi3d.browserRuntime.ui.popup
{
    internal class PopupBlockingLayer : MonoBehaviour
    {
        void Awake()
        {
            gameObject.SetActive(false);

            NotificationHub.Default
                 .Subscribe<PopupNotificationKeys.Show>(
                 this,
                 new FilterByCondition(FilterType.AcceptOnly, publisher => publisher is PopupManager),
                 NewPopup
             );

            NotificationHub.Default
                .Subscribe<PopupNotificationKeys.PopupClosed>(this, PopupClosed);
        }

        void OnDestroy()
        {
            NotificationHub.Default
            .Unsubscribe<PopupNotificationKeys.Show>(this);

            NotificationHub.Default
            .Unsubscribe<PopupNotificationKeys.PopupClosed>(this);
        }

        void NewPopup()
        {
            gameObject.SetActive(true);
        }

        void PopupClosed()
        {
            gameObject.SetActive(false);
        }
    }
}