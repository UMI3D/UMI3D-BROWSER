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

using System;
using System.Collections.Generic;
using umi3d.common;
using umi3dBrowsers.displayer;

namespace umi3d.browserRuntime.ui.tablet.userNotification
{
    public class UserNotificationListModel
    {
        public List<UserNotificationModelContainer> UserNotifications { get; private set; } = new();

        public Action<NotificationDto> AddNotification;
        
        UserNotificationLoader _notificationLoader;

        public UserNotificationListModel(UserNotificationLoader notificationLoader)
        {
            _notificationLoader = notificationLoader;
            _notificationLoader.Notification2DReceived += AddNotification;
        }

        ~UserNotificationListModel()
        {
            _notificationLoader.Notification2DReceived -= AddNotification;
        }
    }
}