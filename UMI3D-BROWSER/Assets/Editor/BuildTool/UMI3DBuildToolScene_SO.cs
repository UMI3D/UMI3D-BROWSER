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
using UnityEditor;

namespace umi3d.browserEditor.BuildTool
{
    //[CreateAssetMenu(fileName = "UMI3D Build Tool Scenes", menuName = "UMI3D/Tools/Build Tool Scenes")]
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

        //public void UpdateScenes()
        //{
        //    var _scenes = new List<SceneDTO>();
        //    for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        //    {
        //        var scene = new SceneDTO();
        //        scene.path = EditorBuildSettings.scenes[i].path;
        //        scene.name = System.IO.Path.GetFileNameWithoutExtension(scene.path);
        //        scene.index = i;
        //        scene.enabled = EditorBuildSettings.scenes[0].enabled;

        //        var sceneFound = scenes.Find(_scene =>  _scene.path == scene.path);
        //        if (!string.IsNullOrEmpty(sceneFound.path))
        //        {
        //            scene.targets = sceneFound.targets;
        //        }

        //        _scenes.Add(scene);
        //    }

        //    scenes = _scenes;
        //} 
    }
}