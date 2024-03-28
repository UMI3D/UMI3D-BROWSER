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
        public VisualElement root;
        public UMI3DBuildToolVersion_SO buildToolVersion_SO;
        public UMI3DBuildToolScene_SO buildToolScene_SO;
        public UMI3DBuildToolTarget_SO buildToolTarget_SO;
        public UMI3DBuildToolSettings_SO buildToolSettings_SO;

        public UMI3DBuildToolVersionView versionView;
        public UMI3DBuildToolScenesContainerView sceneContainerView;
        public UMI3DBuildToolTargetsContainerView targetsContainerView;

        public UMI3DBuildToolMainPanelView(
            VisualElement root,
            UMI3DBuildToolVersion_SO buildToolVersion_SO,
            UMI3DBuildToolScene_SO buildToolScene_SO,
            UMI3DBuildToolTarget_SO buildToolTarget_SO
        )
        {
            this.root = root;
            this.buildToolVersion_SO = buildToolVersion_SO;
            this.buildToolScene_SO = buildToolScene_SO;
            this.buildToolTarget_SO = buildToolTarget_SO;

            versionView = new(
                root,
                buildToolVersion_SO,
                buildToolSettings_SO
            );
            sceneContainerView = new(
                root,
                buildToolScene_SO,
                null,
                null
            );
            targetsContainerView = new(
                root,
                buildToolScene_SO,
                buildToolTarget_SO,
                buildToolVersion_SO,
                buildToolSettings_SO,
                null,
                null
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