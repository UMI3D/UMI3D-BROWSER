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
using UnityEditor.XR.Management.Metadata;
using UnityEditor.XR.Management;
using UnityEngine;
using UnityEngine.UIElements;
using BuildTool;
using umi3d.cdk.collaboration;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.XR.OpenXR.Features;

namespace BuildTool
{
    public class UMI3DBuildTool : EditorWindow
    {
        [SerializeField] private VisualTreeAsset ui = default;
        [SerializeField] UMI3DCollabLoadingParameters loadingParameters;
        
        IBuilToolComponent _uMI3DConfigurator = null;
        IBuilToolComponent _targetAndPlugingSwitcher = null;

        E_Target selectedTarget;

        [MenuItem("Tools/BuildTool")]
        public static void OpenWindow()
        {
            UMI3DBuildTool wnd = GetWindow<UMI3DBuildTool>();
            wnd.titleContent = new GUIContent("UMI3DBuildTool");
        }

        public void CreateGUI()
        {
            _targetAndPlugingSwitcher = new TargetAndPlugingSwitcher();
            _uMI3DConfigurator = new UMI3DConfigurator(loadingParameters);

            VisualElement root = rootVisualElement;
            VisualElement ui_instance = ui.Instantiate();

            root.Add(ui_instance);

            
            DropdownField DD_TargetSelection = root.Q<DropdownField>("DD_TargetSelection");
            DD_TargetSelection.choices.Clear();
            DD_TargetSelection.choices.AddRange(Enum.GetNames(typeof(E_Target)));
            DD_TargetSelection.value = DD_TargetSelection.choices[0];
            DD_TargetSelection.RegisterValueChangedCallback((value) =>
            {
                selectedTarget = (E_Target)Enum.Parse(typeof(E_Target), value.newValue);
            });

            TextField TF_BuildName = root.Q<TextField>("TF_BuildName");
            TF_BuildName.value = PlayerSettings.productName;
            TF_BuildName.RegisterValueChangedCallback(value => 
            { 
                PlayerSettings.productName = value.newValue;
            });

            TextField TF_BuildVersion = root.Q<TextField>("TF_BuildVersion");
            TF_BuildVersion.value = PlayerSettings.bundleVersion;
            TF_BuildVersion.RegisterValueChangedCallback((value) =>
            {
                PlayerSettings.bundleVersion = value.newValue;
            });

            Button B_Build = root.Q<Button>("B_Build");
            B_Build.clicked += () => Build();
        } 
        
        private void Build()
        {
            _uMI3DConfigurator.HandleTarget(selectedTarget);
            _targetAndPlugingSwitcher.HandleTarget(selectedTarget);
            BuildPipeline.BuildPlayer(BuildToolHelper.GetPlayerBuildOptions());
        }
    }
}
