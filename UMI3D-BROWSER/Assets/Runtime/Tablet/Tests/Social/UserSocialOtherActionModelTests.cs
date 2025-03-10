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
using umi3d.common.collaboration.dto.signaling;
using UnityEngine;
using UnityEngine.TestTools;

public class UserSocialOtherActionModelTests
{
    public class SetUserAction
    {
        private UserAction _userAction;
        private UserSocialOtherActionModel _model; // Replace with the actual class name that contains SetUserAction method

        [SetUp]
        public void SetUp()
        {
            _userAction = new UserAction(0, new UserActionDto() {
                name = "TestName",
                description = "TestDescription"
            });

            _model = new UserSocialOtherActionModel(); // Initialize your class
        }

        [Test]
        public void GivenValidUserAction_WhenSetUserAction_ThenPropertiesAreSetCorrectly()
        {
            // Given
            var expectedName = _userAction.name;
            var expectedDescription = _userAction.description;
            var expectedTexture = _userAction.GetTexture().Result;

            // When
            _model.SetUserAction(_userAction);

            // Then
            Assert.AreEqual(expectedName, _model.Name);
            Assert.AreEqual(expectedDescription, _model.Description);
            // Assert.AreEqual(expectedTexture, _yourClass.Texture); Can't test texture : Network
            // Assert.AreEqual(expectedAction, _yourClass.Action); Can't test Action : Function
        }

        [Test]
        public void GivenNullUserAction_WhenSetUserAction_ThenLogError()
        {
            // Given
            UserAction nullUserAction = null;

            // When
            LogAssert.Expect(LogType.Error, "UserAction is null");
            _model.SetUserAction(nullUserAction);

            // Then
            // No further assertions needed as LogAssert.Expect will fail the test if the log is not found
        }
    }
}
