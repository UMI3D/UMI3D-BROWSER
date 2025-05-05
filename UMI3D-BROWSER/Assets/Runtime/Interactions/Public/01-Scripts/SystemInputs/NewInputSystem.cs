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
    public abstract class NewInputSystem : IInputSystem
    {
        public string id => control.path;

        public InputAction action { get; private set; }
        public InputControl control { get; private set; }

        protected NewInputSystem(InputControl control, InputActionType actionType)
        {
            this.control = control;

            action = new InputAction(control.name, actionType, binding: control.path);

            action.started += OnStarted;
            action.performed += OnPerformed;
            action.canceled += OnCanceled;
        }

        ~NewInputSystem()
        {
            action.started -= OnStarted;
            action.performed -= OnPerformed;
            action.canceled -= OnCanceled;
        }

        abstract protected void OnStarted(InputAction.CallbackContext context);
        abstract protected void OnPerformed(InputAction.CallbackContext context);
        abstract protected void OnCanceled(InputAction.CallbackContext context);
    }
}