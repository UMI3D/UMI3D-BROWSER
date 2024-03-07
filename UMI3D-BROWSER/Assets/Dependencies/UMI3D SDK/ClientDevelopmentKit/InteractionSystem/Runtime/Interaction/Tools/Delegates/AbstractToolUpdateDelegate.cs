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
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.cdk.interaction
{
    public abstract class AbstractToolUpdateDelegate : SerializableScriptableObject
    {
        [HideInInspector]
        public List<Tool_SO> tool_SOs = new();

        public virtual void Init(Tool_SO tool_SO)
        {
            tool_SOs.Add(tool_SO);
        }

        /// <summary>
        /// Request a Tool to be updated.
        /// </summary>
        /// <param name="toolId">Id of the Tool.</param>
        /// <param name="releasable">Is the tool releasable.</param>
        /// <param name="reason">Interaction mapping reason.</param>
        /// <returns></returns>
        public abstract bool UpdateTools(
            ulong environmentId,
            ulong toolId,
            bool releasable,
            InteractionMappingReason reason = null
        );

        /// <summary>
        /// Request a Tool to be updated when one element was added on the tool.
        /// </summary>
        /// <param name="toolId">Id of the Tool.</param>
        /// <param name="releasable">Is the tool releasable.</param>
        /// <param name="reason">Interaction mapping reason.</param>
        /// <returns></returns>
        public abstract bool UpdateAddOnTools(
            ulong environmentId,
            ulong toolId,
            bool releasable,
            AbstractInteractionDto abstractInteractionDto,
            InteractionMappingReason reason = null
        );

        /// <summary>
        /// Request a Tool to be updated when one element was removed on the tool.
        /// </summary>
        /// <param name="toolId">Id of the Tool.</param>
        /// <param name="releasable">Is the tool releasable.</param>
        /// <param name="reason">Interaction mapping reason.</param>
        /// <returns></returns>
        public abstract bool UpdateRemoveOnTools(
            ulong environmentId,
            ulong toolId,
            bool releasable,
            AbstractInteractionDto abstractInteractionDto,
            InteractionMappingReason reason = null
        );
    }
}