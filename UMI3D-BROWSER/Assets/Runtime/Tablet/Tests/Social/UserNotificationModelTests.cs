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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using umi3d.browserRuntime.ui.tablet.userNotification;
using umi3d.common;
using UnityEngine;
using UnityEngine.TestTools;

public class UserNotificationModelTests
{
    public class SetDtoTests
    {
        private UserNotificationModel _model;
        private NotificationDto _validDto;
        private NotificationDto _dtoWithNullCallback;

        [SetUp]
        public void SetUp()
        {
            _model = new UserNotificationModel();
            _validDto = new NotificationDto {
                title = "Test Notification",
                callback = new string[] { "Button1", "Button2" }
            };
            _dtoWithNullCallback = new NotificationDto {
                title = "Test Notification",
                callback = null
            };
        }

        // Test case for a valid DTO with callbacks
        [Test]
        public void GivenValidDtoWithCallbacks_WhenSetDtoIsCalled_ThenDescriptionIsSetAndButtonsAreCreated()
        {
            // Given: A valid NotificationDto with callbacks
            // (Setup already provides this)

            // When: SetDto is called with the valid DTO
            _model.SetDto(_validDto);

            // Then: The description is set and buttons are created
            Assert.AreEqual(_validDto.title, _model.Description);
            Assert.AreEqual(_validDto.callback.Length, _model.Buttons.Count);
            Assert.IsTrue(_model.Buttons.All(button => _validDto.callback.Contains(button.Item1)));
        }

        // Test case for a DTO with null callbacks
        [Test]
        public void GivenDtoWithNullCallbacks_WhenSetDtoIsCalled_ThenDescriptionIsSetAndNoButtonsAreCreated()
        {
            // Given: A NotificationDto with null callbacks
            // (Setup already provides this)

            // When: SetDto is called with the DTO having null callbacks
            _model.SetDto(_dtoWithNullCallback);

            // Then: The description is set and no buttons are created
            Assert.AreEqual(_dtoWithNullCallback.title, _model.Description);
            Assert.IsEmpty(_model.Buttons);
        }

        // Test case for a DTO with an empty callback array
        [Test]
        public void GivenDtoWithEmptyCallbacks_WhenSetDtoIsCalled_ThenDescriptionIsSetAndNoButtonsAreCreated()
        {
            // Given: A NotificationDto with an empty callback array
            var dtoWithEmptyCallbacks = new NotificationDto {
                title = "Test Notification",
                callback = Array.Empty<string>()
            };

            // When: SetDto is called with the DTO having an empty callback array
            _model.SetDto(dtoWithEmptyCallbacks);

            // Then: The description is set and no buttons are created
            Assert.AreEqual(dtoWithEmptyCallbacks.title, _model.Description);
            Assert.IsEmpty(_model.Buttons);
        }
    }

    public class SeenTests
    {
        private UserNotificationModel _model;

        [SetUp]
        public void SetUp()
        {
            _model = new UserNotificationModel();
        }

        // Test case for marking a notification as seen
        [Test]
        public void GivenUnseenNotification_WhenSeenIsCalled_ThenIsSeenIsTrue()
        {
            // Given: An unseen notification (IsSeen is initially false)
            Assert.IsFalse(_model.IsSeen, "Notification should initially be unseen.");

            // When: Seen method is called
            _model.Seen();

            // Then: The IsSeen property is set to true
            Assert.IsTrue(_model.IsSeen, "Notification should be marked as seen after calling Seen.");
        }

        // Test case for calling Seen method multiple times
        [Test]
        public void GivenSeenNotification_WhenSeenIsCalledAgain_ThenIsSeenRemainsTrue()
        {
            // Given: A notification that has already been marked as seen
            _model.Seen();
            Assert.IsTrue(_model.IsSeen, "Notification should be marked as seen.");

            // When: Seen method is called again
            _model.Seen();

            // Then: The IsSeen property remains true
            Assert.IsTrue(_model.IsSeen, "Notification should remain seen after calling Seen again.");
        }
    }
}