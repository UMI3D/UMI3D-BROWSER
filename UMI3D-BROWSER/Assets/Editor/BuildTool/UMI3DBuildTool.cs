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
            _targetAndPluginSwitcher = new TargetAndPluginSwitcher();
            _uMI3DConfigurator = new UMI3DConfigurator(loadingParameters);

            VisualElement root = rootVisualElement;
            VisualElement ui_instance = ui.Instantiate();

            root.Add(ui_instance);
            TextField TF_BuildName = root.Q<TextField>("TF_BuildName");
            DropdownField DD_TargetSelection = root.Q<DropdownField>("DD_TargetSelection");
            DropdownField DD_ReleaseCycle = root.Q<DropdownField>("DD_ReleaseCycle");
            VisualElement V_Major = root.Q("V_Major");
            IntegerField IF_Major = V_Major.Q<IntegerField>();
            Button B_Major = V_Major.Q<Button>();
            VisualElement V_Minor = root.Q("V_Minor");
            IntegerField IF_Minor = V_Minor.Q<IntegerField>();
            Button B_Minor = V_Minor.Q<Button>();
            VisualElement V_BuildCount = root.Q("V_BuildCount");
            IntegerField IF_BuildCount = V_BuildCount.Q<IntegerField>();
            Button B_BuildCount = V_BuildCount.Q<Button>();
            TextField TF_AdditionalVersion = root.Q<TextField>("TF_AdditionalVersion");
            Button B_ResetVersion = root.Q<Button>("Reset");
            Label L_OldVersion = root.Q<Label>("L_OldVersion");
            Label L_Version = root.Q<Label>("L_BuildVersion");
            Button B_Apply = root.Q<Button>("B_Apply");
            Button B_Build = root.Q<Button>("B_Build");

            void ApplyChange()
            {
                B_Apply.SetEnabled(false);
                B_Build.SetEnabled(true);
            }
            void WaitChangeValidation()
            {
                B_Apply.SetEnabled(true);
                B_Build.SetEnabled(false);
            }

            // Build name.
            TF_BuildName.value = PlayerSettings.productName;
            TF_BuildName.RegisterValueChangedCallback(value => 
            { 
                PlayerSettings.productName = value.newValue;
                WaitChangeValidation();
            });

            // Device target.
            DD_TargetSelection.choices.Clear();
            DD_TargetSelection.choices.AddRange(Enum.GetNames(typeof(E_Target)));
            DD_TargetSelection.value = DD_TargetSelection.choices[(int)buildTool_SO.Target];
            DD_TargetSelection.RegisterValueChangedCallback(value =>
            {
                buildTool_SO.Target = Enum.Parse<E_Target>(value.newValue);
                selectedTarget = buildTool_SO.Target;
                WaitChangeValidation();
            });

            // Release cycle.
            DD_ReleaseCycle.choices.Clear();
            DD_ReleaseCycle.choices.AddRange(Enum.GetNames(typeof(E_ReleaseCycle)));
            DD_ReleaseCycle.value = DD_ReleaseCycle.choices[(int)buildTool_SO.releaseCycle];
            DD_ReleaseCycle.RegisterValueChangedCallback(value =>
            {
                buildTool_SO.releaseCycle = Enum.Parse<E_ReleaseCycle>(value.newValue);
                buildTool_SO.updateVersion?.Invoke();
                WaitChangeValidation();
            });

            // Major version.
            IF_Major.value = buildTool_SO.majorVersion;
            B_Major.clicked += () =>
            {
                IF_Major.value = ++IF_Major.value;
            };
            IF_Major.RegisterValueChangedCallback(value =>
            {
                buildTool_SO.majorVersion = IF_Major.value;
                buildTool_SO.updateVersion?.Invoke();
                WaitChangeValidation();
            });

            // Minor version
            IF_Minor.value = buildTool_SO.minorVersion;
            B_Minor.clicked += () =>
            {
                IF_Minor.value = ++IF_Minor.value;
            };
            IF_Minor.RegisterValueChangedCallback(value =>
            {
                buildTool_SO.minorVersion = IF_Minor.value;
                buildTool_SO.updateVersion?.Invoke();
                WaitChangeValidation();
            });

            // Build count.
            IF_BuildCount.value = buildTool_SO.buildCountVersion;
            B_BuildCount.clicked += () =>
            {
                IF_BuildCount.value = ++IF_BuildCount.value;
            };
            IF_BuildCount.RegisterValueChangedCallback(value =>
            {
                buildTool_SO.buildCountVersion = IF_BuildCount.value;
                buildTool_SO.updateVersion?.Invoke();
                WaitChangeValidation();
            });

            // Additional information.
            // Like if you want to add unity editor version.
            TF_AdditionalVersion.value = buildTool_SO.additionalVersion;
            TF_AdditionalVersion.RegisterValueChangedCallback((value) =>
            {
                buildTool_SO.additionalVersion = value.newValue;
                buildTool_SO.updateVersion?.Invoke();
                WaitChangeValidation();
            });

            // Reset version.
            B_ResetVersion.clicked += () =>
            {
                IF_Major.value = buildTool_SO.oldMajorVersion;
                IF_Minor.value = buildTool_SO.oldMinorVersion;
                IF_BuildCount.value = buildTool_SO.oldBuildCountVersion;
            };

            L_OldVersion.text = buildTool_SO.OldVersion;
            buildTool_SO.updateOldVersion = () =>
            {
                L_OldVersion.text = buildTool_SO.OldVersion;
            };

            buildTool_SO.updateVersion = () =>
            {
                L_Version.text = buildTool_SO.Version;
                PlayerSettings.bundleVersion = buildTool_SO.Version;
                PlayerSettings.Android.bundleVersionCode = buildTool_SO.BundleVersion;
            };

            B_Build.SetEnabled(false);
            B_Apply.clicked += () =>
            {
                ApplyChange();
                this.ApplyChange();
            };
            B_Build.clicked += () =>
            {
                buildTool_SO.UpdateOldVersionWithNewVersion();
                Build();
            };

            buildTool_SO.updateVersion?.Invoke();
        } 
        
        private void ApplyChange()
        {
            _uMI3DConfigurator.HandleTarget(selectedTarget);
            _targetAndPluginSwitcher.HandleTarget(selectedTarget);
        }

        private void Build()
        {
            BuildPipeline.BuildPlayer(BuildToolHelper.GetPlayerBuildOptions());
        }
    }
}
