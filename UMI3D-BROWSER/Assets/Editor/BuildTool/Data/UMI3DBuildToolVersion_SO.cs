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
using UnityEngine;

namespace umi3d.browserEditor.BuildTool
{
    [CreateAssetMenu(fileName = "UMI3D Build Tool Version", menuName = "UMI3D/Build Tools/Build Tool Version")]
    public class UMI3DBuildToolVersion_SO : PersistentScriptableModel
    {
        public Action<VersionDTO> updateVersion;

        public VersionDTO newVersion;
        public VersionDTO oldVersion;
        public VersionDTO sdkVersion;

        public void ApplyMajorVersion(int value)
        {
            newVersion.majorVersion = value;
            OnNewVersionUpdated();
        }

        public void ApplyMinorVersion(int value)
        {
            newVersion.minorVersion = value;
            OnNewVersionUpdated();
        }

        public void ApplyBuildCountVersion(int value)
        {
            newVersion.buildCountVersion = value;
            OnNewVersionUpdated();
        }

        public void ApplyAdditionalVersion(string value)
        {
            newVersion.additionalVersion = value;
            OnNewVersionUpdated();
        }

        void OnNewVersionUpdated()
        {
            newVersion.date = DateTime.Now.ToString("yyMMdd");

            sdkVersion.additionalVersion = UMI3DVersion.status;
            sdkVersion.majorVersion = int.Parse(UMI3DVersion.major);
            sdkVersion.minorVersion = int.Parse(UMI3DVersion.minor);
            sdkVersion.buildCountVersion = 0;
            sdkVersion.date = UMI3DVersion.date;

            updateVersion?.Invoke(newVersion);

            Save(editorOnly: true);
        }

        public void UpdateOldVersionWithNewVersion()
        {
            oldVersion = newVersion;
            oldVersion.date = DateTime.Now.ToString("yyMMdd");
        }
    }
}
