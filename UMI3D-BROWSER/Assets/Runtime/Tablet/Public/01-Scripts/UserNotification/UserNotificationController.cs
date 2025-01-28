/*
Copyright 2019 - 2025 Inetum

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
using umi3d.browserRuntime.ui.userNotification;
using umi3d.common;
using umi3dBrowsers.displayer;
using UnityEngine;

namespace umi3d.browserRuntime.ui.tablet.userNotification
{
    public class UserNotificationController : MonoBehaviour
    {
        [SerializeField] private Transform content;
        [SerializeField] private GameObject notificationPrefab;
        [SerializeField] private UserNotificationLoader notificationLoader;

        private void Awake()
        {
            notificationLoader.Notification2DReceived += AddNotification;
        }

        private void OnDestroy()
        {
            notificationLoader.Notification2DReceived -= AddNotification;
        }

        public void AddNotification(NotificationDto notificationDto)
        {
            var modelContainer = Instantiate(notificationPrefab, content).GetComponent<UserNotificationModelContainer>();
            modelContainer.Model.SetDto(notificationDto);
            NotificationHub.Default.Notify(this, ID.FromType<UserNotificationNotificationKeys.UserNotificationReceived>());
        }

#if UNITY_EDITOR
        [ContextMenu("Add Notification")]
        private void AddNotificationDebug()
        {
            NotificationDto notification = new NotificationDto();
            notification.content = "Test content";
            notification.callback = new[] { "Yes", "No" };
            AddNotification(notification);
        }
#endif
    }
}