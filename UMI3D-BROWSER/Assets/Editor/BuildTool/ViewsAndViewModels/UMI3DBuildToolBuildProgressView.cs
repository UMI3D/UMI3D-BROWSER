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
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.browserEditor.BuildTool
{
    public class UMI3DBuildToolBuildProgressView 
    {
        public VisualElement root;
        public UMI3DBuildToolTarget_SO buildToolTarget_SO;

        Button B_BuildAllSelected;
        Button B_Abort;
        ProgressBar PB_Progress;

        public UMI3DBuildToolBuildProgressView(VisualElement root, UMI3DBuildToolTarget_SO buildToolTarget_SO)
        {
            this.root = root;
            this.buildToolTarget_SO = buildToolTarget_SO;
            this.buildToolTarget_SO.SelectedTargetsChanged += () =>
            {
                OnUpdateTargetSelected(buildToolTarget_SO.SelectedTargets);
            };
        }

        public void Bind()
        {
            B_BuildAllSelected = root.Q<Button>("B_BuildAllSelected");
            B_Abort = root.Q<Button>("B_Abort");
            PB_Progress = root.Q<ProgressBar>();
        }

        public void Set()
        {
            B_Abort.SetEnabled(false);
            OnUpdateTargetSelected(buildToolTarget_SO.SelectedTargets);
            B_BuildAllSelected.clicked += () =>
            {

            };
        }

        void OnUpdateTargetSelected(params TargetDto[] targets)
        {
            B_BuildAllSelected.text = "BUILD all selected targets";
            PB_Progress.value = 0;
            PB_Progress.title = $"{targets.Length} target(s) to be built";
        }
    }
}