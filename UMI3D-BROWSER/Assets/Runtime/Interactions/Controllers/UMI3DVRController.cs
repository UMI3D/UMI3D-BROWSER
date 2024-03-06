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
    public class UMI3DVRController : MonoBehaviour, IControllerDelegate
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

        public Transform Transform
        {
            get
            {
                return transform;
            }
        }
        public uint BoneType
        {
            get
            {
                return bone.BoneType;
            }
        }
        public Transform BoneTransform
        {
            get
            {
                return bone.transform;
            }
        }
        public Transform ManipulationTransform
        {
            get
            {
                return manipulationTransform.transform;
            }
        }

        private void Awake()
        {
            controller.controllerDelegate = this;
            controller.Init(
                this,
                controlManager
            );

            controlManager.manipulationDelegate = new VRManipulationControlDelegate();
            controlManager.eventDelegate = new VREventControlDelegate();
            controlManager.formDelegate = new VRFormControlDelegate();
            controlManager.linkDelegate = new VRLinkControlDelegate();
            controlManager.parameterDelegate = new VRParameterControlDelegate();
            controlManager.Init(
                this,
                controller
            );

            toolManager.Init();

            projectionManager.ptManipulationNodeDelegate = new ProjectionTreeManipulationNodeDelegate();
            projectionManager.ptEventNodeDelegate = new ProjectionTreeEventNodeDelegate();
            projectionManager.ptFormNodeDelegate = new ProjectionTreeFormNodeDelegate();
            projectionManager.ptLinkNodeDelegate = new ProjectionTreeLinkNodeDelegate();
            projectionManager.ptParameterNodeDelegate = new ProjectionTreeParameterNodeDelegate();
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