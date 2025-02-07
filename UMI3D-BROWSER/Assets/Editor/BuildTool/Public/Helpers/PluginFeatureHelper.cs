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
using UnityEditor.XR.OpenXR.Features;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;

namespace umi3d.browserEditor.BuildTool
{
    public class PluginFeatureHelper 
    {
        /// <summary>
        /// The default instance of <see cref="PluginFeatureHelper"/>.<br/>
        /// </summary>
        public static PluginFeatureHelper @default => _default.Value;
        static readonly Lazy<PluginFeatureHelper> _default = new(() => new());

        public IPluginFeatureDelegate @delegate;
        internal IUnityTargetDelegate unityTargetDelegate = new UnityTargetDelegate();
        internal IUnityPluginFeatureDelegate unityPluginFeatureDelegate = new UnityPluginFeatureDelegate();

        private PluginFeatureHelper() { }

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
                UnityEngine.Debug.LogError($"[PluginFeatureHelper] Could not {(enable ? "enable" : "disable")} [{plugin.name}] plugin on [{targetGroup}].");
                @delegate?.SettingPluginRaisedError(plugin);
            }
        }

        public void EnablePlugins(params Plugin[] plugins)
        {
            foreach (Plugin plugin in plugins)
            {
                SetPlugin(true, plugin);
            }
        }

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

            FeatureHelpers.RefreshFeatures(targetGroup);
            
            OpenXRFeature xrFeature = FeatureHelpers.GetFeatureWithIdForActiveBuildTarget(feature.id);

            if (xrFeature == null)
            {
                UnityEngine.Debug.LogError($"[PluginFeatureHelper] Error: Feature [{feature.name}] is maybe missing.");
                return;
            }

            if (xrFeature.enabled != enable) 
            {
                xrFeature.enabled = enable;
            }
            UnityEngine.Debug.Log($"[PluginFeatureHelper] Notice: Feature [{feature.name}] has been {(enable ? "enabled" : "disabled")} for target [{targetGroup}]");
        }

        public void EnableFeatures(params Feature[] features)
        {
            foreach (Feature feature in features)
            {
                SetXRFeature(true, feature);
            }
        }

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