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
        /// Version number of the browser
        /// </summary>
        public string versionNumber;
        /// <summary>
        /// 
        /// </summary>
        public string bundleVersion;

        /// <summary>
        /// Version of the browser
        /// </summary>
        public string version
        {
            get
            {
                return $"{releaseCycle.GetReleaseInitial()}.{versionNumber}.Date";
            }
        }

        public Action updateVersion;
    }
}
