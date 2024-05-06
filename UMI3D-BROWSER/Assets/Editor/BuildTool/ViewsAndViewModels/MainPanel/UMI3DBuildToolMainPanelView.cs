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
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.browserEditor.BuildTool
{
    public class UMI3DBuildToolMainPanelView 
    {
        umi3d.debug.UMI3DLogger logger;

        public VisualElement root;

        public UMI3DBuildToolVersionView versionView;
        public UMI3DBuildToolScenesContainerView sceneContainerView;
        public UMI3DBuildToolTargetsContainerView targetsContainerView;

        public UMI3DBuildToolMainPanelView(
            VisualElement root,
            VisualTreeAsset scene_VTA,
            Action applyScenes,
            VisualTreeAsset target_VTA,
            Action<E_Target> applyTargetOptions,
            Action<TargetDto[]> buildSelectedTarget
        )
        {
            logger = new(mainTag: nameof(UMI3DBuildToolMainPanelView));

            this.root = root;

            logger.Assert(root != null, nameof(UMI3DBuildToolMainPanelView));

            versionView = new(
                root.Q<TemplateContainer>("UMI3DBuildToolVersion")
            );
            sceneContainerView = new(
                root,
                scene_VTA,
                applyScenes
            );
            targetsContainerView = new(
                root,
                target_VTA,
                applyTargetOptions,
                buildSelectedTarget
            );
        }

        public void Bind()
        {
            versionView.Bind();
            sceneContainerView.Bind();
            targetsContainerView.Bind();
        }

        public void Set()
        {
            versionView.Set();
            sceneContainerView.Set();
            targetsContainerView.Set();
        }

        public void Unbind()
        {
            sceneContainerView.Unbind();
            targetsContainerView.Unbind();
        }
    }
}