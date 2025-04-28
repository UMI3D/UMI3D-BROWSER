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
using umi3d.cdk.interaction;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using Input = umi3d.cdk.interaction.Input;

namespace umi3d.browserRuntime.interactions
{
    public class NewInputSystem : IInputSystem
    {
        public string id => control.path;

        public InputAction action { get; private set; }
        public InputControl control { get; private set; }

        public NewInputSystem(InputControl control, InputActionType actionType)
        {
            this.control = control;

            action = new InputAction(control.name, actionType, binding: control.path);

            action.started += OnStarted;
            action.performed += OnPerformed;
            action.canceled += OnCanceled;
        }

        void OnStarted(InputAction.CallbackContext context)
        {
            started?.Invoke(context.ReadValueAsObject());
        }
        void OnPerformed(InputAction.CallbackContext context)
        {
            performed?.Invoke(context.ReadValueAsObject());
        }
        void OnCanceled(InputAction.CallbackContext context)
        {
            canceled?.Invoke(context.ReadValueAsObject());
        }

        public event Action<object> started;
        public event Action<object> performed;
        public event Action<object> canceled;

        public T ReadValue<T>()
        {
            object value = action.ReadValueAsObject();
            if (value is T valueT) { return valueT; }
            else { return default; }
        }

        public void WriteClassValue<T>(T value) where T : class
        {
            UnityEngine.Debug.LogError($"Error: the new input system does not work with classes.");
        }

        public void WriteStructValue<T>(T value) where T : struct
        {
            using (StateEvent.From(control.device, out var eventPtr))
            {
                control.WriteValueIntoEvent(value, eventPtr);
                InputSystem.QueueEvent(eventPtr);
            }
        }

        public void OnStartedForTest<T>(T value) where T : struct
        {
            started?.Invoke(value);
        }
        public void OnPerformedForTest<T>(T value) where T : struct
        {
            performed?.Invoke(value);
        }
        public void OnCanceledForTest<T>(T value) where T : struct
        {
            canceled?.Invoke(value);
        }
    }

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

        public bool TryToInstantiateInput(out NewInputSystem inputSystem, InputControl control, InputActionType actionType)
        {
            inputSystem = _input.Find(input => input.id == control.path);
            if (inputSystem != null)
            {
                UnityEngine.Debug.LogWarning($"[NewInputSystemManager] Warning: Cannot instantiate input for '{control.path}' because this input already exist.");
                return false;
            }

            inputSystem = new(control, actionType);
            _input.Add(inputSystem);
            return true;
        }

        public void Clear()
        {
            _input.Clear();
        }
    }

    public static class ControllerNewInputSystemExtension
    {
        /// <summary>
        /// Try to add the input corresponding to this <paramref name="control"/> to the list of <paramref name="inputs"/>.<br/>
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
        /// <param name="inputs"></param>
        /// <param name="control"></param>
        /// <param name="actionType"></param>
        /// <returns></returns>
        public static bool TryToAddInput(this Controller controller, List<Input> inputs, InputControl control, InputActionType actionType)
        {
            NewInputSystemManager.@default.TryToInstantiateInput(out NewInputSystem inputSystem, control, actionType);
            return controller.TryToAddInput(inputs, inputSystem);
        }
    }

}