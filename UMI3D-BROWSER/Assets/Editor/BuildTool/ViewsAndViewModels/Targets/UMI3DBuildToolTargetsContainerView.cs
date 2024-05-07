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
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.browserEditor.BuildTool
{
    public class UMI3DBuildToolTargetsContainerView 
    {
        umi3d.debug.UMI3DLogger logger;
        SubGlobal subGlobal = new("BuildTool");

        public VisualElement root;

        public UMI3DBuildToolTarget_SO targetModel;

        public DropdownField DD_CurrentTarget;
        public Button B_ApplyCurrentTarget;
        public ListView LV_Targets;
        public Button B_Build;

        public UMI3DBuildToolTargetsContainerView(VisualElement root)
        {
            logger = new(mainTag: nameof(UMI3DBuildToolTargetsContainerView));

            this.root = root;

            logger.Assert(root != null, nameof(UMI3DBuildToolTargetsContainerView));

            subGlobal.TryGet(out targetModel);
            targetModel.selectedTargetsChanged += OnUpdateTargetSelected;
        }

        public void Bind()
        {
            DD_CurrentTarget = root.Q<DropdownField>("DD_CurrentTarget");
            B_ApplyCurrentTarget = root.Q<Button>("B_ApplyCurrentTarget");
            B_ApplyCurrentTarget.clicked += ApplyTargetOption;
            LV_Targets = root.Q<ListView>("LV_Targets");
            B_Build = root.Q<Button>("B_Build");
            B_Build.clicked += BuildSelectedTarget;
            DD_CurrentTarget.RegisterValueChangedCallback(CurrentTargetValueChanged);
            LV_Targets.itemsAdded += TargetItemAdded;
        }

        public void Set()
        {
            DD_CurrentTarget.choices.Clear();
            DD_CurrentTarget.choices.AddRange(Enum.GetNames(typeof(E_Target)));
            DD_CurrentTarget.SetValueWithoutNotify(targetModel.currentTarget.ToString());
            ApplyTargetOption();

            LV_Targets.reorderable = true;
            LV_Targets.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            LV_Targets.showFoldoutHeader = true;
            LV_Targets.showAddRemoveFooter = true;
            LV_Targets.reorderMode = ListViewReorderMode.Animated;
            LV_Targets.itemsSource = targetModel.targets;
            LV_Targets.makeItem = () =>
            {
                return targetModel.target_VTA.Instantiate();
            };
            LV_Targets.bindItem = (visual, index) =>
            {
                UMI3DBuildToolTargetView targetView = new(
                    root: visual,
                    index
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
            LV_Targets.Q<Toggle>().value = false;

            OnUpdateTargetSelected();
        }

        public void Unbind()
        {
            B_ApplyCurrentTarget.clicked -= ApplyTargetOption;
            B_Build.clicked -= BuildSelectedTarget;
            DD_CurrentTarget.UnregisterValueChangedCallback(CurrentTargetValueChanged);
            LV_Targets.itemsAdded -= TargetItemAdded;
        }

        void CurrentTargetValueChanged(ChangeEvent<string> value)
        {
            ApplyTargetOption();
        }

        void ApplyTargetOption()
        {
            E_Target target = Enum.Parse<E_Target>(DD_CurrentTarget.value);
            targetModel.ApplyCurrentTarget(target);
        }

        void TargetItemAdded(IEnumerable<int> indexes)
        {
            foreach (var index in indexes)
            {
                targetModel.UpdateTarget(index, target =>
                {
                    target.BuildFolder = targetModel.buildFolder;
                    target.Target = E_Target.Quest;
                    return target;
                });
            }
        }

        void OnUpdateTargetSelected()
        {
            TargetDto[] targets = targetModel.SelectedTargets;
            string[] targetsDesc = targets.Select(
                target =>
                {
                    return $"{target.Target}:{target.releaseCycle}";
                }
            ).ToArray();
            LV_Targets.headerTitle = $"Targets: {string.Join(" - ", targetsDesc)}";
            B_Build.text = $"BUILD : {targets.Length} selected target(s)";
            B_Build.SetEnabled(targets.Length != 0);
        }

        void BuildSelectedTarget()
        {
            targetModel.BuildSelectedTargets();
        }
    }
}