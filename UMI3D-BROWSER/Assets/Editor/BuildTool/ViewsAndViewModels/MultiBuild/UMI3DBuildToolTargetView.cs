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
        public UMI3DBuildToolSettings_SO buildToolSettings_SO;
        public int index;

        public UMI3DBuildToolTargetViewModel viewModel;
        public Toggle T_Select;
        public VisualElement V_Path;
        public TextField TF_Path;
        public Button B_Browse;
        public DropdownField DD_TargetSelection;
        public DropdownField DD_ReleaseCycle;

        public UMI3DBuildToolTargetView(
            VisualElement root,
            UMI3DBuildToolTarget_SO buildToolTarget_SO,
            UMI3DBuildToolSettings_SO buildToolSettings_SO,
            int index
        )
        {
            this.root = root;
            this.viewModel = new(
                buildToolTarget_SO
            );
            this.buildToolSettings_SO = buildToolSettings_SO;
            this.index = index;

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
            T_Select.RegisterValueChangedCallback(SelectionValueChanged);

            V_Path = root.Q("V_Path");
            TF_Path = V_Path.Q<TextField>();
            TF_Path.RegisterValueChangedCallback(PathValueChanged);

            B_Browse = V_Path.Q<Button>();
            B_Browse.clicked += Browse;
            
            DD_TargetSelection = root.Q<DropdownField>("DD_TargetSelection");
            DD_TargetSelection.RegisterValueChangedCallback(TargetSelectionValueChanged);

            DD_ReleaseCycle = root.Q<DropdownField>("DD_ReleaseCycle");
            DD_ReleaseCycle.RegisterValueChangedCallback(ReleaseCycleDDValueChanged);
        }

        public void Set()
        {
            // Select
            T_Select.SetValueWithoutNotify(viewModel[index].IsTargetEnabled);

            V_Path.style.display = (buildToolSettings_SO?.useOneBuildFolder ?? true)
                    ? DisplayStyle.None
                    : DisplayStyle.Flex;
            // Path
            (TF_Path.labelElement as INotifyValueChanged<string>).SetValueWithoutNotify("Build Folder");
            TF_Path.SetValueWithoutNotify(GetBuildFolder());
            
            // Device target.
            DD_TargetSelection.choices.Clear();
            DD_TargetSelection.choices.AddRange(Enum.GetNames(typeof(E_Target)));
            DD_TargetSelection.SetValueWithoutNotify(viewModel[index].Target.ToString());

            // Release cycle.
            DD_ReleaseCycle.choices.Clear();
            DD_ReleaseCycle.choices.AddRange(Enum.GetNames(typeof(E_ReleaseCycle)));
            DD_ReleaseCycle.SetValueWithoutNotify(viewModel[index].releaseCycle.ToString());
            DD_ReleaseCycle.tooltip 
                = "Alpha: Dev Build + More logs.\n" +
                "Beta: More logs.\n" +
                "Production: For release.";
        }

        public void Unbind()
        {
            B_Browse.clicked -= Browse;
            T_Select.UnregisterValueChangedCallback(SelectionValueChanged);
            TF_Path.UnregisterValueChangedCallback(PathValueChanged);
            DD_TargetSelection.UnregisterValueChangedCallback(TargetSelectionValueChanged);
            DD_ReleaseCycle.UnregisterValueChangedCallback(ReleaseCycleDDValueChanged);
        }

        string GetBuildFolder()
        {
            return (buildToolSettings_SO?.useOneBuildFolder ?? true)
                    ? viewModel.buildToolTarget_SO.buildFolder
                    : viewModel[index].BuildFolder;
        }

        void SelectionValueChanged(ChangeEvent<bool> value)
        {
            viewModel.Select(index, value.newValue);
        }

        void PathValueChanged(ChangeEvent<string> value)
        {
            viewModel.UpdateBuildFolder(index, value.newValue);
        }

        void TargetSelectionValueChanged(ChangeEvent<string> value)
        {
            viewModel.ApplyTarget(index, Enum.Parse<E_Target>(value.newValue));
        }

        void ReleaseCycleDDValueChanged(ChangeEvent<string> value)
        {
            viewModel.ApplyReleaseCycle(index, Enum.Parse<E_ReleaseCycle>(value.newValue));
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
    }
}