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
using inetum.unityUtils.saveSystem;
using System;

namespace umi3d.browserEditor.BuildTool
{
    //[CreateAssetMenu(fileName = "UMI3D Build Tool Version", menuName = "UMI3D/Tools/Build Tool Version")]
    public class UMI3DBuildToolVersion_SO : SerializableScriptableObject
    {
        public VersionDTO newVersion;
        public VersionDTO oldVersion;

        /// <summary>
        /// Version of the browser
        /// </summary>
        public string Version
        {
            get
            {
                string result = $"";

                if (!string.IsNullOrEmpty(newVersion.additionalVersion))
                {
                    result += $"{newVersion.additionalVersion}_";
                }

                result += $"{newVersion.majorVersion}.{newVersion.minorVersion}.{newVersion.buildCountVersion}_{DateTime.Now.ToString("yy.MM.dd")}";

                return result;
            }
        }
        /// <summary>
        /// Last version of the browser
        /// </summary>
        public string OldVersion
        {
            get
            {
                string result = $"";

                if (!string.IsNullOrEmpty(oldVersion.additionalVersion))
                {
                    result += $"{oldVersion.additionalVersion}_";
                }

                result += $"{oldVersion.majorVersion}.{oldVersion.minorVersion}.{oldVersion.buildCountVersion}_{oldVersion.date}";

                return result;
            }
        }
        /// <summary>
        /// Bundle version for Android.
        /// </summary>
        public int BundleVersion
        {
            get
            {
                return newVersion.majorVersion * 10_000 + newVersion.minorVersion * 100 + newVersion.buildCountVersion;
            }
        }

        public Action updateVersion;
        public Action updateOldVersion;

        public void UpdateOldVersionWithNewVersion()
        {
            oldVersion = newVersion;
            oldVersion.date = DateTime.Now.ToString("yy.MM.dd");
        }
    }
}
