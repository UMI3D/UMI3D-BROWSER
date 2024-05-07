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
using inetum.unityUtils;
using umi3d.cdk;
using umi3dBrowsers.linker;
using umi3dVRBrowsersBase.ui.playerMenu;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace umi3dBrowsers.services.environment
{
    public class LoadingEnvironmentHandler : MonoBehaviour
    {
        [Header("Linkers")]
        [SerializeField] private ConnectionToImmersiveLinker linker;
        private Camera cam;

        [SerializeField] private LayerMask loadingCullingMask;      
        private int defaultCullingMask;

        [SerializeField] private Material skyBoxHomeMaterial;

        /// <summary>
        /// Event raised when loading screen is displayed.
        /// </summary>
        public static UnityEvent OnLoadingScreenDislayed = new UnityEvent();

        /// <summary>
        /// Event raised when loading screen is hidden.
        /// </summary>
        public static UnityEvent OnLoadingScreenHidden = new UnityEvent();

        private void Awake()
        {
            DontDestroyOnLoad(this);
            linker.OnDisplayEnvironmentHandler += () => Display();
            linker.OnStopDisplayEnvironmentHandler += () => StopDisplay();
        }

        private void Start()
        {
            cam = Camera.main;
            Debug.Assert(cam != null, "Impossible to find a camera");

            defaultCullingMask = cam.cullingMask;

            StopDisplay();
        }

        /// <summary>
        /// Displays loading screen
        /// </summary>
        public void Display()
        {
            cam.cullingMask = loadingCullingMask.value;
            cam.clearFlags = CameraClearFlags.Skybox;
            RenderSettings.skybox = skyBoxHomeMaterial;
        }

        /// <summary>
        /// Hides loading screen
        /// </summary>
        public void StopDisplay()
        {
            cam.cullingMask = defaultCullingMask;
            cam.clearFlags = CameraClearFlags.Skybox;

            OnLoadingScreenHidden?.Invoke();
        }
    }
}