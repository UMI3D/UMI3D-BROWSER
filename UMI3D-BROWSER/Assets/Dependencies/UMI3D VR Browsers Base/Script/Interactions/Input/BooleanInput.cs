/*
Copyright 2019 - 2022 Inetum

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

using System.Collections;
using umi3d.cdk;
using umi3d.cdk.userCapture.pose;
using umi3d.common;
using umi3d.common.interaction;
using umi3dVRBrowsersBase.ui.keyboard;
using umi3dVRBrowsersBase.ui.playerMenu;
using UnityEngine;
using UnityEngine.Events;

namespace umi3dVRBrowsersBase.interactions.input
{
    /// <summary>
    /// Input for UMI3D Event.
    /// </summary>
    [System.Serializable]
    public class BooleanInput : AbstractVRInput
    {
        #region Fields

        /// <summary>
        /// Oculus input observer binded to this input.
        /// </summary>
        public VRInputObserver vrInput;

        /// <summary>
        /// Event raised on user input release (first frame).
        /// </summary>
        [SerializeField]
        protected UnityEvent onActionUp = new UnityEvent();

        /// <summary>
        /// Event raised on user input (first frame only).
        /// </summary>
        [SerializeField]
        protected UnityEvent onActionDown = new UnityEvent();


        /// <summary>
        /// True if the rising edge event has been sent through network (to avoid sending falling edge only).
        /// </summary>
        private bool risingEdgeEventSent = false;

        /// <summary>
        /// Is input button down ?
        /// </summary>
        private bool isDown = false;

        public class VRInteractionEvent : UnityEvent<uint> { };

        [HideInInspector]
        public static VRInteractionEvent BooleanEvent = new VRInteractionEvent();

        #endregion

        #region Methods

        /// <summary>
        /// Callback called on oculus input up.
        /// </summary>
        /// <param name="fromAction"></param>
        /// <param name="fromSource"></param>
        /// <see cref="Associate(AbstractInteractionDto)"/>
        private void VRInput_onStateUp()
        {
            if (PlayerMenuManager.Instance.parameterGear.IsHovered
                || PlayerMenuManager.Instance.IsMenuHovered
                || (Keyboard.Instance?.IsOpen ?? false))
                return;

            onActionUp.Invoke();
        }

        /// <summary>
        /// Callback called on oculus input down.
        /// </summary>
        /// <param name="fromAction"></param>
        /// <param name="fromSource"></param>
        /// <see cref="Associate(AbstractInteractionDto)"/>
        private void VRInput_onStateDown()
        {
            if (PlayerMenuManager.Instance.parameterGear.IsHovered
                || PlayerMenuManager.Instance.IsMenuHovered
                || (Keyboard.Instance?.IsOpen ?? false))
                return;

            onActionDown.Invoke();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="interaction"></param>
        /// <param name="toolId"></param>
        /// <param name="hoveredObjectId"></param>
        public override void Associate(ulong environmentId, AbstractInteractionDto interaction, ulong toolId, ulong hoveredObjectId)
        {
            UnityAction<bool> action = (bool pressDown) =>
            {
                if (pressDown)
                {
                    if ((interaction as EventDto).hold)
                    {
                        
                        risingEdgeEventSent = true;
                    }
                    else
                    {
                        
                    }
                    (controller as VRController).IsInputPressed = true;
                    isDown = true;


                    if ((interaction as EventDto).TriggerAnimationId != 0)
                    {
                        BooleanEvent.Invoke(boneType);
                    }

                    onInputDown.Invoke();
                }
                else
                {
                    if ((interaction as EventDto).hold)
                    {
                        if (risingEdgeEventSent)
                        {
                            risingEdgeEventSent = false;
                        }
                    }
                    (controller as VRController).IsInputPressed = false;
                    isDown = false;
                    onInputUp.Invoke();


                    if ((interaction as EventDto).ReleaseAnimationId != 0)
                    {
                        BooleanEvent.Invoke(boneType);
                    }
                }
            };

            base.Associate(environmentId, interaction, toolId, hoveredObjectId);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="manipulation"></param>
        /// <param name="dofs"></param>
        /// <param name="toolId"></param>
        /// <param name="hoveredObjectId"></param>
        public override void Associate(ulong environmentId, ManipulationDto manipulation, DofGroupEnum dofs, ulong toolId, ulong hoveredObjectId)
        {
            throw new System.Exception("Boolean input is not compatible with manipulation");
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Dissociate()
        {
            
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="interaction"></param>
        /// <returns></returns>
        public override bool IsCompatibleWith(AbstractInteractionDto interaction)
        {
            return (interaction is EventDto);
        }

        #endregion
    }
}