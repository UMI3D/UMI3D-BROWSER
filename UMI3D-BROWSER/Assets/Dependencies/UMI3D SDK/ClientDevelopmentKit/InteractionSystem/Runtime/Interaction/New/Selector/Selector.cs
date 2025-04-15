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

using inetum.unityUtils.observation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using umi3d.common;
using umi3d.common.interaction;
using UnityEngine;
using UnityEngine.Windows;

namespace umi3d.cdk.interaction
{
    public class Selector 
    {
        internal Selector(string id) 
        {
            this.id = id;
        }

        /// <summary>
        /// The unique identifier of this selector.<br/>
        /// <br/>
        /// You can set the name of the selector here but it has to be unique.
        /// </summary>
        public readonly string id;

        ISelectorDataDelegate _delegate;
        public ISelectorDataDelegate @delegate
        {
            get => _delegate;
            set
            {
                if (value == null)
                {
                    UnityEngine.Debug.LogError($"[Selector] Error: you are trying to set a null delegate.");
                    return;
                }
                _delegate = value;
            }
        }

        List<Controller> _controllers = new List<Controller>();
        ReadOnlyCollection<Controller> controllers => _controllers.AsReadOnly();
        public void Add(Controller controller)
        {
            if (controller  == null || _controllers.Contains(controller))
            {
                return;
            }

            _controllers.Add(controller);
        }
        public void Remove(Controller controller)
        {
            _controllers.Remove(controller);
        }

        Dictionary<AbstractInteractionDto, ReadOnlyCollection<Input>> inputsByInteractions = new();
        List<(AbstractInteractionDto interaction, Input input)> associations = new();
        /// <summary>
        /// Try to project a tool on controllers.<br/>
        /// <br/>
        /// <list type="number">
        /// <item>Sort interactions.</item>
        /// <item>Loop through each interactions.</item>
        /// <item>For each interaction find the first available <see cref="Controller"/>, and <see cref="Input"/>.</item>
        /// <item>Project each interaction on its corresponding input and controller.</item>
        /// <item>Project each tool on its corresponding controllers.</item>
        /// </list>
        /// </summary>
        /// <param name="tool"></param>
        public void Select(Tool tool)
        {
            ReadOnlyCollection<Input> inputs;
            foreach (AbstractInteractionDto interaction in tool.interactions)
            {
                if (interaction is DrawingInteractionDto drawingInteractionDto)
                {
                } 
                else if (interaction is EventDto eventDto)
                {
                    bool found = @delegate.TryGetInputsForEventDto(
                        out inputs,
                        this, 
                        controllers
                    );

                    if (!found)
                    {
                        UnityEngine.Debug.LogWarning($"[Selector-{id}] Warning: no controller or input available for {eventDto.GetType()}, {eventDto.name}.");
                        continue;
                    }

                    inputsByInteractions.Add(interaction, inputs);
                }
                else if (interaction is BooleanParameterDto booleanParameterDto)
                {

                }

                else if (interaction is StringParameterDto stringParameterDto)
                {

                }

                else if (interaction is FloatParameterDto floatParameterDto)
                {

                }
                else if (interaction is IntegerParameterDto integerParameterDto)
                {

                }

                else if (interaction is ColorParameterDto colorParameterDto)
                {

                }

                else if (interaction is Vector2ParameterDto vector2ParameterDto)
                {

                }
                else if (interaction is Vector3ParameterDto vector3ParameterDto)
                {

                }
                else if (interaction is Vector4ParameterDto vector4ParameterDto)
                {

                }

                else if (interaction is EnumParameterDto<string> enumParameterDto)
                {

                }
                else if (interaction is FloatRangeParameterDto floatRangeParameterDto)
                {

                }
                else if (interaction is IntegerRangeParameterDto integerRangeParameterDto)
                {

                }

                else if (interaction is UploadFileParameterDto uploadFileParameterDto)
                {

                }
                else if (interaction is LocalInfoRequestParameterDto localInfoRequestParameterDto)
                {

                }
            }


            @delegate.TryToAssociateInteractionAndInput(
                associations, 
                new ReadOnlyDictionary<AbstractInteractionDto, ReadOnlyCollection<Input>>(inputsByInteractions)
            );
            inputsByInteractions.Clear();

            foreach (var association in associations)
            {
                // Project this interaction from this tool on this input from this controller.
                association.input.controller.TryToProject(
                    tool, 
                    association.interaction, 
                    association.input, 
                    this
                );
            }
            

            SelectorManager.@default.delegates.ForEach(@delegate =>
            {
                @delegate.ToolSelected(tool, this);
                return Flow.Continue;
            });
        }

        public void Deselect(Tool tool)
        {
            throw new System.NotImplementedException();
        }

        public void HoverEnter(Tool tool, Collider collider, uint boneId, Transform boneTransform, Vector3 position, Vector3 normal, Vector3 direction)
        {
            HoverStateChanged(true, tool, collider, boneId, boneTransform, position, normal, direction);
        }

        public void HoverExit(Tool tool, Collider collider, uint boneId, Transform boneTransform, Vector3 position, Vector3 normal, Vector3 direction)
        {
            HoverStateChanged(false, tool, collider, boneId, boneTransform, position, normal, direction);
        }

        void HoverStateChanged(bool enter, Tool tool, Collider collider, uint boneId, Transform boneTransform, Vector3 position, Vector3 normal, Vector3 direction)
        {
            if (enter)
            {
                tool.OnSelectorHoverEnter(this);
            }
            else
            {
                tool.OnSelectorHoverExit(this);
            }

            ulong hoveredObjectId = UMI3DEnvironmentLoader.GetNodeID(collider);
            HoverStateChangedDto hoverDto = new HoverStateChangedDto()
            {
                toolId = tool.dto.id,
                hoveredObjectId = hoveredObjectId,

                boneType = boneId,
                bonePosition = boneTransform.position.Dto(),
                boneRotation = new Vector4(boneTransform.rotation.x, boneTransform.rotation.y, boneTransform.rotation.z, boneTransform.rotation.w).Dto(),

                normal = normal.Dto(),
                position = position.Dto(),
                direction = direction.Dto(),

                state = enter,
            };
            UMI3DClientServer.SendRequest(hoverDto, true);

            if (!tool.TryCast(out InteractableDto interactableDto)) { return; }
            Animate(
                tool, 
                enter 
                ? interactableDto.HoverEnterAnimationId
                : interactableDto.HoverExitAnimationId
            );
        }

        public void Hover(Tool tool, Collider collider, uint boneId, Transform boneTransform, Vector3 position, Vector3 normal, Vector3 direction)
        {
            ulong hoveredObjectId = UMI3DEnvironmentLoader.GetNodeID(collider);
            HoveredDto hoverDto = new HoveredDto()
            {
                toolId = tool.dto.id,
                hoveredObjectId = hoveredObjectId,

                boneType = boneId,
                bonePosition = boneTransform.position.Dto(),
                boneRotation = new Vector4(boneTransform.rotation.x, boneTransform.rotation.y, boneTransform.rotation.z, boneTransform.rotation.w).Dto(),

                normal = normal.Dto(),
                position = position.Dto(),
                direction = direction.Dto(),
            };
            UMI3DClientServer.SendRequest(hoverDto, false);
        }

        async void Animate(Tool tool, ulong animationId)
        {
            if (animationId == 0) { return; }

            UMI3DEntityInstance entityInstance = UMI3DEnvironmentLoader.Instance.TryGetEntityInstance(tool.environmentId, animationId);
            UMI3DAbstractAnimation animation = entityInstance?.Object as UMI3DAbstractAnimation;

            await animation.SetUMI3DProperty(
                new SetUMI3DPropertyData(
                    tool.environmentId,
                    new SetEntityPropertyDto()
                    {
                        entityId = animationId,
                        property = UMI3DPropertyKeys.AnimationPlaying,
                        value = true
                    },
                    entityInstance
                )
            );

            if (animation != null) animation.Start();
        }
    }
}