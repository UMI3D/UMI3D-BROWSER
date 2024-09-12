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

using System;
using System.Collections.Generic;
using umi3d.common;
using UnityEngine;

namespace umi3dBrowsers.displayer
{
    public class UserNotificationScreen : MonoBehaviour
    {
        [SerializeField] private Transform content;
        [SerializeField] private GameObject notificationPrefab;
        [SerializeField] private UserNotificationLoader notificationLoader;

        public Action OnNotificationReceived;
        public Action OnNotificationScreenOpened;

        private List<UserNotificationElement> _notificationElements = new List<UserNotificationElement>();

        private void Awake()
        {
            notificationLoader.Notification2DReceived += AddNotification;
        }

        private void OnDestroy()
        {
            notificationLoader.Notification2DReceived -= AddNotification;
        }

        private void OnEnable()
        {
            OnNotificationScreenOpened?.Invoke();
        }

        private void OnDisable()
        {
            foreach (var element in _notificationElements)
                if (!element.IsRead)
                    element.IsRead = true;
        }

        public void AddNotification(NotificationDto notificationDto)
        {
            var notificationGameObject = Instantiate(notificationPrefab, content);
            var notification = notificationGameObject.GetComponent<UserNotificationElement>();
            notification.Init(notificationDto);
            _notificationElements.Add(notification);
            OnNotificationReceived?.Invoke();
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