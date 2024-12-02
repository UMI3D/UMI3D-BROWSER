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
using inetum.unityUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using umi3d.baseBrowser.Cursor;
using umi3d.cdk;
using umi3d.cdk.interaction;
using umi3d.common;
using umi3d.common.interaction;
using umi3d.common.interaction.form.ui_toolkit;
using UnityEngine;
using UnityEngine.UIElements;
using static umi3d.common.volume.GeometryTools;

namespace umi3d.baseBrowser.inputs.interactions
{

    public abstract class BaseGroup<InteractionType, E> : BaseInteraction<InteractionType> where InteractionType : common.interaction.AbstractInteractionDto
    {
        protected static List<BaseGroup<InteractionType, E>> s_instances = new();
        protected static Dictionary<BaseGroup<InteractionType, E>, List<E>> s_elementByGroup = new();
        protected static int s_currentIndex;

        /// <summary>
        /// Current manipulation group.
        /// </summary>
        public static BaseGroup<InteractionType, E> CurrentGroup
            => s_instances.Count > 0 ? s_instances[s_currentIndex] : null;

        /// <summary>
        /// Current list of manipulations associated with the <see cref="CurrentGroup"/>
        /// </summary>
        public static List<E> CurrentManipulations
            => CurrentGroup == null
                || s_elementByGroup == null
                || !s_elementByGroup.ContainsKey(CurrentGroup)
            ? null
            : s_elementByGroup[CurrentGroup];

        #region Static methods and properties

        #region Incrementation and decrementation

        /// <summary>
        /// Deactivate current group and activate next one.
        /// </summary>
        public static void NextGroup() => SwicthGroup(s_currentIndex + 1);

        /// <summary>
        /// Deactivate current group and activate previous one.
        /// </summary>
        public static void PreviousGroup() => SwicthGroup(s_currentIndex - 1);

        protected static void SwicthGroup(int i)
        {
            if (s_instances.Count == 0) return;

            if (s_currentIndex < s_instances.Count && s_currentIndex >= 0) (s_instances[s_currentIndex] as BaseManipulationGroup).Deactivate();

            if (s_instances.Count == 0)
            {
                s_currentIndex = -1;
                return;
            }

            if (i < 0) s_currentIndex = s_instances.Count - 1;
            else if (i >= s_instances.Count) s_currentIndex = 0;
            else s_currentIndex = i;

            (s_instances[s_currentIndex] as BaseManipulationGroup).Activate();
        }

        #endregion

        #endregion

        #region Activation, Deactivation, Select

        /// <summary>
        /// Whether or not this group is active.
        /// </summary>
        public bool IsActive { get => m_isActive; protected set => m_isActive = value; }
        [SerializeField]
        [ReadOnly]
        private bool m_isActive;

        protected virtual void Activate()
        {
            IsActive = true;
        }
        protected virtual void Deactivate()
        {
            IsActive = false;
        }

        protected void Select() => SwicthGroup(s_instances.FindIndex(a => a == this));

        #endregion

    }

    /// <summary>
    /// Group of manipulations.
    /// </summary>
    public abstract class BaseManipulationGroup : BaseGroup<ManipulationDto, BaseManipulation>
    {
        /// <summary>
        /// <see cref="BaseManipulation.strength"/>
        /// </summary>
        public float strength;
        /// <summary>
        /// Reference to the <see cref="Cursor.FrameIndicator"/>
        /// </summary>
        public Cursor.FrameIndicator frameIndicator;
        /// <summary>
        /// TODO: not define
        /// </summary>
        public Transform manipulationCursor;

        public static List<DofGroupEnum> DofGroups = new List<DofGroupEnum>
        {
            DofGroupEnum.X,
            DofGroupEnum.Y,
            DofGroupEnum.Z,

            DofGroupEnum.XY,
            DofGroupEnum.XZ,
            DofGroupEnum.YZ,

            DofGroupEnum.RX,
            DofGroupEnum.RY,
            DofGroupEnum.RZ
        };

        protected override void Activate()
        {
            base.Activate();
            BaseManipulation.SelectFirst();
        }
        protected override void Deactivate()
        {
            base.Deactivate();
            foreach (var input in s_elementByGroup[this]) input.Deactivate();
        }

        #region Associate

        public Func<DofGroupEnum, float, FrameIndicator, Transform, BaseManipulation> InstanciateManipulation;

        public override void Associate(ulong environmentId, AbstractInteractionDto interaction, ulong toolId, ulong hoveredObjectId)
            => throw new System.NotImplementedException();

        public override void Associate(ulong environmentId, ManipulationDto manipulation, DofGroupEnum dofs, ulong toolId, ulong hoveredObjectId)
        {
            UnityEngine.Debug.Log("Associate");

            if (!IsAvailableFor(manipulation)) throw new System.Exception($"This input is not available for {manipulation}");

            if (!IsCompatibleWith(manipulation)) throw new System.Exception("Trying to associate an uncompatible interaction !");

            AddGroup();

            this.hoveredObjectId = hoveredObjectId;
            this.toolId = toolId;
            associatedInteraction = manipulation;
            this.environmentId = environmentId;

            BaseManipulation input = InstanciateManipulation(dofs, strength, frameIndicator, manipulationCursor);
            input.Menu = Menu;
            input.bone = bone;
            input.Associate(environmentId, manipulation, dofs, toolId, hoveredObjectId);
            input.Deactivate();
            AddManipulation(input);

            if (s_instances.Count == 1)
            {
                s_currentIndex = 0;
                Activate();
            }
            else Deactivate();
        }

        protected void AddGroup()
        {
            if (s_instances.Contains(this)) return;

            s_instances.Add(this);
            s_elementByGroup.Add(this, new List<BaseManipulation>());
        }

        protected void AddManipulation(BaseManipulation input)
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
                Destroy(input);
            }

            RemoveGroup();
            associatedInteraction = null;
        }

        protected void RemoveGroup()
        {
            if (!s_instances.Contains(this)) return;

            s_instances.Remove(this);
            s_elementByGroup.Remove(this);
        }

        protected void RemoveManipulation(BaseManipulation input)
        {
            if (!s_elementByGroup[this].Contains(input)) return;

            s_elementByGroup[this].Remove(input);
        }

        #endregion

        public bool IsAvailableFor(ManipulationDto manipulation)
            => manipulation == associatedInteraction || IsAvailable();

        public override bool IsCompatibleWith(AbstractInteractionDto interaction)
        {
            if (!(interaction is ManipulationDto manipulationDto)) return false;

            return manipulationDto.dofSeparationOptions.Exists
            (
                (sep) =>
                {
                    foreach (DofGroupDto dof in sep.separations)
                    {
                        if (!DofGroups.Contains(dof.dofs)) return false;
                    }
                    return true;
                }
            );
        }

        public override void UpdateHoveredObjectId(ulong hoveredObjectId)
        {
            base.UpdateHoveredObjectId(hoveredObjectId);
            foreach (var input in s_elementByGroup[this]) input.UpdateHoveredObjectId(hoveredObjectId);
        }
    }

    public abstract class BaseDrawGroup : BaseGroup<DrawingInteractionDto, AbstractUMI3DInput>
    {
        #region Associate

        bool isDrawing = false;
        bool isDrawingActive = false;

        ulong? lineId;
        GameObject mesh;

        EventInteraction toggleInteraction;
        EventInteraction drawInteraction;

        public Func<EventInteraction> InstantiateToggle;
        public Func<EventInteraction> InstantiateInteraction;

        List<Vector3> positions = new();

        public override void Associate(ulong environmentId, AbstractInteractionDto interaction, ulong toolId, ulong hoveredObjectId)
        {
            UnityEngine.Debug.Log($"Associate {this.associatedInteraction}");

            if (interaction is not DrawingInteractionDto drawing) throw new System.Exception($"This input is not compatible with {interaction}");

            if (!IsAvailableFor(drawing)) throw new System.Exception($"This input is not available for {drawing}");

            if (!IsCompatibleWith(drawing)) throw new System.Exception("Trying to associate an uncompatible interaction !");

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

            toggleInteraction.associatedInteraction.name = isDrawingActive ? DeactivateToggleName : ActivateToggleName;

            UnityEngine.Debug.Log($"SwitchDrawing to {isDrawingActive}");

            var eventdto = new common.interaction.EventStateChangedDto
            {
                active = isDrawingActive,
                boneType = bone,
                id = associatedInteraction.id,
                toolId = this.toolId,
                hoveredObjectId = hoveredObjectId,
                bonePosition = (Vector3Dto)boneTransform.position.Dto(),
                boneRotation = (Vector4Dto)boneTransform.rotation.Dto()
            };
            cdk.UMI3DClientServer.SendData(eventdto, true);

            if (isDrawingActive)
            {
                if (associatedInteraction.TriggerAnimationId != 0)
                    StartAnim(environmentId, associatedInteraction.TriggerAnimationId);
            }
            else
            {
                if (associatedInteraction.ReleaseAnimationId != 0)
                    StartAnim(environmentId, associatedInteraction.ReleaseAnimationId);
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

            DrawingManager.Instance.StartDrawing(drawing);
            isDrawing = true;
            mesh = null;
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
                    UnityEngine.Debug.Log("Drawing line found");
                }
                else
                    UnityEngine.Debug.Log($"Drawing line not found {lineEntity != null} {lineEntity.dto} {this.environmentId} {drawing.LineId}");
            }
            else
                UnityEngine.Debug.Log("Drawing line not found");

            if (drawing.MeshId != 0)
            {
                var meshEntity = await UMI3DEnvironmentLoader.Instance.WaitUntilEntityLoaded(this.environmentId, drawing.MeshId, new());
                if (meshEntity is UMI3DNodeInstance node)
                    mesh = node.GameObject;
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
            cdk.UMI3DClientServer.SendData(drawingDto, true);

            DrawingManager.Instance.StopDrawing(drawing);
        }

        protected void Update()
        {
            if (!isDrawingActive || !isDrawing)
                return;

            if (associatedInteraction is not DrawingInteractionDto drawing)
                return;

            Vector3 position = DrawingManager.Instance.GetDrawingWorldPoint(drawing);
            positions.Add(position);

            if(lineId.HasValue)
            {
                var line = UMI3DLineRendererLoader.GetLine(lineId.Value);
                line.positionCount = positions.Count;
                line.useWorldSpace = true;
                line.SetPositions(positions.ToArray());
            }

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
            cdk.UMI3DClientServer.SendData(drawingDto, true);

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
