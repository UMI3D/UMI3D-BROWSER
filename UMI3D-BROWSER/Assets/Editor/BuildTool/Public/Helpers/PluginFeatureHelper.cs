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
using System.Linq;
using UnityEditor;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEngine.XR.Management;

namespace umi3d.browserEditor.BuildTool
{
    public class PluginFeatureHelper 
    {
        /// <summary>
        /// The default instance of <see cref="PluginFeatureHelper"/>.<br/>
        /// </summary>
        public static PluginFeatureHelper @default => _default.Value;
        static readonly Lazy<PluginFeatureHelper> _default = new(() => new());
        PluginFeatureHelper() { }

        public IPluginFeatureDelegate @delegate;
        IUnityTargetDelegate _unityTargetDelegate = new UnityTargetDelegate();
        internal IUnityTargetDelegate unityTargetDelegate
        {
            get => _unityTargetDelegate;
            set
            {
                if (value == null)
                {
                    _unityTargetDelegate = new UnityTargetDelegate();
                }
                else
                {
                    _unityTargetDelegate = value;
                }
            }
        }
        IUnityPluginFeatureDelegate _unityPluginFeatureDelegate = new UnityPluginFeatureDelegate();
        internal IUnityPluginFeatureDelegate unityPluginFeatureDelegate
        {
            get => _unityPluginFeatureDelegate;
            set
            {
                if (value == null)
                {
                    _unityPluginFeatureDelegate = new UnityPluginFeatureDelegate();
                }
                else
                {
                    _unityPluginFeatureDelegate = value;
                }
            }
        }

        #region Plugins

        /// <summary>
        /// Log all the XR plugins.
        /// </summary>
        [MenuItem("Tools/Log XR Plugins")]
        static void LogXRPluginLoaderTypes()
        {
            IReadOnlyList<IXRPackage> allMetadata = XRPackageMetadataStore.GetAllPackageMetadata();

            foreach (IXRPackage metadata in allMetadata)
            {
                foreach (IXRLoaderMetadata loader in metadata.metadata.loaderMetadata)
                {
                    UnityEngine.Debug.Log($"[PluginFeatureHelper] Notice: Plugin: [{loader.loaderName}], [{loader.loaderType}]");
                }
            }
        }

        void SetPlugin(bool enable, Plugin plugin)
        {
            BuildTargetGroup targetGroup = unityTargetDelegate.GetSelectedTargetGroup();

            XRManagerSettings settings = XRGeneralSettingsPerBuildTarget
                .XRGeneralSettingsForBuildTarget(targetGroup)
                .AssignedSettings;

            string loaderTypeName = plugin.loader;

            bool isLoaded = unityPluginFeatureDelegate.IsPluginEnabled(
                loaderTypeName, 
                targetGroup
            );

            bool success = false;
            if (enable)
            {
                if (!isLoaded) 
                {
                    success = unityPluginFeatureDelegate.EnablePlugin(
                        settings,
                        loaderTypeName,
                        targetGroup
                    );
                }
                else { success = true; }
            }
            else
            {
                if (isLoaded) 
                {
                    success = unityPluginFeatureDelegate.DisablePlugin(
                        settings,
                        loaderTypeName,
                        targetGroup
                    );
                }
                else { success = true; }
            }

            if (success)
            {
                UnityEngine.Debug.Log($"[PluginFeatureHelper] Notice: Plugin [{plugin.name}] has been {(enable ? "enabled" : "disabled")} for target [{targetGroup}].");
                // If it looks like the OpenXR plugin is not toggled on or off,
                // it's because there is a UI issue with OpenXR.
                // The issue happen when the player settings are open.
                // So I recommend you to close that window before calling this method.
                if (enable)
                {
                    @delegate?.PluginHasBeenEnabled(plugin);
                }
                else
                {
                    @delegate?.PluginHasBeenDisabled(plugin);
                }
            }
            else
            {
                UnityEngine.Debug.LogError($"[PluginFeatureHelper] Error: Could not {(enable ? "enable" : "disable")} [{plugin.name}] plugin on [{targetGroup}].");
                @delegate?.SettingPluginRaisedError(plugin);
            }
        }

        /// <summary>
        /// Enables the specified plugins by setting their state to enabled.<br/>
        /// <br/>
        /// <example>
        /// Given a list of plugins when enabling the plugins then the plugins are enabled.
        /// <code>
        /// PluginFeatureHelper.@default.EnablePlugins(Plugin.OpenXR);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="plugins">An array of plugins to be enabled.</param>
        public void EnablePlugins(params Plugin[] plugins)
        {
            foreach (Plugin plugin in plugins)
            {
                SetPlugin(true, plugin);
            }
        }

        /// <summary>
        /// Disables all plugins except the specified ones.<br/>
        /// <br/>
        /// <example>
        /// Given a list of plugins when disabling all plugins except the specified ones then only the specified plugins remain enabled if it was already enabled.
        /// <code>
        /// PluginFeatureHelper.@default.DisableAllPlugins(Plugin.OpenXR);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="except">An array of plugins that should not be disabled.</param>
        public void DisableAllPlugins(params Plugin[] except)
        {
            foreach (Plugin plugin in Plugin.allCases)
            {
                if (!except?.Contains(plugin) ?? true)
                {
                    SetPlugin(false, plugin);
                }
            }
        }

        #endregion

        #region Features

        void SetXRFeature(bool enable, Feature feature)
        {
            BuildTargetGroup targetGroup = unityTargetDelegate.GetSelectedTargetGroup();

            unityPluginFeatureDelegate.RefreshFeatures(targetGroup);

            if (!unityPluginFeatureDelegate.SetFeatureWithIdForActiveBuildTarget(feature.id, enable))
            {
                UnityEngine.Debug.LogError($"[PluginFeatureHelper] Error: Feature [{feature.name}] is maybe missing.");
                @delegate?.SettingFeatureFailed(feature);
                return;
            }

            UnityEngine.Debug.Log($"[PluginFeatureHelper] Notice: Feature [{feature.name}] has been {(enable ? "enabled" : "disabled")} for target [{targetGroup}]");
            if (enable)
            {
                @delegate?.FeatureHasBeenEnabled(feature);
            } 
            else
            {
                @delegate?.FeatureHasBeenDisabled(feature);
            }
        }

        /// <summary>
        /// Enables the specified features by setting their state to enabled.<br/>
        /// If no features are provided, logs an error message.<br/>
        /// <br/>
        /// <example>
        /// Given a list of features when enabling the features then the features are enabled.
        /// <code>
        /// PluginFeatureHelper.@default.EnableFeatures(Feature.allMetaQuestCases);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="features">An array of features to be enabled.</param>
        public void EnableFeatures(params Feature[] features)
        {
            if (features == null || features.Length == 0)
            {
                UnityEngine.Debug.LogError($"[PluginFeatureHelper] Error: No features to enable.");
            }

            foreach (Feature feature in features)
            {
                SetXRFeature(true, feature);
            }
        }

        /// <summary>
        /// Disables all features except the specified ones.<br/>
        /// <br/>
        /// <example>
        /// Given a list of features when disabling all features except the specified ones then only the specified features remain enabled if they were already enabled.
        /// <code>
        /// PluginFeatureHelper.@default.DisableAllFeatures(Feature.allMetaQuestCases);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="except">An array of features that should not be disabled.</param>
        public void DisableAllFeatures(params Feature[] except)
        {
            foreach (Feature feature in Feature.allCases)
            {
                if (!except.Contains(feature))
                {
                    SetXRFeature(false, feature);
                }
            }
        }

        #endregion
    }
}