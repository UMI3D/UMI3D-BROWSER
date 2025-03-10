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
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using umi3d.browserRuntime.ui.tablet.userNotification;
using umi3d.common;
using UnityEngine;
using UnityEngine.TestTools;

public class UserNotificationFactoryTests
{
    public class CreateNotificationTests
    {
        private GameObject _factoryGameObject;
        private UserNotificationFactory _factory;
        private Transform _content;
        private UserNotificationModelContainer _userNotificationPrefab;
        private UserNotificationListModelContainer _listModelContainer;

        [SetUp]
        public void SetUp()
        {
            // Create a new GameObject to hold the UserNotificationFactory component
            _factoryGameObject = new GameObject("UserNotificationFactory");
            _factory = _factoryGameObject.AddComponent<UserNotificationFactory>();

            // Set up the content transform
            var contentGameObject = new GameObject("Content");
            _content = contentGameObject.transform;
            _factory.GetType().GetField("_content", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(_factory, _content);


            // Set up the prefab
            var prefabGameObject = new GameObject("UserNotificationPrefab");
            _userNotificationPrefab = prefabGameObject.AddComponent<UserNotificationModelContainer>();
            _factory.GetType().GetField("_userNotifiactionPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(_factory, _userNotificationPrefab);


            // Set up the list model container
            var listModelContainerGameObject = new GameObject("ListModelContainer");
            _listModelContainer = listModelContainerGameObject.AddComponent<UserNotificationListModelContainer>();
            _factory.GetType().GetField("_listModelContainer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(_factory, _listModelContainer);

        }

        [UnityTest]
        public IEnumerator GivenNotificationDto_WhenCreateNotificationIsCalled_ThenNotificationIsCreatedAndAddedToList()
        {
            // Given: A NotificationDto
            var dto = new NotificationDto {
                title = "Test Notification",
                callback = new string[] { "Button1", "Button2" }
            };

            // When: CreateNotification is called
            _factory.CreateNotification(dto);
            yield return null; // Wait one frame for Unity to complete instantiation

            // Then: A notification is created and added to the list
            Assert.AreEqual(1, _content.childCount, "A notification prefab should be instantiated as a child of the content.");
            Assert.AreEqual(1, _listModelContainer.Model.UserNotifications.Count, "The notification should be added to the list model container.");
            Assert.AreEqual(dto.title, _listModelContainer.Model.UserNotifications[0].Model.Description, "The notification's description should be set from the DTO.");
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up by destroying the objects created for the test
            if (_factoryGameObject != null)
                GameObject.DestroyImmediate(_factoryGameObject);
            if (_content.gameObject != null)
                GameObject.DestroyImmediate(_content.gameObject);
            if (_userNotificationPrefab.gameObject != null)
                GameObject.DestroyImmediate(_userNotificationPrefab.gameObject);
            if (_listModelContainer.gameObject != null)
                GameObject.DestroyImmediate(_listModelContainer.gameObject);
        }
    }
}