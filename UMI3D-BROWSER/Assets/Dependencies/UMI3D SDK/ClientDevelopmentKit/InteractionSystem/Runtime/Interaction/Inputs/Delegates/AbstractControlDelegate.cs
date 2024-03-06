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
using UnityEngine.InputSystem;

namespace umi3d.cdk.interaction
{
    public abstract class AbstractControlDelegate<Interaction>
        where Interaction: AbstractInteractionDto
    {
        protected abstract bool CanPerform(InputActionPhase phase);

        /// <summary>
        /// Return a control compatible with <paramref name="interaction"/>.<br/> 
        /// Return null if no control is available.
        /// </summary>
        /// <param name="interaction"></param>
        /// <param name="unused"></param>
        /// <returns></returns>
        public abstract AbstractControlEntity GetControl(
            UMI3DController controller, 
            Interaction interaction
        );

        public virtual void Associate(
            UMI3DController controller,
            AbstractControlEntity control,
            ulong environmentId,
            AbstractInteractionDto interaction,
            ulong toolId,
            ulong hoveredObjectId
        )
        {
            control.controlData.controller = controller;
            control.controlData.isUsed = true;
            control.controlData.environmentId = environmentId;
            control.controlData.toolId = toolId;
            control.controlData.interaction = interaction;
            control.controlData.dissociateHandler = () =>
            {
                Dissociate(control);
            };
            control.controlData.canPerformHandler = CanPerform;
            control.controlData.enableHandler?.Invoke();
        }

        public virtual void Dissociate(AbstractControlEntity control)
        {
            control.controlData.controller = null;
            control.controlData.isUsed = false;
            control.controlData.environmentId = 0;
            control.controlData.toolId = 0;
            control.controlData.interaction = null;
            control.controlData.disableHandler?.Invoke();
        }
    }
}