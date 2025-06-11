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
using System.Threading.Tasks;
using umi3d.browserRuntime.pc;
using umi3d.cdk;
using umi3d.cdk.collaboration.userCapture;
using umi3d.cdk.navigation;
using umi3d.common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace umi3d.browserRuntime.navigation.pc
{
    public class PCUMI3DNavigation : UMI3DNavigation
    {
        [Space]
        [SerializeField] InputActionReference movementInputAction;
        [SerializeField] InputActionReference runInputAction;
        [SerializeField] InputActionReference jumpInputAction;
        
        public bool isActivate { get; private set; } = false;
        bool isInitialized = false;

        PCUMI3DCamera umi3dCamera;
        Transform personalSkeletonContainer;
        Transform cameraTransform;

        async void Start()
        {
            currentNav = this;
            umi3dCamera = GetComponent<PCUMI3DCamera>();
            Debug.Assert(umi3dCamera != null, "PCUMI3DCamera not found");

            if (UMI3DClientServer.Exists)
            {
                CollaborationSkeletonsManager.Instance.navigation = this;
            }

            while (UMI3DPCManager.@default.personalSkeletonContainer == null)
            {
                UnityEngine.Debug.Log($"[PCUMI3DNavigation] Notice: Wait for personalSkeletonContainer to be set.");
                await Task.Yield();
            }
            personalSkeletonContainer = UMI3DPCManager.@default.personalSkeletonContainer;

            while (UMI3DPCManager.@default.camera == null)
            {
                UnityEngine.Debug.Log($"[PCUMI3DNavigation] Notice: Wait for camera to be set.");
                await Task.Yield();
            }
            cameraTransform = UMI3DPCManager.@default.camera.transform;

            frameController = new CommonFrameController(personalSkeletonContainer);

            Rigidbody rigidbody = FindObjectOfType<Rigidbody>();
            continuousMovement = new PCRigidbodyContinuousMovement(movementInputAction, rigidbody, personalSkeletonContainer, runInputAction, jumpInputAction);
            teleportationMovement = new PCRigidbodyTeleportationMovement(personalSkeletonContainer, cameraTransform);

            isInitialized = true;
            UnityEngine.Debug.Log($"[PCUMI3DNavigation] Notice: is initialized");
        }

        void FixedUpdate()
        {
            if (!isInitialized) { return; }

            HandleContinuousMovement();
        }

        public override void Activate()
        {
            isActivate = true;
            umi3dCamera.cameraMode = E_CameraMode.Navigation;
        }

        public override void Disable()
        {
            isActivate = false;
            umi3dCamera.cameraMode = E_CameraMode.Free;
        }

        public override NavigationData GetNavigationData()
        {
            throw new NotImplementedException();
        }

        protected override void MoveContinuously(ulong environmentId, NavigateDto data)
        {
            UnityEngine.Debug.Log($"[PCUMI3DNavigation] Error: NotImp - MoveContinuously. For now teleport.");
            Teleport(environmentId, new TeleportDto() { position = data.position, rotation = personalSkeletonContainer.localRotation.Dto() });
        }
    }
}