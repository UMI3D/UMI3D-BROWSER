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
using UnityEngine.SceneManagement;

namespace umi3d.browserEditor.BuildTool
{
    public class BuildToolHelper
    {
        public static BuildPlayerOptions GetPlayerBuildOptions()
        {
            BuildPlayerOptions playerBuildOptions = BuildPlayerWindow.DefaultBuildMethods.GetBuildPlayerOptions(new BuildPlayerOptions());
            return playerBuildOptions;
        }

        public static BuildPlayerOptions GetPlayerBuildOptions(VersionDTO version, TargetDto target)
        {
            BuildPlayerOptions pbo = new();
            //pbo.scenes = new[] { SceneManager.sceneCountInBuildSettings };
            pbo.locationPathName = $"{target.BuildFolder}/{version.buildCountVersion}.{version.date}";
            pbo.target = target.Target.GetBuildTarget();
            pbo.options = BuildOptions.None;

            //BuildPlayerOptions playerBuildOptions = BuildPlayerWindow.DefaultBuildMethods.GetBuildPlayerOptions(pbo);

            return pbo;
        }
    }
}

