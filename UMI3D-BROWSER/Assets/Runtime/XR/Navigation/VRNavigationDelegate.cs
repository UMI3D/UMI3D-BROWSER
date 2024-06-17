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
using umi3d.cdk.navigation;
using umi3d.common;
using umi3dVRBrowsersBase.interactions;
using umi3dVRBrowsersBase.navigation;
using UnityEngine;

namespace umi3d.browserRuntime.navigation
{
    public sealed class VRNavigationDelegate : INavigationDelegate
    {
        umi3d.debug.UMI3DLogger logger = new();

        Transform cameraTransform;
        Transform playerTransform;
        Transform personalSkeletonContainer;
        SnapTurn snapTurn;
        VRInputObserver leftObs;
        VRInputObserver rightObs;

        /// <summary>
        /// Is player active ?
        /// </summary>
        public bool isActive = false;
        public UMI3DNodeInstance globalVehicle;
        public bool vehicleFreeHead = false;
        public UMI3DNodeInstance globalFrame;

        public void Init(
            UnityEngine.Object context,
            Transform cameraTransform,
            Transform playerTransform,
            Transform personalSkeletonContainer,
            SnapTurn snapTurn,
            VRInputObserver leftObs,
            VRInputObserver rightObs
        )
        {
            logger.MainContext = context;
            logger.MainTag = nameof(VRNavigationDelegate);

            this.cameraTransform = cameraTransform;
            this.playerTransform = playerTransform;
            this.personalSkeletonContainer = personalSkeletonContainer;

            this.snapTurn = snapTurn;
            this.leftObs = leftObs;
            this.rightObs = rightObs;
        }

        public void Activate() 
        {
            isActive = true;
            snapTurn.enabled = true;
            leftObs.enabled = true;
            rightObs.enabled = true;
        }

        public void Disable() 
        {
            isActive = false;
            snapTurn.enabled = false;
            leftObs.enabled = false;
            rightObs.enabled = false;
        }

        public void ViewpointTeleport(ulong environmentId, ViewpointTeleportDto data)
        {
            personalSkeletonContainer.rotation = data.rotation.Quaternion();
            if (cameraTransform != null)
            {
                float angle = Vector3.SignedAngle(
                    personalSkeletonContainer.forward,
                    Vector3.ProjectOnPlane(
                        cameraTransform.forward,
                        Vector3.up
                    ),
                    Vector3.up
                );
                personalSkeletonContainer.Rotate(0, -angle, 0);
            }

            personalSkeletonContainer.position = data.position.Struct();
            if (cameraTransform != null)
            {
                Vector3 translation = personalSkeletonContainer.position - cameraTransform.position;
                personalSkeletonContainer.Translate(translation, Space.World);
            }
        }

        public void Teleport(ulong environmentId, TeleportDto data)
        {
            personalSkeletonContainer.rotation = data.rotation.Quaternion();
            if (cameraTransform != null)
            {
                float angle = Vector3.SignedAngle(
                    personalSkeletonContainer.forward, 
                    Vector3.ProjectOnPlane(
                        cameraTransform.forward, 
                        Vector3.up
                    ), 
                    Vector3.up
                );
                personalSkeletonContainer.Rotate(0, -angle, 0);
            }

            personalSkeletonContainer.position = data.position.Struct();
            if (cameraTransform != null)
            {
                Vector3 translation = Vector3.ProjectOnPlane(
                    personalSkeletonContainer.position - cameraTransform.position,
                    Vector3.up
                );
                personalSkeletonContainer.Translate(translation, Space.World);
            }
        }

        public void Navigate(ulong environmentId, NavigateDto data)
        {
            Teleport(
                environmentId, 
                new TeleportDto() 
                { 
                    position = data.position, 
                    rotation = playerTransform.rotation.Dto() 
                }
            );
        }

        public void UpdateFrame(ulong environmentId, FrameRequestDto data)
        {
            if (data.FrameId == 0)
            {
                // bind the personalSkeletonContainer to the Scene.
                personalSkeletonContainer.SetParent(
                    UMI3DLoadingHandler.Instance.transform, 
                    true
                );
                globalFrame.Delete -= GlobalFrameDeleted;
                globalFrame = null;
            }
            else
            {
                // bind the personalSkeletonContainer to the Frame.
                UMI3DNodeInstance Frame = UMI3DEnvironmentLoader.GetNode(environmentId, data.FrameId);
                if (Frame != null)
                {
                    globalFrame = Frame;
                    personalSkeletonContainer.SetParent(
                        Frame.transform, 
                        true
                    );
                    globalFrame.Delete += GlobalFrameDeleted;
                }
            }
        }

        void GlobalFrameDeleted()
        {
            personalSkeletonContainer.SetParent(
                UMI3DLoadingHandler.Instance.transform, 
                true
            );
        }

        public NavigationData GetNavigationData()
        {
            // TODO update speed.
            return new NavigationData
            {
                speed = Vector3.zero.Dto(),
                jumping = false,
                crouching = false
            };
        }
    }
}