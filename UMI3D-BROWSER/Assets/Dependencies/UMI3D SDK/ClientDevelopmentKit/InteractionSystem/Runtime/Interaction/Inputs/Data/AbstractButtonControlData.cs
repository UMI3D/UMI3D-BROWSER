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

using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace umi3d.cdk.interaction
{
    public abstract class AbstractButtonControlData : AbstractControlData
    {
        /// <summary>
        /// Action phase.
        /// </summary>
        public InputActionPhase phase;
        [HideInInspector] public bool shouldDissociateAsSoonAsPossible;

        public Action<InputActionPhase> actionPerformed;
        public Func<InputActionPhase, bool> canPerform;

        public AbstractButtonControlData()
        {
            phase = InputActionPhase.Canceled;
        }

        /// <summary>
        /// The action performed method add to the <see cref="InputAction.performed"/> event.<br/>
        /// See <see cref="actionPerformed"/>.
        /// </summary>
        /// <param name="obj"></param>
        public void ActionPerformed(InputActionPhase ctx)
        {
            if (canPerform?.Invoke(ctx) ?? true)
            {
                actionPerformed?.Invoke(ctx);
            }

            if (shouldDissociateAsSoonAsPossible)
            {
                dissociate?.Invoke();
                shouldDissociateAsSoonAsPossible = false;
            }
        }
    }
}