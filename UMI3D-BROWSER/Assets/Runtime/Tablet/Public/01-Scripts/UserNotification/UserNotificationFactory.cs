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

using umi3d.cdk.collaboration;
using umi3d.common;
using UnityEngine;

namespace umi3d.browserRuntime.ui.tablet.userNotification
{
    [RequireComponent(typeof(UserNotificationListModelContainer))]
    public class UserNotificationFactory : MonoBehaviour
    {
        [SerializeField] Transform _content;
        [SerializeField] UserNotificationModelContainer _userNotifiactionPrefab;

        UserNotificationListModelContainer _listModelContainer;

        private void Awake()
        {
            _listModelContainer = GetComponent<UserNotificationListModelContainer>();

            _listModelContainer.Model.AddNotification += CreateNotification;
        }

        private void OnDestroy()
        {
            _listModelContainer.Model.AddNotification -= CreateNotification;
        }

        /// <summary>
        /// Instantiates a notification model container, sets its data based on the provided NotificationDto, and adds it to the notification list.<br/>
        /// <br/>
        /// <example>
        /// Given a NotificationDto when CreateNotification is called then a notification is instantiated and added to the list.
        /// <code>
        /// userNotificationFactory.CreateNotification(dto);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="dto">The data transfer object containing notification details.</param>
        internal void CreateNotification(NotificationDto dto)
        {
            var modelContainer = Instantiate(_userNotifiactionPrefab, _content);
            modelContainer.Model.SetDto(dto);

            _listModelContainer.Model.UserNotifications.Add(modelContainer);
        }

#if UNITY_EDITOR
        [ContextMenu("Add Test User")]
        void AddTestUser()
        {
            CreateNotification(new NotificationDto() {
                title = "Test",
                callback = new string[] { "test" }
            });
        }
#endif
    }
}