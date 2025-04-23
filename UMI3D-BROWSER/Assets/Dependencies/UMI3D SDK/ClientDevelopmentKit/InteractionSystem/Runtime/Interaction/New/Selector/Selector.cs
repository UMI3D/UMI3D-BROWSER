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
using System.Linq;
using umi3d.common;
using umi3d.common.interaction;
using UnityEngine;
using UnityEngine.Windows;

namespace umi3d.cdk.interaction
{
    public sealed class Selector 
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

        ISelectorDataDelegate _dataDelegate;
        public ISelectorDataDelegate dataDelegate
        {
            get => _dataDelegate;
            set
            {
                if (value == null)
                {
                    UnityEngine.Debug.LogError($"[Selector] Error: you are trying to set a null delegate.");
                    return;
                }
                _dataDelegate = value;
            }
        }

        IClientServerCommunicationSelectorDelegate _clientServerCommunicationSelectorDelegate = new ClientServerCommunicationSelectorDelegate();
        public IClientServerCommunicationSelectorDelegate clientServerCommunicationSelectorDelegate
        {
            get => _clientServerCommunicationSelectorDelegate;
            set
            {
                if (value == null)
                {
                    _clientServerCommunicationSelectorDelegate = new ClientServerCommunicationSelectorDelegate();
                    return;
                }
                _clientServerCommunicationSelectorDelegate = value;
            }
        }

        List<Controller> _controllers = new();
        public ReadOnlyCollection<Controller> controllers => _controllers.AsReadOnly();
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

        List<Tool> _projectedTools = new();
        public ReadOnlyCollection<Tool> projectedTools => _projectedTools.AsReadOnly();

        Dictionary<AbstractInteractionDto, ReadOnlyCollection<Input>> inputsByInteractions = new();
        List<(AbstractInteractionDto interaction, Input input)> associations = new();
        /// <summary>
        /// Try to project a tool on controllers.<br/>
        /// <br/>
        /// <list type="number">
        /// <item>Sort interactions.</item>
        /// <item>Loop through each interactions.</item>
        /// <item>For each interactions find the first available <see cref="Controller"/>, and <see cref="Input"/>.</item>
        /// <item>Project each interactions on its corresponding input and controller.</item>
        /// <item>Project each tool on its corresponding controllers.</item>
        /// </list>
        /// </summary>
        /// <param name="tool"></param>
        public void Select(Tool tool)
        {
            if (_projectedTools.Count >= dataDelegate.toolCountLimitation) 
            {
                UnityEngine.Debug.Log($"[Selector-{id}] Log: Try to select tool: '{tool?.dto?.name ?? "Tool with no name"}' but count limitation prevents it.");
                return; 
            }

            ReadOnlyCollection<Input> inputs;
            foreach (AbstractInteractionDto interaction in tool.interactions)
            {
                if (interaction is DrawingInteractionDto drawingInteractionDto)
                {
                } 
                else if (interaction is EventDto eventDto)
                {
                    bool found = dataDelegate.TryGetInputsForEventDto(
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

            dataDelegate.AssociateInteractionAndInput(
                associations, 
                new ReadOnlyDictionary<AbstractInteractionDto, ReadOnlyCollection<Input>>(inputsByInteractions)
            );
            inputsByInteractions.Clear();

            if (tool.interactions.Count > associations.Count)
            {
                UnityEngine.Debug.LogWarning($"[Selector-{id}] Warning: Not all interactions have been associated to an input.");
            }

            foreach (var association in associations)
            {
                InteractionManager.@default.TryToFetchInteraction(
                    out Interaction interaction, 
                    tool.environmentId, 
                    association.interaction.id
                );
                ProjectionManager.@default.Project(
                    this, 
                    association.input.controller, 
                    tool, 
                    interaction, 
                    association.input
                );
            }
            associations.Clear();

            _projectedTools.Add(tool);
        }

        List<Projection> _projectionToRelease = new();
        public void Deselect(Tool tool)
        {
            _projectedTools.Remove(tool);
            _projectionToRelease.AddRange(ProjectionManager.@default.projections.Where(projection => projection.tool == tool));
            foreach (Projection projection in _projectionToRelease)
            {
                ProjectionManager.@default.Release(projection);
            }
            _projectionToRelease.Clear();
        }

        public void Switch(Tool toolToRelease, Tool toolToProject)
        {

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
            clientServerCommunicationSelectorDelegate.SendRequest(hoverDto, true);

            if (!tool.TryCast(out InteractableDto interactableDto)) { return; }
            clientServerCommunicationSelectorDelegate.Animate(
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
            clientServerCommunicationSelectorDelegate.SendRequest(hoverDto, false);
        }
    }
}