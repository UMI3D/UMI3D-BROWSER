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
using System;
using umi3d.browserEditor.BuildTool;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class BuildTargetHelperTests
{
    public class InitTest
    {
        [SetUp]
        public void SetUp()
        {
            BuildTargetHelper.hasBeenInitialized = false;
        }

        [Test]
        public void GivenNotInitialized_WhenInitCalled_ThenTargetIsSet()
        {
            // Given
            var target = E_Target.SteamVR;

            // When
            BuildTargetHelper.Init(target);

            // Then
            Assert.AreEqual(target, BuildTargetHelper.@default.target);
            Assert.IsTrue(BuildTargetHelper.hasBeenInitialized);
        }

        [Test]
        public void GivenAlreadyInitialized_WhenInitCalled_ThenLogWarningAndTargetNotChanged()
        {
            // Given
            var initialTarget = E_Target.Quest;
            var newTarget = E_Target.Windows;
            BuildTargetHelper.Init(initialTarget);

            // When
            LogAssert.Expect(LogType.Warning, "[BuildTargetHelper] Warning: has already been initialized.");
            BuildTargetHelper.Init(newTarget);

            // Then
            Assert.AreEqual(initialTarget, BuildTargetHelper.@default.target);
            Assert.IsTrue(BuildTargetHelper.hasBeenInitialized);
        }
    }

    public class defaultTest
    {
        [SetUp]
        public void SetUp()
        {
            BuildTargetHelper.hasBeenInitialized = false;
        }

        [Test]
        public void GivenNotInitialized_WhenAccessingDefault_ThenThrowsExceptionAndLogsError()
        {
            // Given
            // The BuildTargetHelper has not been initialized

            // When
            // Expect an error log and an exception when accessing the default property
            LogAssert.Expect(LogType.Error, "[BuildTargetHelper] Error: you must call BuildTargetHelper.Init before using this property.");

            // Then
            // Assert that accessing the default property throws an exception
            var ex = Assert.Throws<Exception>(() => { var instance = BuildTargetHelper.@default; });
            Assert.AreEqual("Not initialized", ex.Message);
        }

        [Test]
        public void GivenInitialized_WhenAccessingDefault_ThenReturnsInstance()
        {
            // Given
            // The BuildTargetHelper has been initialized
            BuildTargetHelper.hasBeenInitialized = true;

            // When
            // Access the default property
            var instance = BuildTargetHelper.@default;

            // Then
            // Assert that the instance is not null
            Assert.IsNotNull(instance);
        }
    }

    public class SwitchTargetTest
    {
        public class UnityTargetTestDelegate : IUnityTargetDelegate
        {
            public BuildTarget ActiveTarget { get; set; }
            public BuildTargetGroup SelectedTargetGroup { get; set; }
            public bool SwitchTargetResult { get; set; }
            public string[] CompilationSymbols { get; set; }

            public BuildTarget GetActiveTarget()
            {
                return ActiveTarget;
            }

            public BuildTargetGroup GetSelectedTargetGroup()
            {
                return SelectedTargetGroup;
            }

            public bool SwitchTarget(BuildTargetGroup targetGroup, BuildTarget target)
            {
                SelectedTargetGroup = targetGroup;
                ActiveTarget = target;
                return SwitchTargetResult;
            }

            public string[] GetCompilationSymbols(UnityEditor.Build.NamedBuildTarget target)
            {
                return CompilationSymbols;
            }

            public void SetCompilationSymbols(UnityEditor.Build.NamedBuildTarget target, string[] symbols)
            {
                CompilationSymbols = symbols;
            }
        }

        public class MockDelegate : ITargetDelegate
        {
            public E_Target OldTarget { get; private set; }
            public E_Target NewTarget { get; private set; }
            public bool BuildTargetFailedToChangeCalled { get; private set; }

            public void TargetHasChanged(E_Target oldTarget, E_Target newTarget)
            {
                OldTarget = oldTarget;
                NewTarget = newTarget;
            }

            public void BuildTargetFailedToChange()
            {
                BuildTargetFailedToChangeCalled = true;
            }

            public void BuildTargetHasChanged(BuildTargetGroup targetGroup, BuildTarget target)
            {
                // Implement if needed
            }

            public void SymbolsHaveChanged(string[] oldSymbols, string[] newSymbols, UnityEditor.Build.NamedBuildTarget target)
            {
                // Implement if needed
            }
        }

        UnityTargetTestDelegate unityDelegate;
        MockDelegate mockDelegate;

        [SetUp]
        public void SetUp()
        {
            BuildTargetHelper.hasBeenInitialized = false;
            unityDelegate = new UnityTargetTestDelegate();
            mockDelegate = new MockDelegate();
            BuildTargetHelper.Init(E_Target.Quest);
            BuildTargetHelper.@default.unityDelegate = unityDelegate;
            BuildTargetHelper.@default.@delegate = mockDelegate;
        }

        [Test]
        public void GivenQuestTarget_WhenSwitchingTargetToQuest_ThenBuildTargetAndSymbolsAreUpdated()
        {
            // Given
            unityDelegate.CompilationSymbols = new[] { "UMI3D_XR", "AnotherSymbol" };
            unityDelegate.SwitchTargetResult = true;

            // When
            E_Target target = E_Target.Quest;
            BuildTargetHelper.@default.SwitchTarget(target);

            // Then
            Assert.AreEqual(BuildTargetGroup.Android, unityDelegate.SelectedTargetGroup);
            Assert.AreEqual(BuildTarget.Android, unityDelegate.ActiveTarget);
            Assert.Contains("UMI3D_XR", unityDelegate.CompilationSymbols);
            Assert.Contains("AnotherSymbol", unityDelegate.CompilationSymbols);
            CollectionAssert.DoesNotContain(unityDelegate.CompilationSymbols, "UMI3D_PC");
            Assert.AreEqual(target, BuildTargetHelper.@default.target);
            Assert.AreEqual(target, mockDelegate.NewTarget);
        }

        [Test]
        public void GivenQuestTarget_WhenSwitchingTargetToSteamVR_ThenBuildTargetAndSymbolsAreUpdated()
        {
            // Given
            unityDelegate.CompilationSymbols = new[] { "UMI3D_XR" };
            unityDelegate.SwitchTargetResult = true;

            // When
            E_Target target = E_Target.SteamVR;
            BuildTargetHelper.@default.SwitchTarget(target);

            // Then
            Assert.AreEqual(BuildTargetGroup.Standalone, unityDelegate.SelectedTargetGroup);
            Assert.AreEqual(BuildTarget.StandaloneWindows64, unityDelegate.ActiveTarget);
            Assert.Contains("UMI3D_XR", unityDelegate.CompilationSymbols);
            Assert.AreEqual(target, BuildTargetHelper.@default.target);
            Assert.AreEqual(target, mockDelegate.NewTarget);
        }

        [Test]
        public void GivenQuestTarget_WhenSwitchingTargetToWindows_ThenBuildTargetAndSymbolsAreUpdated()
        {
            // Given
            unityDelegate.CompilationSymbols = new[] { "UMI3D_XR" };
            unityDelegate.SwitchTargetResult = true;

            // When
            E_Target target = E_Target.Windows;
            BuildTargetHelper.@default.SwitchTarget(target);

            // Then
            Assert.AreEqual(BuildTargetGroup.Standalone, unityDelegate.SelectedTargetGroup);
            Assert.AreEqual(BuildTarget.StandaloneWindows64, unityDelegate.ActiveTarget);
            Assert.Contains("UMI3D_PC", unityDelegate.CompilationSymbols);
            CollectionAssert.DoesNotContain(unityDelegate.CompilationSymbols, "UMI3D_XR");
            Assert.AreEqual(target, BuildTargetHelper.@default.target);
            Assert.AreEqual(target, mockDelegate.NewTarget);
        }

        [Test]
        public void GivenNoCompilationSymbols_WhenSwitchingTargetToWindows_ThenBuildTargetAndSymbolsAreUpdated()
        {
            // Given
            unityDelegate.CompilationSymbols = null;
            unityDelegate.SwitchTargetResult = true;

            // When
            E_Target target = E_Target.Windows;
            BuildTargetHelper.@default.SwitchTarget(target);

            // Then
            Assert.AreEqual(BuildTargetGroup.Standalone, unityDelegate.SelectedTargetGroup);
            Assert.AreEqual(BuildTarget.StandaloneWindows64, unityDelegate.ActiveTarget);
            Assert.Contains("UMI3D_PC", unityDelegate.CompilationSymbols);
            Assert.AreEqual(target, BuildTargetHelper.@default.target);
            Assert.AreEqual(target, mockDelegate.NewTarget);
        }

        [Test]
        public void GivenSwitchTargetFails_WhenSwitchingTarget_ThenLogsErrorAndInvokesDelegate()
        {
            // Given
            E_Target target = E_Target.Quest;
            unityDelegate.SwitchTargetResult = false;

            // When
            LogAssert.Expect(LogType.Error, "[BuildTargetHelper] Error: Switching target failed.");
            BuildTargetHelper.@default.SwitchTarget(target);

            // Then
            Assert.AreEqual(BuildTargetGroup.Android, unityDelegate.SelectedTargetGroup);
            Assert.AreEqual(BuildTarget.Android, unityDelegate.ActiveTarget);
            Assert.IsTrue(mockDelegate.BuildTargetFailedToChangeCalled);
        }
    }
}