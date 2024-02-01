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
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.browserEditor.BuildTool
{
    public class UMI3DBuildToolVersionView 
    {
        public VisualElement root;

        public UMI3DBuildToolVersionViewModel viewModel;
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
        public Button B_ResetVersion;

        public UMI3DBuildToolVersionView(VisualElement root, UMI3DBuildToolVersion_SO buildToolVersion_SO)
        {
            this.root = root;
            this.viewModel = new(buildToolVersion_SO);
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
            B_ResetVersion = root.Q<Button>("Reset");
        }

        public void Set()
        {
            // Major version.
            IF_Major.value = viewModel.NewVersion.majorVersion;
            B_Major.clicked += () =>
            {
                IF_Major.value = ++IF_Major.value;
            };
            IF_Major.RegisterValueChangedCallback(value =>
            {
                viewModel.ApplyMajorVersion(value.newValue);
            });

            // Minor version
            IF_Minor.value = viewModel.NewVersion.minorVersion;
            B_Minor.clicked += () =>
            {
                IF_Minor.value = ++IF_Minor.value;
            };
            IF_Minor.RegisterValueChangedCallback(value =>
            {
                viewModel.ApplyMinorVersion(value.newValue);
            });

            // Build count.
            IF_BuildCount.value = viewModel.NewVersion.buildCountVersion;
            B_BuildCount.clicked += () =>
            {
                IF_BuildCount.value = ++IF_BuildCount.value;
            };
            IF_BuildCount.RegisterValueChangedCallback(value =>
            {
                viewModel.ApplyBuildCountVersion(value.newValue);
            });

            // Additional information.
            // Like if you want to add unity editor version.
            TF_AdditionalVersion.value = viewModel.NewVersion.additionalVersion;
            TF_AdditionalVersion.RegisterValueChangedCallback((value) =>
            {
                viewModel.ApplyAdditionalVersion(value.newValue);
            });

            // Reset version.
            B_ResetVersion.clicked += () =>
            {
                IF_Major.value = viewModel.OldVersion.majorVersion;
                IF_Minor.value = viewModel.OldVersion.minorVersion;
                IF_BuildCount.value = viewModel.OldVersion.buildCountVersion;
                TF_AdditionalVersion.value = viewModel.OldVersion.additionalVersion;
            };
        }
    }
}