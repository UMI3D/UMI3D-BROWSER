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
using UnityEngine;
using UnityEngine.InputSystem;

namespace umi3d.browserRuntime.navigation.pc
{
    public class UMI3DPCCamera : UMI3DCamera
    {
        public InputActionReference mouseDelta;
        public InputActionReference lookAroundAction;

        public E_CameraMode cameraMode;

        bool isInitialized = false;

        async void Start()
        {
            UMI3DPCManager.@default.camera = this;

            while (!IsUMI3DPCManagerInitialized())
            {
                await Task.Yield();
            }
            UnityEngine.Debug.Log($"[PCCamera] Notice: UMI3DPCManager is initialized.");

            try
            {
                movement = new PCCameraMovement(this, mouseDelta, lookAroundAction)
                {
                    personalSkeletonContainer = UMI3DPCManager.@default.player.personalSkeletonContainer,
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
            UnityEngine.Debug.Log($"[PCCamera] Notice: PCCamera is initialized.");
        }

        bool IsUMI3DPCManagerInitialized()
        {
            bool isInitialized = true;

            if (UMI3DPCManager.@default.player == null)
            {
                UnityEngine.Debug.Log($"[PCCamera] Notice: Waiting for player to be set.");
                isInitialized = false;
            }
            
            if (UMI3DPCManager.@default.player?.personalSkeletonContainer == null)
            {
                UnityEngine.Debug.Log($"[PCCamera] Notice: Waiting for personalSkeletonContainer to be set.");
                isInitialized = false;
            }

            if (UMI3DPCManager.@default.viewpointPivot == null)
            {
                UnityEngine.Debug.Log($"[PCCamera] Notice: Waiting for viewpointPivot to be set.");
                isInitialized = false;
            }

            if (UMI3DPCManager.@default.neckPivot == null)
            {
                UnityEngine.Debug.Log($"[PCCamera] Notice: Waiting for neckPivot to be set.");
                isInitialized = false;
            }

            if (UMI3DPCManager.@default.head == null)
            {
                UnityEngine.Debug.Log($"[PCCamera] Notice: Waiting for head to be set.");
                isInitialized = false;
            }

            return isInitialized;
        }

        protected override bool IsInitialized()
        {
            return isInitialized;
        }
    }
}