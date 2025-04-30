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

using System;
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

            try
            {
                AssociateInteractionToInput(tool);

                Project(tool);
            }
            catch (Exception)
            {
                inputsByInteractions.Clear();
                associations.Clear();
                throw;
            }

            SelectorManager.@default.lastSelectorUsed = this;
            SelectorManager.@default.lastSelectorSelected = this;
        }

        void SwitchThroughInteractions(AbstractInteractionDto interactionDto, out bool haveInputsBeenFound, out ReadOnlyCollection<Input> inputs, out Action<Projection> projectionSetup)
        {
            switch (interactionDto)
            {
                case DrawingInteractionDto drawing:
                    haveInputsBeenFound = dataDelegate.TryGetInputsFor(out inputs, drawing);
                    projectionSetup = projection =>
                    {

                    };
                    break;

                case EventDto eventDto:
                    haveInputsBeenFound = dataDelegate.TryGetInputsFor(out inputs, eventDto);
                    projectionSetup = projection =>
                    {
                        projection.input.onStarted += obj =>
                        {
                            if (eventDto.hold) { projection.SendEventStateChanged(true); }
                            else { projection.SendEventTriggered(); }
                            projection.Animate(eventDto.triggerAnimationId);
                        };
                        projection.input.onCanceled += obj =>
                        {
                            if (eventDto.hold) { projection.SendEventStateChanged(false); }
                            projection.Animate(eventDto.releaseAnimationId);
                        };
                    };
                    break;

                // Parameters
                case BooleanParameterDto boolean:
                    haveInputsBeenFound = dataDelegate.TryGetInputsFor(out inputs, boolean);
                    projectionSetup = projection =>
                    {
                        projection.input.onPerformed += obj =>
                        {
                            projection.SendParameterSetting();
                        };
                    };
                    break;

                case FloatParameterDto @float:
                    haveInputsBeenFound = dataDelegate.TryGetInputsFor(out inputs, @float);
                    projectionSetup = projection =>
                    {
                        projection.input.onPerformed += obj =>
                        {
                            projection.SendParameterSetting();
                        };
                    };
                    break;

                case IntegerParameterDto integer:
                    haveInputsBeenFound = dataDelegate.TryGetInputsFor(out inputs, integer);
                    projectionSetup = projection =>
                    {
                        projection.input.onPerformed += obj =>
                        {
                            projection.SendParameterSetting();
                        };
                    };
                    break;

                case Vector2ParameterDto vector2:
                    haveInputsBeenFound = dataDelegate.TryGetInputsFor(out inputs, vector2);
                    projectionSetup = projection =>
                    {
                        projection.input.onPerformed += obj =>
                        {
                            projection.SendParameterSetting();
                        };
                    };
                    break;

                case Vector3ParameterDto vector3:
                    haveInputsBeenFound = dataDelegate.TryGetInputsFor(out inputs, vector3);
                    projectionSetup = projection =>
                    {
                        projection.input.onPerformed += obj =>
                        {
                            projection.SendParameterSetting();
                        };
                    };
                    break;

                case Vector4ParameterDto vector4:
                    haveInputsBeenFound = dataDelegate.TryGetInputsFor(out inputs, vector4);
                    projectionSetup = projection =>
                    {
                        projection.input.onPerformed += obj =>
                        {
                            projection.SendParameterSetting();
                        };
                    };
                    break;

                case FloatRangeParameterDto floatRange:
                    haveInputsBeenFound = dataDelegate.TryGetInputsFor(out inputs, floatRange);
                    projectionSetup = projection =>
                    {
                        projection.input.onPerformed += obj =>
                        {
                            projection.SendParameterSetting();
                        };
                    };
                    break;

                case IntegerRangeParameterDto integerRange:
                    haveInputsBeenFound = dataDelegate.TryGetInputsFor(out inputs, integerRange);
                    projectionSetup = projection =>
                    {
                        projection.input.onPerformed += obj =>
                        {
                            projection.SendParameterSetting();
                        };
                    };
                    break;

                case StringParameterDto @string:
                    haveInputsBeenFound = dataDelegate.TryGetInputsFor(out inputs, @string);
                    projectionSetup = projection =>
                    {
                        projection.input.onPerformed += obj =>
                        {
                            projection.SendParameterSetting();
                        };
                    };
                    break;

                case ColorParameterDto color:
                    haveInputsBeenFound = dataDelegate.TryGetInputsFor(out inputs, color);
                    projectionSetup = projection =>
                    {
                        projection.input.onPerformed += obj =>
                        {
                            projection.SendParameterSetting();
                        };
                    };
                    break;

                case EnumParameterDto<string> @enum:
                    haveInputsBeenFound = dataDelegate.TryGetInputsFor(out inputs, @enum);
                    projectionSetup = projection =>
                    {
                        projection.input.onPerformed += obj =>
                        {
                            projection.SendParameterSetting();
                        };
                    };
                    break;

                case UploadFileParameterDto uploadFile:
                    haveInputsBeenFound = dataDelegate.TryGetInputsFor(out inputs, uploadFile);
                    projectionSetup = projection =>
                    {
                        projection.input.onPerformed += obj =>
                        {
                            projection.SendUploadFile("TODO");
                        };
                    };
                    break;

                case LocalInfoRequestParameterDto localInfo:
                    haveInputsBeenFound = dataDelegate.TryGetInputsFor(out inputs, localInfo);
                    projectionSetup = projection =>
                    {
                        projection.input.onPerformed += obj =>
                        {
                            projection.SendParameterSetting();
                        };
                    };
                    break;

                default:
                    UnityEngine.Debug.LogError($"[Selector] Error: Unhandled case: {interactionDto.GetType()}");
                    haveInputsBeenFound = false;
                    inputs = null;
                    projectionSetup = null;
                    break;
            }
        }

        List<(Interaction interaction, Input input)> associations = new();
        Dictionary<Interaction, ReadOnlyCollection<Input>> inputsByInteractions = new();
        void AssociateInteractionToInput(Tool tool)
        {
            foreach (AbstractInteractionDto interactionDto in tool.interactions)
            {
                InteractionManager.@default.InstantiateOrGet(
                    out Interaction interaction, 
                    tool.environmentId, 
                    interactionDto
                );
                if (interaction == null)
                {
                    UnityEngine.Debug.LogError($"[Selector-{id}] Error: cannot get interaction from environment id: {tool.environmentId} and dto: {interactionDto?.GetType()}, {interactionDto?.name}.");
                    continue;
                }

                ReadOnlyCollection<Input> inputs = null;
                Action<Projection> projectionSetup = null;
                try
                {
                    SwitchThroughInteractions(interactionDto, out bool haveInputsBeenFound, out inputs, out projectionSetup);

                    if (!haveInputsBeenFound)
                    {
                        UnityEngine.Debug.LogWarning($"[Selector-{id}] Warning: no controller or input available for {interactionDto.GetType()}, {interactionDto.name}.");
                        continue;
                    }
                }
                catch (Exception)
                {
                    continue;
                }

                inputsByInteractions.Add(interaction, inputs);
                interaction.projectionSetup = projectionSetup;
            }

            dataDelegate.AssociateInteractionAndInput(
                associations, 
                new ReadOnlyDictionary<Interaction, ReadOnlyCollection<Input>>(inputsByInteractions)
            );
            inputsByInteractions.Clear();

            if (tool.interactions.Count > associations.Count)
            {
                UnityEngine.Debug.LogWarning($"[Selector-{id}] Warning: Not all interactions have been associated to an input.");
            }
        }

        void Project(Tool tool)
        {
            foreach (var association in associations)
            {
                Projection projection = ProjectionManager.@default.Project(
                    this, 
                    association.input.controller, 
                    tool, 
                    association.interaction, 
                    association.input
                );
                association.interaction.projectionSetup(projection);
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

        #region Hovering

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
                tool.environmentId, 
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

        #endregion
    }
}