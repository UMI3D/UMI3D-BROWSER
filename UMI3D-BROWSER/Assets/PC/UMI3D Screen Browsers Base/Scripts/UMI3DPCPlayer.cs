/*
Copyright 2019 - 2024 Inetum

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

using inetum.unityUtils.observation;
using System;
using System.Collections.Generic;
using umi3d.baseBrowser.Navigation;
using umi3d.browserRuntime.navigation;
using umi3d.cdk.collaboration.userCapture;
using umi3d.cdk.navigation;
using umi3d.cdk.notification;
using umi3d.common;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace umi3d.baseBrowser
{
    public class UMI3DPCPlayer : MonoBehaviour
    {
        public BaseFPSData fpsData;
        public Transform personalSkeletonContainer;
        public Transform playerTransform;
        public Transform skeleton;

        [Header("Camera")]
        public Transform viewpointPivot;
        public Transform neckPivot;
        public Transform head;

        [Header("Player Collision Debugger")]
        public UMI3DCollisionManager.CollisionDebugger.E_Collision collisionToDebug;

        [HideInInspector] public UMI3DNavigation navigation = new();

        UMI3DCollisionManager collisionManager;
        UMI3DCameraManager cameraManager;
        UMI3DMovementManager movementManager;
        PCNavigationDelegate navigationDelegate;
        UMI3DPlayerCapsuleColliderDelegate colliderDelegate;

        void Awake()
        {
            KeyboardAndMouseFpsNavigation concreteFPSNavigation = new KeyboardAndMouseFpsNavigation()
            {
                data = fpsData,
            };

            colliderDelegate = new()
            {
                playerTransform = playerTransform,
                data = fpsData
            };
            colliderDelegate.Init();
            collisionManager = new()
            {
                data = fpsData,
                playerTransform = playerTransform,
                colliderDelegate = colliderDelegate,
                collisionDebugger = () => new() { collision = collisionToDebug}
            };
            cameraManager = new()
            {
                data = fpsData,
                playerTransform = playerTransform,
                viewpointPivot = viewpointPivot,
                neckPivot = neckPivot,
                head = head,
                concreteFPSNavigation = concreteFPSNavigation
            };
            movementManager = new()
            {
                data = fpsData,
                playerTransform = playerTransform,
                skeleton = skeleton,
                collisionManager = collisionManager,
                concreteFPSNavigation = concreteFPSNavigation
            };
            navigationDelegate = new()
            {
                data = fpsData,
                playerTransform = playerTransform,
                personalSkeletonContainer = personalSkeletonContainer,
                cameraTransform = viewpointPivot.GetChild(0),
                collisionManager = collisionManager,
            };
            navigation.Init(navigationDelegate);

            // SKELETON SERVICE
            CollaborationSkeletonsManager.Instance.navigation = navigationDelegate; //also use to init manager via Instance call

#if !UNITY_EDITOR
            fpsData.navigationMode = E_NavigationMode.Default;
#endif
        }

        private void Update()
        {
            cameraManager.HandleView();

            if (!navigationDelegate.isActive)
                return; 

            colliderDelegate.ComputeCollider();
            movementManager.ComputeMovement();
        }

        private void OnDrawGizmosSelected()
        {
            colliderDelegate?.DrawGizmos();
        }

        [ContextMenu(itemName:"Func Change View")]
        void TestViewMode()
        {
            if(fpsData.navigationMode == E_NavigationMode.Default)
            {
                //define Dto and his values
                OmniscientViewDto newView = new OmniscientViewDto();
                newView.distance = 10;
                newView.flyingSpeed = 25;
                newView.cameraXAngle = new Vector2Dto(){ X = 0, Y = 90 };
                newView.fieldOfView = 50;
                newView.nearPlane = 3;
                newView.farPlane = 1000;

                //positions
                collisionManager.playerTransform.position = new Vector3(collisionManager.playerTransform.position.x, 
                    collisionManager.playerTransform.position.y+ newView.distance,
                    collisionManager.playerTransform.position.z);
                cameraManager.playerTransform.position = collisionManager.playerTransform.position;
                colliderDelegate.playerTransform.position = collisionManager.playerTransform.position;
                movementManager.playerTransform.position = collisionManager.playerTransform.position;
                navigationDelegate.playerTransform.position = collisionManager.playerTransform.position;
                

                //camera Angle limits
                fpsData.maxXCameraAngle = new Vector2 (newView.cameraXAngle.X, newView.cameraXAngle.Y);
                //camera FOV, position, nearPlane and FarPlane
                PerspectiveCameraPropertiesDto cam = new PerspectiveCameraPropertiesDto()
                {
                    fieldOfView = newView.fieldOfView,
                    localPosition = new Vector3Dto() {X = 0, Y = 0.198f, Z = 0.1243f },
                    nearPlane = newView.nearPlane,
                    farPlane = newView.farPlane,
                };
                Notification notif = new Notification("", this, new Dictionary<string, object>() { { UMI3DClientNotificatonKeys.Info.CameraProperties, cam } });
                cameraManager.CameraPropertiesReception(notif);
                //Camera base Rotation
                Quaternion test = new Quaternion();
                Vector3 quaternion = test.eulerAngles;
                cameraManager.viewpointPivot.SetPositionAndRotation(
                    cameraManager.viewpointPivot.transform.position,
                    Quaternion.Euler(
                        45,
                        cameraManager.viewpointPivot.rotation.eulerAngles.y,
                        cameraManager.viewpointPivot.rotation.eulerAngles.z
                    )
                );
                navigationDelegate.cameraTransform = cameraManager.viewpointPivot;
                
                //physics and collisions
                fpsData.flyingSpeed = newView.flyingSpeed;

                fpsData.navigationMode = E_NavigationMode.Omniscient;
            }
            else
            {
                //define Dto and his values
                ImmersiveViewDto newView = new ImmersiveViewDto();
                newView.nearPlane = 0.17f;
                newView.farPlane = 150000;
                newView.fieldOfView = 60;
                newView.cameraXAngle = new Vector2Dto() { X = -90, Y = 90 };

                //camera
                fpsData.maxXCameraAngle = new Vector2(newView.cameraXAngle.X, newView.cameraXAngle.Y);
                PerspectiveCameraPropertiesDto cam = new PerspectiveCameraPropertiesDto()
                {
                    fieldOfView = newView.fieldOfView,
                    localPosition = new Vector3Dto() { X = 0, Y = 0.198f, Z = 0.1243f },
                    nearPlane = newView.nearPlane,
                    farPlane = newView.farPlane,
                };
                Notification notif = new Notification("", this, new Dictionary<string, object>() { { UMI3DClientNotificatonKeys.Info.CameraProperties, cam } });
                cameraManager.CameraPropertiesReception(notif);
                
                fpsData.navigationMode = E_NavigationMode.Default;
            }
        }
    }
}