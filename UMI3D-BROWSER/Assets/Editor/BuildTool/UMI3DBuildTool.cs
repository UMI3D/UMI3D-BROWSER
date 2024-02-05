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
using System.Linq;
using umi3d.cdk.collaboration;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace umi3d.browserEditor.BuildTool
{
    public class UMI3DBuildTool : EditorWindow
    {
        [SerializeField] private VisualTreeAsset ui = default;
        [SerializeField] private VisualTreeAsset target_VTA = default;
        [SerializeField] private VisualTreeAsset path_VTA = default;
        [SerializeField] private VisualTreeAsset scene_VTA = default;

        [SerializeField] UMI3DCollabLoadingParameters loadingParameters;

        [SerializeField] UMI3DBuildToolVersion_SO buildToolVersion_SO;
        [SerializeField] UMI3DBuildToolTarget_SO buildToolTarget_SO;
        [SerializeField] UMI3DBuildToolScene_SO buildToolScene_SO;

        IBuilToolComponent _uMI3DConfigurator = null;
        IBuilToolComponent _targetAndPluginSwitcher = null;

        TargetDto targetDTO;
        VersionDTO oldVersionDTO;
        VersionDTO versionDTO;

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
            
            TemplateContainer T_Installer = root.Q<TemplateContainer>("T_Installer");
            TextField TF_Installer = T_Installer.Q<TextField>();
            Button B_Installer = T_Installer.Q<Button>();
            TemplateContainer T_License = root.Q<TemplateContainer>("T_License");
            TextField TF_License = T_License.Q<TextField>();
            Button B_License = T_License.Q<Button>();
            ListView LV_Targets = root.Q<ListView>("LV_Targets");
            ListView LV_Scenes = root.Q<ListView>("LV_Scenes");

            // Build name.
            TF_BuildName.value = PlayerSettings.productName;
            TF_BuildName.RegisterValueChangedCallback(value =>
            {
                PlayerSettings.productName = value.newValue;
            });

            UMI3DBuildToolVersionView versionView = new(
                root, 
                buildToolVersion_SO, 
                newVersion =>
                {
                    versionDTO = newVersion;
                    UpdateVersion();
                }
            );
            versionView.Bind();
            versionView.Set();
            
            // Scenes.
            LV_Scenes.reorderable = true;
            LV_Scenes.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            LV_Scenes.showFoldoutHeader = true;
            LV_Scenes.headerTitle = "Scenes";
            LV_Scenes.showAddRemoveFooter = true;
            LV_Scenes.reorderMode = ListViewReorderMode.Animated;
            LV_Scenes.itemsSource = buildToolScene_SO.scenes;
            LV_Scenes.makeItem = () =>
            {
                return scene_VTA.Instantiate(); ;
            };
            LV_Scenes.bindItem = (visual, index) =>
            {
                UMI3DBuildToolSceneView sceneView = new(
                    root: visual,
                    buildToolScene_SO: buildToolScene_SO,
                    index: index
                );
                sceneView.Bind();
                sceneView.Set();
            };

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
                        targetDTO = newTarget;
                        ApplyChange();
                    }, 
                    build: Build
                );
                targetView.Bind();
                targetView.Set();
            };
        } 
        
        private void ApplyChange()
        {
            _uMI3DConfigurator.HandleTarget(targetDTO.Target);
            _targetAndPluginSwitcher.HandleTarget(targetDTO.Target);
            EditorBuildSettings.scenes = buildToolScene_SO.GetScenesForTarget(targetDTO.Target).Select(scene =>
            {
                return new EditorBuildSettingsScene(scene.path, true);
            }).ToArray();
        }

        void UpdateVersion()
        {
            PlayerSettings.bundleVersion = versionDTO.VersionFromNow;
            PlayerSettings.Android.bundleVersionCode = versionDTO.BundleVersion;
        }

        private void Build()
        {
            var report = BuildPipeline.BuildPlayer(
                BuildToolHelper.GetPlayerBuildOptions(
                    versionDTO, 
                    targetDTO
                )
            );

            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
            }
            else if (summary.result == BuildResult.Failed)
            {
                Debug.Log($"Build failed: {summary.outputPath}");
            }
        }
    }
}
