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

namespace umi3d.browserEditor.BuildTool
{
    public class UMI3DBuildToolBuildProgressView 
    {
        public VisualElement root;
        public UMI3DBuildToolTarget_SO buildToolTarget_SO;

        Button B_BuildAllSelected;

        public UMI3DBuildToolBuildProgressView(
            VisualElement root,
            UMI3DBuildToolTarget_SO buildToolTarget_SO,
            Action<TargetDto[]> buildSelectedTarget
        )
        {
            this.root = root;
            this.buildToolTarget_SO = buildToolTarget_SO;
        }

        public void Bind()
        {
            B_BuildAllSelected = root.Q<Button>("B_BuildAllSelected");
        }

        public void Set()
        {
        }
    }
}