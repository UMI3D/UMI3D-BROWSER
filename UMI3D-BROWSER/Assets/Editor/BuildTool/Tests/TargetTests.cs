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
using NUnit.Framework;
using umi3d.browserEditor.BuildTool;
using UnityEditor;

public class TargetTests
{
    public class GetBuildTargetTest
    {
        [Test]
        public void GivenQuestTarget_WhenGetBuildTarget_ThenReturnsAndroid()
        {
            // Given
            E_Target target = E_Target.Quest;

            // When
            BuildTarget result = target.GetBuildTarget();

            // Then
            Assert.AreEqual(BuildTarget.Android, result);
        }

        [Test]
        public void GivenFocusTarget_WhenGetBuildTarget_ThenReturnsAndroid()
        {
            // Given
            E_Target target = E_Target.Focus;

            // When
            BuildTarget result = target.GetBuildTarget();

            // Then
            Assert.AreEqual(BuildTarget.Android, result);
        }

        [Test]
        public void GivenPicoTarget_WhenGetBuildTarget_ThenReturnsAndroid()
        {
            // Given
            E_Target target = E_Target.Pico;

            // When
            BuildTarget result = target.GetBuildTarget();

            // Then
            Assert.AreEqual(BuildTarget.Android, result);
        }

        [Test]
        public void GivenSteamVRTarget_WhenGetBuildTarget_ThenReturnsStandaloneWindows64()
        {
            // Given
            E_Target target = E_Target.SteamVR;

            // When
            BuildTarget result = target.GetBuildTarget();

            // Then
            Assert.AreEqual(BuildTarget.StandaloneWindows64, result);
        }

        [Test]
        public void GivenWindowsTarget_WhenGetBuildTarget_ThenReturnsStandaloneWindows64()
        {
            // Given
            E_Target target = E_Target.Windows;

            // When
            BuildTarget result = target.GetBuildTarget();

            // Then
            Assert.AreEqual(BuildTarget.StandaloneWindows64, result);
        }
    }
}