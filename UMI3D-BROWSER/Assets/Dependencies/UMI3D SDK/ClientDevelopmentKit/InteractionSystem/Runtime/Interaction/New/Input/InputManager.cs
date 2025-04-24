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
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine.InputSystem;

namespace umi3d.cdk.interaction
{
    public sealed class InputManager 
    {
        #region Initialize

        static Lazy<InputManager> _default = new(() => new());
        public static InputManager @default => _default.Value;

        InputManager()
        {

        }

        #endregion

        List<Input> _inputs = new List<Input>();
        ReadOnlyCollection<Input> inputs => _inputs.AsReadOnly();

        public Input InstantiateInput(InputControl control, InputActionType actionType)
        {
            Input input = new(control, actionType);
            _inputs.Add(input);

            return input;
        }

        /// <summary>
        /// Try to get the input corresponding to this <paramref name="control"/> for this controller.<br/>
        /// <br/>
        /// If no input corresponding to this control is found then create one with this <paramref name="actionType"/>.<br/>
        /// <br/>
        /// Which actionType to choose:
        /// <list type="bullet">
        /// <item>
        /// Value:<br/>
        /// Action used to read a continuous or single value (e.g., joystick position, trigger pressure).<br/>
        /// Calls the following phases:<br/>
        /// - started: When the input starts changing.<br/>
        /// - performed: On every value update.<br/>
        /// - canceled: When the input is canceled (e.g., returns to a neutral value).
        /// </item>
        /// <item>
        /// Button:<br/>
        /// Action triggered by a button press or release (e.g., a key or gamepad button).<br/>
        /// Calls the following phases:<br/>
        /// - started: When a button is pressed.<br/>
        /// - performed: When the button reaches its activation threshold (default: full press).<br/>
        /// - canceled: When the button is released.
        /// </item>
        /// <item>
        /// PassThrough:<br/>
        /// Action that directly passes input without state or context management (useful for continuous input or multiple simultaneous inputs, e.g., multiple joystick movements).<br/>
        /// Only calls the performed phase on every input update.<br/>
        /// Does not handle started or canceled phases, as there is no state tracking.
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="input"></param>
        /// <param name="controller"></param>
        /// <param name="control"></param>
        /// <param name="actionType"></param>
        /// <returns></returns>
        public bool TryGetInput(out Input input, Controller controller, InputControl control, InputActionType actionType)
        {
            input = controller.inputs.FirstOrDefault(input => input.control == control);
            if (input == null)
            {
                input = InstantiateInput(control, actionType);
                controller.Add(input);
                return true;
            }

            return input.isAvailable;
        }
    }
}