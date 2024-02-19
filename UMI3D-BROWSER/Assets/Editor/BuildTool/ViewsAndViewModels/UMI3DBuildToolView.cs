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
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.browserEditor.BuildTool
{
    public class UMI3DBuildToolView
    {
        public VisualElement root;
        public VisualTreeAsset ui = default;
        public VisualTreeAsset target_VTA = default;
        public VisualTreeAsset path_VTA = default;
        public VisualTreeAsset scene_VTA = default;
        public UMI3DBuildToolKeystore_SO buildToolKeystore_SO;
        public UMI3DBuildToolVersion_SO buildToolVersion_SO;
        public UMI3DBuildToolTarget_SO buildToolTarget_SO;
        public UMI3DBuildToolScene_SO buildToolScene_SO;
        public UMI3DBuildToolSettings_SO buildToolSettings_SO;
        public Action<VersionDTO> updateVersion;
        public Action<TargetDto> updateTarget;
        Action<TargetDto> applyTargetOptions;
        public Action<TargetDto> buildTarget;
        public Action<TargetDto[]> buildSelectedTarget;

        public UMI3DBuildToolViewModel viewModel;
        public TemplateContainer T_Installer;
        public TextField TF_Installer;
        public Button B_Installer;
        public TemplateContainer T_License;
        public TextField TF_License;
        public Button B_License;
        public TemplateContainer T_BuildFolder;
        public TextField TF_BuildFolder;
        public Button B_BuildFolder;
        public ListView LV_Targets;
        public ListView LV_Scenes;

        public UMI3DBuildToolView(
            VisualElement root,
            VisualTreeAsset ui,
            VisualTreeAsset target_VTA,
            VisualTreeAsset path_VTA,
            VisualTreeAsset scene_VTA,
            UMI3DBuildToolKeystore_SO buildToolKeystore_SO,
            UMI3DBuildToolVersion_SO buildToolVersion_SO,
            UMI3DBuildToolTarget_SO buildToolTarget_SO,
            UMI3DBuildToolScene_SO buildToolScene_SO,
            UMI3DBuildToolSettings_SO buildToolSettings_SO,
            Action<VersionDTO> updateVersion,
            Action<TargetDto> updateTarget,
            Action<TargetDto> applyTargetOptions,
            Action<TargetDto> buildTarget,
            Action<TargetDto[]> buildSelectedTarget
        )
        {
            this.root = root;
            this.ui = ui;
            this.target_VTA = target_VTA;
            this.path_VTA = path_VTA;
            this.scene_VTA = scene_VTA;
            this.buildToolKeystore_SO = buildToolKeystore_SO;
            this.buildToolVersion_SO = buildToolVersion_SO;
            this.buildToolTarget_SO = buildToolTarget_SO;
            this.buildToolScene_SO = buildToolScene_SO;
            this.buildToolSettings_SO = buildToolSettings_SO;
            this.updateVersion = updateVersion;
            this.updateTarget = updateTarget;
            this.applyTargetOptions = applyTargetOptions;
            this.buildTarget = buildTarget;
            this.buildSelectedTarget = buildSelectedTarget;
            this.viewModel = new(buildToolTarget_SO);
        }

        public void Bind()
        {
            root.Add(ui.Instantiate());
            T_Installer = root.Q<TemplateContainer>("T_Installer");
            TF_Installer = T_Installer.Q<TextField>();
            B_Installer = T_Installer.Q<Button>();
            T_License = root.Q<TemplateContainer>("T_License");
            TF_License = T_License.Q<TextField>();
            B_License = T_License.Q<Button>();
            T_BuildFolder = root.Q<TemplateContainer>("T_SingleBuildFolder");
            TF_BuildFolder = T_BuildFolder.Q<TextField>();
            B_BuildFolder = T_BuildFolder.Q<Button>();
            LV_Targets = root.Q<ListView>("LV_Targets");
            LV_Scenes = root.Q<ListView>("LV_Scenes");
        }

        public void Set()
        {
            UMI3DBuildToolVersionView versionView = new(
                root,
                buildToolVersion_SO,
                buildToolSettings_SO,
                updateVersion: updateVersion
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
            (TF_Installer.labelElement as INotifyValueChanged<string>).SetValueWithoutNotify("Installer");
            TF_Installer.SetValueWithoutNotify(buildToolTarget_SO.installer);
            TF_Installer.RegisterValueChangedCallback(value =>
            {
                viewModel.UpdateInstaller(value.newValue);
            });
            B_Installer.clicked += () =>
            {
                viewModel.BrowseInstaller(path =>
                {
                    TF_Installer.SetValueWithoutNotify(path);
                });
            };
            (TF_License.labelElement as INotifyValueChanged<string>).SetValueWithoutNotify("License");
            TF_License.SetValueWithoutNotify(buildToolTarget_SO.license);
            TF_License.RegisterValueChangedCallback(value =>
            {
                viewModel.UpdateLicense(value.newValue);
            });
            B_License.clicked += () =>
            {
                viewModel.BrowseLicense(path =>
                {
                    TF_License.SetValueWithoutNotify(path);
                });
            };
            T_BuildFolder.style.display = (buildToolSettings_SO?.useOneBuildFolder ?? true)
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;
            (TF_BuildFolder.labelElement as INotifyValueChanged<string>).SetValueWithoutNotify("Build Folder");
            TF_BuildFolder.SetValueWithoutNotify(buildToolTarget_SO.buildFolder);
            TF_BuildFolder.RegisterValueChangedCallback(value =>
            {
                viewModel.UpdateBuildFolder(value.newValue);
            });
            B_BuildFolder.clicked += () =>
            {
                viewModel.BrowseBuildFolder(path =>
                {
                    TF_BuildFolder.SetValueWithoutNotify(path);
                });
            };

            // Targets.
            LV_Targets.reorderable = true;
            LV_Targets.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            LV_Targets.showFoldoutHeader = true;
            LV_Targets.headerTitle = "Targets";
            LV_Targets.showAddRemoveFooter = true;
            LV_Targets.reorderMode = ListViewReorderMode.Animated;
            LV_Targets.itemsSource = buildToolTarget_SO.targets;
            LV_Targets.itemsAdded += indexes =>
            {
                foreach (var index in indexes)
                {
                    var target = buildToolTarget_SO.targets[index];
                    target.BuildFolder = buildToolTarget_SO.buildFolder;
                    target.Target = E_Target.Quest;
                    buildToolTarget_SO.targets[index] = target;
                }
            };
            LV_Targets.makeItem = () =>
            {
                return target_VTA.Instantiate();
            };
            LV_Targets.bindItem = (visual, index) =>
            {
                UMI3DBuildToolTargetView targetView = new(
                    root: visual,
                    buildToolTarget_SO,
                    buildToolVersion_SO,
                    buildToolSettings_SO,
                    index,
                    updateTarget,
                    applyTargetOptions,
                    refreshView: index =>
                    {
                        for (int i = 0; i < buildToolTarget_SO.targets.Count; i++)
                        {
                            if (i != index)
                            {
                                LV_Targets.RefreshItem(i);
                            }
                        }
                    },
                    buildTarget
                );
                targetView.Bind();
                targetView.Set();
                visual.userData = targetView;
            };
            LV_Targets.unbindItem = (visual, index) =>
            {
                UMI3DBuildToolTargetView targetView = visual.userData as UMI3DBuildToolTargetView;
                targetView.Unbind();
            };

            UMI3DBuildToolBuildProgressView progressView = new(
                root,
                buildToolTarget_SO,
                buildSelectedTarget
            );
            progressView.Bind();
            progressView.Set();
        }
    }
}