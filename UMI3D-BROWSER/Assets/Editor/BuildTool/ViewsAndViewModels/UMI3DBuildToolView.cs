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
using static umi3d.browserEditor.BuildTool.UMI3DBuildTool;

namespace umi3d.browserEditor.BuildTool
{
    public class UMI3DBuildToolView
    {
        public VisualElement root;

        public UMI3DBuildToolMainPanelView mainPanelView;
        public UMI3DBuildToolConfigurationPanelView configurationPanel;

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
            Action<E_Target> applyTargetOptions,
            Action<TargetDto[]> buildSelectedTarget
        )
        {
            this.root = root;

            root.Add(ui.Instantiate());

            mainPanelView = new(
                root.Q<TemplateContainer>("UMI3DBuildToolMainPanel"),
                buildToolVersion_SO,
                buildToolScene_SO,
                buildToolTarget_SO,
                buildToolSettings_SO,
                scene_VTA,
                applyScenes,
                target_VTA,
                applyTargetOptions,
                buildSelectedTarget
            );

            configurationPanel = new UMI3DBuildToolConfigurationPanelView(
                root.Q<TemplateContainer>("UMI3DBuildToolConfigurationPanel"),
                buildToolTarget_SO,
                buildToolKeystore_SO,
                buildToolSettings_SO
            );
        }

        public void Bind()
        {
            mainPanelView.Bind();
            configurationPanel.Bind();
        }

        public void Set()
        {
            mainPanelView.Set();

            configurationPanel.Set();
        }

        public void ChangePanel(E_BuildToolPanel panel)
        {
            switch (panel)
            {
                case E_BuildToolPanel.Main:
                    mainPanelView.root.style.display = DisplayStyle.Flex;
                    configurationPanel.root.style.display = DisplayStyle.None;
                    break;
                case E_BuildToolPanel.Configuration:
                    mainPanelView.root.style.display = DisplayStyle.None;
                    configurationPanel.root.style.display = DisplayStyle.Flex;
                    break;
                case E_BuildToolPanel.History:
                    break;
                case E_BuildToolPanel.Build:
                    break;
                default:
                    break;
            }
        }
    }
}