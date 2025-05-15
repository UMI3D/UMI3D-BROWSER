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
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace umi3d.browserRuntime.interactions
{
    public class DoubleControlInputSystem : NewInputSystem, IParameterInputSystem<float>
    {
        public DoubleControlInputSystem(DoubleControl control) : base(control, InputActionType.PassThrough)
        {
        }

        public event Action<float> performed;

        public void Clear()
        {
            performed = null;
        }

        public void Perform(float value)
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
            Perform((float)context.ReadValue<double>());
        }

        protected override void OnStarted(InputAction.CallbackContext context)
        {
        }
    }
}