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

using inetum.unityUtils;
using System;
using umi3d.cdk.interaction;
using umi3d.cdk.userCapture.tracking;
using UnityEngine;

namespace umi3d.browserRuntime.interaction
{
    public class UMI3DVRController : MonoBehaviour
    {
        [Header("Manager")]
        public UMI3DController controller;
        public UMI3DControlManager controlManager;
        public UMI3DToolManager toolManager;
        public ProjectionManager projectionManager;

        [Header("Hierarchy References")]
        public Tracker bone;
        /// <summary>
        /// Transform reference to track translation and rotation.
        /// </summary>
        public Transform manipulationTransform;

        private void Awake()
        {
            controller.Init(
                this,
                controlManager
            );
            controlManager.Init(
                this,
                controller
            );
            toolManager.Init();
            projectionManager.Init(
                this,
                controller,
                controlManager,
                toolManager
            );
        }

        private void OnEnable()
        {
            controller.Enable();
        }

        private void OnDisable()
        {
            controller.Disable();
        }
    }
}