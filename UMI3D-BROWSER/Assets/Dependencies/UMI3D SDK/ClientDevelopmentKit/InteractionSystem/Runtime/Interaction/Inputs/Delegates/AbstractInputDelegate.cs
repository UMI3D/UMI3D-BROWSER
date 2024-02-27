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
    public abstract class AbstractInputDelegate<Interaction>: SerializableScriptableObject
        where Interaction: AbstractInteractionDto
    {
        public ControlModel model;

        public virtual void Init(ControlModel model)
        {
            this.model = model;
        }

        /// <summary>
        /// Return the id of a control.<br/> 
        /// Return null if no control is available.
        /// </summary>
        /// <param name="interaction"></param>
        /// <param name="unused"></param>
        /// <returns></returns>
        public abstract Guid? GetControlId(Interaction interaction, bool unused = true);
    }
}