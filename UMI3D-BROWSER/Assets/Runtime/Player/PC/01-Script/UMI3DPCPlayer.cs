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

using umi3d.browserRuntime.pc;
using UnityEngine;

namespace umi3d.browserRuntime.player.pc
{
    public class UMI3DPCPlayer : UMI3DPlayer
    {
        //public BaseFPSData fpsData;

        [Header("Camera")]
        public Transform viewpointPivot;
        public Transform neckPivot;
        public Transform head;

        //[Header("Player Collision Debugger")]
        //public UMI3DCollisionManager.CollisionDebugger.E_Collision collisionToDebug;

        //[HideInInspector] public UMI3DNavigation navigation = new();

        //UMI3DCollisionManager collisionManager;
        //UMI3DCameraManager cameraManager;
        //UMI3DMovementManager movementManager;
        //PCNavigationDelegate navigationDelegate;
        //UMI3DPlayerCapsuleColliderDelegate colliderDelegate;

        void Awake()
        {
//            KeyboardAndMouseFpsNavigation concreteFPSNavigation = new KeyboardAndMouseFpsNavigation()
//            {
//                data = fpsData,
//            };

//            colliderDelegate = new()
//            {
//                playerTransform = personalSkeletonContainer.transform,
//                data = fpsData
//            };
//            colliderDelegate.Init();
//            collisionManager = new()
//            {
//                data = fpsData,
//                playerTransform = personalSkeletonContainer.transform,
//                colliderDelegate = colliderDelegate,
//                collisionDebugger = () => new() { collision = collisionToDebug}
//            };
//            cameraManager = new()
//            {
//                data = fpsData,
//                playerTransform = personalSkeletonContainer.transform,
//                viewpointPivot = viewpointPivot,
//                neckPivot = neckPivot,
//                head = head,
//                concreteFPSNavigation = concreteFPSNavigation
//            };
//            movementManager = new()
//            {
//                data = fpsData,
//                playerTransform = personalSkeletonContainer.transform,
//                skeleton = personalSkeleton.transform,
//                collisionManager = collisionManager,
//                concreteFPSNavigation = concreteFPSNavigation
//            };
//            navigationDelegate = new()
//            {
//                data = fpsData,
//                playerTransform = personalSkeletonContainer.transform,
//                personalSkeletonContainer = personalSkeletonContainer.transform,
//                cameraTransform = viewpointPivot.GetChild(0),
//                collisionManager = collisionManager,
//            };
//            navigation.Init(navigationDelegate);

//            // SKELETON SERVICE
//            CollaborationSkeletonsManager.Instance.navigation = navigationDelegate; //also use to init manager via Instance call

//#if !UNITY_EDITOR
//            fpsData.navigationMode = E_NavigationMode.Default;
//#endif
        }

        void Start()
        {
            mainCamera = Camera.main;
            cameraOffset = mainCamera.transform.parent.gameObject;

            UMI3DPCManager.@default.player = this;
        }

        private void Update()
        {
            //cameraManager.HandleView();

            //if (!navigationDelegate.isActive)
            //    return; 

            //colliderDelegate.ComputeCollider();
            //movementManager.ComputeMovement();
        }

        private void OnDrawGizmosSelected()
        {
            //colliderDelegate?.DrawGizmos();
        }
    }
}
