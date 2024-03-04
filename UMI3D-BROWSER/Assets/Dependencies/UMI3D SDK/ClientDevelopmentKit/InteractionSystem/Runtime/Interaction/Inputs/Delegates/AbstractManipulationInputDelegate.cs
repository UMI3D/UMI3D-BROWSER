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

using inetum.unityUtils;
using System;
using umi3d.common;
using umi3d.common.interaction;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

namespace umi3d.cdk.interaction
{
    public abstract class AbstractManipulationInputDelegate : AbstractInputDelegate<ManipulationDto>
    {
        public DofGroupDto dof;

        public override void Associate(
            AbstractControlData control,
            ulong environmentId,
            AbstractInteractionDto interaction,
            ulong toolId,
            ulong hoveredObjectId
        )
        {
            if (interaction is not ManipulationDto manipInteraction)
            {
                throw new Exception($"[UMI3D] Control: Interaction is not an {nameof(ManipulationDto)}.");
            }
            if (control is not AbstractManipulationControlData manipControl)
            {
                throw new Exception($"[UMI3D] Control: control is not an {nameof(AbstractManipulationControlData)}.");
            }

            base.Associate(
                control,
                environmentId,
                interaction,
                toolId,
                hoveredObjectId
            );

            control.Enable();
            uint boneType = control
                .controller
                .controllerData_SO
                .BoneType;
            Vector3Dto bonePosition = control
                .controller
                .controllerData_SO
                .BoneTransform
                .position
                .Dto();
            Vector4Dto boneRotation = control
                .controller
                .controllerData_SO
                .BoneTransform
                .rotation
                .Dto();

            manipControl.messageSender.messageHandler = () =>
            {
                ManipulationRequestDto arg = null;
                arg.boneType = boneType;
                arg.bonePosition = bonePosition;
                arg.boneRotation = boneRotation;
                arg.id = interaction.id;
                arg.toolId = toolId;
                arg.hoveredObjectId = hoveredObjectId;
                UMI3DClientServer.SendRequest(arg, true);
            };

            manipControl.actionPerformed += phase =>
            {
                switch (phase)
                {
                    case UnityEngine.InputSystem.InputActionPhase.Started:
                        manipControl.networkMessage 
                            = CoroutineManager
                                .Instance
                                .AttachCoroutine(
                                    manipControl
                                    .messageSender
                                    .NetworkMessageSender()
                                );
                        break;
                    case UnityEngine.InputSystem.InputActionPhase.Canceled:
                        CoroutineManager
                            .Instance
                            .DetachCoroutine(
                                manipControl.networkMessage
                            );
                        break;
                    default:
                        break;
                }
            };
            //TODO

            manipControl.dissociate = () =>
            {
                Dissociate(control);
            };
        }

        /// <summary>
        /// Find the best <see cref="DofGroupOptionDto"/>> for this controller.
        /// </summary>
        /// <param name="options">Options to search in</param>
        /// <returns></returns>
        public abstract DofGroupOptionDto FindBest(DofGroupOptionDto[] options);
    }
}