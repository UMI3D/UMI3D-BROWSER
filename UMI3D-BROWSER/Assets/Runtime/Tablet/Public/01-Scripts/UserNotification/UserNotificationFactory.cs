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

        private void CreateNotification(NotificationDto dto)
        {
            var modelContainer = Instantiate(_userNotifiactionPrefab, _content);
            modelContainer.Model.SetDto(dto);

            _listModelContainer.Model.UserNotifications.Add(modelContainer);
        }
    }
}