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
using UnityEngine;

namespace umi3d.cdk.interaction
{
    public abstract class AbstractToolSelectionDelegate : SerializableScriptableObject
    {
        /// <summary>
        /// Request the selection of a Tool.
        /// Be careful,this method could be called before the tool is added for async loading reasons.
        /// Returns true if the tool has been successfuly selected, false otherwise.
        /// </summary>
        /// <param name="toolId">Id of the tool to release.</param>
        /// <param name="releasable">The selected tool releasable.</param>
        /// <param name="hoveredObjectId">The id of the hovered object.</param>
        /// <param name="reason">Interaction mapping reason.</param>
        /// <returns></returns>
        public abstract bool SelectTool(
            ulong environmentId,
            ulong toolId,
            bool releasable,
            ulong hoveredObjectId,
            InteractionMappingReason reason = null
        );

        /// <summary>
        /// Request a Tool to be released.
        /// </summary>
        /// <param name="toolId">Id of the tool to release.</param>
        /// <param name="reason">Interaction mapping reason.</param>
        public abstract void ReleaseTool(
            ulong environmentId,
            ulong toolId,
            InteractionMappingReason reason = null
        );

        /// <summary>
        /// Return true if the tool is currently projected on a controller.
        /// </summary>
        /// <param name="id">Id of the tool.</param>
        /// <returns></returns>
        public abstract bool IsToolSelected(ulong environmentId, ulong id);
    }
}