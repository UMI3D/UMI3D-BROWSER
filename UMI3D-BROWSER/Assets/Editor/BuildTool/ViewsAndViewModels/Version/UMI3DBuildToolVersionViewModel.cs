/*
Copyright 2019 - 2023 Inetum

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

using inetum.unityUtils;
using System;
using UnityEditor;
using UnityEngine;

namespace umi3d.browserEditor.BuildTool
{
    public class UMI3DBuildToolVersionViewModel 
    {
        SubGlobal subGlobal = new("BuildTool");

        public UMI3DBuildToolVersion_SO buildToolVersion_SO;
        public Action<VersionDTO> updateVersion;

        public UMI3DBuildToolVersionViewModel(Action<VersionDTO> updateVersion)
        {
            subGlobal.TryGet(out buildToolVersion_SO);
            this.updateVersion = updateVersion;
        }

        public VersionDTO NewVersion
        {
            get
            {
                return buildToolVersion_SO.newVersion;
            }
        }

        public VersionDTO OldVersion
        {
            get
            {
                return buildToolVersion_SO.oldVersion;
            }
        }

        public VersionDTO SDKVersion
        {
            get
            {
                return buildToolVersion_SO.sdkVersion;
            }
        }

        public void ApplyMajorVersion(int value)
        {
            buildToolVersion_SO.newVersion.majorVersion = value;
            UpdateDate();
            UpdateSDKVersion();
            updateVersion?.Invoke(NewVersion);
            Save();
        }

        public void ApplyMinorVersion(int value)
        {
            buildToolVersion_SO.newVersion.minorVersion = value;
            UpdateDate();
            UpdateSDKVersion();
            updateVersion?.Invoke(NewVersion);
            Save();
        }

        public void ApplyBuildCountVersion(int value)
        {
            buildToolVersion_SO.newVersion.buildCountVersion = value;
            UpdateDate();
            UpdateSDKVersion();
            updateVersion?.Invoke(NewVersion);
            Save();
        }

        public void ApplyAdditionalVersion(string value)
        {
            buildToolVersion_SO.newVersion.additionalVersion = value;
            UpdateDate();
            UpdateSDKVersion();
            updateVersion?.Invoke(NewVersion);
            Save();
        }

        public void Save()
        {
            EditorUtility.SetDirty(buildToolVersion_SO);
        }

        void UpdateSDKVersion()
        {
            buildToolVersion_SO.sdkVersion.additionalVersion = UMI3DVersion.status;
            buildToolVersion_SO.sdkVersion.majorVersion = int.Parse(UMI3DVersion.major);
            buildToolVersion_SO.sdkVersion.minorVersion = int.Parse(UMI3DVersion.minor);
            buildToolVersion_SO.sdkVersion.buildCountVersion = 0;
            buildToolVersion_SO.sdkVersion.date = UMI3DVersion.date;
        }

        void UpdateDate()
        {
            buildToolVersion_SO.newVersion.date = DateTime.Now.ToString("yyMMdd");
        }
    }
}