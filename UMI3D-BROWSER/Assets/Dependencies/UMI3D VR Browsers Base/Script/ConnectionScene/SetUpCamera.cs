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
using umi3d.browserRuntime.NotificationKeys;
using umi3d.browserRuntime.navigation;
using umi3d.cdk;
using umi3dBrowsers.linker;
using umi3dVRBrowsersBase.interactions;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;
using umi3dVRBrowsersBase.rendering;

namespace umi3dVRBrowsersBase.connection
{
    public class SetUpCamera : SingleBehaviour<SetUpCamera>
    {
        #region Fields
        private ARCameraManager cameraManager;

        public MainContainerLinker mainContainerLinker;

        Dictionary<string, System.Object> info = new();
        #endregion

        #region Methods

        private void Start()
        {
            cameraManager = Camera.main.GetComponent<ARCameraManager>();

            if (mainContainerLinker == null)
                Debug.LogError("No MainContainerLinker reference.");
        }

        public void SwitchARMR()
        {
            if (cameraManager.enabled)
                SwitchToVR();
            else
                SwitchToMR();
        }

        private void SwitchToMR()
        {
            cameraManager.enabled = true;
            Camera.main.clearFlags = CameraClearFlags.SolidColor;
            //Camera.main.backgroundColor = Color.black;

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = Color.black;

            (UMI3DEnvironmentLoader.Instance.LoadingParameters as UMI3DLoadingParameters).SetMR();

            info[LocomotionNotificationKeys.Info.Controller] = Controller.LeftAndRight;
            info[LocomotionNotificationKeys.Info.SnapTurnActiveState] = ActiveState.Disable;
            info[LocomotionNotificationKeys.Info.TeleportationActiveState] = ActiveState.Disable;
            NotificationHub.Default.Notify(this, LocomotionNotificationKeys.System, info);

            mainContainerLinker.Skybox.gameObject.SetActive(false);
        }

        private void SwitchToVR()
        {
            cameraManager.enabled = false;
            Camera.main.clearFlags = CameraClearFlags.Skybox;

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
            RenderSettings.ambientLight = new Color(54f/255f, 58f/255f, 66f/255f);

            (UMI3DEnvironmentLoader.Instance.LoadingParameters as UMI3DLoadingParameters).SetVR();

            info[LocomotionNotificationKeys.Info.Controller] = Controller.LeftAndRight;
            info[LocomotionNotificationKeys.Info.SnapTurnActiveState] = ActiveState.Enable;
            info[LocomotionNotificationKeys.Info.TeleportationActiveState] = ActiveState.Enable;
            NotificationHub.Default.Notify(this, LocomotionNotificationKeys.System, info);

            mainContainerLinker.Skybox.gameObject.SetActive(true);
        }

        #endregion
    }
}