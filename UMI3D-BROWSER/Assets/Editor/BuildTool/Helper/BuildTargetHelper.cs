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

using NUnit.Framework;
using System.Collections.Generic;
using umi3d.browserRuntime.conditionalCompilation;
using UnityEditor;

namespace umi3d.browserEditor.BuildTool
{
    public static class BuildTargetHelper 
    {
        static debug.UMI3DLogger logger 
            = new(mainTag: nameof(BuildTargetHelper));

        /// <summary>
        /// Switch target.<br/>
        /// Return -1 if an error occurred.<br/>
        /// Return 0 if the current target is already good.<br/>
        /// Return 1 if the target has been changed successfully.<br/>
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static int SwitchTarget(TargetDto target)
        {
            switch (target.Target)
            {
                case E_Target.Quest:
                case E_Target.Focus:
                case E_Target.Pico:
                case E_Target.SteamXR:
                    ChangeDeviceConditionalCompilation(MultiDevice.XR);
                    break;
                default:
                    break;
            }

            switch (target.Target)
            {
                case E_Target.Quest:
                case E_Target.Focus:
                case E_Target.Pico:
                    return ChangeBuildTarget(
                        BuildTargetGroup.Android, 
                        BuildTarget.Android
                    );
                case E_Target.SteamXR:
                    return ChangeBuildTarget(
                        BuildTargetGroup.Standalone,
                        BuildTarget.StandaloneWindows64
                    );
                default:
                    return -1;
            }
        }

        static void ChangeDeviceConditionalCompilation(MultiDevice device)
        {
            var deviceSymbols 
                = MultiDeviceExtensions.GetAllSymbols();

            List<string> newDef = new();
            bool shouldUpdate;

            var androidDef = PlayerSettings.GetScriptingDefineSymbols(
                UnityEditor.Build.NamedBuildTarget.Android
            ).Split(';');
            shouldUpdate = device.UpdateSymbols(
                androidDef,
                deviceSymbols,
                ref newDef
            );
            if (shouldUpdate) 
            {
                PlayerSettings.SetScriptingDefineSymbols(
                    UnityEditor.Build.NamedBuildTarget.Android,
                    newDef.ToArray()
                );
            }

            var standaloneDef = PlayerSettings.GetScriptingDefineSymbols(
                UnityEditor.Build.NamedBuildTarget.Standalone
            ).Split(';');
            shouldUpdate = device.UpdateSymbols(
                standaloneDef,
                deviceSymbols,
                ref newDef
            );
            if (shouldUpdate)
            {
                PlayerSettings.SetScriptingDefineSymbols(
                    UnityEditor.Build.NamedBuildTarget.Standalone,
                    newDef.ToArray()
                );
            }
        }

        /// <summary>
        /// Switch target.<br/>
        /// Return -1 if an error occurred.<br/>
        /// Return 0 if the current target is already good.<br/>
        /// Return 1 if the target has been changed successfully.<br/>
        /// </summary>
        /// <param name="buildTargetGroup"></param>
        /// <param name="buildTarget"></param>
        /// <returns></returns>
        static int ChangeBuildTarget(BuildTargetGroup buildTargetGroup, BuildTarget buildTarget)
        {
            var oldTarget = EditorUserBuildSettings.activeBuildTarget;
            var oldTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

            if (oldTarget == buildTarget && oldTargetGroup == buildTargetGroup)
            {
                logger.Default(
                    nameof(ChangeBuildTarget),
                    $"[UMI3D] Current target is {buildTarget}"
                );
                return 0;
            }

            var result = EditorUserBuildSettings.SwitchActiveBuildTarget(
                buildTargetGroup, 
                buildTarget
            );
            // buildTargetGroup is not set correctly with EditorUserBuildSettings.SwitchActiveBuildTarget.
            EditorUserBuildSettings.selectedBuildTargetGroup = buildTargetGroup;

            if (!result)
            {
                logger.Error(
                    nameof(ChangeBuildTarget),
                    $"[UMI3D] Switching target failed"
                );
                return -1;
            }
            else
            {
                logger.Default(
                    nameof(ChangeBuildTarget),
                    $"[UMI3D] Target switch from {oldTarget} to {buildTarget}"
                );
                return 1;
            }
        }
    }
}