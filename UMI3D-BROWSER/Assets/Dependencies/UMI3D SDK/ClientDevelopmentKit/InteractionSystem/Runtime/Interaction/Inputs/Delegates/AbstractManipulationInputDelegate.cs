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
            ManipulationControlData manipData;
            if (control is PhysicalManipulationControlData phManipControl)
            {
                manipData = phManipControl.manipulationData;
            }
            else if (control is UIManipulationControlData uiManipControl)
            {
                manipData = uiManipControl.manipulationData;
            }
            else
            {
                throw new Exception($"[UMI3D] Control: control is not a manipulation.");
            }

            base.Associate(
                control,
                environmentId,
                interaction,
                toolId,
                hoveredObjectId
            );

            manipData.frameOfReference = UMI3DEnvironmentLoader
                .GetNode(
                    environmentId,
                    manipInteraction.frameOfReference
                )
                .gameObject
                .transform;

            uint boneType = control
                .controlData
                .controller
                .controllerData_SO
                .BoneType;
            Vector3Dto bonePosition = control
                .controlData
                .controller
                .controllerData_SO
                .BoneTransform
                .position
                .Dto();
            Vector4Dto boneRotation = control
                .controlData
                .controller
                .controllerData_SO
                .BoneTransform
                .rotation
                .Dto();

            manipData.messageSender.messageHandler = () =>
            {
                ManipulationRequestDto request = new(); 
                manipData.SetRequestTranslationAndRotation(
                    dof.dofs, 
                    request,
                    control
                        .controlData
                        .controller
                        .controllerData_SO
                        .ManipulationTransform
                );
                request.boneType = boneType;
                request.bonePosition = bonePosition;
                request.boneRotation = boneRotation;
                request.id = interaction.id;
                request.toolId = toolId;
                request.hoveredObjectId = hoveredObjectId;
                UMI3DClientServer.SendRequest(request, true);
            };

            control.controlData.actionPerformedHandler += phase =>
            {
                switch (phase)
                {
                    case UnityEngine.InputSystem.InputActionPhase.Started:
                        manipData.initialPosition = 
                            control
                                .controlData
                                .controller
                                .controllerData_SO
                                .ManipulationTransform
                                .position;
                        manipData.initialRotation =
                           control
                               .controlData
                               .controller
                               .controllerData_SO
                               .ManipulationTransform
                               .rotation;

                        manipData.messageSender.networkMessage
                            = CoroutineManager
                                .Instance
                                .AttachCoroutine(
                                    manipData
                                    .messageSender
                                    .NetworkMessageSender()
                                );
                        break;
                    case UnityEngine.InputSystem.InputActionPhase.Canceled:
                        CoroutineManager
                            .Instance
                            .DetachCoroutine(
                                manipData.messageSender.networkMessage
                            );
                        break;
                    default:
                        break;
                }
            };
            //TODO
        }

        public override AbstractControlData GetControl(ManipulationDto interaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Find the best <see cref="DofGroupOptionDto"/>> for this controller.
        /// </summary>
        /// <param name="options">Options to search in</param>
        /// <returns></returns>
        public abstract DofGroupOptionDto FindBest(DofGroupOptionDto[] options);
    }
}