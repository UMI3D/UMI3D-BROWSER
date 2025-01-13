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
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace umi3d.browserEditor.scenes
{
    public static class SceneSwitcherEditor
    {
        /// <summary>
        /// Path of the folder that contains scenes.
        /// </summary>
        const string FOLDER_PATH = "Assets/New UI/03-Scenes";

        static string[] scenePaths;

        [InitializeOnLoadMethod]
        public static void FindScenes()
        {
            string[] sceneGUIDs = AssetDatabase.FindAssets("t:Scene", new[] { FOLDER_PATH });
            scenePaths = sceneGUIDs.Select(guid => AssetDatabase.GUIDToAssetPath(guid)).ToArray();
        }

        [MenuItem("Scenes/Start Scene", priority = 0)]
        public static void LoadStartScene()
        {
            LoadScene(GetScenePath("StartScene(new)"));
        }

        [MenuItem("Scenes/Connection Scene", priority = 1)]
        public static void LoadConnectionScene()
        {
            LoadScene(GetScenePath("ConnectionScene(new)"));
        }

        [MenuItem("Scenes/MainImmersive Scene", priority = 2)]
        public static void LoadMainImmersiveScene()
        {
            LoadScene(GetScenePath("MainImmersive(new)"));
        }

        [MenuItem("Scenes/PC/Window Bar", priority = 3)]
        public static void LoadPCWindowBar()
        {
            LoadScene(GetScenePath("WindowBar"));
        }

        [MenuItem("Scenes/PC/UI In Game", priority = 4)]
        public static void LoadPCUIInGame()
        {
            LoadScene(GetScenePath("PC_InGameUI"));
        }

        static string GetScenePath(string sceneName)
        {
            if (scenePaths == null || scenePaths.Length == 0)
            {
                UnityEngine.Debug.LogError($"Error: no scenes found.");
                return null;
            }

            foreach (string scenePath in scenePaths)
            {
                if (scenePath.Contains(sceneName))
                {
                    return scenePath;
                }
            }

            return null;
        }

        static void LoadScene(string path)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
        }
    }
}
