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

public class UserSocialModelTests
{
    public class SetUserTests
    {
        private UserSocialModel _userSocial;
        private UMI3DUser _umi3dUser;
        private UserDto _userDto;

        [SetUp]
        public void SetUp()
        {
            _userSocial = new UserSocialModel();

            _userDto = new() { id = 0, login = $"Test User" };
            _userDto.userActions = new List<UserActionDto>() {
                new UserAction(0, new UserActionDto() { isPrimary = true }),
                new UserAction(0, new UserActionDto() { isPrimary = false})
            };

            _umi3dUser = new UMI3DUser(0, _userDto);
        }

        [Test]
        public void GivenValidUser_WhenSetUser_ThenUserPropertiesAreSetCorrectly()
        {
            // Given
            var expectedName = "Test User";

            // When
            _userSocial.SetUser(_umi3dUser);

            // Then
            Assert.AreEqual(expectedName, _userSocial.Name);
            //Assert.AreEqual(expectedPlace, _userSocial.Place); Can't test place name : Network
            Assert.AreEqual(100, _userSocial.Volume);
            Assert.IsFalse(_userSocial.IsMute);
            Assert.AreEqual(1, _userSocial.PrimaryActions.Count);
            Assert.AreEqual(1, _userSocial.OtherActions.Count);
        }

        [Test]
        public void GivenUserWithMoreThanMaxPrimaryActions_WhenSetUser_ThenExcessPrimaryActionsAreAddedToOtherActions()
        {
            // Given
            _umi3dUser.userActions.Clear();
            for (int i = 0; i < UserSocialModel.k_maxPrimaryAction + 2; i++)
            {
                _umi3dUser.userActions.Add(new UserAction(0, new UserActionDto() { isPrimary = true }));
            }

            // When
            _userSocial.SetUser(_umi3dUser);

            // Then
            Assert.AreEqual(UserSocialModel.k_maxPrimaryAction, _userSocial.PrimaryActions.Count);
            Assert.AreEqual(2, _userSocial.OtherActions.Count);
        }

        [Test]
        public void GivenNullUser_WhenSetUser_ThenLogError()
        {
            // Given
            UMI3DUser nullUser = null;

            // When
            LogAssert.Expect(LogType.Error, "User cannot be null");
            _userSocial.SetUser(nullUser);

            // Then
            // No further assertions needed as LogAssert.Expect will fail the test if the log is not found
        }

        [Test]
        public void GivenUserWithNullLogin_WhenSetUser_ThenLogError()
        {
            // Given
            _userDto.login = null;

            // When
            LogAssert.Expect(LogType.Error, "User login cannot be null");
            _userSocial.SetUser(_umi3dUser);

            // Then
            // No further assertions needed as LogAssert.Expect will fail the test if the log is not found
        }
    }

    public class UpdateVolumeTests
    {
        private UserSocialModel _userSocial;
        private UMI3DUser _umi3dUser;
        private UserDto _userDto;

        [SetUp]
        public void SetUp()
        {
            _userDto = new() { id = 0, login = $"Test User" };

            _umi3dUser = new UMI3DUser(0, _userDto);

            _userSocial = new UserSocialModel();
        }

        [Test]
        public void GivenValidVolume_WhenUpdateVolume_ThenVolumeIsUpdatedCorrectly()
        {
            // Given
            _userSocial.SetUser(_umi3dUser);
            float newVolume = 75f;

            // When
            _userSocial.UpdateVolume(newVolume);

            // Then
            Assert.AreEqual(newVolume, _userSocial.Volume);
            Assert.IsFalse(_userSocial.IsMute);
            // Assuming UpdateAudioManagerFor and _updateNotifier.Notify() have their own tests or mocks
        }

        [Test]
        public void GivenZeroVolume_WhenUpdateVolume_ThenUserIsMuted()
        {
            // Given
            _userSocial.SetUser(_umi3dUser);
            float newVolume = 0f;

            // When
            _userSocial.UpdateVolume(newVolume);

            // Then
            Assert.AreEqual(newVolume, _userSocial.Volume);
            Assert.IsTrue(_userSocial.IsMute);
            // Assuming UpdateAudioManagerFor and _updateNotifier.Notify() have their own tests or mocks
        }

        [Test]
        public void GivenNegativeVolume_WhenUpdateVolume_ThenVolumeEqualZeroAndIsMuted()
        {
            // Given
            _userSocial.SetUser(_umi3dUser);
            float newVolume = -10f;

            // When
            _userSocial.UpdateVolume(newVolume);

            // Then
            Assert.AreEqual(0, _userSocial.Volume);
            Assert.IsTrue(_userSocial.IsMute);
        }

        [Test]
        public void GivenUserNull_WhenUpdateMute_ThenLogErrorIfUserIsNull()
        {
            // Given

            // When
            _userSocial.UpdateVolume(50.0f);
            LogAssert.Expect(LogType.Error, "User cannot be null when updating volume.");

            // Then
            // No further assertions needed as LogAssert.Expect will fail the test if the log is not found
        }
    }

    public class UpdateMuteTests
    {
        private UserSocialModel _userSocial;
        private UMI3DUser _umi3dUser;
        private UserDto _userDto;

        [SetUp]
        public void SetUp()
        {
            _userDto = new() { id = 0, login = $"Test User" };

            _umi3dUser = new UMI3DUser(0, _userDto);

            _userSocial = new UserSocialModel();
        }

        [Test]
        public void GivenMuteTrue_WhenUpdateMute_ThenUserIsMuted()
        {
            // Given
            _userSocial.SetUser(_umi3dUser);
            bool mute = true;

            // When
            _userSocial.UpdateMute(mute);

            // Then
            Assert.IsTrue(_userSocial.IsMute);
            Assert.AreEqual(0, _userSocial.Volume);
            // Assuming UpdateAudioManagerFor and _updateNotifier.Notify() have their own tests or mocks
        }

        [Test]
        public void GivenMuteFalse_WhenUpdateMute_ThenUserIsUnmuted()
        {
            // Given
            _userSocial.SetUser(_umi3dUser);
            bool mute = false;
            _userSocial.UpdateVolume(50f); // Set a non-zero volume

            // When
            _userSocial.UpdateMute(mute);

            // Then
            Assert.IsFalse(_userSocial.IsMute);
            Assert.AreEqual(50f, _userSocial.Volume);
            // Assuming UpdateAudioManagerFor and _updateNotifier.Notify() have their own tests or mocks
        }

        [Test]
        public void GivenUserNull_WhenUpdateMute_ThenLogErrorIfUserIsNull()
        {
            // Given

            // When
            _userSocial.UpdateMute(true);
            LogAssert.Expect(LogType.Error, "User cannot be null when updating mute.");

            // Then
            // No further assertions needed as LogAssert.Expect will fail the test if the log is not found
        }
    }
}
