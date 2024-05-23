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

            versionModel.updateNewVersionHandler += UpdateNewVersion;
            versionModel.updateOldVersionHandler += UpdateOldVersion;
            versionModel.updateSDKVersionHandler += UpdateSDKVersion;

            B_Major.clicked += IncreaseMajor;
            IF_Major.RegisterValueChangedCallback(MajorValueChanged);
            B_Minor.clicked += IncreaseMinor;
            IF_Minor.RegisterValueChangedCallback(MinorValueChanged);
            B_BuildCount.clicked += IncreaseBuildCount;
            IF_BuildCount.RegisterValueChangedCallback(BuildCountValueChanged);
            TF_AdditionalVersion.RegisterValueChangedCallback(AdditionalInfoValueChanged);
        }

        public void Set()
        {
            IF_Major.SetValueWithoutNotify(NewVersion.majorVersion);
            IF_Minor.SetValueWithoutNotify(NewVersion.minorVersion);
            IF_BuildCount.SetValueWithoutNotify(NewVersion.buildCountVersion);
            // Additional information.
            // If you want to add unity editor version or the name of the feature you are developing.
            TF_AdditionalVersion.SetValueWithoutNotify(NewVersion.additionalVersion);

            L_SDKVersion.text = $"SDK:   {versionModel.sdkVersion.Version}";
            L_OldVersion.text = $"Old:   {versionModel.oldVersion.Version}";
            L_Version.text = $"New:   {NewVersion.VersionFromNow}";

            UpdateBorderColor(L_SDKVersion.parent, settingModel.sdkColor);
            //UpdateBorderColor(L_OldVersion.parent, settingModel.oldVersionColor);
            UpdateBorderColor(L_Version.parent, settingModel.versionColor);
        }

        void UpdateBorderColor(VisualElement visual, Color color)
        {
            visual.style.borderTopColor = color;
            visual.style.borderRightColor = color;
            visual.style.borderBottomColor = color;
            visual.style.borderLeftColor = color;
        }

        void UpdateNewVersion(VersionDTO version)
        {
            L_Version.text = $"New:   {version.VersionFromNow}";
        }

        void UpdateOldVersion(VersionDTO version)
        {
            L_OldVersion.text = $"Old:   {version.Version}";
        }

        void UpdateSDKVersion(VersionDTO version)
        {
            L_SDKVersion.text = $"SDK:   {version.Version}";
        }

        #region Major

        void IncreaseMajor()
        {
            IF_Major.value = ++IF_Major.value;
        }

        void MajorValueChanged(ChangeEvent<int> value)
        {
            versionModel.ApplyMajorVersion(value.newValue);
        }

        #endregion

        #region Minor

        void IncreaseMinor()
        {
            IF_Minor.value = ++IF_Minor.value;
        }

        void MinorValueChanged(ChangeEvent<int> value)
        {
            versionModel.ApplyMinorVersion(value.newValue);
        }

        #endregion

        #region BuildCount

        void IncreaseBuildCount()
        {
            IF_BuildCount.value = ++IF_BuildCount.value;
        }

        void BuildCountValueChanged(ChangeEvent<int> value)
        {
            versionModel.ApplyBuildCountVersion(value.newValue);
        }

        #endregion

        void AdditionalInfoValueChanged(ChangeEvent<string> value)
        {
            versionModel.ApplyAdditionalVersion(value.newValue);
        }
    }
}