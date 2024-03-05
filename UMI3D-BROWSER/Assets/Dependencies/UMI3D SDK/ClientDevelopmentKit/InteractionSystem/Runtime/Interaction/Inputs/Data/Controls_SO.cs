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

using inetum.unityUtils.saveSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace umi3d.cdk.interaction
{
    [CreateAssetMenu(fileName = "UMI3D [ControllerName] Control Data", menuName = "UMI3D/Interactions/Control/Control Data")]
    public class Controls_SO : SerializableScriptableObject
    {
        [Header("Buttons")]
        /// <summary>
        /// The physical button type controls for EventDto interactions.<br/>
        /// Value can only be 0/1 (down/up).
        /// </summary>
        [Tooltip("The physical button type controls for EventDto interactions")]
        public List<PhysicalButtonControlEntity> physicalButtonControls = new();

        [Space()]
        /// <summary>
        /// The ui button type control prefabs for EventDto interactions.<br/>
        /// Value can only be 0/1 (down/up).
        /// </summary>
        [Tooltip("The ui button type control prefabs for EventDto interactions")]
        public List<UIButtonControlEntity> uIButtonControlPrefabs;
        [HideInInspector] 
        public List<UIButtonControlEntity> uIButtonControls = new();

        [Header("Manipulations")]
        public List<PhysicalManipulationControlEntity> physicalManipulationControls = new();

        [Space()]
        public List<UIManipulationControlEntity> uIManipulationControlPrefabs = new();
        [HideInInspector]
        public List<UIManipulationControlEntity> uIManipulationControls = new();

        [Space()]
        /// <summary>
        /// The button type controls for shortcuts.<br/>
        /// Value can only be 0/1 (down/up).
        /// </summary>
        [Tooltip("The button type controls for shortcuts")]
        public List<PhysicalButtonControlEntity> shortcuts = new();
    }
}