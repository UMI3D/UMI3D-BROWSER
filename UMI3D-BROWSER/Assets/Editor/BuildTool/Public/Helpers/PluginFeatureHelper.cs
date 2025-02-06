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
        internal IUnityTargetDelegate unityDelegate = new UnityTargetDelegate();

        void test(E_Target target)
        {
            // Plugins
            switch (target)
            {
                case E_Target.Quest:
                case E_Target.Focus:
                case E_Target.Pico:
                case E_Target.SteamVR:
                    DisableAllPlugins(Plugin.OpenXR);
                    EnablePlugin(Plugin.OpenXR);
                    break;

                case E_Target.Windows:
                    DisableAllPlugins();
                    break;
            }

            // Features
            switch (target)
            {
                case E_Target.Quest:
                    DisableAllFeatures(E_Feature.Meta);
                    EnableFeatures(E_Feature.Meta);
                    break;
                case E_Target.SteamVR:
                    break;
                case E_Target.Focus:
                    DisableAllFeatures(E_Feature.Vive);
                    EnableFeatures(E_Feature.Vive);
                    break;
                case E_Target.Pico:
                    DisableAllFeatures(E_Feature.Pico);
                    EnableFeatures(E_Feature.Pico);
                    break;
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

            //FoveationFeature

            var features = OpenXRSettings.Instance.GetFeatures();

            foreach (var feature in features)
            {
                UnityEngine.Debug.Log($"Feature: {feature.name}, Enabled: {feature.enabled}");
            }
        }

        void SetPlugin(bool enable, Plugin plugin)
        {
            BuildTargetGroup targetGroup = unityDelegate.GetSelectedTargetGroup();

            XRManagerSettings settings = XRGeneralSettingsPerBuildTarget
                .XRGeneralSettingsForBuildTarget(targetGroup)
                .AssignedSettings;

            string loaderTypeName = plugin.loader;

            bool success = false;
            if (enable)
            {
                bool isLoaded = XRPackageMetadataStore.IsLoaderAssigned(
                    loaderTypeName,
                    targetGroup
                );

                if (isLoaded) { return; }

                success = XRPackageMetadataStore.AssignLoader(
                    settings,
                    loaderTypeName,
                    targetGroup
                );
            }
            else
            {
                bool isLoaded = XRPackageMetadataStore.IsLoaderAssigned(
                    loaderTypeName,
                    targetGroup
                );

                if (!isLoaded) { return; }

                success = XRPackageMetadataStore.RemoveLoader(
                    settings,
                    loaderTypeName,
                    targetGroup
                );
            }

            if (success)
            {
                UnityEngine.Debug.Log($"[BuildTargetHelper] Notice: Plugin [{plugin.name}] has been {(enable ? "enabled" : "disabled")} for target [{targetGroup}].");
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
                UnityEngine.Debug.LogError($"[BuildTargetHelper] Could not {(enable ? "enabled" : "disabled")} {plugin.name} plugin on [{targetGroup}].");
            }
        }

        public void EnablePlugin(Plugin plugin)
        {
            SetPlugin(true, plugin);
        }

        public void DisableAllPlugins(params Plugin[] except)
        {
            foreach (Plugin plugin in Plugin.allCases)
            {
                if (!except.Contains(plugin))
                {
                    SetPlugin(false, plugin);
                }
            }
        }

        #endregion

        #region Features

        public void OpenXRFeature(bool enable, string[] featuresId)
        {
            BuildTargetGroup targetGroup = unityDelegate.GetSelectedTargetGroup();

            FeatureHelpers.RefreshFeatures(targetGroup);
            try
            {
                OpenXRFeature[] features = FeatureHelpers.GetFeaturesWithIdsForActiveBuildTarget(featuresId);
                foreach (OpenXRFeature feature in features)
                {
                    if (feature.enabled == enable) { continue; }
                    feature.enabled = enable;
                    UnityEngine.Debug.Log($"[BuildTargetHelper] Notice: Feature [{feature.name}] has been {(enable ? "enabled" : "disabled")} for target [{targetGroup}]");
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"[BuildTargetHelper] Error: A feature is maybe missing.");
                UnityEngine.Debug.LogException(ex);
            }
        }

        public void EnableFeatures(E_Feature features)
        {
            OpenXRFeature(true, features.GetFeatures());
        }

        public void DisableAllFeatures(params E_Feature[] except)
        {
            List<string> features = new();

            foreach (E_Feature feature in Enum.GetValues(typeof(E_Feature)))
            {
                if (!except.Contains(feature))
                {
                    features.AddRange(feature.GetFeatures());
                }
            }

            OpenXRFeature(false, features.ToArray());
        }

        #endregion
    }
}