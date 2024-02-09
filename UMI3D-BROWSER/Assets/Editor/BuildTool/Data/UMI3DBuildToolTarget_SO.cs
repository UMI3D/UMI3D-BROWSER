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
using inetum.unityUtils.saveSystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace umi3d.browserEditor.BuildTool
{
    //[CreateAssetMenu(fileName = "UMI3D Build Tool", menuName = "UMI3D/Tools/Build Tool")]
    public class UMI3DBuildToolTarget_SO : SerializableScriptableObject
    {
        public Action SelectedTargetsChanged;

        public string installer;
        public string license;
        public List<TargetDto> targets = new();

        public TargetDto[] SelectedTargets
        {
            get
            {
                return targets.Where(target =>
                {
                    return target.IsTargetEnabled;
                }).ToArray();
            }
        }
    }
}
