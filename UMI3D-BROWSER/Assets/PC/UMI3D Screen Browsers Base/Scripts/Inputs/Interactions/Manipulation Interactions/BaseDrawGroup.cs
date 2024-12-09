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
using System;
using System.Collections.Generic;
using System.Linq;
using umi3d.cdk;
using umi3d.cdk.interaction;
using umi3d.common;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.baseBrowser.inputs.interactions
{
    public abstract class BaseDrawGroup : BaseGroup<DrawingInteractionDto, AbstractUMI3DInput>
    {
        #region Associate

        bool isDrawing = false;
        bool isDrawingActive = false;

        ulong? lineId;
        List<UMI3DNodeInstance> meshes = new();

        EventInteraction toggleInteraction;
        EventInteraction drawInteraction;

        public Func<EventInteraction> InstantiateToggle;
        public Func<EventInteraction> InstantiateInteraction;

        List<Vector3> positions = new();

        [SerializeField] private float lastUpdateTime = 0f;
        [SerializeField] private float timeSynchronization = 0.3f;
        [SerializeField] private float minDistance = 0.01f;

        public override void Associate(ulong environmentId, AbstractInteractionDto interaction, ulong toolId, ulong hoveredObjectId)
        {
            if (interaction is not DrawingInteractionDto drawing) 
                throw new System.Exception($"This input is not compatible with {interaction}");

            if (!IsAvailableFor(drawing))
                throw new System.Exception($"This input is not available for {drawing}");

            if (!IsCompatibleWith(drawing)) 
                throw new System.Exception("Trying to associate an incompatible interaction !");

            isDrawing = false;
            isDrawingActive = false;

            AddGroup();

            this.hoveredObjectId = hoveredObjectId;
            this.toolId = toolId;
            associatedInteraction = drawing;
            this.environmentId = environmentId;

            EventDto toggleEvent = new() {
                name = ActivateToggleName
            };
            EventDto drawEvent = new()
            {
                name = this.associatedInteraction.name,
                hold = true
            };

            drawInteraction = InstantiateInteraction();

            drawInteraction.Menu = Menu;
            drawInteraction.bone = bone;
            drawInteraction.Associate(environmentId, drawEvent, toolId, hoveredObjectId);
            AddInput(drawInteraction);

            drawInteraction.PressedUpOverrider = DrawUp;
            drawInteraction.PressedDownOverrider = DrawDown;

            drawInteraction.HideMenuItem();

            toggleInteraction = InstantiateToggle();

            toggleInteraction.Menu = Menu;
            toggleInteraction.bone = bone;
            toggleInteraction.Associate(environmentId, toggleEvent, toolId, hoveredObjectId);
            AddInput(toggleInteraction);

            toggleInteraction.PressedUpOverrider = ToggleUp;
            toggleInteraction.PressedDownOverrider = ToggleDown;

        }

        string ActivateToggleName => $"Activate {this.associatedInteraction.name}";
        string DeactivateToggleName => $"Deactivate {this.associatedInteraction.name}";

        private void ToggleDown()
        {
            if (associatedInteraction == null) return;

            if (associatedInteraction is not DrawingInteractionDto drawing)
                return;

            if (!DrawingManager.IsAvailableFor(drawing))
                return;

            isDrawingActive = DrawingManager.SwitchDrawing(this.environmentId,drawing);

            toggleInteraction.HideMenuItem();

            toggleInteraction.associatedInteraction.name = isDrawingActive ? DeactivateToggleName : ActivateToggleName;
            toggleInteraction.MenuItem.Name = toggleInteraction.associatedInteraction.name;

            toggleInteraction.ShowMenuItem();

            var eventDto = new common.interaction.EventStateChangedDto
            {
                active = isDrawingActive,
                boneType = bone,
                id = associatedInteraction.id,
                toolId = this.toolId,
                hoveredObjectId = hoveredObjectId,
                bonePosition = (Vector3Dto)boneTransform.position.Dto(),
                boneRotation = (Vector4Dto)boneTransform.rotation.Dto()
            };
            cdk.UMI3DClientServer.SendRequest(eventDto, true);

            if (isDrawingActive)
            {
                if (associatedInteraction.TriggerAnimationId != 0)
                    StartAnim(environmentId, associatedInteraction.TriggerAnimationId);

                drawInteraction.ShowMenuItem();
            }
            else
            {
                if (associatedInteraction.ReleaseAnimationId != 0)
                    StartAnim(environmentId, associatedInteraction.ReleaseAnimationId);

                drawInteraction.HideMenuItem();
            }
        }

        private void ToggleUp()
        {
            
        }

        private async void DrawDown()
        {
            if (associatedInteraction is not DrawingInteractionDto drawing)
                return;

            positions.Clear();
            lastUpdateTime = Time.time;

            DrawingManager.Instance.StartDrawing(drawing);
            isDrawing = true;
            meshes.Clear();
            lineId = null;

            if (drawing.LineId != 0)
            {
                var lineEntity = await UMI3DEnvironmentLoader.Instance.WaitUntilEntityLoaded(this.environmentId, drawing.LineId, new());
                if ((lineEntity?.dto as GlTFNodeDto)?.extensions?.umi3d is UMI3DLineDto lineDto && lineEntity is UMI3DNodeInstance node)
                {
                    var template = UMI3DLineRendererLoader.GetOrCreateLine(node.GameObject, lineDto.clientLineId);
                    var c = UMI3DLineRendererLoader.CopyLine(this.gameObject, template);
                    lineId = c.Item2;
                    c.Item1.positionCount = 0;
                }
            }


            if (drawing.MeshIds != null)
                foreach (var meshId in drawing.MeshIds)
                {
                    var meshEntity = await UMI3DEnvironmentLoader.Instance.WaitUntilEntityLoaded(this.environmentId, meshId, new());
                    if (meshEntity is UMI3DNodeInstance node)
                        meshes.Add(node);
                }

        }

        private void DrawUp()
        {
            if (associatedInteraction is not DrawingInteractionDto drawing)
                return;
            isDrawing = false;

            var drawingDto = new common.interaction.DrawingDto
            {
                drawingEnd = true,
                clientLineId = lineId.HasValue ? lineId.Value : 0,
                positions = positions.Select(p => p.Dto()).ToList(),

                boneType = bone,
                id = associatedInteraction.id,
                toolId = this.toolId,
                hoveredObjectId = hoveredObjectId,
                bonePosition = (Vector3Dto)boneTransform.position.Dto(),
                boneRotation = (Vector4Dto)boneTransform.rotation.Dto()
            };
            cdk.UMI3DClientServer.SendRequest(drawingDto, true);

            DrawingManager.Instance.StopDrawing(drawing);
        }

        protected void Update()
        {
            if (!isDrawingActive || !isDrawing)
                return;

            if (associatedInteraction is not DrawingInteractionDto drawing)
                return;

            Vector3? positionTMP = DrawingManager.Instance.GetDrawingWorldPoint(drawing, meshes);
            if (!positionTMP.HasValue)
                return;
            Vector3 position = positionTMP.Value;

            if (positions.Count > 0 && Vector3.Distance(position, positions.Last()) < this.minDistance)
            {
                return;
            }

            positions.Add(position);

            if(lineId.HasValue)
            {
                var line = UMI3DLineRendererLoader.GetLine(lineId.Value);
                line.positionCount = positions.Count;
                line.useWorldSpace = true;
                line.SetPositions(positions.ToArray());
            }

            if (Time.time >= timeSynchronization + lastUpdateTime)
            {
                //todo add delay
                var drawingDto = new common.interaction.DrawingDto
                {
                    drawingEnd = false,
                    clientLineId = lineId.HasValue ? lineId.Value : 0,
                    positions = positions.Select(p => p.Dto()).ToList(),

                    boneType = bone,
                    id = associatedInteraction.id,
                    toolId = this.toolId,
                    hoveredObjectId = hoveredObjectId,
                    bonePosition = (Vector3Dto)boneTransform.position.Dto(),
                    boneRotation = (Vector4Dto)boneTransform.rotation.Dto()
                };
                cdk.UMI3DClientServer.SendRequest(drawingDto, true);
                lastUpdateTime = Time.time;
            }
        }

        protected async void StartAnim(ulong environmentId, ulong id)
        {
            var anim = UMI3DAbstractAnimation.Get(environmentId, id);
            if (anim != null)
            {
                await anim.SetUMI3DProperty(
                    new SetUMI3DPropertyData(
                         environmentId,
                         new SetEntityPropertyDto()
                         {
                             entityId = id,
                             property = UMI3DPropertyKeys.AnimationPlaying,
                             value = true
                         },
                        UMI3DEnvironmentLoader.GetEntity(environmentId, id))
                    );
                anim.Start();
            }
        }

        public override void Associate(ulong environmentId, ManipulationDto manipulation, DofGroupEnum dofs, ulong toolId, ulong hoveredObjectId)
        => throw new System.NotImplementedException();


        protected void AddGroup()
        {
            if (s_instances.Contains(this)) return;

            s_instances.Add(this);
            s_elementByGroup.Add(this, new List<AbstractUMI3DInput>());
        }

        protected void AddInput(AbstractUMI3DInput input)
        {
            if (s_elementByGroup[this].Contains(input)) return;

            s_elementByGroup[this].Add(input);
        }

        #endregion

        #region Dissociate

        public override void Dissociate()
        {
            var manipulations = s_elementByGroup[this];
            for (int i = manipulations.Count - 1; i >= 0; i--)
            {
                var input = manipulations[i];
                input.Dissociate();
                RemoveManipulation(input);
                //Destroy(input);
            }

            if (associatedInteraction is DrawingInteractionDto drawing)
                DrawingManager.SwitchDrawing(this.environmentId, drawing, true);

            RemoveGroup();
            associatedInteraction = null;
            toggleInteraction = null;
            drawInteraction = null;
            isDrawing = false;
            isDrawingActive = false;
        }

        protected void RemoveGroup()
        {
            if (!s_instances.Contains(this)) return;

            s_instances.Remove(this);
            s_elementByGroup.Remove(this);
        }

        protected void RemoveManipulation(AbstractUMI3DInput input)
        {
            if (!s_elementByGroup[this].Contains(input)) return;

            s_elementByGroup[this].Remove(input);
        }

        #endregion

        public bool IsAvailableFor(DrawingInteractionDto drawInteraction)
            => drawInteraction == associatedInteraction || IsAvailable();

        public override bool IsCompatibleWith(AbstractInteractionDto interaction)
            => interaction is DrawingInteractionDto;

        public override void UpdateHoveredObjectId(ulong hoveredObjectId)
        {
            base.UpdateHoveredObjectId(hoveredObjectId);
            foreach (var input in s_elementByGroup[this]) input.UpdateHoveredObjectId(hoveredObjectId);
        }
    }
}
