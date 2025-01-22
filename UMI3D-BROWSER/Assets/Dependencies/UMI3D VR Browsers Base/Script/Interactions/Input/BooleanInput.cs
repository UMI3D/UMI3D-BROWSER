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

using System.Collections;
using umi3d.cdk;
using umi3d.common;
using umi3d.common.interaction;
using umi3dVRBrowsersBase.ui.playerMenu;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.Events;
using static umi3d.common.volume.GeometryTools;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;
using umi3d.baseBrowser.inputs.interactions;
using System.Linq;
using umi3d.cdk.interaction;

namespace umi3dVRBrowsersBase.interactions.input
{
    /// <summary>
    /// Input for UMI3D Event.
    /// </summary>
    [System.Serializable]
    public class BooleanInput : AbstractVRInput, IDrawerData
    {
        #region Fields

        /// <summary>
        /// Oculus input observer binded to this input.
        /// </summary>
        public VRInputObserver vrInput;

        /// <summary>
        /// Event raised on user input release (first frame).
        /// </summary>
        [SerializeField]
        protected UnityEvent onActionUp = new UnityEvent();

        /// <summary>
        /// Event raised on user input (first frame only).
        /// </summary>
        [SerializeField]
        protected UnityEvent onActionDown = new UnityEvent();


        /// <summary>
        /// True if the rising edge event has been sent through network (to avoid sending falling edge only).
        /// </summary>
        private bool risingEdgeEventSent = false;

        /// <summary>
        /// Is input button down ?
        /// </summary>
        private bool isDown = false;

        public class VRInteractionEvent : UnityEvent<uint> { };

        [HideInInspector]
        public static VRInteractionEvent BooleanEvent = new VRInteractionEvent();

        public ulong environmentId { get; protected set; }

        public AbstractUMI3DInput Input => this;
        public Transform BoneTransform => this.boneTransform;
        public uint BoneType => this.boneType;

        public ulong ToolId => this.toolId;

        public ulong HoveredObjectId => this.hoveredObjectId;

        public ulong? LineId { get; set; }
        public ulong DrawingID { get; set; } = 0;
        public List<UMI3DNodeInstance> Meshes { get; set; } = new();
        public List<Vector3> Positions { get; set; } = new();
        public float LastUpdateTime { get => lastUpdateTime; set => lastUpdateTime = value; }
        public float TimeSynchronization { get => timeSynchronization; set => timeSynchronization = value; }
        public float MinDistance { get => minDistance; set => minDistance = value; }

        public ulong LastSurfaceId { get; set; } = 0;

        bool isDrawing = false;

        [SerializeField] private float lastUpdateTime = 0f;
        [SerializeField] private float timeSynchronization = 0.3f;
        [SerializeField] private float minDistance = 0.01f;


        #endregion

        #region Methods

        /// <summary>
        /// Callback called on oculus input up.
        /// </summary>
        /// <param name="fromAction"></param>
        /// <param name="fromSource"></param>
        /// <see cref="Associate(AbstractInteractionDto)"/>
        private void VRInput_onStateUp()
        {
            if (PlayerMenuManager.Instance.IsMenuHovered)
                return;

            onActionUp.Invoke();
        }

        /// <summary>
        /// Callback called on oculus input down.
        /// </summary>
        /// <param name="fromAction"></param>
        /// <param name="fromSource"></param>
        /// <see cref="Associate(AbstractInteractionDto)"/>
        private void VRInput_onStateDown()
        {
            if (PlayerMenuManager.Instance.IsMenuHovered)
                return;

            onActionDown.Invoke();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="interaction"></param>
        /// <param name="toolId"></param>
        /// <param name="hoveredObjectId"></param>
        public override void Associate(ulong environmentId, AbstractInteractionDto interaction, ulong toolId, ulong hoveredObjectId)
        {
            if (associatedInteraction != null)
            {
                throw new System.Exception("This input is already binded to a interaction ! (" + associatedInteraction + ")");
            }

            if (IsCompatibleWith(interaction))
            {
                this.environmentId = environmentId;

                vrInput.AddOnStateUpListener(VRInput_onStateUp);
                vrInput.AddOnStateDownListener(VRInput_onStateDown);

                risingEdgeEventSent = false;
                isDrawing = false;

                UnityAction<bool> action = async (bool pressDown) =>
                {
                    DrawingInteractionDto drawing = interaction as DrawingInteractionDto;
                    if (pressDown)
                    {
                        if ((interaction as EventDto).hold || drawing != null)
                        {
                            UMI3DClientServer.SendRequest(new EventStateChangedDto()
                            {
                                active = true,
                                boneType = boneType,
                                id = interaction.id,
                                toolId = toolId,
                                hoveredObjectId = hoveredObjectId,
                                bonePosition = boneTransform.position.Dto(),
                                boneRotation = boneTransform.rotation.Dto(),
                            }, true);
                            risingEdgeEventSent = true;
                            isDrawing = drawing != null;

                            if (isDrawing)
                            {
                                DrawingManager.Instance.StartDrawing(drawing, this);
                                await DrawingManager.Instance.Init(this, drawing);
                            }

                        }
                        else
                        {
                            UMI3DClientServer.SendRequest(new EventTriggeredDto()
                            {
                                boneType = boneType,
                                toolId = toolId,
                                id = interaction.id,
                                hoveredObjectId = hoveredObjectId,
                                bonePosition = boneTransform.position.Dto(),
                                boneRotation = boneTransform.rotation.Dto(),
                            }, true);
                        }
                        (controller as VRController).IsInputPressed = true;
                        isDown = true;


                        if ((interaction as EventDto).triggerAnimationId != 0)
                        {
                            BooleanEvent.Invoke(boneType);
                            StartAnim((interaction as EventDto).triggerAnimationId);
                        }

                        onInputDown.Invoke();
                    }
                    else
                    {
                        if ((interaction as EventDto).hold || drawing != null)
                        {
                            if (risingEdgeEventSent)
                            {
                                if (drawing != null)
                                {
                                    umi3d.cdk.UMI3DClientServer.SendRequest(new umi3d.common.interaction.DrawingDto
                                    {
                                        drawingEnd = true,
                                        clientDrawingId = DrawingID,
                                        clientLineId = LineId.HasValue ? LineId.Value : 0,
                                        positions = Positions.Select(p => p.Dto()).ToList(),

                                        surfaceId = LastSurfaceId,

                                        boneType = boneType,
                                        id = associatedInteraction.id,
                                        toolId = this.toolId,
                                        hoveredObjectId = hoveredObjectId,
                                        bonePosition = (Vector3Dto)boneTransform.position.Dto(),
                                        boneRotation = (Vector4Dto)boneTransform.rotation.Dto()
                                    }, true);
                                    DrawingManager.Instance.StopDrawing(drawing, this);
                                }
                                UMI3DClientServer.SendRequest(new EventStateChangedDto()
                                {
                                    active = false,
                                    boneType = boneType,
                                    id = interaction.id,
                                    toolId = toolId,
                                    bonePosition = boneTransform.position.Dto(),
                                    boneRotation = boneTransform.rotation.Dto(),
                                }, true);
                                risingEdgeEventSent = false;
                               
                            }

                        }
                        (controller as VRController).IsInputPressed = false;
                        isDown = false;
                        isDrawing = false;
                        LineId = null;
                        DrawingID = 0;
                        Positions.Clear();
                        Meshes.Clear();
                        onInputUp.Invoke();


                        if ((interaction as EventDto).releaseAnimationId != 0)
                        {
                            BooleanEvent.Invoke(boneType);
                            StartAnim((interaction as EventDto).releaseAnimationId);
                        }
                    }
                };

                onActionDown.AddListener(() => { action.Invoke(true); });
                onActionUp.AddListener(() => { action.Invoke(false); });

                base.Associate(environmentId, interaction, toolId, hoveredObjectId);
            }
            else
            {
                throw new System.Exception("Trying to associate an incompatible interaction !");
            }
        }

        protected async void StartAnim(ulong id)
        {
            var anim = UMI3DAbstractAnimation.Get(UMI3DGlobalID.EnvironmentId, id);
            if (anim != null)
            {
                await anim.SetUMI3DProperty(
                    new SetUMI3DPropertyData(
                        UMI3DGlobalID.EnvironmentId,
                         new SetEntityPropertyDto()
                         {
                             entityId = id,
                             property = UMI3DPropertyKeys.AnimationPlaying,
                             value = true
                         },
                        UMI3DEnvironmentLoader.GetEntity(UMI3DGlobalID.EnvironmentId, id))
                    );
                anim.Start();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="manipulation"></param>
        /// <param name="dofs"></param>
        /// <param name="toolId"></param>
        /// <param name="hoveredObjectId"></param>
        public override void Associate(ulong environmentId, ManipulationDto manipulation, DofGroupEnum dofs, ulong toolId, ulong hoveredObjectId)
        {
            throw new System.Exception("Boolean input is not compatible with manipulation");
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Dissociate()
        {
            if (associatedInteraction == null)
                return;

            if ((associatedInteraction as EventDto).hold && risingEdgeEventSent)
            {
                onActionUp.AddListener(() => StartCoroutine(WaitAndDissociate()));
            }
            else
            {
                DissociateInternal();
            }
        }

        /// <summary>
        /// Dissociates after the end of the current frame.
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitAndDissociate()
        {
            yield return new WaitForEndOfFrame();
            DissociateInternal();
        }

        /// <summary>
        /// Performs dissociation.
        /// </summary>
        private void DissociateInternal()
        {
            base.Dissociate();

            onActionUp.RemoveAllListeners();
            onActionDown.RemoveAllListeners();

            vrInput.RemoveOnStateUpListener(VRInput_onStateUp);
            vrInput.RemoveOnStateDownListener(VRInput_onStateDown);

            if (isDown)
            {
                (controller as VRController).IsInputPressed = false;
                isDown = false;
            }

            risingEdgeEventSent = false;
            isDrawing = false;
            DrawingID = 0;
            LineId = null;
            Positions.Clear();
            Meshes.Clear();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="interaction"></param>
        /// <returns></returns>
        public override bool IsCompatibleWith(AbstractInteractionDto interaction)
        {
            return (interaction is EventDto);
        }

        #endregion


        protected void Update()
        {
            if (!isDrawing)
                return;

            if (associatedInteraction is not DrawingInteractionDto drawing)
                return;

            DrawingManager.Instance.DrawUpdate(drawing, null, this);
        }
        
    }
}