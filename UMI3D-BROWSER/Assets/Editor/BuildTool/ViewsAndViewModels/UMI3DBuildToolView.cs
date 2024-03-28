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
        public Action<TargetDto> buildTarget;
        public Action applyScenes;
        public Action<TargetDto> applyTargetOptions;
        public Action<TargetDto[]> buildSelectedTarget;

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
            Action applyScenes,
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
            this.applyScenes = applyScenes;
            this.applyTargetOptions = applyTargetOptions;
            this.buildSelectedTarget = buildSelectedTarget;
            this.buildTarget = buildTarget;
        }

        public void Bind()
        {
            root.Add(ui.Instantiate());
        }

        public void Set()
        {
            UMI3DBuildToolVersionView versionView = new(
                root,
                buildToolVersion_SO,
                buildToolSettings_SO
            );
            versionView.Bind();
            versionView.Set();

            // Scenes.
            var scenePanel 
                = new UMI3DBuildToolScenesContainerView(
                    root,
                    buildToolScene_SO,
                    scene_VTA,
                    applyScenes
                );
            scenePanel.Bind();
            scenePanel.Set();

            // Path
            var configurationPanel 
                = new UMI3DBuildToolConfigurationPanelView(
                root,
                buildToolTarget_SO,
                buildToolSettings_SO
            );
            configurationPanel.Bind();
            configurationPanel.Set();

            // Targets.
            var targetsContainer 
                = new UMI3DBuildToolTargetsContainerView(
                root,
                buildToolScene_SO,
                buildToolTarget_SO,
                buildToolVersion_SO,
                buildToolSettings_SO,
                buildTarget,
                target_VTA
            );
            targetsContainer.Bind();
            targetsContainer.Set();

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