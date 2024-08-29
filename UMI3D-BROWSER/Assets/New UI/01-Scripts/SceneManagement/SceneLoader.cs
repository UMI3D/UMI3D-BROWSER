/*
Copyright 2019 - 2022 Inetum

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
using System.Collections;
using System.Collections.Generic;
using umi3d.browserRuntime.conditionalCompilation;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace umi3dBrowsers.sceneManagement
{
    /// <summary>
    /// Helper component to load a Unity Scene.
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        /// <summary>
        /// Name of the scene to load.
        /// </summary>
        [SerializeField] private MultiDeviceReference<List<SceneToLoad>> scenesToLoad;

        private void Start()
        {
            foreach(var sceneToLoad in scenesToLoad.Reference)
            {
                if (sceneToLoad.SetNewSceneAsActive)
                {
                    StartCoroutine(LoadScene(sceneToLoad));
                }
                else
                {
                    SceneManager.LoadScene(sceneToLoad.SceneName, sceneToLoad.LoadSceneMode);            
                }
            }
        }

        /// <summary>
        /// Loads asynchronously a scene.
        /// </summary>
        /// <returns></returns>
        private IEnumerator LoadScene(SceneToLoad sceneToLoad)
        {
            AsyncOperation indicator = SceneManager.LoadSceneAsync(sceneToLoad.SceneName, sceneToLoad.LoadSceneMode);

            yield return new WaitUntil(() => indicator.isDone);

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneToLoad.SceneName));         
        }

        public void ReloadScene()
        {
            StartCoroutine(_ReloadScene());
        }

        private IEnumerator _ReloadScene()
        {
            foreach(var sceneToLoad in scenesToLoad.Reference)
            {
                if (sceneToLoad.AlwaysReload)
                {
                    AsyncOperation indicator = SceneManager.UnloadSceneAsync(sceneToLoad.SceneName);

                    yield return new WaitUntil(() => indicator.isDone);

                    indicator = SceneManager.LoadSceneAsync(sceneToLoad.SceneName, sceneToLoad.LoadSceneMode);

                    yield return new WaitUntil(() => indicator.isDone);

                    SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneToLoad.SceneName));
                }
            }
        }

        [Serializable]
        public class SceneToLoad
        {
            [SerializeField] private string sceneName;
            public string SceneName => sceneName;
            [SerializeField] private LoadSceneMode loadSceneMode;
            public LoadSceneMode LoadSceneMode => loadSceneMode;
            [SerializeField] private bool setNewSceneAsActive = true;
            public bool SetNewSceneAsActive => setNewSceneAsActive;
            [SerializeField] private bool alwaysReload = true;
            public bool AlwaysReload => alwaysReload;
        }
    }
}