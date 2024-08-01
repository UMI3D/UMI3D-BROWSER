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
using UnityEditor;

namespace umi3d.browserEditor.BuildTool
{
    [Flags]
    public enum E_Target
    {
        Quest = 1, 
        Focus = 2, 
        Pico = 4, 
        SteamXR = 8,
        Windows = 16,
    }

    public static class TargetExt
    {
        public static BuildTarget GetBuildTarget(this E_Target target)
        {
            return target switch
            {
                E_Target.Quest => BuildTarget.Android,
                E_Target.Focus => BuildTarget.Android,
                E_Target.Pico => BuildTarget.Android,
                E_Target.SteamXR => BuildTarget.StandaloneWindows64,
                E_Target.Windows => BuildTarget.StandaloneWindows,
                _ => BuildTarget.StandaloneWindows
            };
        }
    }
}