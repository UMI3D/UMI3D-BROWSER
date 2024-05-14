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

using System.Collections;
using System.Collections.Generic;
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
        public string sceneToLoad;

        /// <summary>
        /// Mode used to load the scene.
        /// </summary>
        public LoadSceneMode loadSceneMode;

        public bool setNewSceneAsActive = true;

        [SerializeField] private Camera debugCamera;

        private void Start()
        {
            if (debugCamera != null) debugCamera.gameObject.SetActive(false);   

            if (setNewSceneAsActive)
            {             
                StartCoroutine(LoadScene());
            }
            else 
                SceneManager.LoadScene(sceneToLoad, loadSceneMode);
        }

        /// <summary>
        /// Loads asynchronously a scene.
        /// </summary>
        /// <returns></returns>
        private IEnumerator LoadScene()
        {
            AsyncOperation indicator = SceneManager.LoadSceneAsync(sceneToLoad, loadSceneMode);

            yield return new WaitUntil(() => indicator.isDone);

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneToLoad));
        }

        public void ReloadScene()
        {
            StartCoroutine(_ReloadScene());
        }

        private IEnumerator _ReloadScene()
        {
            AsyncOperation indicator = SceneManager.UnloadSceneAsync(sceneToLoad);

            yield return new WaitUntil(() => indicator.isDone);

            indicator = SceneManager.LoadSceneAsync(sceneToLoad, loadSceneMode);

            yield return new WaitUntil(() => indicator.isDone);

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneToLoad));
        }
    }
}