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
    [Serializable]
    public abstract class AbstractButtonControlData : AbstractControlData
    {
        public AbstractButtonControlType type;

        /// <summary>
        /// Action phase.
        /// </summary>
        [HideInInspector] public InputActionPhase phase;
        [HideInInspector] public bool shouldDissociateAsSoonAsPossible;

        public Action<InputActionPhase> actionPerformed;
        public Func<InputActionPhase, bool> canPerform;

        public override AbstractControlType Type 
        {
            get
            {
                return type;
            }
        }

        /// <summary>
        /// The action performed method add to the <see cref="InputAction.performed"/> event.<br/>
        /// See <see cref="actionPerformed"/>.
        /// </summary>
        /// <param name="obj"></param>
        public void ActionPerformed(InputActionPhase phase)
        {
            if (canPerform?.Invoke(phase) ?? true)
            {
                actionPerformed?.Invoke(phase);
            }

            if (shouldDissociateAsSoonAsPossible)
            {
                dissociate?.Invoke();
                shouldDissociateAsSoonAsPossible = false;
            }
        }

        public override void Dissociate()
        {
            if (phase != InputActionPhase.Canceled)
            {
                shouldDissociateAsSoonAsPossible = true;
            }
            else
            {
                dissociate?.Invoke();
            }
        }
    }
}