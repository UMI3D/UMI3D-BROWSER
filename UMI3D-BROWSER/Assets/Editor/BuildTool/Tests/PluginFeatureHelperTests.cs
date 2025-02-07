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
using System.Collections.Generic;
using System.Linq;
using umi3d.browserEditor.BuildTool;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.Management;

public class PluginFeatureHelperTests
{
    public class EnablePluginsTest
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

        public class UnityPluginFeatureTestDelegate : IUnityPluginFeatureDelegate
        {
            public bool isPluginEnabled;
            public bool pluginResult = true;

            public bool IsPluginEnabled(string loaderTypeName, BuildTargetGroup targetGroup)
            {
                return isPluginEnabled;
            }

            public bool EnablePlugin(XRManagerSettings settings, string loaderTypeName, BuildTargetGroup targetGroup)
            {
                return pluginResult;
            }

            public bool DisablePlugin(XRManagerSettings settings, string loaderTypeName, BuildTargetGroup targetGroup)
            {
                return pluginResult;
            }
        }

        public class TestPluginFeatureDelegate : IPluginFeatureDelegate
        {
            public bool result = false;

            public void PluginHasBeenEnabled(Plugin plugin)
            {
                result = true;
            }

            public void PluginHasBeenDisabled(Plugin plugin)
            {
                result = true;
            }

            public void SettingPluginRaisedError(Plugin plugin)
            {
                result = false;
            }
        }

        UnityTargetTestDelegate unityTargetTestDelegate;
        UnityPluginFeatureTestDelegate unityPluginFeatureTestDelegate;
        TestPluginFeatureDelegate testPluginFeatureDelegate;

        [SetUp]
        public void SetUp()
        {
            unityTargetTestDelegate = new UnityTargetTestDelegate();
            unityPluginFeatureTestDelegate = new UnityPluginFeatureTestDelegate();
            testPluginFeatureDelegate = new TestPluginFeatureDelegate();

            PluginFeatureHelper.@default.unityTargetDelegate = unityTargetTestDelegate;
            PluginFeatureHelper.@default.unityPluginFeatureDelegate = unityPluginFeatureTestDelegate;
            PluginFeatureHelper.@default.@delegate = testPluginFeatureDelegate;
        }

        [Test]
        public void GivenNoPlugins_WhenEnablePlugins_ThenPluginsAreEnabled()
        {
            // Given
            unityTargetTestDelegate.SelectedTargetGroup = BuildTargetGroup.Standalone;
            unityPluginFeatureTestDelegate.isPluginEnabled = false;
            unityPluginFeatureTestDelegate.pluginResult = true;

            // When
            PluginFeatureHelper.@default.EnablePlugins(Plugin.OpenXR, Plugin.Oculus);

            // Then
            Assert.IsTrue(testPluginFeatureDelegate.result);
        }

        [Test]
        public void GivenNoPlugins_WhenEnablePlugins_ThenLogErrorIfEnableFails()
        {
            // Given
            unityTargetTestDelegate.SelectedTargetGroup = BuildTargetGroup.Standalone;
            unityPluginFeatureTestDelegate.isPluginEnabled = false;
            unityPluginFeatureTestDelegate.pluginResult = false;

            // When
            LogAssert.Expect(LogType.Error, $"[PluginFeatureHelper] Could not enabled {Plugin.OpenXR.name} plugin on [{BuildTargetGroup.Standalone}].");
            PluginFeatureHelper.@default.EnablePlugins(Plugin.OpenXR);

            // Then
            Assert.False(testPluginFeatureDelegate.result);
        }

        [Test]
        public void GivenPlugins_WhenEnableSamePlugins_ThenNoError()
        {
            // Given
            unityTargetTestDelegate.SelectedTargetGroup = BuildTargetGroup.Standalone;
            unityPluginFeatureTestDelegate.isPluginEnabled = true;
            unityPluginFeatureTestDelegate.pluginResult = true;

            // When
            PluginFeatureHelper.@default.EnablePlugins(Plugin.OpenXR);

            // Then
            Assert.IsTrue(testPluginFeatureDelegate.result);
        }
    }

    public class DisableAllPluginsTest
    {
        public class UnityTargetTestDelegate : IUnityTargetDelegate
        {
            public BuildTarget ActiveTarget;
            public BuildTargetGroup SelectedTargetGroup;
            public bool SwitchTargetResult;
            public string[] CompilationSymbols;

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

        public class UnityPluginFeatureTestDelegate : IUnityPluginFeatureDelegate
        {
            public bool isPluginEnabled;
            public bool pluginResult = true;

            public bool IsPluginEnabled(string loaderTypeName, BuildTargetGroup targetGroup)
            {
                return isPluginEnabled;
            }

            public bool EnablePlugin(XRManagerSettings settings, string loaderTypeName, BuildTargetGroup targetGroup)
            {
                return pluginResult;
            }

            public bool DisablePlugin(XRManagerSettings settings, string loaderTypeName, BuildTargetGroup targetGroup)
            {
                return pluginResult;
            }
        }

        public class TestPluginFeatureDelegate : IPluginFeatureDelegate
        {
            public bool result = false;
            public List<Plugin> plugins = new();

            public void PluginHasBeenEnabled(Plugin plugin)
            {
                result = true;
                plugins.Add(plugin);
            }

            public void PluginHasBeenDisabled(Plugin plugin)
            {
                result = true;
                plugins.Remove(plugin);
            }

            public void SettingPluginRaisedError(Plugin plugin)
            {
                result = false;
            }
        }

        UnityTargetTestDelegate unityTargetTestDelegate;
        UnityPluginFeatureTestDelegate unityPluginFeatureTestDelegate;
        TestPluginFeatureDelegate testPluginFeatureDelegate;

        [SetUp]
        public void SetUp()
        {
            unityTargetTestDelegate = new UnityTargetTestDelegate();
            unityPluginFeatureTestDelegate = new UnityPluginFeatureTestDelegate();
            testPluginFeatureDelegate = new TestPluginFeatureDelegate();

            unityTargetTestDelegate.SelectedTargetGroup = BuildTargetGroup.Standalone;

            PluginFeatureHelper.@default.unityTargetDelegate = unityTargetTestDelegate;
            PluginFeatureHelper.@default.unityPluginFeatureDelegate = unityPluginFeatureTestDelegate;
            PluginFeatureHelper.@default.@delegate = testPluginFeatureDelegate;
        }

        [Test]
        public void WhenDisableAllPlugins_ThenAllPluginsAreDisabled()
        {
            testPluginFeatureDelegate.plugins.AddRange(Plugin.allCases);

            // When
            PluginFeatureHelper.@default.DisableAllPlugins();

            // Then
            Assert.IsTrue(testPluginFeatureDelegate.result);
            Assert.AreEqual(0, testPluginFeatureDelegate.plugins.Count);
        }

        [Test]
        public void WhenDisableAllPluginsExceptOne_ThenOnlyOnePluginLeft()
        {
            // Given
            testPluginFeatureDelegate.plugins.AddRange(Plugin.allCases);

            // When
            PluginFeatureHelper.@default.DisableAllPlugins(Plugin.OpenXR);

            // Then
            Assert.IsTrue(testPluginFeatureDelegate.result);
            Assert.AreEqual(1, testPluginFeatureDelegate.plugins.Count);
            Assert.Contains(Plugin.OpenXR, testPluginFeatureDelegate.plugins);
        }

        [Test]
        public void GivenNoPlugins_WhenDisableAllPlugins_ThenNoError()
        {
            // Given
            testPluginFeatureDelegate.plugins = new();

            // When
            PluginFeatureHelper.@default.DisableAllPlugins();

            // Then
            Assert.IsTrue(testPluginFeatureDelegate.result);
            Assert.AreEqual(0, testPluginFeatureDelegate.plugins.Count);
        }

        [Test]
        public void WhenDisableAllPlugins_ThenLogErrorIfDisableFails()
        {
            // Given
            unityPluginFeatureTestDelegate.pluginResult = false;
            unityPluginFeatureTestDelegate.isPluginEnabled = true;

            // When
            for (int i = 0; i < Plugin.allCases.Length; i++)
            {
                LogAssert.Expect(LogType.Error, $"[PluginFeatureHelper] Could not disable [{Plugin.allCases[i].name}] plugin on [{BuildTargetGroup.Standalone}].");
            }
            PluginFeatureHelper.@default.DisableAllPlugins();

            // Then
            Assert.False(testPluginFeatureDelegate.result);
        }
    }
}