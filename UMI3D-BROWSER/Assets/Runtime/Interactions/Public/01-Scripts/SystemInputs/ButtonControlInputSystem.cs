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
    public class ButtonControlInputSystem : NewInputSystem, IEventInputSystem, IParameterInputSystem<bool>
    {
        public ButtonControlInputSystem(ButtonControl button) : base(button, UnityEngine.InputSystem.InputActionType.Button)
        {
        }

        #region IEventInputSystem

        public event Action started;
        public event Action canceled;

        public void PressDown()
        {
            try
            {
                started?.Invoke();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
        }

        public void PressUp()
        {
            try
            {
                canceled?.Invoke();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
        }

        void IEventInputSystem.Clear()
        {
            started = null;
            canceled = null;
        }

        #endregion

        #region IParameterInputSystem<bool>

        public event Action<bool> performed;

        public void Perform(bool value)
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

        void IParameterInputSystem<bool>.Clear()
        {
            performed = null;
        }

        #endregion

        public void Clear()
        {
            (this as IEventInputSystem).Clear();
            (this as IParameterInputSystem<bool>).Clear();
        }

        protected override void OnStarted(InputAction.CallbackContext context)
        {
            PressDown();
        }

        protected override void OnPerformed(InputAction.CallbackContext context)
        {
        }

        protected override void OnCanceled(InputAction.CallbackContext context)
        {
            PressUp();
        }
    }
}