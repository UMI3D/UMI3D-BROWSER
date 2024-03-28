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
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.browserEditor.BuildTool
{
    public class UMI3DBuildToolScenePanelView 
    {
        public VisualElement root;
        public UMI3DBuildToolScene_SO buildToolScene_SO;
        public VisualTreeAsset scene_VTA = default;
        public Action applyScenes;

        public UMI3DBuildToolPanelView panelView;
        public EnumFlagsField EFF_Filter 
            = new(
                "Filter on targets", 
                E_Target.Quest
            );
        public ListView LV_Scenes;

        public UMI3DBuildToolScenePanelView(
            VisualElement root,
            UMI3DBuildToolScene_SO buildToolScene_SO,
            VisualTreeAsset scene_VTA,
            Action applyScenes
        )
        {
            this.root = root;
            this.buildToolScene_SO = buildToolScene_SO;
            this.scene_VTA = scene_VTA;
            this.applyScenes = applyScenes;
            panelView = new(root);
        }

        public void Bind()
        {
            panelView.Bind();
            LV_Scenes = root.Q<ListView>("LV_Scenes");
        }

        public void Set()
        {
            panelView.SetTitle("Scene Panel");

            // Todo add a filter.
            //root.Add(EFF_Filter);
            //EFF_Filter.SetValueWithoutNotify(viewModel[index].targets);
            //EFF_Filter.RegisterValueChangedCallback(TargetValueChanged);

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
                    index: index,
                    applyScenes
                );
                sceneView.Bind();
                sceneView.Set();
                visual.userData = sceneView;
            };
        }

        public void Unbind() 
        {
            panelView.UnBind();
        }
    }
}