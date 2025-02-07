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
using UnityEditor;
using UnityEditor.XR.Management.Metadata;
using UnityEngine;
using UnityEngine.XR.Management;

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
    }

    internal class UnityPluginFeatureDelegate: IUnityPluginFeatureDelegate { } 
}