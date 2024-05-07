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
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.browserEditor.BuildTool
{
    [CreateAssetMenu(fileName = "UMI3D Build Tool Scenes", menuName = "UMI3D/Build Tools/Build Tool Scenes")]
    public class UMI3DBuildToolScene_SO : PersistentScriptableModel
    {
        public Action SelectedScenesChanged;

        public List<SceneDTO> scenes = new();
        public VisualTreeAsset scene_VTA;

        public SceneDTO this[int index]
        {
            get
            {
                return scenes[index];
            }
            set
            {
                scenes[index] = value;
            }
        }

        public int Count
        {
            get
            {
                return scenes.Count;
            }
        }

        public SceneDTO[] GetScenesForTarget(E_Target target)
        {
            return scenes.Where(scene =>
            {
                return scene.enabled && scene.targets.HasFlag(target);
            }).ToArray();
        }

        #region path

        public void UpdatedScenePath(int index, string path)
        {
            var indexOfAsset = path.IndexOf("Asset");
            if (indexOfAsset > 0)
            {
                path = path.Substring(indexOfAsset);
            }
            UpdateScene(index, scene =>
            {
                scene.path = path;
                return scene;
            });
        }

        public void BrowseScenePath(int index, Action<string> updateView)
        {
            string directory = string.IsNullOrEmpty(scenes[index].path)
                ? Application.dataPath
                : scenes[index].path;

            string path = EditorUtility.OpenFilePanel(
                title: "Scene Path",
                directory,
                extension: "unity"
            );

            UpdatedScenePath(index, path);
            updateView?.Invoke(path);
        }

        #endregion

        public void UpdatedTarget(int index, E_Target targets)
        {
            UpdateScene(index, scene =>
            {
                scene.targets = targets;
                return scene;
            });
        }

        public void Select(int index, bool isSelected)
        {
            UpdateScene(index, scene =>
            {
                scene.enabled = isSelected;
                return scene;
            });
        }

        public void UpdateScene(int index, Func<SceneDTO, SceneDTO> change)
        {
            scenes[index] = change(scenes[index]);
            SelectedScenesChanged?.Invoke();
            Save(editorOnly: true);
        }
    }
}