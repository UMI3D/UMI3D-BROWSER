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
        public UMI3DBuildToolSettings_SO buildToolSettings_SO;
        public int index;
        public Action<TargetDto> applyTargetOptions;
        public Action<TargetDto> buildTarget;

        public UMI3DBuildToolTargetViewModel viewModel;
        public Toggle T_Select;
        public VisualElement V_Path;
        public TextField TF_Path;
        public Button B_Browse;
        public DropdownField DD_TargetSelection;
        public DropdownField DD_ReleaseCycle;
        public Button B_Apply;
        public Button B_Build;

        public UMI3DBuildToolTargetView(
            VisualElement root,
            UMI3DBuildToolTarget_SO buildToolTarget_SO,
            UMI3DBuildToolVersion_SO buildToolVersion_SO,
            UMI3DBuildToolSettings_SO buildToolSettings_SO,
            int index,
            Action<TargetDto> applyTargetOptions,
            Action<int> refreshView,
            Action<TargetDto> buildTarget
        )
        {
            this.root = root;
            this.viewModel = new(
                buildToolTarget_SO,
                applyTargetOptions,
                refreshView
            );
            this.buildToolVersion_SO = buildToolVersion_SO;
            this.buildToolSettings_SO = buildToolSettings_SO;
            this.index = index;
            this.applyTargetOptions = applyTargetOptions;
            this.buildTarget = buildTarget;

            var targetDto = viewModel[index];
            if (targetDto.Id.Equals(new())) 
            {
                targetDto.Id = Guid.NewGuid();
                viewModel[index] = targetDto;
            }
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
            T_Select.SetValueWithoutNotify(viewModel[index].IsTargetEnabled);

            V_Path.style.display = (buildToolSettings_SO?.useOneBuildFolder ?? true)
                    ? DisplayStyle.None
                    : DisplayStyle.Flex;
            // Path
            TF_Path.label = "Build Folder";
            TF_Path.RegisterValueChangedCallback(PathValueChanged);
            TF_Path.SetValueWithoutNotify(viewModel[index].BuildFolder);
            B_Browse.clicked += Browse;

            // Device target.
            DD_TargetSelection.choices.Clear();
            DD_TargetSelection.choices.AddRange(Enum.GetNames(typeof(E_Target)));
            DD_TargetSelection.SetValueWithoutNotify(viewModel[index].Target.ToString());
            DD_TargetSelection.RegisterValueChangedCallback(TargetSelectionValueChanged);

            // Release cycle.
            DD_ReleaseCycle.choices.Clear();
            DD_ReleaseCycle.choices.AddRange(Enum.GetNames(typeof(E_ReleaseCycle)));
            DD_ReleaseCycle.SetValueWithoutNotify(viewModel[index].releaseCycle.ToString());
            DD_ReleaseCycle.RegisterValueChangedCallback(ReleaseCycleDDValueChanged);

            ApplyChangeView(viewModel[index].isApplied);
            B_Apply.clicked += Apply;

            B_Build.clicked += Build;
        }

        public void Unbind()
        {
            B_Browse.clicked -= Browse;
            TF_Path.RegisterValueChangedCallback(PathValueChanged);
            DD_TargetSelection.UnregisterValueChangedCallback(TargetSelectionValueChanged);
            DD_ReleaseCycle.UnregisterValueChangedCallback(ReleaseCycleDDValueChanged);
            B_Apply.clicked -= Apply;
            B_Build.clicked -= Build;
        }

        void PathValueChanged(ChangeEvent<string> value)
        {
            viewModel.UpdateBuildFolder(index, value.newValue);
            ApplyChange(false);
        }

        void TargetSelectionValueChanged(ChangeEvent<string> value)
        {
            viewModel.ApplyTarget(index, Enum.Parse<E_Target>(value.newValue));
            ApplyChange(false);
        }

        void Browse()
        {
            viewModel.BrowseBuildFolder(
                index,
                updateView: path =>
                {
                    TF_Path.SetValueWithoutNotify(path);
                }
            );
        }

        void ReleaseCycleDDValueChanged(ChangeEvent<string> value)
        {
            viewModel.ApplyReleaseCycle(index, Enum.Parse<E_ReleaseCycle>(value.newValue));
            ApplyChange(false);
        }

        void Apply()
        {
            ApplyChange(true);
        }

        void ApplyChangeView(bool isApplied)
        {
            var selectedColor = buildToolSettings_SO?.selectedTargetColor ?? new Color(0.5f, 1, 0);
            B_Apply.style.backgroundColor = isApplied ? selectedColor : StyleKeyword.Null;
            B_Build.SetEnabled(isApplied);
        }

        void ApplyChange(bool isApplied)
        {
            ApplyChangeView(isApplied);

            viewModel.ApplyChange(index, isApplied);
        }

        void Build()
        {
            buildToolVersion_SO.UpdateOldVersionWithNewVersion();
            applyTargetOptions?.Invoke(viewModel[index]);
            buildTarget?.Invoke(viewModel[index]);
        }
    }
}