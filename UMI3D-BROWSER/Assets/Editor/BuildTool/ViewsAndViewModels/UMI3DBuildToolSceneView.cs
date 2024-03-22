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
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.browserEditor.BuildTool
{
    public class UMI3DBuildToolSceneView 
    {
        public VisualElement root;
        public UMI3DBuildToolScene_SO buildToolScene_SO;
        public int index;

        public UMI3DBuildToolSceneViewModel viewModel;
        public Toggle T_Select;
        public VisualElement V_Path;
        public TextField TF_Path;
        public Button B_Browse;
        public EnumFlagsField EFF_Targets = new("Targets", E_Target.Quest);

        public UMI3DBuildToolSceneView(
            VisualElement root,
            UMI3DBuildToolScene_SO buildToolScene_SO,
            int index,
            Action applyScenes
        )
        {
            this.root = root;
            this.buildToolScene_SO = buildToolScene_SO;
            this.index = index;
            this.viewModel = new(buildToolScene_SO, applyScenes);
        }

        public void Bind()
        {
            T_Select = root.Q<Toggle>("T_Select");
            V_Path = root.Q("V_Path");
            TF_Path = V_Path.Q<TextField>();
            B_Browse = V_Path.Q<Button>();
        }

        public void Set()
        {
            // Select
            T_Select.SetValueWithoutNotify(viewModel[index].enabled);
            T_Select.RegisterValueChangedCallback(SelectValueChanged);

            // Path
            (TF_Path.labelElement as INotifyValueChanged<string>).SetValueWithoutNotify("Scene Path");
            //TF_Path.label = "Scene Path";
            TF_Path.SetValueWithoutNotify(viewModel[index].path);
            TF_Path.RegisterValueChangedCallback(PathValueChanged);
            B_Browse.clicked += Browse;

            root.Q("V_Container").Add(EFF_Targets);
            EFF_Targets.SetValueWithoutNotify(viewModel[index].targets);
            EFF_Targets.RegisterValueChangedCallback(TargetValueChanged);
        }

        public void Unbind()
        {
            T_Select.UnregisterValueChangedCallback(SelectValueChanged);
            TF_Path.UnregisterValueChangedCallback(PathValueChanged);
            B_Browse.clicked -= Browse;
            EFF_Targets.UnregisterValueChangedCallback(TargetValueChanged);
            root.Q("V_Container").Remove(EFF_Targets);
        }

        void SelectValueChanged(ChangeEvent<bool> value)
        {
            viewModel.Select(index, value.newValue);
        }
        
        void PathValueChanged(ChangeEvent<string> value)
        {
            viewModel.UpdatedScenePath(index, value.newValue);
        }

        void TargetValueChanged(ChangeEvent<Enum> value)
        {
            var sceneDTO = viewModel[index];
            sceneDTO.targets = (E_Target)value.newValue;
            viewModel[index] = sceneDTO;
        }

        public void Browse()
        {
            viewModel.BrowseScenePath(
                index,
                path =>
                {
                    TF_Path.SetValueWithoutNotify(path);
                }
            );
        }
    }
}