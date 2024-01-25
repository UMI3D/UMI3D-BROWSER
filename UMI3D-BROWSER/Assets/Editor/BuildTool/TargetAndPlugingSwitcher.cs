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
using UnityEditor;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine;
using UnityEngine.XR.OpenXR.Features;

namespace umi3d.browserEditor.BuildTool
{
    public class TargetAndPlugingSwitcher : IBuilToolComponent
    {
        public void HandleTarget(E_Target selectedTarget)
        {
            switch (selectedTarget)
            {
                case E_Target.Quest:
                    ChangeBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                    DisableAllPlugins(BuildTargetGroup.Android);
                    EnablePlugin(BuildTargetGroup.Android, E_Plugin.OpenXR);
                    DisableAllFeatures(BuildTargetGroup.Android);
                    EnableOpenXRFeature(BuildTargetGroup.Android, BuildStaticNames.FEATURE_META_QUEST);
                    EnableOpenXRFeature(BuildTargetGroup.Android, BuildStaticNames.INPUT_OCULUS_TOUCH);
                    EnableOpenXRFeature(BuildTargetGroup.Android, BuildStaticNames.INPUT_METAQUEST_PRO);
                    break;
                case E_Target.SteamXR:
                    ChangeBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);
                    DisableAllPlugins(BuildTargetGroup.Standalone);
                    EnablePlugin(BuildTargetGroup.Android, E_Plugin.OpenXR);
                    break;
                case E_Target.Focus:
                    ChangeBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                    DisableAllPlugins(BuildTargetGroup.Android);
                    EnablePlugin(BuildTargetGroup.Android, E_Plugin.OpenXR);
                    DisableAllFeatures(BuildTargetGroup.Android);
                    EnableOpenXRFeature(BuildTargetGroup.Android, BuildStaticNames.FEATURE_VIVE_SUPPORT);
                    EnableOpenXRFeature(BuildTargetGroup.Android, BuildStaticNames.INPUT_VIVEFocus3);
                    break;
                case E_Target.Pico:
                    ChangeBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                    DisableAllPlugins(BuildTargetGroup.Android);
                    EnablePlugin(BuildTargetGroup.Android, E_Plugin.OpenXR);
                    DisableAllFeatures(BuildTargetGroup.Android);
                    EnableOpenXRFeature(BuildTargetGroup.Android, BuildStaticNames.FEATURE_PICO_OPENXR);
                    EnableOpenXRFeature(BuildTargetGroup.Android, BuildStaticNames.FEATURE_PICO_SUPPORT);
                    EnableOpenXRFeature(BuildTargetGroup.Android, BuildStaticNames.INPUT_PICO4_TOUCH);
                    EnableOpenXRFeature(BuildTargetGroup.Android, BuildStaticNames.INPUT_PICONeo3_TOUCH);
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
            var buildTargetSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(buildTargetGroup);
            var pluginsSettings = buildTargetSettings.AssignedSettings;
            var success = XRPackageMetadataStore.RemoveLoader(pluginsSettings, GetLoaderName(plugin), buildTargetGroup);
            if (success)
            {
                Debug.Log($"[UMI3D] XR Plug-in Management: Disabled {plugin} plugin on {buildTargetGroup}");
            }
            else
            {
                UnityEngine.Debug.LogError($"[UMI3D] Could not disabled {plugin} plugin on {buildTargetGroup}");
            }
        }

        private void EnablePlugin(BuildTargetGroup buildTargetGroup, E_Plugin plugin)
        {
            var buildTargetSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(buildTargetGroup);
            var pluginsSettings = buildTargetSettings.AssignedSettings;
            var success = XRPackageMetadataStore.AssignLoader(pluginsSettings, GetLoaderName(plugin), buildTargetGroup);
            if (success)
            {
                Debug.Log($"[UMI3D] XR Plug-in Management: Enabled {plugin} plugin on {buildTargetGroup}");
                // If it looks like the OpenXR plugin is not toggled on its because there is a UI issue with OpenXR, But don't worry the plugin is activated
            }
            else
            {
                UnityEngine.Debug.LogError($"[UMI3D] Could not enabled {plugin} plugin on {buildTargetGroup}");
            }
        }


        private void DisableAllPlugins(BuildTargetGroup buildTargetGroup)
        {
            if (buildTargetGroup == BuildTargetGroup.Standalone)
            {
                DisablePlugin(BuildTargetGroup.Standalone, E_Plugin.OpenXR);
                DisablePlugin(BuildTargetGroup.Standalone, E_Plugin.OpenVR);
                DisablePlugin(BuildTargetGroup.Standalone, E_Plugin.WaveXR);
            }
            else if (buildTargetGroup == BuildTargetGroup.Android)
            {
                DisablePlugin(BuildTargetGroup.Android, E_Plugin.OpenXR);
                DisablePlugin(BuildTargetGroup.Android, E_Plugin.Oculus);
                DisablePlugin(BuildTargetGroup.Android, E_Plugin.OpenVR);
                DisablePlugin(BuildTargetGroup.Android, E_Plugin.PicoXR);
                DisablePlugin(BuildTargetGroup.Android, E_Plugin.WaveXR);
            }
        }

        #endregion

        #region Features

        private void DisableOpenXRFeature(BuildTargetGroup buildTargetGroup, string featureId)
        {
            GetFeatureById(buildTargetGroup, featureId).enabled = false;
        }

        private void EnableOpenXRFeature(BuildTargetGroup buildTargetGroup, string featureId)
        {
            GetFeatureById(buildTargetGroup, featureId).enabled = true;
        }

        private OpenXRFeature GetFeatureById(BuildTargetGroup buildTargetGroup, string featureId)
        {
            try
            {
                FeatureHelpers.RefreshFeatures(buildTargetGroup);

                List<string> names = new() { featureId };
                var features = FeatureHelpers.GetFeaturesWithIdsForActiveBuildTarget(names.ToArray());
                return features[0];
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
                throw new Exception("[UMI3D] Make sure you have installed the needed plugin to use the " + featureId);
            }
        }

        private void DisableAllFeatures(BuildTargetGroup buildTargetGroup)
        {
            if (buildTargetGroup == BuildTargetGroup.Standalone)
            {

            }
            else if (buildTargetGroup == BuildTargetGroup.Android)
            {
                DisableOpenXRFeature(BuildTargetGroup.Android, BuildStaticNames.FEATURE_META_QUEST);
                DisableOpenXRFeature(BuildTargetGroup.Android, BuildStaticNames.INPUT_OCULUS_TOUCH);
                DisableOpenXRFeature(BuildTargetGroup.Android, BuildStaticNames.INPUT_METAQUEST_PRO);
                DisableOpenXRFeature(BuildTargetGroup.Android, BuildStaticNames.FEATURE_PICO_OPENXR);
                DisableOpenXRFeature(BuildTargetGroup.Android, BuildStaticNames.FEATURE_PICO_SUPPORT);
                DisableOpenXRFeature(BuildTargetGroup.Android, BuildStaticNames.INPUT_PICO4_TOUCH);
                DisableOpenXRFeature(BuildTargetGroup.Android, BuildStaticNames.INPUT_PICONeo3_TOUCH);
                DisableOpenXRFeature(BuildTargetGroup.Android, BuildStaticNames.FEATURE_VIVE_SUPPORT);
                DisableOpenXRFeature(BuildTargetGroup.Android, BuildStaticNames.INPUT_VIVEFocus3);
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

