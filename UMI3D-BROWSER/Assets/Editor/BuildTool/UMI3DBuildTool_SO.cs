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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace umi3d.browserEditor.BuildTool
{
    [CreateAssetMenu(fileName = "UMI3D Build Tool", menuName = "UMI3D/Tools/Build Tool")]
    public class UMI3DBuildTool_SO : ScriptableObject
    {
        public E_Target Target = E_Target.Quest;
        public E_ReleaseCycle releaseCycle = E_ReleaseCycle.Alpha;
        
        /// <summary>
        /// New additional version.
        /// </summary>
        public string additionalVersion;
        /// <summary>
        /// New major version.
        /// </summary>
        public int majorVersion;
        /// <summary>
        /// New minor version.
        /// </summary>
        public int minorVersion;
        /// <summary>
        /// New build count version.
        /// </summary>
        public int buildCountVersion;
        /// <summary>
        /// Previous additional version.
        /// </summary>
        public string oldAdditionalVersion;
        /// <summary>
        /// Previous major version.
        /// </summary>
        public int oldMajorVersion;
        /// <summary>
        /// Previous minor version.
        /// </summary>
        public int oldMinorVersion;
        /// <summary>
        /// Previous build count version.
        /// </summary>
        public int oldBuildCountVersion;
        /// <summary>
        /// Previous date version.
        /// </summary>
        public string oldDateVersion;

        /// <summary>
        /// Version of the browser
        /// </summary>
        public string Version
        {
            get
            {
                string result = $"{releaseCycle.GetReleaseInitial()}_";

                if (!string.IsNullOrEmpty(additionalVersion))
                {
                    result += $"{additionalVersion}_";
                }

                result += $"{majorVersion}.{minorVersion}.{buildCountVersion}_{DateTime.Now.ToString("yy.MM.dd")}";

                return result;
            }
        }
        public string OldVersion
        {
            get
            {
                string result = $"";

                if (!string.IsNullOrEmpty(oldAdditionalVersion))
                {
                    result += $"{oldAdditionalVersion}_";
                }

                result += $"{oldMajorVersion}.{oldMinorVersion}.{oldBuildCountVersion}_{oldDateVersion}";

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
                return majorVersion * 10_000 + minorVersion * 100 + buildCountVersion;
            }
        }

        public Action updateVersion;
        public Action updateOldVersion;

        public void UpdateOldVersionWithNewVersion()
        {
            oldMajorVersion = majorVersion;
            oldMinorVersion = minorVersion;
            oldBuildCountVersion = buildCountVersion;
            oldAdditionalVersion = additionalVersion;
            oldDateVersion = DateTime.Now.ToString("yy.MM.dd");
        }
    }
}
