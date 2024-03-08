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
    public sealed class UIButtonControlEntity : AbstractControlEntity, HasButtonControlData
    {
        public ButtonControlData buttonData = new();
        public UIInputType input = new();

        public UIButtonControlEntity()
        {
            controlData.canDissociateHandler = value =>
            {
                if (value is not InputActionPhase phase)
                {
                    return true;
                }

                return phase == InputActionPhase.Canceled;
            };
            controlData.enableHandler += Enable;
            controlData.disableHandler += Disable;
        }

        public ButtonControlData ButtonControlData
        {
            get
            {
                return buttonData;
            }
            set
            {
                buttonData = value;
            }
        }

        public void Disable()
        {
            try
            {
                input.UIInputAction.performed -= UIActionPerformed;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"[UMI3D] Control: ui input type action Exception");
                UnityEngine.Debug.LogException(e);
            }
            
        }

        public void Enable()
        {
            try
            {
                input.UIInputAction.performed += UIActionPerformed;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"[UMI3D] Control: ui input type action Exception");
                UnityEngine.Debug.LogException(e);
            }
        }

        void UIActionPerformed(System.Object value)
        {
            if (value is not InputActionPhase)
            {
                throw new Exception($"[UMI3D] Control: ui input value is not InputActionPhase");
            }
            controlData.ActionPerformed((InputActionPhase)value);
        }
    }
}