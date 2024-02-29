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
using umi3d.common;
using umi3d.common.interaction;
using UnityEngine;
using UnityEngine.InputSystem;

namespace umi3d.cdk.interaction
{
    public abstract class AbstractEventInputDelegate : AbstractInputDelegate<EventDto>
    {
        public bool tryToFindInputForHoldableEvent;

        public override void Associate(
            AbstractControlData control,
            ulong environmentId,
            AbstractInteractionDto interaction,
            ulong toolId,
            ulong hoveredObjectId
        )
        {
            if (interaction is not EventDto evtInteraction)
            {
                throw new Exception($"[UMI3D] Control: Interaction is not an {nameof(EventDto)}.");
            }
            if (control is not AbstractButtonControlData buttonControl)
            {
                throw new Exception($"[UMI3D] Control: control is not an {nameof(AbstractButtonControlData)}.");
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

            buttonControl.actionPerformed = phase =>
            {
                buttonControl.phase = phase;
                InteractionRequestDto request = null;
                switch (phase)
                {
                    case InputActionPhase.Started:
                        if (evtInteraction.hold)
                        {
                            request = new EventStateChangedDto()
                            {
                                active = true
                            };
                        }
                        else
                        {
                            request = new EventTriggeredDto();
                        }
                        request.boneType = boneType;
                        request.toolId = toolId;
                        request.id = interaction.id;
                        request.hoveredObjectId = hoveredObjectId;
                        request.bonePosition = bonePosition;
                        request.boneRotation = boneRotation;

                        UMI3DClientServer.SendRequest(request, true);
                        if (evtInteraction.TriggerAnimationId != 0)
                        {
                            //TODO
                            StartAnim(evtInteraction.TriggerAnimationId);
                        }
                        break;
                    case InputActionPhase.Canceled:
                        if (evtInteraction.hold)
                        {
                            request = new EventStateChangedDto()
                            {
                                active = false,
                                boneType = boneType,
                                id = interaction.id,
                                toolId = toolId,
                                hoveredObjectId = hoveredObjectId,
                                bonePosition = bonePosition,
                                boneRotation = boneRotation,
                            };

                            UMI3DClientServer.SendRequest(request, true);
                            if (evtInteraction.ReleaseAnimationId != 0)
                            {
                                //TODO
                                StartAnim(evtInteraction.ReleaseAnimationId);
                            }
                        }
                        break;
                    default:
                        break;
                }
            };
            buttonControl.canPerform = CanPerform;
            buttonControl.dissociate = () =>
            {
                Dissociate(control);
            };
        }

        protected abstract bool CanPerform(InputActionPhase phase);

        public override void Dissociate(AbstractControlData control)
        {
            base.Dissociate(control);
            control.Disable();
        }

        public override Guid? GetControlId(EventDto interaction, bool unused = true)
        {
            throw new System.NotImplementedException();
            //model.
        }

        async void StartAnim(ulong id)
        {
            UMI3DEntityInstance entity = UMI3DEnvironmentLoader
                .Instance
                .TryGetEntityInstance(
                UMI3DGlobalID.EnvironmentId,
                id
            );

            if (entity == null)
            {
                return;
            }

            UMI3DAbstractAnimation animation = entity.Object as UMI3DAbstractAnimation;
            if (animation == null)
            {
                return;
            }

            await animation.SetUMI3DProperty(
            new SetUMI3DPropertyData(
                UMI3DGlobalID.EnvironmentId,
                new SetEntityPropertyDto()
                {
                    entityId = id,
                    property = UMI3DPropertyKeys.AnimationPlaying,
                    value = true
                },
                entity)
            );
            animation.Start();
        }
    }
}