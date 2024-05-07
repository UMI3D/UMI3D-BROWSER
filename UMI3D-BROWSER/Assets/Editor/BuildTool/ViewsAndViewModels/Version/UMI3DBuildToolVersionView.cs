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

using inetum.unityUtils;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.browserEditor.BuildTool
{
    public class UMI3DBuildToolVersionView 
    {
        SubGlobal subGlobal = new("BuildTool");

        public VisualElement root;

        public UMI3DBuildToolVersion_SO versionModel;
        public UMI3DBuildToolSettings_SO settingModel;

        public VisualElement V_Major;
        public IntegerField IF_Major;
        public Button B_Major;
        public VisualElement V_Minor;
        public IntegerField IF_Minor;
        public Button B_Minor;
        public VisualElement V_BuildCount;
        public IntegerField IF_BuildCount;
        public Button B_BuildCount;
        public TextField TF_AdditionalVersion;
        public Label L_SDKVersion;
        public Label L_OldVersion;
        public Label L_Version;

        VersionDTO NewVersion
        {
            get
            {
                return versionModel.newVersion;
            }
        }

        public UMI3DBuildToolVersionView(VisualElement root)
        {
            this.root = root;

            subGlobal.TryGet(out versionModel);
            subGlobal.TryGet(out settingModel);
            versionModel.updateVersion = version =>
            {
                L_Version.text = version.VersionFromNow;
            };
        }

        public void Bind()
        {
            V_Major = root.Q("V_Major");
            IF_Major = V_Major.Q<IntegerField>();
            B_Major = V_Major.Q<Button>();
            V_Minor = root.Q("V_Minor");
            IF_Minor = V_Minor.Q<IntegerField>();
            B_Minor = V_Minor.Q<Button>();
            V_BuildCount = root.Q("V_BuildCount");
            IF_BuildCount = V_BuildCount.Q<IntegerField>();
            B_BuildCount = V_BuildCount.Q<Button>();
            TF_AdditionalVersion = root.Q<TextField>("TF_AdditionalVersion");
            L_SDKVersion = root.Q<Label>("L_SDKVersion");
            L_OldVersion = root.Q<Label>("L_OldVersion");
            L_Version = root.Q<Label>("L_BuildVersion");
        }

        public void Set()
        {
            // Major version.
            B_Major.clicked += () =>
            {
                IF_Major.value = ++IF_Major.value;
            };
            IF_Major.SetValueWithoutNotify(NewVersion.majorVersion);
            IF_Major.RegisterValueChangedCallback(value =>
            {
                versionModel.ApplyMajorVersion(value.newValue);
            });

            // Minor version
            B_Minor.clicked += () =>
            {
                IF_Minor.value = ++IF_Minor.value;
            };
            IF_Minor.SetValueWithoutNotify(NewVersion.minorVersion);
            IF_Minor.RegisterValueChangedCallback(value =>
            {
                versionModel.ApplyMinorVersion(value.newValue);
            });

            // Build count.
            B_BuildCount.clicked += () =>
            {
                IF_BuildCount.value = ++IF_BuildCount.value;
            };
            IF_BuildCount.SetValueWithoutNotify(NewVersion.buildCountVersion);
            IF_BuildCount.RegisterValueChangedCallback(value =>
            {
                versionModel.ApplyBuildCountVersion(value.newValue);
            });

            // Additional information.
            // Like if you want to add unity editor version.
            TF_AdditionalVersion.SetValueWithoutNotify(NewVersion.additionalVersion);
            TF_AdditionalVersion.RegisterValueChangedCallback((value) =>
            {
                versionModel.ApplyAdditionalVersion(value.newValue);
            });

            L_SDKVersion.text = versionModel.sdkVersion.Version;
            L_OldVersion.text = versionModel.oldVersion.Version;
            L_Version.text = NewVersion.VersionFromNow;

            UpdateBorderColor(L_SDKVersion.parent, settingModel.sdkColor);
            UpdateBorderColor(L_OldVersion.parent, settingModel.oldVersionColor);
            UpdateBorderColor(L_Version.parent, settingModel.versionColor);
        }

        void UpdateBorderColor(VisualElement visual, Color color)
        {
            visual.style.borderTopColor = color;
            visual.style.borderRightColor = color;
            visual.style.borderBottomColor = color;
            visual.style.borderLeftColor = color;
        }
    }
}