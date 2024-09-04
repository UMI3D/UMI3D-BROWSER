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

using System.Collections.Generic;
using umi3d.baseBrowser.notification;
using umi3d.common;
using UnityEngine;

namespace umi3dBrowsers.displayer
{
    public class NotificationScreen : MonoBehaviour
    {
        [SerializeField] private Transform content;
        [SerializeField] private GameObject notificationPrefab;
        [SerializeField] private NotificationLoader notificationLoader;

        private List<NotificationElement> notificationElements = new List<NotificationElement>();

        private void Awake()
        {
            foreach (Transform child in content)
                Destroy(child);

            notificationLoader.Notification2DReceived += AddNotification;
        }

        private void OnDestroy()
        {
            notificationLoader.Notification2DReceived -= AddNotification;
        }

        private void OnDisable()
        {
            foreach (var element in notificationElements)
                if (!element.IsRead)
                    element.IsRead = true;
        }

        public void AddNotification(NotificationDto notificationDto)
        {
            var notificationGameObject = Instantiate(notificationPrefab, content);
            var notification = notificationGameObject.GetComponent<NotificationElement>();
            notification.Init(notificationDto);
        }
    }
}