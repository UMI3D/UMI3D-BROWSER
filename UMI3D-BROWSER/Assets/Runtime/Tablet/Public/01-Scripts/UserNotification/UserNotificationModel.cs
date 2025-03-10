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
using System;
using System.Collections.Generic;
using umi3d.cdk;
using umi3d.common;
using umi3d.common.interaction;

namespace umi3d.browserRuntime.ui.tablet.userNotification
{
    public class UserNotificationModel 
    {
        public string Description { get; private set; } = "";
        public bool IsSeen { get; private set; } = false;
        public List<Tuple<string, Action>> Buttons { get; private set; } = new List<Tuple<string, Action>>();

        Notifier _setNotifier;
        Notifier _updateNotifier;

        public UserNotificationModel()
        {
            _setNotifier = NotificationHub.Default.GetNotifier(this, ID.FromType<UserNotificationNotificationKeys.UserNotificationSet>());
            _setNotifier[UserNotificationNotificationKeys.UserNotificationSet.Description] = Description;
            _setNotifier[UserNotificationNotificationKeys.UserNotificationSet.IsSeen] = IsSeen;
            _setNotifier[UserNotificationNotificationKeys.UserNotificationSet.Buttons] = Buttons;

            _updateNotifier = NotificationHub.Default.GetNotifier(this, ID.FromType<UserNotificationNotificationKeys.UserNotificationUpdate>());
        }

        /// <summary>
        /// Sets the notification model's description and buttons based on the provided NotificationDto.<br/>
        /// <br/>
        /// <example>
        /// Given a NotificationDto with a title and callback array when SetDto is called then the model's description is set and buttons are created.
        /// <code>
        /// userNotificationModel.SetDto(notificationDto);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="notificationDto">The data transfer object containing notification details.</param>
        public void SetDto(NotificationDto notificationDto)
        {
            Description = notificationDto.title;
            _setNotifier[UserNotificationNotificationKeys.UserNotificationSet.Description] = Description;

            if (notificationDto.callback != null)
            {
                for (int i = 0; i < notificationDto.callback.Length; i++)
                {
                    Buttons.Add(new(notificationDto.callback[i], () => {
                        var callbackDto = new NotificationCallbackDto() {
                            id = notificationDto.id,
                            callback = i == 0
                        };
                        UMI3DClientServer.SendRequest(callbackDto, true);
                    }));
                }
                _setNotifier[UserNotificationNotificationKeys.UserNotificationSet.Buttons] = Buttons;
            }

            _setNotifier.Notify();
        }

        /// <summary>
        /// Marks the notification as seen and notifies observers of the change.<br/>
        /// <br/>
        /// <example>
        /// Given an unseen notification when Seen is called then the notification is marked as seen.
        /// <code>
        /// model.Seen();
        /// </code>
        /// </example>
        /// </summary>
        public void Seen()
        {
            IsSeen = true;
            _updateNotifier[UserNotificationNotificationKeys.UserNotificationUpdate.IsSeen] = IsSeen;
            _updateNotifier.Notify();
        }
    }
}