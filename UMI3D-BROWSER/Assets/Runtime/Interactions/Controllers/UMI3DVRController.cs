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
using umi3d.cdk.collaboration;
using umi3d.cdk.interaction;
using umi3d.cdk.userCapture.tracking;
using UnityEngine;

namespace umi3d.browserRuntime.interaction
{
    public class UMI3DVRController : MonoBehaviour, IControllerDelegate
    {
        //[SerializeField]
        umi3d.debug.UMI3DLogger logger = new();

        [Header("Manager Data")]
        public AbstractControllerData_SO controllerData_SO;
        public Controls_SO controls_SO;
        public Tool_SO tool_SO;
        public ProjectionTree_SO projectionTree_SO;
        public ProjectionEventSystem eventSystem;

        public UMI3DController controller = new();
        public UMI3DControlManager controlManager = new();
        public UMI3DToolManager toolManager = new();
        public UMI3DProjectionManager projectionManager = new();

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
            UnityEngine.Physics.queriesHitBackfaces = true;

            logger.MainContext = this;
            logger.MainTag = nameof(UMI3DVRController);

            Initialize();

            var manipulationManager = new ManipulationManager();
            var eventManager = new EventManager();
            var formManager = new FormManager();
            var linkManager = new LinkManager();
            var parameterManager = new ParameterManager();

            controller.controllerData_SO = controllerData_SO;
            controller.controllerDelegate = this;
            controller.Init(
                this,
                controlManager,
                toolManager,
                projectionManager
            );

            controlManager.controls_SO = controls_SO;
            controlManager.manipulationDelegate 
                = new VRManipulationControlDelegate(manipulationManager);
            controlManager.eventDelegate 
                = new VREventControlDelegate(eventManager);
            controlManager.formDelegate 
                = new VRFormControlDelegate(formManager);
            controlManager.linkDelegate 
                = new VRLinkControlDelegate(linkManager);
            controlManager.parameterDelegate 
                = new VRParameterControlDelegate(parameterManager);
            controlManager.Init(
                this,
                controller
            );

            toolManager.tool_SO = tool_SO;
            toolManager.Init(
                this,
                controller
            );

            projectionManager.projectionTree_SO = projectionTree_SO;
            projectionManager.eventSystem = eventSystem;
            projectionManager.ptManipulationNodeDelegate = manipulationManager;
            projectionManager.ptEventNodeDelegate = eventManager;
            projectionManager.ptFormNodeDelegate = formManager;
            projectionManager.ptLinkNodeDelegate = linkManager;
            projectionManager.ptParameterNodeDelegate = parameterManager;
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

        static bool isInitialized = false;
        static void Initialize()
        {
            if (isInitialized)
            {
                return;
            }
            else
            {
                isInitialized = true;
            }

            UMI3DCollabLoadingParameters.unknownOperationHandlerDto += UMI3DToolManager.UnknownOperationHandler;
            UMI3DCollabLoadingParameters.unknownOperationHandlerByte += UMI3DToolManager.UnknownOperationHandler;
        }
    }
}