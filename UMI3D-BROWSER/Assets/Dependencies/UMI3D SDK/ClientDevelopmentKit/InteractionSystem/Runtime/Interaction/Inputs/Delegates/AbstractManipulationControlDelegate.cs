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
    public abstract class AbstractManipulationControlDelegate : AbstractControlDelegate<ManipulationDto>
    {
        public DofGroupDto dof;

        public override void Associate(
            UMI3DController controller,
            AbstractControlEntity control,
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
            if (control is not HasManipulationControlData manipControl)
            {
                throw new Exception($"[UMI3D] Control: control is not a {nameof(HasManipulationControlData)}.");
            }

            base.Associate(
                controller,
                control,
                environmentId,
                interaction,
                toolId,
                hoveredObjectId
            );

            manipControl.ManipulationControlData.frameOfReference = UMI3DEnvironmentLoader
                .GetNode(
                    environmentId,
                    manipInteraction.frameOfReference
                )
                .gameObject
                .transform;

            uint boneType = control
                .controlData
                .controller
                .BoneType;
            Vector3Dto bonePosition = control
                .controlData
                .controller
                .BoneTransform
                .position
                .Dto();
            Vector4Dto boneRotation = control
                .controlData
                .controller
                .BoneTransform
                .rotation
                .Dto();

            manipControl
                .ManipulationControlData
                .messageSender
                .messageHandler = () =>
            {
                ManipulationRequestDto request = new();
                manipControl.ManipulationControlData.SetRequestTranslationAndRotation(
                    dof.dofs, 
                    request,
                    control
                        .controlData
                        .controller
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
                        manipControl.ManipulationControlData.initialPosition = 
                            control
                                .controlData
                                .controller
                                .ManipulationTransform
                                .position;
                        manipControl.ManipulationControlData.initialRotation =
                           control
                               .controlData
                               .controller
                               .ManipulationTransform
                               .rotation;

                        manipControl.ManipulationControlData.messageSender.networkMessage
                            = CoroutineManager
                                .Instance
                                .AttachCoroutine(
                                    manipControl.ManipulationControlData
                                    .messageSender
                                    .NetworkMessageSender()
                                );
                        break;
                    case UnityEngine.InputSystem.InputActionPhase.Canceled:
                        CoroutineManager
                            .Instance
                            .DetachCoroutine(
                                manipControl
                                .ManipulationControlData
                                .messageSender
                                .networkMessage
                            );
                        break;
                    default:
                        break;
                }
            };
        }

        public override AbstractControlEntity GetControl(UMI3DController controller, ManipulationDto interaction)
        {
            var model = controller.controlManager.model;
            var physicalManipulation = model.GetPhysicalManipulation(dof);
            if (physicalManipulation != null)
            {
                return physicalManipulation;
            }

            return model.GetUIManipulation(dof);
        }

        /// <summary>
        /// Find the best <see cref="DofGroupOptionDto"/>> for this controller.
        /// </summary>
        /// <param name="options">Options to search in</param>
        /// <returns></returns>
        public abstract DofGroupOptionDto FindBest(DofGroupOptionDto[] options);
    }
}