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

namespace umi3d.browserEditor.BuildTool
{
    public class UMI3DBuildToolTargetViewModel 
    {
        public UMI3DBuildToolTarget_SO buildToolTarget_SO;
        public Action<TargetDto> updateTarget;
        public Action<int> rebuildView;

        public TargetDto this[int index]
        {
            get
            {
                return buildToolTarget_SO.targets[index];
            }
        }

        public int Count
        {
            get
            {
                return buildToolTarget_SO.targets.Count;
            }
        }

        public UMI3DBuildToolTargetViewModel(UMI3DBuildToolTarget_SO buildToolTarget_SO, Action<TargetDto> updateTarget, Action<int> rebuildView)
        {
            this.buildToolTarget_SO = buildToolTarget_SO;
            this.updateTarget = updateTarget;
            this.rebuildView = rebuildView;
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
                    rebuildView?.Invoke(_index);
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
            Save();
        }

        public void ApplyBuildFolder(int index, string buildFolder)
        {
            var targetDTO = buildToolTarget_SO.targets[index];
            targetDTO.BuildFolder = buildFolder;
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