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

using umi3d.cdk.navigation;
using UnityEngine;
using UnityEngine.InputSystem;

namespace umi3d.browserRuntime.navigation.pc
{
    public class PCCameraMovement : CameraMovement
    {
        public InputActionReference mouseDelta;
        public InputActionReference lookAroundAction;

        internal GameObject personalSkeletonContainer;
        internal Transform viewpointPivot;
        internal Transform neckPivot;
        internal Transform head;

        PCUMI3DCamera umi3dCamera;

        Vector2 rotationInput = Vector2.zero;
        Vector2 viewpointRotation = Vector2.zero;
        Vector2 headRotation = Vector2.zero;
        Vector2 neckRotation = Vector2.zero;
        Vector2 personalSkeletonContainerRotation = Vector2.zero;

        public PCCameraMovement(PCUMI3DCamera umi3dCamera, InputActionReference mouseDelta, InputActionReference lookAroundAction)
        {
            this.umi3dCamera = umi3dCamera;
            this.mouseDelta = mouseDelta;
            this.lookAroundAction = lookAroundAction;

            mouseDelta.action.Enable();
            lookAroundAction.action.Enable();
            lookAroundAction.action.started += LookAroundStarted;
            lookAroundAction.action.canceled += LookAroundCanceled;
        }

        void LookAroundStarted(InputAction.CallbackContext obj)
        {
            if (umi3dCamera.cameraMode != E_CameraMode.Locked && umi3dCamera.cameraMode != E_CameraMode.Free)
            {
                umi3dCamera.cameraMode = E_CameraMode.NeckMovement;
            }
        }

        void LookAroundCanceled(InputAction.CallbackContext obj)
        {
            if (umi3dCamera.cameraMode != E_CameraMode.Locked && umi3dCamera.cameraMode != E_CameraMode.Free)
            {
                umi3dCamera.cameraMode = E_CameraMode.Navigation;
            }
        }

        public override bool CanMove()
        {
            return umi3dCamera.cameraMode != E_CameraMode.Locked;
        }

        public override void HandleInput(Vector2 angularSpeed)
        {
            Vector2 delta = mouseDelta.action.ReadValue<Vector2>();
            rotationInput = new Vector2(-delta.y, delta.x) * angularSpeed * Time.deltaTime;
        }

        public override void Move(Vector2 maxCameraAngle, Vector2 maxHeadXAngle, float maxNeckXAngle)
        {
            // Vertical Axis.
            float viewpointXRotation = Mathf.Clamp(
                (viewpointPivot.localRotation.eulerAngles.NormalizeAngle() + ((Vector3)rotationInput).NormalizeAngle()).x,
                -maxCameraAngle.x,
                maxCameraAngle.x
            );

            // Horizontal Axis
            float viewpointYRotation = 0f;
            float personalSkeletonContainerYRotation = 0f;
            switch (umi3dCamera.cameraMode)
            {
                case E_CameraMode.Locked:
                    viewpointYRotation = 0f;
                    personalSkeletonContainerYRotation = 0f;
                    break;

                case E_CameraMode.Navigation:
                    viewpointYRotation = 0f;
                    personalSkeletonContainerYRotation = (viewpointPivot.rotation.eulerAngles.NormalizeAngle() + ((Vector3)rotationInput).NormalizeAngle()).y;
                    break;

                case E_CameraMode.NeckMovement:
                    viewpointYRotation = Mathf.Clamp(
                        (viewpointPivot.localRotation.eulerAngles.NormalizeAngle() + ((Vector3)rotationInput).NormalizeAngle()).y,
                        -maxCameraAngle.y,
                        maxCameraAngle.y
                    );
                    personalSkeletonContainerYRotation = 0f;
                    break;
                    
                case E_CameraMode.Free:
                    viewpointYRotation = (viewpointPivot.localRotation.eulerAngles.NormalizeAngle() + ((Vector3)rotationInput).NormalizeAngle()).y;
                    personalSkeletonContainerYRotation = 0f;
                    break;

                default:
                    viewpointYRotation = 0f;
                    personalSkeletonContainerYRotation = 0f;
                    break;
            }

            // Rotations
            viewpointRotation = new Vector2(viewpointXRotation, viewpointYRotation);
            headRotation = new Vector2(
                Mathf.Clamp(viewpointXRotation, maxHeadXAngle.x, maxHeadXAngle.y) / 2,
                viewpointYRotation / 2
            );
            neckRotation = new Vector2(
                Mathf.Clamp(viewpointXRotation, -maxNeckXAngle, maxNeckXAngle) / 2,
                viewpointYRotation / 2
            );
            personalSkeletonContainerRotation = new Vector2(0f, personalSkeletonContainerYRotation);

            viewpointPivot.localRotation = Quaternion.Euler(viewpointRotation);
            head.localRotation = Quaternion.Euler(headRotation);
            neckPivot.localRotation = Quaternion.Euler(neckRotation);

            if (umi3dCamera.cameraMode == E_CameraMode.Navigation)
            {
                personalSkeletonContainer.transform.rotation = Quaternion.Euler(personalSkeletonContainerRotation); 
            }
        }
    }
}