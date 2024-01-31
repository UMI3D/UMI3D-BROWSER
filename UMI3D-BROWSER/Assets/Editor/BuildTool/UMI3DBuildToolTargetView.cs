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

using Newtonsoft.Json.Bson;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.browserEditor.BuildTool
{
    public class UMI3DBuildToolTargetView 
    {
        public VisualElement root;
        public UMI3DBuildToolTarget_SO buildToolTarget_SO;
        public UMI3DBuildToolVersion_SO buildToolVersion_SO;
        public int index;
        public Action<E_Target> updateTarget;
        public Action build;

        public VisualElement V_Path;
        public TextField TF_Path;
        public Button B_Browse;
        public DropdownField DD_TargetSelection;
        public DropdownField DD_ReleaseCycle;
        public Button B_Apply;
        public Button B_Build;

        public UMI3DBuildToolTargetView(VisualElement root, UMI3DBuildToolTarget_SO buildToolTarget_SO, UMI3DBuildToolVersion_SO buildToolVersion_SO, int index, Action<E_Target> updateTarget, Action build)
        {
            this.root = root;
            this.buildToolTarget_SO = buildToolTarget_SO;
            this.buildToolVersion_SO = buildToolVersion_SO;
            this.index = index;
            this.updateTarget = updateTarget;

        }

        public void Bind()
        {
            V_Path = root.Q("V_Path");
            TF_Path = V_Path.Q<TextField>();
            B_Browse = V_Path.Q<Button>();
            DD_TargetSelection = root.Q<DropdownField>("DD_TargetSelection");
            DD_ReleaseCycle = root.Q<DropdownField>("DD_ReleaseCycle");
            B_Apply = root.Q<Button>("B_Apply");
            B_Build = root.Q<Button>("B_Build");
        }

        public void Set()
        {
            // Path
            TF_Path.label = "Build Folder";
            B_Browse.clicked += () =>
            {
                UnityEngine.Debug.Log($"Todo");
            };

            // Device target.
            DD_TargetSelection.choices.Clear();
            DD_TargetSelection.choices.AddRange(Enum.GetNames(typeof(E_Target)));
            DD_TargetSelection.value = DD_TargetSelection.choices[(int)buildToolTarget_SO.targets[index].Target];
            DD_TargetSelection.RegisterValueChangedCallback(value =>
            {
                var target = buildToolTarget_SO.targets[index];
                target.Target = Enum.Parse<E_Target>(value.newValue);
                buildToolTarget_SO.targets[index] = target;
                ApplyChangeView(false);
            });

            // Release cycle.
            DD_ReleaseCycle.choices.Clear();
            DD_ReleaseCycle.choices.AddRange(Enum.GetNames(typeof(E_ReleaseCycle)));
            DD_ReleaseCycle.value = DD_ReleaseCycle.choices[(int)buildToolTarget_SO.targets[index].releaseCycle];
            DD_ReleaseCycle.RegisterValueChangedCallback(value =>
            {
                var target = buildToolTarget_SO.targets[index];
                target.releaseCycle = Enum.Parse<E_ReleaseCycle>(value.newValue);
                buildToolTarget_SO.targets[index] = target;
                ApplyChangeView(false);
            });

            ApplyChangeView(buildToolTarget_SO.targets[index].isApplied);

            B_Apply.clicked += ApplyChange;
            B_Build.clicked += () =>
            {
                buildToolVersion_SO.UpdateOldVersionWithNewVersion();
                build?.Invoke();
            };
        }

        public void ApplyChange()
        {
            for (int i = 0; i < buildToolTarget_SO.targets.Count; i++)
            {
                if (i != index)
                {
                    var target = buildToolTarget_SO.targets[i];
                    target.isApplied = false;
                    buildToolTarget_SO.targets[i] = target;
                }
            }
            ApplyChangeView(true);
            updateTarget?.Invoke(buildToolTarget_SO.targets[index].Target);
        }

        void ApplyChangeView(bool isApplied)
        {
            B_Apply.SetEnabled(!isApplied);
            B_Build.SetEnabled(isApplied);
            var target = buildToolTarget_SO.targets[index];
            target.isApplied = isApplied;
            buildToolTarget_SO.targets[index] = target;
        }
    }
}