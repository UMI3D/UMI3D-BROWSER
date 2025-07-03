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
using umi3d.baseBrowser.Navigation;
using umi3d.cdk.collaboration;
using umi3d.cdk.collaboration.userCapture;
using umi3d.cdk.navigation;
using umi3d.cdk.notification;
using umi3d.common;
using UnityEngine;

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

        [Header("Player Omniscient View")]
        public Vector3 previousPosition = Vector3.zero;
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

            //Listenner invoked when the ViewMode change
            NotificationHub.Default.Subscribe(
                this,
                UMI3DClientNotificatonKeys.CameraPropertiesNotification,
                (Callback)cameraManager.CameraPropertiesReception);
            UMI3DCollaborationClientServer.Instance.OnLeaving.AddListener(OnUserDeconnected);
            UMI3DNavigation.OnChangeView += ListennerNavigationChange;
        }

        private void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(
                this,
                UMI3DClientNotificatonKeys.CameraPropertiesNotification
            );
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

        public void OnUserDeconnected()
        {
            UMI3DNavigation.OnChangeView -= ListennerNavigationChange;
            fpsData.navigationMode = E_NavigationMode.Default;
            fpsData.maxXCameraAngle = new Vector2(-60, 70);
        }

        public void OmniscientView(OmniscientViewDto dto)
        {
            //Camera
            cameraManager.cam.orthographic = false;
            cameraManager.cam.fieldOfView = dto.fieldOfView;
            cameraManager.cam.nearClipPlane = dto.nearPlane;
            cameraManager.cam.farClipPlane = dto.farPlane;

            cameraManager.zoomMinMax = dto.zoomLimit.Struct();
            cameraManager.zoomSpeed = dto.zoomSpeed;

            //positions
            previousPosition = collisionManager.playerTransform.position;

            collisionManager.playerTransform.position = new Vector3(collisionManager.playerTransform.position.x,
                collisionManager.playerTransform.position.y + dto.distance,
                collisionManager.playerTransform.position.z);
            cameraManager.playerTransform.position = collisionManager.playerTransform.position;
            colliderDelegate.playerTransform.position = collisionManager.playerTransform.position;
            movementManager.playerTransform.position = collisionManager.playerTransform.position;
            navigationDelegate.playerTransform.position = collisionManager.playerTransform.position;

            //camera Angle limits
            fpsData.maxXCameraAngle = new Vector2(dto.cameraXAngle.X, dto.cameraXAngle.Y);

            //Camera base Rotation
            Quaternion quaternion = new Quaternion();
            Vector3 euler = quaternion.eulerAngles;
            cameraManager.viewpointPivot.SetPositionAndRotation(
                cameraManager.viewpointPivot.transform.position,
                Quaternion.Euler(
                    45,
                    cameraManager.viewpointPivot.rotation.eulerAngles.y,
                    cameraManager.viewpointPivot.rotation.eulerAngles.z
                ));

            navigationDelegate.cameraTransform = cameraManager.viewpointPivot;

            //Navigation physics and collisions
            fpsData.flyingSpeed = dto.flyingSpeed;

            fpsData.navigationMode = E_NavigationMode.Omniscient;
        }
        public void ImmersiveView(ImmersiveViewDto dto)
        {
            //camera
            cameraManager.cam.orthographic = false;
            cameraManager.cam.fieldOfView = dto.fieldOfView;
            cameraManager.cam.nearClipPlane = dto.nearPlane;
            cameraManager.cam.farClipPlane = dto.farPlane;
            fpsData.maxXCameraAngle = new Vector2(dto.cameraXAngle.X, dto.cameraXAngle.Y);

            //positions
            collisionManager.playerTransform.position = previousPosition;
            cameraManager.playerTransform.position = collisionManager.playerTransform.position;
            colliderDelegate.playerTransform.position = collisionManager.playerTransform.position;
            movementManager.playerTransform.position = collisionManager.playerTransform.position;
            navigationDelegate.playerTransform.position = collisionManager.playerTransform.position;

            previousPosition = Vector3.zero;

            fpsData.navigationMode = E_NavigationMode.Default;
        }

        private void ListennerNavigationChange()
        {
            OmniscientView(UMI3DNavigation.dto);
        }
    }
}