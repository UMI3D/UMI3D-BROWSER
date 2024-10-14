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
using System.Linq;
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;

namespace umi3d.browserEditor.BuildTool
{
    public static class FeatureHelper
    {
        public static void SwitchFeatures(E_Target target)
        {
            switch (target)
            {
                case E_Target.Quest:
                    DisableAllFeatures(BuildTargetGroup.Android, except: E_Feature.Meta);
                    EnableFeatures(BuildTargetGroup.Android, E_Feature.Meta);
                    break;
                case E_Target.SteamXR:
                    break;
                case E_Target.Focus:
                    DisableAllFeatures(BuildTargetGroup.Android, except: E_Feature.Vive);
                    EnableFeatures(BuildTargetGroup.Android, E_Feature.Vive);
                    break;
                case E_Target.Pico:
                    DisableAllFeatures(BuildTargetGroup.Android, except: E_Feature.Pico);
                    EnableFeatures(BuildTargetGroup.Android, E_Feature.Pico);
                    break;
            }
        }

        static void OpenXRFeature(bool enable, BuildTargetGroup buildTargetGroup, string featureId)
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

        static void EnableFeatures(BuildTargetGroup buildTargetGroup, E_Feature features)
        {
            foreach (var feature in features.GetFeatures())
            {
                OpenXRFeature(true, buildTargetGroup, feature);
            }
        }

        static void DisableAllFeatures(BuildTargetGroup buildTargetGroup, params E_Feature[] except)
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
    }
}

