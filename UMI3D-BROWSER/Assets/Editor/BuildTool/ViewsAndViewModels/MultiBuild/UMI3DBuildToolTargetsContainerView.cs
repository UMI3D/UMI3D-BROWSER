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
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.browserEditor.BuildTool
{
    public class UMI3DBuildToolTargetsContainerView 
    {
        public VisualElement root;
        public UMI3DBuildToolScene_SO buildToolScene_SO;
        public UMI3DBuildToolTarget_SO buildToolTarget_SO;
        public UMI3DBuildToolVersion_SO buildToolVersion_SO;
        public UMI3DBuildToolSettings_SO buildToolSettings_SO;
        public Action<TargetDto> buildTarget;
        public VisualTreeAsset target_VTA;

        public ListView LV_Targets;

        public UMI3DBuildToolTargetsContainerView(
            VisualElement root,
            UMI3DBuildToolScene_SO buildToolScene_SO,
            UMI3DBuildToolTarget_SO buildToolTarget_SO,
            UMI3DBuildToolVersion_SO buildToolVersion_SO,
            UMI3DBuildToolSettings_SO buildToolSettings_SO,
            Action<TargetDto> buildTarget,
            VisualTreeAsset target_VTA
        )
        {
            this.root = root;
            this.buildToolScene_SO = buildToolScene_SO;
            this.buildToolTarget_SO = buildToolTarget_SO;
            this.buildToolVersion_SO = buildToolVersion_SO;
            this.buildToolSettings_SO = buildToolSettings_SO;
            this.buildTarget = buildTarget;
            this.target_VTA = target_VTA;
        }

        public void Bind()
        {
            LV_Targets = root.Q<ListView>("LV_Targets");
        }

        public void Set()
        {
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
                    null,
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
        }

        public void Unbind()
        {

        }
    }
}