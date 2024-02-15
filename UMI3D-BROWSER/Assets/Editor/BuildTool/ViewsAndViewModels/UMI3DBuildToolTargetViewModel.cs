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

namespace umi3d.browserEditor.BuildTool
{
    public class UMI3DBuildToolTargetViewModel 
    {
        public UMI3DBuildToolTarget_SO buildToolTarget_SO;
        public Action<TargetDto> updateTarget;
        public Action<int> refreshView;

        public TargetDto this[int index]
        {
            get
            {
                return buildToolTarget_SO.targets[index];
            }
            set
            {
                buildToolTarget_SO.targets[index] = value;
            }
        }

        public int Count
        {
            get
            {
                return buildToolTarget_SO.targets.Count;
            }
        }

        public UMI3DBuildToolTargetViewModel(UMI3DBuildToolTarget_SO buildToolTarget_SO, Action<TargetDto> updateTarget, Action<int> refreshView)
        {
            this.buildToolTarget_SO = buildToolTarget_SO;
            this.updateTarget = updateTarget;
            this.refreshView = refreshView;
        }

        public void ApplyChange(int index, bool isApplied)
        {
            void InternalApplyChange(int _index, bool isApplied)
            {
                var targetDTO = buildToolTarget_SO.targets[_index];
                targetDTO.isApplied = isApplied;
                buildToolTarget_SO.targets[_index] = targetDTO;
                if (isApplied)
                {
                    updateTarget?.Invoke(this[_index]);
                    refreshView?.Invoke(_index);
                }
            }
            
            if (isApplied)
            {
                for (int i = 0; i < Count; i++)
                {
                    if (i != index)
                    {
                        InternalApplyChange(i, false);
                    }
                }
            }
            InternalApplyChange(index, isApplied);

            Save();
        }

        public void Select(int index, bool isSelected)
        {
            var targetDTO = buildToolTarget_SO.targets[index];
            targetDTO.IsTargetEnabled = isSelected;
            buildToolTarget_SO.targets[index] = targetDTO;
            buildToolTarget_SO.SelectedTargetsChanged?.Invoke();
            Save();
        }

        public void BrowseBuildFolder(int index, Action<string> updateView)
        {
            var folder = string.IsNullOrEmpty(buildToolTarget_SO.installer)
                ? Application.dataPath
                : this[index].BuildFolder;

            string path = EditorUtility.OpenFolderPanel(
                title: "Build folder",
                folder,
                defaultName: ""
            );

            UpdateBuildFolder(index, path);
            updateView?.Invoke(path);
        }

        public void UpdateBuildFolder(int index, string path)
        {
            var targetDTO = buildToolTarget_SO.targets[index];
            targetDTO.BuildFolder = path;
            buildToolTarget_SO.targets[index] = targetDTO;
            Save();
        }

        public void ApplyTarget(int index, E_Target target)
        {
            var targetDTO = buildToolTarget_SO.targets[index];
            targetDTO.Target = target;
            buildToolTarget_SO.targets[index] = targetDTO;
            Save();
        }

        public void ApplyReleaseCycle(int index, E_ReleaseCycle releaseCycle)
        {
            var target = buildToolTarget_SO.targets[index];
            target.releaseCycle = releaseCycle;
            buildToolTarget_SO.targets[index] = target;
            Save();
        }

        public void Save()
        {
            EditorUtility.SetDirty(buildToolTarget_SO);
        }
    }
}