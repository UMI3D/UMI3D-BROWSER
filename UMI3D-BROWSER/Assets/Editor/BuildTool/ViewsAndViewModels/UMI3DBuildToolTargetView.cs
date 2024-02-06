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
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.browserEditor.BuildTool
{
    public class UMI3DBuildToolTargetView 
    {
        public VisualElement root;
        public UMI3DBuildToolVersion_SO buildToolVersion_SO;
        public int index;
        public Action build;

        public UMI3DBuildToolTargetViewModel viewModel;
        public Toggle T_Select;
        public VisualElement V_Path;
        public TextField TF_Path;
        public Button B_Browse;
        public DropdownField DD_TargetSelection;
        public DropdownField DD_ReleaseCycle;
        public Button B_Apply;
        public Button B_Build;

        public UMI3DBuildToolTargetView(VisualElement root, UMI3DBuildToolTarget_SO buildToolTarget_SO, UMI3DBuildToolVersion_SO buildToolVersion_SO, int index, Action<TargetDto> updateTarget, Action build)
        {
            this.root = root;
            this.viewModel = new(buildToolTarget_SO, updateTarget);
            this.buildToolVersion_SO = buildToolVersion_SO;
            this.index = index;
            this.build = build;
        }

        public void Bind()
        {
            T_Select = root.Q<Toggle>("T_Select");
            V_Path = root.Q("V_Path");
            TF_Path = V_Path.Q<TextField>();
            B_Browse = V_Path.Q<Button>();
            DD_TargetSelection = root.Q<DropdownField>("DD_TargetSelection");
            DD_ReleaseCycle = root.Q<DropdownField>("DD_ReleaseCycle");
            B_Apply = root.Q<Button>("B_Apply");
            B_Build = root.Q<Button>("B_Build");
        }

        public void Set()
        {
            // Select
            T_Select.RegisterValueChangedCallback(value =>
            {
                viewModel.Select(index, value.newValue);
            });

            // Path
            TF_Path.label = "Build Folder";
            B_Browse.clicked += () =>
            {
                viewModel.ApplyBuildFolder(
                    index, 
                    EditorUtility.OpenFolderPanel(
                        title: "Build folder", 
                        viewModel[index].BuildFolder, 
                        defaultName: ""
                    )
                );
                TF_Path.value = viewModel[index].BuildFolder;
            };

            // Device target.
            DD_TargetSelection.choices.Clear();
            DD_TargetSelection.choices.AddRange(Enum.GetNames(typeof(E_Target)));
            DD_TargetSelection.value = viewModel[index].Target.ToString();
            DD_TargetSelection.RegisterValueChangedCallback(value =>
            {
                viewModel.ApplyTarget(index, Enum.Parse<E_Target>(value.newValue));
                ApplyChange(false);
            });

            // Release cycle.
            DD_ReleaseCycle.choices.Clear();
            DD_ReleaseCycle.choices.AddRange(Enum.GetNames(typeof(E_ReleaseCycle)));
            DD_ReleaseCycle.value = DD_ReleaseCycle.choices[(int)viewModel[index].releaseCycle];
            DD_ReleaseCycle.RegisterValueChangedCallback(value =>
            {
                viewModel.ApplyReleaseCycle(index, Enum.Parse<E_ReleaseCycle>(value.newValue));
                ApplyChange(false);
            });

            B_Apply.clicked += () =>
            {
                ApplyChange(true);
            };
            B_Build.clicked += () =>
            {
                buildToolVersion_SO.UpdateOldVersionWithNewVersion();
                build?.Invoke();
            };

            T_Select.value = viewModel[index].IsTargetEnabled;
            TF_Path.value = viewModel[index].BuildFolder;
            ApplyChange(viewModel[index].isApplied);
        }

        void ApplyChange(bool isApplied)
        {
            B_Apply.style.backgroundColor = isApplied ? new Color(0.5f, 1, 0) : StyleKeyword.Null;
            B_Build.SetEnabled(isApplied);
            viewModel.ApplyChange(index, isApplied);
        }
    }
}