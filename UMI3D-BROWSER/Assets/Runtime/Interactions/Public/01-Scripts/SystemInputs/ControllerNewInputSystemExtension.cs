/*
Copyright 2019 - 2025 Inetum

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

using System;
using System.Collections.Generic;
using umi3d.cdk.interaction;
using UnityEngine;
using UnityEngine.InputSystem;
using Input = umi3d.cdk.interaction.Input;

namespace umi3d.browserRuntime.interactions
{
    public static class ControllerNewInputSystemExtension
    {
        /// <summary>
        /// Try to add the input corresponding to this <paramref name="control"/> to the list of <paramref name="inputs"/>.<br/>
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="control"></param>
        /// <returns></returns>
        public static bool TryToAddInput(this Controller controller, List<Input> inputs, InputControl control)
        {
            NewInputSystemManager.@default.TryToInstantiateInput(out NewInputSystem inputSystem, control);
            return controller.TryToAddInput(inputs, inputSystem);
        }
    }
}