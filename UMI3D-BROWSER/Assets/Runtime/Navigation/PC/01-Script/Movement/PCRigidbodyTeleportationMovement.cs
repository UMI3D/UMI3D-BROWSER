/*
Copyright 2019 - 2025 Inetum

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
using umi3d.cdk.navigation;
using umi3d.common;
using UnityEngine;

namespace umi3d.browserRuntime.navigation.pc
{
    public class PCRigidbodyTeleportationMovement : TeleportationMovement
    {
        Transform personalSkeletonContainer;
        Transform cameraTransform;

        public PCRigidbodyTeleportationMovement(Transform personalSkeletonContainer, Transform cameraTransform)
        {
            this.personalSkeletonContainer = personalSkeletonContainer;
            this.cameraTransform = cameraTransform;
        }

        public override void Move(ulong environmentId, TeleportDto data)
        {
            personalSkeletonContainer.localPosition = data.position.Struct();
            personalSkeletonContainer.localRotation = data.rotation.Quaternion();
        }

        public override void Move(ulong environmentId, ViewpointTeleportDto data)
        {
            // Rotation
            personalSkeletonContainer.rotation = data.rotation.Quaternion();
            cameraTransform.parent.localRotation = Quaternion.identity;
            float angle = Vector3.SignedAngle(
                personalSkeletonContainer.forward,
                Vector3.ProjectOnPlane(
                    cameraTransform.forward,
                    Vector3.up
                ),
                Vector3.up
            );
            personalSkeletonContainer.Rotate(0, -angle, 0);

            // Position
            personalSkeletonContainer.position = data.position.Struct();
            if (cameraTransform != null)
            {
                Vector3 translation = personalSkeletonContainer.position - cameraTransform.position;
                personalSkeletonContainer.Translate(translation, Space.World);
            }
        }
    }
}