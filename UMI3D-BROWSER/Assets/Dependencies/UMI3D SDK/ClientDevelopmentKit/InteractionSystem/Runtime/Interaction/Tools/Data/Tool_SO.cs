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
    [CreateAssetMenu(fileName = "UMI3D Tool Data For [ControllerName]", menuName = "UMI3D/Interactions/Tool Data")]
    public class Tool_SO : SerializableScriptableObject
    {
        /// <summary>
        /// The currently hovered tool.
        /// </summary>
        public AbstractTool currentHoverTool;

        /// <summary>
        /// The currently projected tools.
        /// </summary>
        public List<AbstractTool> projectedTools = new();

        /// <summary>
        /// A Tool:[Input] dictionary
        /// </summary>
        public Dictionary<ulong, AbstractControlEntity[]> inputsByTool = new();
    }
}