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

using UnityEditor;
using UnityEditor.UIElements;
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
        public EnumFlagsField EFF_Targets;

        public UMI3DBuildToolSceneView(VisualElement root, UMI3DBuildToolScene_SO buildToolScene_SO, int index)
        {
            this.root = root;
            this.buildToolScene_SO = buildToolScene_SO;
            this.index = index;
            this.viewModel = new(buildToolScene_SO);
        }

        public void Bind()
        {
            T_Select = root.Q<Toggle>("T_Select");
            V_Path = root.Q("V_Path");
            TF_Path = V_Path.Q<TextField>();
            B_Browse = V_Path.Q<Button>();
            EFF_Targets = new EnumFlagsField("Targets", E_Target.Quest);
        }

        public void Set()
        {
            // Select
            T_Select.value = viewModel[index].enabled;
            T_Select.RegisterValueChangedCallback(value =>
            {
                viewModel.Select(index, value.newValue);
            });

            // Path
            TF_Path.label = "Scene Path";
            TF_Path.value = viewModel[index].path;
            B_Browse.clicked += () =>
            {
                viewModel.ApplyScenePath(
                    index,
                    EditorUtility.OpenFilePanel(
                        title: "Scene Path",
                        viewModel[index].path,
                        extension: "unity"
                    )
                );
                TF_Path.value = viewModel[index].path;
            };

            root.Q("V_Container").Add(EFF_Targets);
            EFF_Targets.value = viewModel[index].targets;
            EFF_Targets.RegisterValueChangedCallback(value =>
            {
                var sceneDTO = viewModel[index];
                sceneDTO.targets = (E_Target)value.newValue;
                viewModel[index] = sceneDTO;
            });
        }
    }
}