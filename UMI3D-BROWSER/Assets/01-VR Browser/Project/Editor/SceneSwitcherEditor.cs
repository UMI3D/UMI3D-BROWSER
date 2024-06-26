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
using UnityEditor.SceneManagement;

namespace umi3d
{
    public static class SceneSwitcherEditor
    {
        [MenuItem("Scenes/Start Scene")]
        static void LoadStartScene()
        {
            LoadScene("Assets/Dependencies/UMI3D VR Browsers Base/Scene/StartScene.unity");
        }

        [MenuItem("Scenes/Connection Scene")]
        static void LoadConnectionScene()
        {
            LoadScene("Assets/Dependencies/UMI3D VR Browsers Base/Scene/ConnectionScene.unity");
        }

        [MenuItem("Scenes/MainImmersive Scene")]
        static void LoadMainImmersiveScene()
        {
            LoadScene("Assets/Project/Common/Scenes/MainImmersive.unity");
        }

        static void LoadScene(string path)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
        }
    }
}
