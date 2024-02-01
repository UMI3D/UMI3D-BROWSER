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

using System;
using UnityEditor;
using UnityEngine;

namespace umi3d.browserEditor.BuildTool
{
    public class UMI3DBuildToolVersionViewModel 
    {
        public UMI3DBuildToolVersion_SO buildToolVersion_SO;
        public Action<VersionDTO> updateVersion;

        public UMI3DBuildToolVersionViewModel(UMI3DBuildToolVersion_SO buildToolVersion_SO, Action<VersionDTO> updateVersion)
        {
            this.buildToolVersion_SO = buildToolVersion_SO;
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

        public void ApplyMajorVersion(int value)
        {
            buildToolVersion_SO.newVersion.majorVersion = value;
            updateVersion?.Invoke(NewVersion);
            Save();
        }

        public void ApplyMinorVersion(int value)
        {
            buildToolVersion_SO.newVersion.minorVersion = value;
            updateVersion?.Invoke(NewVersion);
            Save();
        }

        public void ApplyBuildCountVersion(int value)
        {
            buildToolVersion_SO.newVersion.buildCountVersion = value;
            updateVersion?.Invoke(NewVersion);
            Save();
        }

        public void ApplyAdditionalVersion(string value)
        {
            buildToolVersion_SO.newVersion.additionalVersion = value;
            updateVersion?.Invoke(NewVersion);
            Save();
        }

        public void Save()
        {
            EditorUtility.SetDirty(buildToolVersion_SO);
        }
    }
}