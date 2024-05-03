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

using inetum.unityUtils;
using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.browserEditor.BuildTool
{
    public class UMI3DBuildToolScenesContainerView 
    {
        SubGlobal subGlobal = new("BuildTool");

        public VisualElement root;
        public UMI3DBuildToolScene_SO buildToolScene_SO;
        public VisualTreeAsset scene_VTA;
        public Action applyScenes;

        public ListView LV_Scenes;

        public UMI3DBuildToolScenesContainerView(
            VisualElement root,
            VisualTreeAsset scene_VTA,
            Action applyScenes
        )
        {
            this.root = root;
            this.scene_VTA = scene_VTA;
            this.applyScenes = applyScenes;

            subGlobal.TryGet(out buildToolScene_SO);
        }

        public void Bind()
        {
            LV_Scenes = root.Q<ListView>("LV_Scenes");
        }

        public void Set()
        {
            LV_Scenes.reorderable = true;
            LV_Scenes.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            LV_Scenes.showFoldoutHeader = true;
            LV_Scenes.headerTitle = "Scenes";
            LV_Scenes.showAddRemoveFooter = true;
            LV_Scenes.reorderMode = ListViewReorderMode.Animated;
            LV_Scenes.itemsSource = buildToolScene_SO.scenes;
            LV_Scenes.makeItem = () =>
            {
                var visual = scene_VTA.Instantiate();
                UMI3DBuildToolSceneView sceneView = new(applyScenes);
                visual.userData = sceneView;
                return visual;
            };
            LV_Scenes.bindItem = (visual, index) =>
            {
                UMI3DBuildToolSceneView sceneView 
                    = visual.userData as UMI3DBuildToolSceneView;
                sceneView.root = visual;
                sceneView.index = index;
                sceneView.Bind();
                sceneView.Set();
            };
            LV_Scenes.unbindItem = (visual, index) =>
            {
                UMI3DBuildToolSceneView sceneView 
                    = visual.userData as UMI3DBuildToolSceneView;
                sceneView.Unbind();
            };
            LV_Scenes.Q<Toggle>().value = false;
        }

        public void Unbind() 
        {
        }
    }
}