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
        
        Transform personalSkeletonContainer;
        new Rigidbody rigidbody;

        async void Start()
        {
            currentNav = this;

            while (!IsManagerInitialized())
            {
                await Task.Yield();
            }

            if (UMI3DClientServer.Exists)
            {
                CollaborationSkeletonsManager.Instance.navigation = this;
            }

            personalSkeletonContainer = UMI3DPCManager.@default.personalSkeletonContainer;
            Transform cameraTransform = UMI3DPCManager.@default.camera.transform;

            rigidbody = FindObjectOfType<Rigidbody>();
            continuousMovement = new PCRigidbodyContinuousMovement(movementInputAction, rigidbody, personalSkeletonContainer, runInputAction, jumpInputAction);
            teleportationMovement = new PCRigidbodyTeleportationMovement(personalSkeletonContainer, cameraTransform);
            frameController = new FrameController(personalSkeletonContainer);

            isInitialized = true;
            UnityEngine.Debug.Log($"[PCUMI3DNavigation] Notice: is initialized");
        }

        void FixedUpdate()
        {
            if (!IsInitialized()) { return; }

            HandleContinuousMovement();
        }

        bool isInitialized = false;
        public override bool IsInitialized() => isInitialized;

        bool IsManagerInitialized()
        {
            bool isInitialized = true;

            if (UMI3DPCManager.@default.personalSkeletonContainer == null)
            {
                UnityEngine.Debug.Log($"[PCUMI3DNavigation] Notice: Wait for personalSkeletonContainer to be set.");
                isInitialized = false;
            }

            if (UMI3DPCManager.@default.camera == null)
            {
                UnityEngine.Debug.Log($"[PCUMI3DNavigation] Notice: Wait for camera to be set.");
                isInitialized = false;
            }

            return isInitialized;
        }

        public override NavigationData GetNavigationData()
        {
            return new NavigationData()
            {
                crouching = false,
                grounded = continuousMovement.IsGrounded(rideHeight),
                jumping = continuousMovement.IsJumping,
                speed = rigidbody.velocity.Dto()
            };
        }

        protected override void MoveContinuously(ulong environmentId, NavigateDto data)
        {
            UnityEngine.Debug.Log($"[PCUMI3DNavigation] Error: NotImp - MoveContinuously. For now teleport.");
            Teleport(environmentId, new TeleportDto() { position = data.position, rotation = personalSkeletonContainer.localRotation.Dto() });
        }
    }
}