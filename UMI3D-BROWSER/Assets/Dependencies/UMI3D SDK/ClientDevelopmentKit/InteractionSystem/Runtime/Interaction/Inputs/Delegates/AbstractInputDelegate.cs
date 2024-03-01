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
    public abstract class AbstractInputDelegate<Interaction>: SerializableScriptableObject
        where Interaction: AbstractInteractionDto
    {
        public ControlModel model;

        public virtual void Init(ControlModel model)
        {
            this.model = model;
        }

        /// <summary>
        /// Return a control compatible with <paramref name="interaction"/>.<br/> 
        /// Return null if no control is available.
        /// </summary>
        /// <param name="interaction"></param>
        /// <param name="unused"></param>
        /// <returns></returns>
        public abstract AbstractControlData GetControl(Interaction interaction);

        public void Associate(
            Guid controlId,
            ulong environmentId,
            AbstractInteractionDto interaction,
            ulong toolId,
            ulong hoveredObjectId
        )
        {
            var control = model
                .controls_SO[controlId, default(AbstractButtonControlType)];
            if (control == null)
            {
                throw new ArgumentNullException($"[UMI3D] Control: control is null for id: {controlId}");
            }
            if (control.isUsed)
            {
                throw new Exception($"[UMI3D] Control: control is already used for {control.GetType().Name}, {typeof(Interaction).Name}.");
            }

            Associate(
                control,
                environmentId,
                interaction,
                toolId,
                hoveredObjectId
            );
        }

        public virtual void Associate(
            AbstractControlData control,
            ulong environmentId,
            AbstractInteractionDto interaction,
            ulong toolId,
            ulong hoveredObjectId
        )
        {
            control.isUsed = true;
            control.environmentId = environmentId;
            control.toolId = toolId;
            control.interaction = interaction;
        }

        public void Dissociate(Guid controlId)
        {
            var control = model
                .controls_SO[controlId, default(AbstractButtonControlType)];

            if (control == null)
            {
                throw new ArgumentNullException($"[UMI3D] Control: control is null for id: {controlId}");
            }

            control.Dissociate();
        }

        public virtual void Dissociate(AbstractControlData control)
        {
            control.isUsed = false;
            control.environmentId = 0;
            control.toolId = 0;
            control.interaction = null;
        }
    }
}