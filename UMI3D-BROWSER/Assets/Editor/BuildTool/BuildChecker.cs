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
using umi3d.cdk.collaboration;
using umi3d.common;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace umi3d.browserEditor.BuildTool
{
    public class BuildChecker : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            CheckCollabLoadingParameters();
        }

        void CheckCollabLoadingParameters()
        {
#if UNITY_ANDROID
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(UMI3DCollabLoadingParameters)}");
            string logs = $"[{nameof(UMI3DCollabLoadingParameters)}] supported formats:\n";

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                UMI3DCollabLoadingParameters loadingParam = AssetDatabase.LoadAssetAtPath<UMI3DCollabLoadingParameters>(path);

                foreach (string format in loadingParam.supportedformats)
                {
                    logs += $"{format}\n";
                }

                if (loadingParam.supportedformats.Contains(UMI3DAssetFormat.unity_android_urp))
                {
                    throw new BuildFailedException("Impossible to build with " + UMI3DAssetFormat.unity_android_urp + " format not added to supported format");
                }

                logs += "\n";
            }

            Debug.Log(logs);
#endif
        }
    } 
}
