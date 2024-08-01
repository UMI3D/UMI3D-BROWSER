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
using UnityEditor.XR.Management.Metadata;
using UnityEditor.XR.Management;
using UnityEditor;
using UnityEngine;

namespace umi3d.browserEditor.BuildTool
{
    public static class PluginHelper 
    {
        public static void SwitchPlugins(E_Target target)
        {
            switch (target)
            {
                case E_Target.Quest:
                case E_Target.Focus:
                case E_Target.Pico:
                    DisableAllPlugins(BuildTargetGroup.Android, except: E_Plugin.OpenXR);
                    break;
                case E_Target.SteamXR:
                    DisableAllPlugins(BuildTargetGroup.Standalone, except: E_Plugin.OpenXR);
                    break;
                case E_Target.Windows:
                    DisableAllPlugins(BuildTargetGroup.Android);
                    break;
            }

            switch (target)
            {
                case E_Target.Quest:
                case E_Target.Focus:
                case E_Target.Pico:
                    EnablePlugin(BuildTargetGroup.Android, E_Plugin.OpenXR);
                    break;
                case E_Target.SteamXR:
                    EnablePlugin(BuildTargetGroup.Standalone, E_Plugin.OpenXR);
                    break;
            }
        }

        static void DisablePlugin(BuildTargetGroup buildTargetGroup, E_Plugin plugin)
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

        static void EnablePlugin(BuildTargetGroup buildTargetGroup, E_Plugin plugin)
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
                // The issue happen when the player settings are open. So I recommend you to close that window before calling this method.
            }
            else
            {
                UnityEngine.Debug.LogError($"[UMI3D] Could not enabled {plugin} plugin on {buildTargetGroup}");
            }
        }

        static void DisableAllPlugins(BuildTargetGroup buildTargetGroup, params E_Plugin[] except)
        {
            foreach (E_Plugin plugin in Enum.GetValues(typeof(E_Plugin)))
            {
                if (!except.Contains(plugin))
                {
                    DisablePlugin(buildTargetGroup, plugin);
                }
            }
        }

        static string GetLoaderName(E_Plugin plugin) => plugin switch
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