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
using UnityEditor;
using UnityEngine;

namespace umi3d.browserEditor.BuildTool
{
    [CreateAssetMenu(fileName = "UMI3D Build Tool Target", menuName = "UMI3D/Build Tools/Build Tool Target")]
    public class UMI3DBuildToolTarget_SO : SerializableScriptableObject
    {
        public Action SelectedTargetsChanged;

        public string installer;
        public string license;
        public string buildFolder;
        public E_Target currentTarget;
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

        public TargetDto[] GetSelectedTargets(BuildTarget buildTarget, E_ReleaseCycle releaseCycle)
        {
            return targets.Where(target =>
            {
                switch (target.Target)
                {
                    case E_Target.Quest:
                    case E_Target.Focus:
                    case E_Target.Pico:
                        if (buildTarget != BuildTarget.Android)
                        {
                            return false;
                        }
                        break;
                    case E_Target.SteamXR:
                        if (buildTarget != BuildTarget.StandaloneWindows)
                        {
                            return false;
                        }
                        break;
                    default:
                        return false;
                }

                return target.IsTargetEnabled && target.releaseCycle == releaseCycle;
            }).ToArray();
        }
    }
}
