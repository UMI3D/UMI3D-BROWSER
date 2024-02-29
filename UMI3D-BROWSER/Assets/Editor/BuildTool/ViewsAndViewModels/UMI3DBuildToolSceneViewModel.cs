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

using UnityEditor;

namespace umi3d.browserEditor.BuildTool
{
    public class UMI3DBuildToolSceneViewModel
    {
        public UMI3DBuildToolScene_SO buildToolScene_SO;

        public UMI3DBuildToolSceneViewModel(UMI3DBuildToolScene_SO buildToolScene_SO)
        {
            this.buildToolScene_SO = buildToolScene_SO;
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

        public void ApplyScenePath(int index, string path)
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

        public void Save()
        {
            EditorUtility.SetDirty(buildToolScene_SO);
        }
    }
}