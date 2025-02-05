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
using UnityEngine;

namespace umi3d.browserEditor.BuildTool
{
    internal interface IUnityTargetDelegate 
    {
        BuildTarget GetActiveTarget()
        {
            return EditorUserBuildSettings.activeBuildTarget;
        }

        BuildTargetGroup GetSelectedTargetGroup()
        {
            return EditorUserBuildSettings.selectedBuildTargetGroup;
        }

        bool SwitchTarget(BuildTargetGroup targetGroup, BuildTarget target)
        {
            bool result = EditorUserBuildSettings.SwitchActiveBuildTarget(
                targetGroup,
                target
            );
            // buildTargetGroup is not set correctly with EditorUserBuildSettings.SwitchActiveBuildTarget.
            EditorUserBuildSettings.selectedBuildTargetGroup = targetGroup;

            return result;
        }

        string[] GetCompilationSymbols(UnityEditor.Build.NamedBuildTarget target)
        {
            return PlayerSettings
                .GetScriptingDefineSymbols(target)
                .Split(';');
        }

        void SetCompilationSymbols(UnityEditor.Build.NamedBuildTarget target, string[] symbols)
        {
            PlayerSettings.SetScriptingDefineSymbols(
                target,
                symbols
            );
        }
    }

    internal class UnityTargetDelegate : IUnityTargetDelegate { }
}