/*
Copyright 2019 - 2021 Inetum

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
using umi3d.cdk.collaboration;

using UnityEngine;
using UnityEngine.XR.Hands;

namespace umi3d.runtimeBrowser.handTracking
{
    /// <summary>
    /// Restricts the display of the hand according to UMI3D logic.
    /// </summary>
    public class HandDisplayRestrictor : MonoBehaviour
    {
        [SerializeField, Tooltip("Hand mesh for which display is restricted.")]
        private XRHandMeshController meshController;

        private void Start()
        {
            meshController = meshController == null ? GetComponent<XRHandMeshController>() : meshController;

            // when server change if controllers should be visible, restrict or not their display
            UMI3DUser.OnAreTrackedControllersVisible.AddListener((UMI3DUser user) =>
            {
                IUMI3DClientServer server = UMI3DClientServer.Instance;

                if (user.id != server.GetUserId())
                    return;

                if (user.areTrackedControllersVisible)
                    EnableDisplay();
                else
                    DisableDisplay();
            });
        }

        /// <summary>
        /// Authorize display.
        /// </summary>
        private void EnableDisplay()
        {
            meshController.hideMeshWhenTrackingIsLost = true;
            meshController.showMeshWhenTrackingIsAcquired = true;
            meshController.handMeshRenderer.enabled = true;
        }

        /// <summary>
        /// Prevent display.
        /// </summary>
        private void DisableDisplay()
        {
            meshController.hideMeshWhenTrackingIsLost = false;
            meshController.showMeshWhenTrackingIsAcquired = false;
            meshController.handMeshRenderer.enabled = false;
        }
    }
}