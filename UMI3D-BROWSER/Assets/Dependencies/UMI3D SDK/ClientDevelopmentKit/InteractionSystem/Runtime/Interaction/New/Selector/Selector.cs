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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.cdk.interaction
{
    public sealed class Selector : ISelector
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

        IBoneRepresentable _boneRepresentable;
        public IBoneRepresentable boneRepresentable
        {
            get => _boneRepresentable;
            set
            {
                if (value == null)
                {
                    UnityEngine.Debug.LogError($"[Selector] Error: you are trying to set a null delegate.");
                    return;
                }
                _boneRepresentable = value;
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

        public bool canProjectMoreTool => _projectedTools.Count < _dataDelegate.toolCountLimitation;

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
            bool TryToAddInputToDictionary(AbstractInteractionDto interaction, bool found)
            {
                if (!found)
                {
                    UnityEngine.Debug.LogWarning($"[Selector-{id}] Warning: no controller or input available for {interaction.GetType()}, {interaction.name}.");
                    return false;
                }

                inputsByInteractions.Add(interaction, inputs);
                return true;
            }

            foreach (AbstractInteractionDto interaction in tool.interactions)
            {
                if (interaction is DrawingInteractionDto)
                {
                    bool found = dataDelegate.TryGetInputsForDrawingInteractionDto(out inputs);

                    if (!TryToAddInputToDictionary(interaction, found)) { continue; }
                } 
                else if (interaction is EventDto)
                {
                    bool found = dataDelegate.TryGetInputsForEventDto(out inputs);

                    if (!TryToAddInputToDictionary(interaction, found)) { continue; }
                }

                // Parameters
                else if (interaction is BooleanParameterDto)
                {
                    bool found = dataDelegate.TryGetInputsForBooleanParameterDto(out inputs);

                    if (!TryToAddInputToDictionary(interaction, found)) { continue; }
                }

                else if (interaction is FloatParameterDto)
                {
                    bool found = dataDelegate.TryGetInputsForFloatParameterDto(out inputs);

                    if (!TryToAddInputToDictionary(interaction, found)) { continue; }
                }
                else if (interaction is IntegerParameterDto)
                {
                    bool found = dataDelegate.TryGetInputsForIntegerParameterDto(out inputs);

                    if (!TryToAddInputToDictionary(interaction, found)) { continue; }
                }

                else if (interaction is Vector2ParameterDto)
                {
                    bool found = dataDelegate.TryGetInputsForVector2ParameterDto(out inputs);

                    if (!TryToAddInputToDictionary(interaction, found)) { continue; }
                }
                else if (interaction is Vector3ParameterDto)
                {
                    bool found = dataDelegate.TryGetInputsForVector3ParameterDto(out inputs);

                    if (!TryToAddInputToDictionary(interaction, found)) { continue; }
                }
                else if (interaction is Vector4ParameterDto)
                {
                    bool found = dataDelegate.TryGetInputsForVector4ParameterDto(out inputs);

                    if (!TryToAddInputToDictionary(interaction, found)) { continue; }
                }

                else if (interaction is FloatRangeParameterDto)
                {
                    bool found = dataDelegate.TryGetInputsForFloatRangeParameterDto(out inputs);

                    if (!TryToAddInputToDictionary(interaction, found)) { continue; }
                }
                else if (interaction is IntegerRangeParameterDto)
                {
                    bool found = dataDelegate.TryGetInputsForIntegerRangeParameterDto(out inputs);

                    if (!TryToAddInputToDictionary(interaction, found)) { continue; }
                }

                else if (interaction is StringParameterDto)
                {
                    bool found = dataDelegate.TryGetInputsForStringParameterDto(out inputs);

                    if (!TryToAddInputToDictionary(interaction, found)) { continue; }
                }
                else if (interaction is ColorParameterDto)
                {
                    bool found = dataDelegate.TryGetInputsForColorParameterDto(out inputs);

                    if (!TryToAddInputToDictionary(interaction, found)) { continue; }
                }
                else if (interaction is EnumParameterDto<string>)
                {
                    bool found = dataDelegate.TryGetInputsForEnumParameterDto(out inputs);

                    if (!TryToAddInputToDictionary(interaction, found)) { continue; }
                }
                else if (interaction is UploadFileParameterDto)
                {
                    bool found = dataDelegate.TryGetInputsForUploadFileParameterDto(out inputs);

                    if (!TryToAddInputToDictionary(interaction, found)) { continue; }
                }
                else if (interaction is LocalInfoRequestParameterDto)
                {
                    bool found = dataDelegate.TryGetInputsForLocalInfoRequestParameterDto(out inputs);

                    if (!TryToAddInputToDictionary(interaction, found)) { continue; }
                }
                else
                {
                    UnityEngine.Debug.Log($"[Selector] Error: Unhandled case: {interaction.GetType()}");
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
            tool.selector = this;
            
            var request = new ToolProjectedDto
            {
                environmentId = tool.environmentId,
                toolId = tool.dto.id,

                boneType = boneRepresentable.bone
            };
            clientServerCommunicationSelectorDelegate.SendRequest(request, true);

            SelectorManager.@default.lastSelectorUsed = this;
            SelectorManager.@default.lastSelectorSelected = this;
        }

        List<Projection> _projectionToRelease = new();
        public void Deselect(Tool tool)
        {
            _projectedTools.Remove(tool);
            tool.selector = null;

            var request = new ToolReleasedDto
            {
                environmentId = tool.environmentId,
                toolId = tool.dto.id,

                boneType = boneRepresentable.bone
            };
            clientServerCommunicationSelectorDelegate.SendRequest(request, true);

            _projectionToRelease.AddRange(ProjectionManager.@default.projections.Where(projection => projection.tool == tool));
            foreach (Projection projection in _projectionToRelease)
            {
                ProjectionManager.@default.Release(projection);
            }
            _projectionToRelease.Clear();

            SelectorManager.@default.lastSelectorUsed = this;
            SelectorManager.@default.lastSelectorDeselected = this;
        }

        public void Switch(Tool toolToRelease, Tool toolToProject)
        {

        }

        public ulong hoveredObjectId { get; internal set; }
        public void HoverEnter(Tool tool, Collider collider, Vector3 position, Vector3 normal, Vector3 direction)
        {
            HoverStateChanged(true, tool, collider, position, normal, direction);
        }

        public void HoverExit(Tool tool, Collider collider, Vector3 position, Vector3 normal, Vector3 direction)
        {
            HoverStateChanged(false, tool, collider, position, normal, direction);
        }

        void HoverStateChanged(bool enter, Tool tool, Collider collider, Vector3 position, Vector3 normal, Vector3 direction)
        {
            if (enter)
            {
                tool.OnSelectorHoverEnter(this);
                hoveredObjectId = UMI3DEnvironmentLoader.GetNodeID(collider);
            }
            else
            {
                tool.OnSelectorHoverExit(this);
                hoveredObjectId = 0;
            }


            IBoneRepresentable bone = boneRepresentable;

            HoverStateChangedDto hoverDto = new HoverStateChangedDto()
            {
                toolId = tool.dto.id,
                hoveredObjectId = hoveredObjectId,

                boneType = bone.bone,
                bonePosition = bone.bonePosition.Dto(),
                boneRotation = new Vector4(bone.boneRotation.x, bone.boneRotation.y, bone.boneRotation.z, bone.boneRotation.w).Dto(),

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

        public void Hover(Tool tool, Collider collider, Vector3 position, Vector3 normal, Vector3 direction)
        {
            hoveredObjectId = UMI3DEnvironmentLoader.GetNodeID(collider);
            
            IBoneRepresentable bone = boneRepresentable;
            
            HoveredDto hoverDto = new HoveredDto()
            {
                toolId = tool.dto.id,
                hoveredObjectId = hoveredObjectId,

                boneType = bone.bone,
                bonePosition = bone.bonePosition.Dto(),
                boneRotation = new Vector4(bone.boneRotation.x, bone.boneRotation.y, bone.boneRotation.z, bone.boneRotation.w).Dto(),

                normal = normal.Dto(),
                position = position.Dto(),
                direction = direction.Dto(),
            };
            clientServerCommunicationSelectorDelegate.SendRequest(hoverDto, false);
        }
    }
}