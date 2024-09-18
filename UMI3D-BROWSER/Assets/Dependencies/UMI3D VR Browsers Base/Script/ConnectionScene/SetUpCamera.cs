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

using umi3d.cdk;
using umi3d.cdk.userCapture.tracking;
using umi3dVRBrowsersBase.interactions;
using umi3dVRBrowsersBase.navigation;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace umi3dVRBrowsersBase.connection
{
    public class SetUpCamera : MonoBehaviour
    {
        #region Fields
        public TrackedSubskeletonBone viewpoint;

        private ARCameraManager cameraManager;

        public SnapTurn snapTurn;
        public VRInputObserver rightObs;
        public VRInputObserver leftObs;

        #endregion

        #region Methods

        private void Awake()
        {
            cameraManager = viewpoint.GetComponent<ARCameraManager>();
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
            (UMI3DEnvironmentLoader.Instance.LoadingParameters as UMI3DLoadingParameters).SetMR();
            snapTurn.enabled = false;
            rightObs.enabled = false;
            leftObs.enabled = false;
        }

        private void SwitchToVR()
        {
            cameraManager.enabled = false;
            (UMI3DEnvironmentLoader.Instance.LoadingParameters as UMI3DLoadingParameters).SetVR();
            snapTurn.enabled = true;
            rightObs.enabled = true;
            leftObs.enabled = true;
        }

        #endregion
    }
}