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

using UnityEditor;
using UnityEditor.XR.Management.Metadata;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR.Features;

namespace umi3d.browserEditor.BuildTool
{
    internal interface IUnityPluginFeatureDelegate 
    {
        bool IsPluginEnabled(string loaderTypeName, BuildTargetGroup targetGroup)
        {
            return XRPackageMetadataStore.IsLoaderAssigned(
                loaderTypeName,
                targetGroup
            );
        }

        bool EnablePlugin(XRManagerSettings settings, string loaderTypeName, BuildTargetGroup targetGroup)
        {
            return XRPackageMetadataStore.AssignLoader(
                settings,
                loaderTypeName,
                targetGroup
            );
        }

        bool DisablePlugin(XRManagerSettings settings, string loaderTypeName, BuildTargetGroup targetGroup)
        {
            return XRPackageMetadataStore.RemoveLoader(
                settings,
                loaderTypeName,
                targetGroup
            );
        }

        void RefreshFeatures(BuildTargetGroup targetGroup)
        {
            FeatureHelpers.RefreshFeatures(targetGroup);
        }

        bool SetFeatureWithIdForActiveBuildTarget(string featureId, bool enable)
        {
            OpenXRFeature xrFeature = FeatureHelpers.GetFeatureWithIdForActiveBuildTarget(featureId);

            if (xrFeature == null) { return false; }

            if (xrFeature.enabled != enable)
            {
                xrFeature.enabled = enable;
            }
            return true;
        }
    }

    internal class UnityPluginFeatureDelegate: IUnityPluginFeatureDelegate { } 
}