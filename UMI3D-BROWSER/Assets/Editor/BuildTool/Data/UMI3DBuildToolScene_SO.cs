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

using inetum.unityUtils.saveSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace umi3d.browserEditor.BuildTool
{
    [CreateAssetMenu(fileName = "UMI3D Build Tool Scenes", menuName = "UMI3D/Build Tools/Build Tool Scenes")]
    public class UMI3DBuildToolScene_SO : SerializableScriptableObject
    {
        public List<SceneDTO> scenes;

        public SceneDTO[] GetScenesForTarget(E_Target target)
        {
            return scenes.Where(scene =>
            {
                return scene.targets.HasFlag(target);
            }).ToArray();
        }
    }
}