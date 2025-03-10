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
using umi3d.browserRuntime.ui.tablet.social;
using umi3d.cdk.collaboration;
using UnityEngine;
using UnityEngine.TestTools;

public class UserSocialFactoryTests
{
    public class CreateUserTests
    {
        private UserSocialFactory _factory;
        private UserSocialModelContainer _userSocialPrefab;
        private Transform _content;
        private UserSocialListModelContainer _listModelContainer;

        [SetUp]
        public void SetUp()
        {
            _factory = new GameObject().AddComponent<UserSocialFactory>();

            _userSocialPrefab = new GameObject().AddComponent<UserSocialModelContainer>();
            _content = new GameObject().transform;
            _listModelContainer = new GameObject().AddComponent<UserSocialListModelContainer>();

            _factory.GetType().GetField("_content", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(_factory, _content);
            _factory.GetType().GetField("_userSocialPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(_factory, _userSocialPrefab);
            _factory.GetType().GetField("_listModelContainer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(_factory, _listModelContainer);
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(_factory.gameObject);
            GameObject.DestroyImmediate(_userSocialPrefab);
            GameObject.DestroyImmediate(_content.gameObject);
            GameObject.DestroyImmediate(_listModelContainer.gameObject);
        }

        [Test]
        public void GivenAvailablePrefab_WhenCreateUser_ThenUserIsAddedToModelContainer()
        {
            // Given
            var user = new UMI3DUser(0, new umi3d.common.collaboration.dto.signaling.UserDto() { login = "Test"});
            var modelContainer = new GameObject().AddComponent<UserSocialModelContainer>();
            _factory.GetType().GetField("_availablePrefabs", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(_factory, new Queue<UserSocialModelContainer>(new List<UserSocialModelContainer>() { modelContainer }));

            // When
            _factory.CreateUser(user);

            // Then
            Assert.IsTrue(modelContainer.gameObject.activeSelf);
            Assert.AreEqual(user, modelContainer.Model.User);
            Assert.IsTrue(_listModelContainer.Model.Users.ContainsKey(user));
            Assert.AreEqual(modelContainer, _listModelContainer.Model.Users[user]);
        }

        [Test]
        public void GivenNoAvailablePrefab_WhenCreateUser_ThenNewPrefabIsInstantiatedAndUserIsAddedToModelContainer()
        {
            // Given
            var user = new UMI3DUser(0, new umi3d.common.collaboration.dto.signaling.UserDto() { login = "Test" });

            // When
            _factory.CreateUser(user);

            // Then
            var instantiatedModelContainer = _content.GetComponentInChildren<UserSocialModelContainer>();
            Assert.IsNotNull(instantiatedModelContainer);
            Assert.IsTrue(instantiatedModelContainer.gameObject.activeSelf);
            Assert.AreEqual(user, instantiatedModelContainer.Model.User);
            Assert.IsTrue(_listModelContainer.Model.Users.ContainsKey(user));
            Assert.AreEqual(instantiatedModelContainer, _listModelContainer.Model.Users[user]);
        }

        [Test]
        public void GivenNullUser_WhenCreateUser_ThenLogError()
        {
            // Given
            UMI3DUser user = null;

            // When
            LogAssert.Expect(LogType.Error, "User cannot be null");
            _factory.CreateUser(user);

            // Then
        }
    }

    public class RemoveUserTests
    {
        private UserSocialFactory _factory;
        private UserSocialModelContainer _userSocialPrefab;
        private Transform _content;
        private UserSocialListModelContainer _listModelContainer;

        [SetUp]
        public void SetUp()
        {
            _factory = new GameObject().AddComponent<UserSocialFactory>();

            _userSocialPrefab = new GameObject().AddComponent<UserSocialModelContainer>();
            _content = new GameObject().transform;
            _listModelContainer = new GameObject().AddComponent<UserSocialListModelContainer>();

            _factory.GetType().GetField("_content", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(_factory, _content);
            _factory.GetType().GetField("_userSocialPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(_factory, _userSocialPrefab);
            _factory.GetType().GetField("_listModelContainer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(_factory, _listModelContainer);
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(_factory.gameObject);
            GameObject.DestroyImmediate(_userSocialPrefab);
            GameObject.DestroyImmediate(_content.gameObject);
            GameObject.DestroyImmediate(_listModelContainer.gameObject);
        }

        [Test]
        public void GivenExistingUser_WhenRemoveUser_ThenUserIsRemovedAndModelContainerIsRecycled()
        {
            // Given
            var user = new UMI3DUser(0, new umi3d.common.collaboration.dto.signaling.UserDto() { login = "Test" });
            var modelContainer = new GameObject().AddComponent<UserSocialModelContainer>();
            _listModelContainer.Model.Users.Add(user, modelContainer);

            // When
            _factory.RemoveUser(user);

            // Then
            Assert.IsFalse(modelContainer.gameObject.activeSelf);
            Assert.IsTrue(_factory.AvailablePrefabsCount == 1);
            Assert.IsTrue(_listModelContainer.Model.Users.Count == 0);
        }

        [Test]
        public void GivenNullUser_WhenRemoveUser_ThenLogError()
        {
            // Given
            UMI3DUser user = null;

            // When
            LogAssert.Expect(LogType.Error, "User cannot be null");
            _factory.RemoveUser(user);

            // Then
        }

        [Test]
        public void GivenNonExistingUser_WhenRemoveUser_ThenLogError()
        {
            // Given
            var user = new UMI3DUser(0, new umi3d.common.collaboration.dto.signaling.UserDto() { login = "Test" });

            // When
            LogAssert.Expect(LogType.Error, "User does not exist in the list");
            _factory.RemoveUser(user);

            // Then
            // No changes should be made to the model container or available prefabs
            Assert.IsTrue(_listModelContainer.Model.Users.Count == 0);
            Assert.IsTrue(_factory.AvailablePrefabsCount == 0);
        }
    }
}