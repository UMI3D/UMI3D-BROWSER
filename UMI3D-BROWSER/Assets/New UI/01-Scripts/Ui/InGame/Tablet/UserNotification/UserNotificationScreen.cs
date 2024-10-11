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
using umi3d.common;
using umi3dBrowsers.displayer;
using UnityEngine;

namespace umi3d.browserRuntime.ui.inGame.tablet.userNotification
{
    public class UserNotificationScreen : MonoBehaviour
    {
        [SerializeField] private Transform content;
        [SerializeField] private GameObject notificationPrefab;
        [SerializeField] private UserNotificationLoader notificationLoader;

        private List<UserNotificationElement> _notificationElements = new List<UserNotificationElement>();

        private void Awake()
        {
            notificationLoader.Notification2DReceived += AddNotification;
            NotificationHub.Default.Subscribe(this, TabletNotificationKeys.OpenUserNotification, Open);
            NotificationHub.Default.Subscribe(this, TabletNotificationKeys.CloseScreens, Close);
        }

        private void OnDestroy()
        {
            notificationLoader.Notification2DReceived -= AddNotification;
            NotificationHub.Default.Unsubscribe(this);
        }

        public void AddNotification(NotificationDto notificationDto)
        {
            var notificationGameObject = Instantiate(notificationPrefab, content);
            var notification = notificationGameObject.GetComponent<UserNotificationElement>();
            notification.Init(notificationDto);
            _notificationElements.Add(notification);
            NotificationHub.Default.Notify(this, TabletNotificationKeys.UserNotificationReceived);
        }

        public void Open()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

#if UNITY_EDITOR
        [ContextMenu("Add Notification")]
        private void AddNotificationDebug()
        {
            NotificationDto notification = new NotificationDto();
            notification.content = "Test content";
            notification.callback = new [] { "Yes", "No" };
            AddNotification(notification);
        }
#endif
    }
}