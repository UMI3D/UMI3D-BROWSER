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
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.browserEditor.BuildTool
{
    public class UMI3DBuildToolTargetsContainerView 
    {
        umi3d.debug.UMI3DLogger logger;

        public VisualElement root;
        public UMI3DBuildToolScene_SO buildToolScene_SO;
        public UMI3DBuildToolTarget_SO buildToolTarget_SO;
        public UMI3DBuildToolVersion_SO buildToolVersion_SO;
        public UMI3DBuildToolSettings_SO buildToolSettings_SO;
        public VisualTreeAsset target_VTA;
        public Action<E_Target> applyTargetOptions;
        public Action<TargetDto[]> buildSelectedTarget;

        public DropdownField DD_CurrentTarget;
        public Button B_ApplyCurrentTarget;
        public ListView LV_Targets;
        public Button B_Build;

        public UMI3DBuildToolTargetsContainerView(
            VisualElement root,
            UMI3DBuildToolScene_SO buildToolScene_SO,
            UMI3DBuildToolTarget_SO buildToolTarget_SO,
            UMI3DBuildToolVersion_SO buildToolVersion_SO,
            UMI3DBuildToolSettings_SO buildToolSettings_SO,
            VisualTreeAsset target_VTA,
            Action<E_Target> applyTargetOptions,
            Action<TargetDto[]> buildSelectedTarget
        )
        {
            logger = new(mainTag: nameof(UMI3DBuildToolTargetsContainerView));

            this.root = root;
            this.buildToolScene_SO = buildToolScene_SO;
            this.buildToolTarget_SO = buildToolTarget_SO;
            this.buildToolVersion_SO = buildToolVersion_SO;
            this.buildToolSettings_SO = buildToolSettings_SO;
            this.target_VTA = target_VTA;
            this.applyTargetOptions = applyTargetOptions;
            this.buildSelectedTarget = buildSelectedTarget;

            logger.Assert(root != null, nameof(UMI3DBuildToolTargetsContainerView));
            logger.Assert(buildToolScene_SO != null, nameof(UMI3DBuildToolTargetsContainerView));
            logger.Assert(buildToolTarget_SO != null, nameof(UMI3DBuildToolTargetsContainerView));
            logger.Assert(buildToolVersion_SO != null, nameof(UMI3DBuildToolTargetsContainerView));
            logger.Assert(buildToolSettings_SO != null, nameof(UMI3DBuildToolTargetsContainerView));
            logger.Assert(target_VTA != null, nameof(UMI3DBuildToolTargetsContainerView));
            logger.Assert(buildSelectedTarget != null, nameof(UMI3DBuildToolTargetsContainerView));
        }

        public void Bind()
        {
            this.buildToolTarget_SO.SelectedTargetsChanged += () =>
            {
                OnUpdateTargetSelected(buildToolTarget_SO.SelectedTargets);
            };
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
            DD_CurrentTarget.SetValueWithoutNotify(buildToolTarget_SO.currentTarget.ToString());
            ApplyTargetOption();

            LV_Targets.reorderable = true;
            LV_Targets.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            LV_Targets.showFoldoutHeader = true;
            LV_Targets.headerTitle = "Targets";
            LV_Targets.showAddRemoveFooter = true;
            LV_Targets.reorderMode = ListViewReorderMode.Animated;
            LV_Targets.itemsSource = buildToolTarget_SO.targets;
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

            OnUpdateTargetSelected(buildToolTarget_SO.SelectedTargets);
        }

        public void Unbind()
        {
            this.buildToolTarget_SO.SelectedTargetsChanged -= () =>
            {
                OnUpdateTargetSelected(buildToolTarget_SO.SelectedTargets);
            };
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
            buildToolTarget_SO.currentTarget = target;
            applyTargetOptions?.Invoke(target);

            EditorUtility.SetDirty(buildToolTarget_SO);
        }

        void TargetItemAdded(IEnumerable<int> indexes)
        {
            foreach (var index in indexes)
            {
                var target = buildToolTarget_SO.targets[index];
                target.BuildFolder = buildToolTarget_SO.buildFolder;
                target.Target = E_Target.Quest;
                buildToolTarget_SO.targets[index] = target;
            }
        }

        void BuildSelectedTarget()
        {
            //var currentTarget = 

            E_ReleaseCycle[] releases = (E_ReleaseCycle[])Enum.GetValues(typeof(E_ReleaseCycle));
            for (int i = releases.Length - 1; i >= 0; i--)
            {
                buildSelectedTarget?.Invoke(
                    buildToolTarget_SO.GetSelectedTargets(
                        BuildTarget.Android,
                        releases[i]
                    )
                );
            }

            for (int i = releases.Length - 1; i >= 0; i--)
            {
                buildSelectedTarget?.Invoke(
                    buildToolTarget_SO.GetSelectedTargets(
                        BuildTarget.StandaloneWindows,
                        releases[i]
                    )
                );
            }
        }

        void OnUpdateTargetSelected(params TargetDto[] targets)
        {
            B_Build.text = "BUILD all selected targets";
        }
    }
}