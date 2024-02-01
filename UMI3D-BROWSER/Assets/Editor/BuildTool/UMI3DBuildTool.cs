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
using System.Collections.Generic;
using umi3d.cdk.collaboration;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.browserEditor.BuildTool
{
    public class UMI3DBuildTool : EditorWindow
    {
        [SerializeField] private VisualTreeAsset ui = default;
        [SerializeField] private VisualTreeAsset target_VTA = default;
        [SerializeField] private VisualTreeAsset path_VTA = default;

        [SerializeField] UMI3DCollabLoadingParameters loadingParameters;

        [SerializeField] UMI3DBuildToolSaveSystem buildToolSaveSystem;
        [SerializeField] UMI3DBuildToolVersion_SO buildToolVersion_SO;
        [SerializeField] UMI3DBuildToolTarget_SO buildToolTarget_SO;

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
            //if (buildToolSaveSystem.LoadSaveDataFromDisk())
            //{
            //    UnityEngine.Debug.Log($"Save file");
            //    for (int i = 0; i < buildToolSaveSystem.saveData.ssoStacks.Count; i++)
            //    {
            //        var guid = new GUID(buildToolSaveSystem.saveData.ssoStacks[i].guid);
            //        string path = AssetDatabase.GUIDToAssetPath(guid);
            //        buildTool_SO = AssetDatabase.LoadAssetAtPath<UMI3DBuildTool_SO>(path);
            //    }
            //}

            _targetAndPluginSwitcher = new TargetAndPluginSwitcher();
            _uMI3DConfigurator = new UMI3DConfigurator(loadingParameters);

            VisualElement root = rootVisualElement;
            VisualElement ui_instance = ui.Instantiate();

            root.Add(ui_instance);
            TextField TF_BuildName = root.Q<TextField>("TF_BuildName");
            
            TemplateContainer T_Installer = root.Q<TemplateContainer>("T_Installer");
            TextField TF_Installer = T_Installer.Q<TextField>();
            Button B_Installer = T_Installer.Q<Button>();
            TemplateContainer T_License = root.Q<TemplateContainer>("T_License");
            TextField TF_License = T_License.Q<TextField>();
            Button B_License = T_License.Q<Button>();
            ListView LV_Targets = root.Q<ListView>("LV_Targets");

            Label L_OldVersion = root.Q<Label>("L_OldVersion");
            Label L_Version = root.Q<Label>("L_BuildVersion");

            // Build name.
            TF_BuildName.value = PlayerSettings.productName;
            TF_BuildName.RegisterValueChangedCallback(value =>
            {
                PlayerSettings.productName = value.newValue;
            });

            UMI3DBuildToolVersionView versionView = new(root, buildToolVersion_SO);
            versionView.Bind();
            versionView.Set();

            // Path
            TF_Installer.label = "Installer";
            B_Installer.clicked += () =>
            {
                UnityEngine.Debug.Log($"Todo");
            };
            TF_License.label = "License";

            // Targets.
            LV_Targets.reorderable = true;
            LV_Targets.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            LV_Targets.showFoldoutHeader = true;
            LV_Targets.headerTitle = "Targets";
            LV_Targets.showAddRemoveFooter = true;
            LV_Targets.reorderMode = ListViewReorderMode.Animated;
            LV_Targets.itemsSource = buildToolTarget_SO.targets;
            LV_Targets.makeItem = () =>
            {
                var visual = target_VTA.Instantiate();
                return visual;
            };
            LV_Targets.bindItem = (visual, index) =>
            {
                UMI3DBuildToolTargetView targetView = new(
                    root: visual,
                    buildToolTarget_SO: buildToolTarget_SO,
                    buildToolVersion_SO: buildToolVersion_SO,
                    index: index,
                    updateTarget: newTarget =>
                    {
                        selectedTarget = newTarget.Target;
                        ApplyChange();
                    }, 
                    build: Build
                );
                targetView.Bind();
                targetView.Set();
            };

            L_OldVersion.text = buildToolVersion_SO.OldVersion;
            buildToolVersion_SO.updateOldVersion = () =>
            {
                L_OldVersion.text = buildToolVersion_SO.OldVersion;
            };

            buildToolVersion_SO.updateVersion = () =>
            {
                L_Version.text = buildToolVersion_SO.Version;
                PlayerSettings.bundleVersion = buildToolVersion_SO.Version;
                PlayerSettings.Android.bundleVersionCode = buildToolVersion_SO.BundleVersion;
            };

            buildToolVersion_SO.updateVersion?.Invoke();
        } 
        
        private void ApplyChange()
        {
            buildToolSaveSystem.SaveDataToDisk(new[] { (buildToolTarget_SO, 1) });
            _uMI3DConfigurator.HandleTarget(selectedTarget);
            _targetAndPluginSwitcher.HandleTarget(selectedTarget);
        }

        private void Build()
        {
            BuildPipeline.BuildPlayer(BuildToolHelper.GetPlayerBuildOptions());
        }
    }
}
