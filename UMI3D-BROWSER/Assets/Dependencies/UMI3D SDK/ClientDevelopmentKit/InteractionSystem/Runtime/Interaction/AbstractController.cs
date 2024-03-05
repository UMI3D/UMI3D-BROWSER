﻿/*
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
using System.Collections.Generic;
using umi3d.common;
using umi3d.common.userCapture;
using UnityEngine;

namespace umi3d.cdk.interaction
{
    /// <summary>
    /// A controller is a set of inputs.<br/>
    /// <br/>
    /// 
    /// <example>
    /// For example: The following device can be seen as a controller.
    /// <list type="bullet">
    /// <item>A VR controller.</item>
    /// <item>A keyboard and a mouse.</item>
    /// </list>
    /// </example>
    /// 
    /// You have as many controller as you have of selector.<br/>
    /// <example>
    /// For example: A VR headset has 2 controllers with laser to select. A computer as 1 mouse to select (mouse and trackpad can be seen as the same input).
    /// </example>
    /// </summary>
    [System.Serializable]
    public abstract class AbstractController : MonoBehaviour
    {
        [Header("Controller")]
        public AbstractControllerData_SO controllerData_SO;
        public AbstractControllerDelegate controllerDelegate;

        [Header("Manager")]
        public UMI3DControlManager controlManager;
        public UMI3DToolManager toolManager;
        public ProjectionManager projectionManager;

        [Header("Hierarchy References")]
        [SerializeField, ConstEnum(typeof(BoneType), typeof(uint))]
        public uint boneType;
        public Transform boneTransform;
        /// <summary>
        /// Transform reference to track translation and rotation.
        /// </summary>
        public Transform manipulationTransform;

        public readonly static List<AbstractController> activeControllers = new();

        private void Awake()
        {
            controlManager.Init(
                this,
                this
            );
            toolManager.Init();
            projectionManager.Init(
                this, 
                controllerDelegate,
                controlManager, 
                toolManager
            );
        }

        private void OnEnable()
        {
            activeControllers.Add(this);
        }

        private void OnDisable()
        {
            activeControllers.Remove(this);
        }

        /// <summary>
        /// Clear all menus and the projected tools
        /// </summary>
        public virtual void Clear()
        {
            projectionManager.Release(null, null);

            //toolManager.ReleaseTool(UMI3DGlobalID.EnvironmentId, toolManager.toolDelegate.Tool.id);
        }
    }
}