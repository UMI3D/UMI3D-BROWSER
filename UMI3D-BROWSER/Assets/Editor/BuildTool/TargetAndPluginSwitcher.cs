/*
Copyright 2019 - 2024 Inetum

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
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine;
using UnityEngine.XR.OpenXR.Features;

namespace umi3d.browserEditor.BuildTool
{
    public class TargetAndPluginSwitcher : IBuilToolComponent
    {
        public void HandleTarget(E_Target selectedTarget)
        {
            switch (selectedTarget)
            {
                case E_Target.Quest:
                    ChangeBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                    DisableAllPlugins(BuildTargetGroup.Android, except: E_Plugin.OpenXR);
                    EnablePlugin(BuildTargetGroup.Android, E_Plugin.OpenXR);
                    DisableAllFeatures(BuildTargetGroup.Android, except: E_Feature.Meta);
                    EnableFeatures(BuildTargetGroup.Android, E_Feature.Meta);
                    break;
                case E_Target.SteamXR:
                    ChangeBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);
                    DisableAllPlugins(BuildTargetGroup.Android, except: E_Plugin.OpenXR);
                    EnablePlugin(BuildTargetGroup.Standalone, E_Plugin.OpenXR);
                    break;
                case E_Target.Focus:
                    ChangeBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                    DisableAllPlugins(BuildTargetGroup.Android, except: E_Plugin.OpenXR);
                    EnablePlugin(BuildTargetGroup.Android, E_Plugin.OpenXR);
                    DisableAllFeatures(BuildTargetGroup.Android, except: E_Feature.Vive);
                    EnableFeatures(BuildTargetGroup.Android, E_Feature.Vive);
                    break;
                case E_Target.Pico:
                    ChangeBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                    DisableAllPlugins(BuildTargetGroup.Android, except: E_Plugin.OpenXR);
                    EnablePlugin(BuildTargetGroup.Android, E_Plugin.OpenXR);
                    DisableAllFeatures(BuildTargetGroup.Android, except: E_Feature.Pico);
                    EnableFeatures(BuildTargetGroup.Android, E_Feature.Pico);
                    break;
            }
        }

        private void ChangeBuildTarget(BuildTargetGroup buildTargetGroup, BuildTarget buildTarget)
        {
            var oldTarget = EditorUserBuildSettings.activeBuildTarget;

            var result = EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, buildTarget);

            if (!result)
            {
                UnityEngine.Debug.LogError($"[UMI3D] Switching target failed");
            }
            else
            {
                UnityEngine.Debug.Log($"[UMI3D] target switch from {oldTarget} to {buildTarget}");
            }
        }

        #region Plugins

        private void DisablePlugin(BuildTargetGroup buildTargetGroup, E_Plugin plugin)
        {
            var loader = GetLoaderName(plugin);

            if (!XRPackageMetadataStore.IsLoaderAssigned(loader, buildTargetGroup))
            {
                return;
            }

            var settings = XRGeneralSettingsPerBuildTarget
                .XRGeneralSettingsForBuildTarget(buildTargetGroup)
                .AssignedSettings;

            var success = XRPackageMetadataStore.RemoveLoader(
                settings, 
                loader, 
                buildTargetGroup
            );

            if (success)
            {
                Debug.Log($"[UMI3D] XR Plug-in Management: Disabled [{plugin}] plugin on {buildTargetGroup}");
            }
            else
            {
                UnityEngine.Debug.LogError($"[UMI3D] Could not disabled [{plugin}] plugin on {buildTargetGroup}");
            }
        }

        private void EnablePlugin(BuildTargetGroup buildTargetGroup, E_Plugin plugin)
        {
            var loader = GetLoaderName(plugin);

            if (XRPackageMetadataStore.IsLoaderAssigned(loader, buildTargetGroup))
            {
                return;
            }

            var settings = XRGeneralSettingsPerBuildTarget
                .XRGeneralSettingsForBuildTarget(buildTargetGroup)
                .AssignedSettings;

            var success = XRPackageMetadataStore.AssignLoader(
                settings, 
                loader, 
                buildTargetGroup
            );

            if (success)
            {
                Debug.Log($"[UMI3D] XR Plug-in Management: Enabled {plugin} plugin on {buildTargetGroup}");
                // If it looks like the OpenXR plugin is not toggled on its because there is a UI issue with OpenXR, But don't worry the plugin is activated.
                // The issue happen when the player setings are open. So I recommand you to close that window before calling this method.
            }
            else
            {
                UnityEngine.Debug.LogError($"[UMI3D] Could not enabled {plugin} plugin on {buildTargetGroup}");
            }
        }


        private void DisableAllPlugins(BuildTargetGroup buildTargetGroup, params E_Plugin[] except)
        {
            foreach (E_Plugin plugin in Enum.GetValues(typeof(E_Plugin)))
            {
                if (!except.Contains(plugin))
                {
                    DisablePlugin(buildTargetGroup, plugin);
                }
            }
        }

        #endregion

        #region Features

        private void OpenXRFeature(bool enable, BuildTargetGroup buildTargetGroup, string featureId)
        {
            try
            {
                FeatureHelpers.RefreshFeatures(buildTargetGroup);

                var features = FeatureHelpers.GetFeaturesWithIdsForActiveBuildTarget(new[] { featureId });
                if (features[0].enabled == enable)
                {
                    return;
                }
                features[0].enabled = enable;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
                throw new Exception("[UMI3D] XR Plug-in Management: Make sure you have installed the needed plugin to use the " + featureId);
            }

            UnityEngine.Debug.Log($"[UMI3D] XR Plug-in Management: {(enable ? "Enable" : "Disable")} {featureId} feature on {buildTargetGroup}");
        }

        private void EnableFeatures(BuildTargetGroup buildTargetGroup, E_Feature features)
        {
            foreach (var feature in features.GetFeatures())
            {
                OpenXRFeature(true, buildTargetGroup, feature);
            }
        }

        private void DisableAllFeatures(BuildTargetGroup buildTargetGroup, params E_Feature[] except)
        {
            foreach (E_Feature features in Enum.GetValues(typeof(E_Feature)))
            {
                if (!except.Contains(features))
                {
                    foreach (var feature in features.GetFeatures())
                    {
                        OpenXRFeature(false, buildTargetGroup, feature);
                    }
                }
            }
        }


        #endregion

        string GetLoaderName(E_Plugin plugin) => plugin switch
        {
            E_Plugin.OpenXR => BuildStaticNames.LOADER_OPEN_XR,
            E_Plugin.Oculus => BuildStaticNames.LOADER_OCULUS,
            E_Plugin.OpenVR => BuildStaticNames.LOADER_OPEN_VR,
            E_Plugin.PicoXR => BuildStaticNames.LOADER_PICO,
            E_Plugin.WaveXR => BuildStaticNames.LOADER_WAVE_XR,
            _ => throw new NotImplementedException()
        };
    }
}

