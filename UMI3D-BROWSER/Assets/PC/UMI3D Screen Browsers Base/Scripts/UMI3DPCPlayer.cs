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

using System;
using System.Collections.Generic;
using umi3d.baseBrowser.Navigation;
using umi3d.browserRuntime.navigation;
using umi3d.cdk.collaboration.userCapture;
using umi3d.cdk.navigation;
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
                OmniscientViewDto newView = new OmniscientViewDto();
                fpsData.navigationMode = E_NavigationMode.Omniscient;

                //positions
                collisionManager.playerTransform.position = new Vector3(collisionManager.playerTransform.position.x, collisionManager.playerTransform.position.y+10, collisionManager.playerTransform.position.z);
                cameraManager.playerTransform.position = collisionManager.playerTransform.position;
                colliderDelegate.playerTransform.position = collisionManager.playerTransform.position;
                movementManager.playerTransform.position = collisionManager.playerTransform.position;
                navigationDelegate.playerTransform.position = collisionManager.playerTransform.position;
                Quaternion test = new Quaternion();
                //Vector3 quaternion = test.ToEulerAngles();
                //camera
                fpsData.maxXCameraAngle = new Vector2(0, 90);
                //cameraManager.viewpointPivot.SetPositionAndRotation(cameraManager.viewpointPivot.transform.position,
                    //new Quaternion.EulerAngles(cameraManager.viewpointPivot.rotation.x + 45, cameraManager.viewpointPivot.rotation.y, cameraManager.viewpointPivot.rotation.z));
                //navigationDelegate.cameraTransform = cameraManager.viewpointPivot;


                /*PerspectiveCameraPropertiesDto cam = new PerspectiveCameraPropertiesDto();
                cam.fieldOfView = 50;
                cameraManager.CameraPropertiesReception(cam);*/

                //physics and collisions
                fpsData.gravity = 0;
                fpsData.maxJumpAltitude = 0;
                fpsData.obstacleLayer = 0;
                fpsData.navmeshLayer = 0;
                fpsData.topSphereCenter = new Vector3(0, 0, 0);
                fpsData.capsuleRadius = 0;
                fpsData.maxAltitudeToCheckGround = 0;
                fpsData.maxSlopeAngle = 0;
                fpsData.maxStepHeight = 0;
                fpsData.stepEpsilon = 0;
                fpsData.lateralSpeed.x = 3;
                fpsData.flyingSpeed = 25;
            }
            else
            {
                fpsData.navigationMode = E_NavigationMode.Default;

                //camera
                fpsData.maxXCameraAngle = new Vector2(-90, 90);

                //collisions and physics
                fpsData.gravity = -9.807f;
                fpsData.maxJumpAltitude = 1;
                fpsData.obstacleLayer = LayerMask.GetMask("Navmesh", "Obstacle");
                fpsData.navmeshLayer = LayerMask.GetMask("Navmesh");
                fpsData.topSphereCenter = new Vector3(0, 1.4f, 0);
                fpsData.capsuleRadius = 0.3f;
                fpsData.maxAltitudeToCheckGround = 25;
                fpsData.maxSlopeAngle = 45;
                fpsData.maxStepHeight = 0.3f;
                fpsData.stepEpsilon = 0.05f;
                fpsData.lateralSpeed.x = 1;
                fpsData.flyingSpeed = 5;
            }
        }
    }
}