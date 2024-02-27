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
        public List<AbstractUMI3DInput> inputs;

        /// <summary>
        /// Return an input for a given parameter.<br/> 
        /// Return null if no input is available.
        /// </summary>
        /// <param name="param"></param>
        /// <param name="unused"></param>
        /// <returns></returns>
        public abstract AbstractUMI3DInput FindInput(Interaction param, bool unused = true);
    }
}