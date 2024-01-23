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
using System.Collections;
using System.Collections.Generic;
using UnityEditor.XR.Management.Metadata;
using UnityEditor.XR.Management;
using UnityEditor;
using UnityEngine;

using UnityEngine.XR.OpenXR.Features;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR;

namespace BuildTool
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
                    EnableOpenXRFeature(BuildTargetGroup.Android, BuildStaticNames.META_QUEST_FEATURE);
                    break;
                case E_Target.Vive:
                    ChangeBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);
                    DisableAllPlugins(BuildTargetGroup.Standalone);
                    EnablePlugin(BuildTargetGroup.Standalone, E_Plugin.WaveXr);
                    break;
                case E_Target.Focus:
                    ChangeBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                    DisableAllPlugins(BuildTargetGroup.Android);
                    EnablePlugin(BuildTargetGroup.Android, E_Plugin.WaveXr);
                    break;
                case E_Target.Pico:
                    ChangeBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                    DisableAllPlugins(BuildTargetGroup.Android);
                    EnablePlugin(BuildTargetGroup.Android, E_Plugin.Pico);
                    EnablePlugin(BuildTargetGroup.Android, E_Plugin.OpenXR);
                    DisableOpenXRFeature(BuildTargetGroup.Android, BuildStaticNames.META_QUEST_FEATURE);
                    break;
            }
        }

        private void ChangeBuildTarget(BuildTargetGroup buildTargetGroup, BuildTarget buildTarget)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, buildTarget);
        }

        private void EnablePlugin(BuildTargetGroup buildTargetGroup, E_Plugin plugin)
        {
            var buildTargetSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(buildTargetGroup);
            var pluginsSettings = buildTargetSettings.AssignedSettings;
            var success = XRPackageMetadataStore.AssignLoader(pluginsSettings, GetLoaderName(plugin), buildTargetGroup);
            if (success)
            {
                Debug.Log($"XR Plug-in Management: Enabled {plugin} plugin on {buildTargetGroup}");
                Debug.Log("If it looks like the OpenXR plugin is not toggled on its because there is a" +
                    " UI issue with OpenXR, But don't worry the plugin is activated");
            }
        }

        private void DisablePlugin(BuildTargetGroup buildTargetGroup, E_Plugin plugin)
        {
            var buildTargetSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(buildTargetGroup);
            var pluginsSettings = buildTargetSettings.AssignedSettings;
            var success = XRPackageMetadataStore.RemoveLoader(pluginsSettings, GetLoaderName(plugin), buildTargetGroup);
            if (success)
            {
                Debug.Log($"XR Plug-in Management: Disabled {plugin} plugin on {buildTargetGroup}");
            }
        }

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
                throw new Exception("Make sure you have installed the needed plugin to use the " + featureId);
            }
        }


        private void DisableAllPlugins(BuildTargetGroup buildTargetGroup)
        {
            if (buildTargetGroup == BuildTargetGroup.Standalone)
            {
                DisablePlugin(BuildTargetGroup.Standalone, E_Plugin.OpenXR);
                DisablePlugin(BuildTargetGroup.Standalone, E_Plugin.OpenVR);
                DisablePlugin(BuildTargetGroup.Standalone, E_Plugin.WaveXr);
            }
            else if (buildTargetGroup == BuildTargetGroup.Android)
            {
                DisablePlugin(BuildTargetGroup.Android, E_Plugin.OpenXR);
                DisablePlugin(BuildTargetGroup.Android, E_Plugin.Oculus);
                DisablePlugin(BuildTargetGroup.Android, E_Plugin.OpenVR);
                DisablePlugin(BuildTargetGroup.Android, E_Plugin.Pico);
                DisablePlugin(BuildTargetGroup.Android, E_Plugin.WaveXr);
            }
        }

        string GetLoaderName(E_Plugin plugin) => plugin switch
        {
            E_Plugin.OpenXR => BuildStaticNames.OPEN_XR,
            E_Plugin.Oculus => BuildStaticNames.OCULUS,
            E_Plugin.OpenVR => BuildStaticNames.OPEN_VR,
            E_Plugin.Pico => BuildStaticNames.PICO,
            E_Plugin.WaveXr => BuildStaticNames.WAVE_XR,
            _ => throw new NotImplementedException()
        };
    }
}

