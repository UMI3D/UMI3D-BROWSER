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

using inetum.unityUtils;
using System;
using UnityEditor;
using UnityEngine;

namespace umi3d.browserEditor.BuildTool
{
    public class UMI3DBuildToolSceneViewModel
    {
        SubGlobal subGlobal = new("BuildTool");

        public UMI3DBuildToolScene_SO buildToolScene_SO;
        public Action applyScenes;

        public UMI3DBuildToolSceneViewModel(Action applyScenes)
        {
            subGlobal.TryGet(out buildToolScene_SO);
            this.applyScenes = applyScenes;
        }

        public SceneDTO this[int index]
        {
            get
            {
                return buildToolScene_SO.scenes[index];
            }
            set
            {
                buildToolScene_SO.scenes[index] = value;
            }
        }

        public int Count
        {
            get
            {
                return buildToolScene_SO.scenes.Count;
            }
        }

        public void Select(int index, bool isSelected)
        {
            var sceneDTO = buildToolScene_SO.scenes[index];
            sceneDTO.enabled = isSelected;
            buildToolScene_SO.scenes[index] = sceneDTO;
            Save();
        }

        public void UpdatedScenePath(int index, string path)
        {
            var sceneDTO = buildToolScene_SO.scenes[index];
            var indexOfAsset = path.IndexOf("Asset");
            if (indexOfAsset > 0)
            {
                path = path.Substring(indexOfAsset);
            }
            sceneDTO.path = path;
            buildToolScene_SO.scenes[index] = sceneDTO;
            Save();
        }

        public void BrowseScenePath(int index, Action<string> updateView)
        {
            string directory = string.IsNullOrEmpty(buildToolScene_SO.scenes[index].path)
                ? Application.dataPath
                : buildToolScene_SO.scenes[index].path;

            string path = EditorUtility.OpenFilePanel(
                title: "Scene Path",
                directory,
                extension: "unity"
            );

            UpdatedScenePath(index, path);
            updateView?.Invoke(path);
        }

        public void Save()
        {
            applyScenes?.Invoke();
            EditorUtility.SetDirty(buildToolScene_SO);
        }
    }
}