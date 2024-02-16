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

using UnityEditor;

namespace umi3d.browserEditor.BuildTool
{
    public static class BuildTargetHelper 
    {
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
                    return ChangeBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                case E_Target.SteamXR:
                    return ChangeBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
                default:
                    return -1;
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
                UnityEngine.Debug.Log($"[UMI3D] Current target is {buildTarget}");
                return 0;
            }

            var result = EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, buildTarget);

            if (!result)
            {
                UnityEngine.Debug.LogError($"[UMI3D] Switching target failed");
                return -1;
            }
            else
            {
                UnityEngine.Debug.Log($"[UMI3D] Target switch from {oldTarget} to {buildTarget}");
                return 1;
            }
        }
    }
}