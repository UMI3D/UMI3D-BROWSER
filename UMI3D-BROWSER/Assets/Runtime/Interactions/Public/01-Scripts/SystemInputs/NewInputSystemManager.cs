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
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace umi3d.browserRuntime.interactions
{
    public class NewInputSystemManager
    {
        #region Initialize

        static Lazy<NewInputSystemManager> _default = new(() => new());
        public static NewInputSystemManager @default => _default.Value;

        NewInputSystemManager()
        {
        }

        #endregion

        List<NewInputSystem> _input = new();
        ReadOnlyCollection<NewInputSystem> input => _input.AsReadOnly();

        public bool TryToInstantiateInput(out NewInputSystem inputSystem, InputControl control)
        {
            inputSystem = _input.Find(input => input.id == control.path);
            if (inputSystem != null) { return false; }

            switch (control)
            {
                case ButtonControl button:
                    inputSystem = new ButtonControlInputSystem(button);
                    break;

                case AxisControl axisControl:
                    inputSystem = new AxisControlInputSystem(axisControl);
                    break;

                case IntegerControl integerControl:
                    inputSystem = new IntegerControlInputSystem(integerControl);
                    break;

                case DoubleControl doubleControl:
                    inputSystem = new DoubleControlInputSystem(doubleControl);
                    break;

                case Vector2Control vector2Control:
                    inputSystem = new Vector2ControlInputSystem(vector2Control);
                    break;

                case Vector3Control vector3Control:
                    inputSystem = new Vector3ControlInputSystem(vector3Control);
                    break;

                case QuaternionControl quatControl:
                    inputSystem = new QuaternionControlInputSystem(quatControl);
                    break;

                default:
                    UnityEngine.Debug.Log($"[NewInputSystemManager] Error: unhandled case: {control?.GetType().ToString() ?? "control null"}");
                    return false;
            }

            _input.Add(inputSystem);
            return true;
        }

        public void Clear()
        {
            _input.Clear();
        }
    }
}