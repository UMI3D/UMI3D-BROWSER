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
using umi3d.cdk.interaction;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace umi3d.browserRuntime.interactions
{
    public class Vector3ControlInputSystem : NewInputSystem, IParameterInputSystem<Vector3>
    {
        public Vector3ControlInputSystem(Vector3Control control): base(control, InputActionType.PassThrough)
        {
        }

        public event Action<Vector3> performed;

        public void Clear()
        {
            performed = null;
        }

        public void Perform(Vector3 value)
        {
            try
            {
                performed?.Invoke(value);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
        }

        protected override void OnCanceled(InputAction.CallbackContext context)
        {
        }

        protected override void OnPerformed(InputAction.CallbackContext context)
        {
            Perform(context.ReadValue<Vector3>());
        }

        protected override void OnStarted(InputAction.CallbackContext context)
        {
        }
    }
}