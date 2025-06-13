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
using umi3d.cdk.navigation;
using UnityEngine;
using UnityEngine.InputSystem;

namespace umi3d.browserRuntime.navigation.pc
{
    public class PCUMI3DCamera : UMI3DCamera
    {
        [Space]
        public InputActionReference mouseDelta;
        public InputActionReference lookAroundAction;

        async void Start()
        {
            camera = Camera.main;

            UMI3DPCManager.@default.umi3dCamera = this;
            UMI3DPCManager.@default.camera = Camera.main;

            while (!IsManagerInitialized())
            {
                await Task.Yield();
            }

            try
            {
                movement = new PCCameraMovement(this, mouseDelta, lookAroundAction)
                {
                    personalSkeletonContainer = UMI3DPCManager.@default.player.personalSkeletonContainer.transform,
                    viewpointPivot = UMI3DPCManager.@default.viewpointPivot,
                    neckPivot = UMI3DPCManager.@default.neckPivot,
                    head = UMI3DPCManager.@default.head
                };
                properties = new PCCameraProperties();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
                return;
            }

            isInitialized = true;
            UnityEngine.Debug.Log($"[PCCamera] Notice: is initialized.");
        }

        void Update()
        {
            if (!IsInitialized()) { return; }

            HandleView();
        }

        bool IsManagerInitialized()
        {
            bool isInitialized = true;

            if (UMI3DPCManager.@default.player == null)
            {
                UnityEngine.Debug.Log($"[PCUMI3DCamera] Notice: Waiting for player to be set.");
                isInitialized = false;
            }
            
            if (UMI3DPCManager.@default.personalSkeletonContainer == null)
            {
                UnityEngine.Debug.Log($"[PCUMI3DCamera] Notice: Waiting for personalSkeletonContainer to be set.");
                isInitialized = false;
            }

            if (UMI3DPCManager.@default.viewpointPivot == null)
            {
                UnityEngine.Debug.Log($"[PCUMI3DCamera] Notice: Waiting for viewpointPivot to be set.");
                isInitialized = false;
            }

            if (UMI3DPCManager.@default.neckPivot == null)
            {
                UnityEngine.Debug.Log($"[PCUMI3DCamera] Notice: Waiting for neckPivot to be set.");
                isInitialized = false;
            }

            if (UMI3DPCManager.@default.head == null)
            {
                UnityEngine.Debug.Log($"[PCUMI3DCamera] Notice: Waiting for head to be set.");
                isInitialized = false;
            }

            return isInitialized;
        }


        bool isInitialized = false;
        protected override bool IsInitialized() => isInitialized;
    }
}