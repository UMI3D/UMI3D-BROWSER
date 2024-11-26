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

using umi3d.cdk.collaboration;
using umi3d.common;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace umi3d.browserEditor.BuildTool
{
    public class CollabLoadingParameterBuildChecker : IPreprocessBuildWithReport
    {
        /// <summary>
        /// Change this value if you want to by-pass this check.
        /// </summary>
        const bool check = true;

        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            if (!check)
            {
                return;
            }

            string[] guids = AssetDatabase.FindAssets($"t:{nameof(UMI3DCollabLoadingParameters)}");
            if (guids.Length == 0)
            {
                UnityEngine.Debug.Log($"Error: no UMI3DCollabLoadingParameters found.");
                return;
            }

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                UMI3DCollabLoadingParameters loadingParam = AssetDatabase.LoadAssetAtPath<UMI3DCollabLoadingParameters>(path);

                string missingFormats = "Missing formats: ";
                bool areFormatsMissing = false;

                CheckFormat(UMI3DAssetFormat.gltf, loadingParam, ref areFormatsMissing, ref missingFormats);
                CheckFormat(UMI3DAssetFormat.obj, loadingParam, ref areFormatsMissing, ref missingFormats);
                CheckFormat(UMI3DAssetFormat.fbx, loadingParam, ref areFormatsMissing, ref missingFormats);
                CheckFormat(UMI3DAssetFormat.png, loadingParam, ref areFormatsMissing, ref missingFormats);
                CheckFormat(UMI3DAssetFormat.jpg, loadingParam, ref areFormatsMissing, ref missingFormats);
#if UNITY_STANDALONE
                CheckFormat(UMI3DAssetFormat.unity_standalone_urp, loadingParam, ref areFormatsMissing, ref missingFormats);
#elif UNITY_ANDROID
                CheckFormat(UMI3DAssetFormat.unity_android_urp, loadingParam, ref areFormatsMissing, ref missingFormats);
#endif

                if (areFormatsMissing)
                {
                    string exceptionText = "Impossible to build with formats not added to supported formats:\n" 
                        + $"Path : {path}\n"
                        + missingFormats;
                    throw new BuildFailedException(exceptionText);
                }
            }
        }

        void CheckFormat(string format, UMI3DCollabLoadingParameters loadingParam, ref bool areFormatsMissing, ref string missingFormats)
        {
            if (!loadingParam.supportedformats.Contains(format))
            {
                areFormatsMissing = true;
                missingFormats += $"{UMI3DAssetFormat.gltf}; ";
            }
        }
    } 
}
