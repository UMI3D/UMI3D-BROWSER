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
using umi3d.cdk.collaboration;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.browserEditor.BuildTool
{
    public class UMI3DBuildTool : EditorWindow
    {
        [SerializeField] private VisualTreeAsset ui = default;
        [SerializeField] UMI3DCollabLoadingParameters loadingParameters;
        [SerializeField] UMI3DBuildTool_SO buildTool_SO;

        IBuilToolComponent _uMI3DConfigurator = null;
        IBuilToolComponent _targetAndPluginSwitcher = null;

        E_Target selectedTarget;

        [MenuItem("Tools/BuildTool")]
        public static void OpenWindow()
        {
            UMI3DBuildTool wnd = GetWindow<UMI3DBuildTool>();
            wnd.titleContent = new GUIContent("UMI3DBuildTool");
        }

        public void CreateGUI()
        {
            _targetAndPluginSwitcher = new TargetAndPlugingSwitcher();
            _uMI3DConfigurator = new UMI3DConfigurator(loadingParameters);

            VisualElement root = rootVisualElement;
            VisualElement ui_instance = ui.Instantiate();

            root.Add(ui_instance);

            DropdownField DD_TargetSelection = root.Q<DropdownField>("DD_TargetSelection");
            DD_TargetSelection.choices.Clear();
            DD_TargetSelection.choices.AddRange(Enum.GetNames(typeof(E_Target)));
            DD_TargetSelection.value = DD_TargetSelection.choices[(int)buildTool_SO.Target];
            DD_TargetSelection.RegisterValueChangedCallback(value =>
            {
                buildTool_SO.Target = Enum.Parse<E_Target>(value.newValue);
                selectedTarget = buildTool_SO.Target;
            });

            DropdownField DD_ReleaseCycle = root.Q<DropdownField>("DD_ReleaseCycle");
            DD_ReleaseCycle.choices.Clear();
            DD_ReleaseCycle.choices.AddRange(Enum.GetNames(typeof(E_ReleaseCycle)));
            DD_ReleaseCycle.value = DD_ReleaseCycle.choices[(int)buildTool_SO.releaseCycle];
            DD_ReleaseCycle.RegisterValueChangedCallback(value =>
            {
                buildTool_SO.releaseCycle = Enum.Parse<E_ReleaseCycle>(value.newValue);
            });

            TextField TF_BuildName = root.Q<TextField>("TF_BuildName");
            TF_BuildName.value = PlayerSettings.productName;
            TF_BuildName.RegisterValueChangedCallback(value => 
            { 
                PlayerSettings.productName = value.newValue;
            });

            TextField TF_BuildVersionNumber = root.Q<TextField>("TF_BuildVersionNumber");
            TF_BuildVersionNumber.value = buildTool_SO.versionNumber;
            TF_BuildVersionNumber.RegisterValueChangedCallback((value) =>
            {
                buildTool_SO.versionNumber = value.newValue;
            });

            Label L_Version = root.Q<Label>("L_Version");
            buildTool_SO.updateVersion = () =>
            {
                L_Version.text = buildTool_SO.version;
            };

            Button B_Build = root.Q<Button>("B_Build");
            B_Build.clicked += () => Build();
        } 
        
        private void Build()
        {
            _uMI3DConfigurator.HandleTarget(selectedTarget);
            _targetAndPluginSwitcher.HandleTarget(selectedTarget);
            BuildPipeline.BuildPlayer(BuildToolHelper.GetPlayerBuildOptions());
        }
    }
}
