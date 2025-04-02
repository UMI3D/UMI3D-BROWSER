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

using inetum.unityUtils.observation;
using System.Collections.Generic;
using System.Linq;
using umi3d.baseBrowser.inputs.interactions;
using umi3d.browserRuntime.notificationKeys;
using umi3d.cdk;
using umi3d.cdk.interaction;
using umi3d.cdk.menu;
using umi3d.cdk.menu.interaction;
using umi3d.cdk.userCapture.tracking;
using umi3d.common;
using umi3d.common.interaction;
using umi3dBrowsers.interaction.selection.zoneselection;
using umi3dVRBrowsersBase.interactions.input;
using umi3dVRBrowsersBase.interactions.selection.cursor;
using umi3dVRBrowsersBase.ui.playerMenu;
using UnityEngine;

namespace umi3dVRBrowsersBase.interactions
{
    public partial class VRController : AbstractController
    {
        #region Fields

        /// <summary>
        /// Type of this controller
        /// </summary>
        public ControllerType type;

        /// <summary>
        /// Asoociated bone.
        /// </summary>
        public Tracker bone;

        #region Inputs Fields

        /// <summary>
        /// Id of the current hovered UMI3DNode.
        /// </summary>
        public ulong hoveredObjectId;

        public bool IsInputPressed = false;

        #endregion Inputs Fields

        private float timeSinceLastInput = 0;

        /// <summary>
        /// Time to wait after last input before considering it as unused.
        /// </summary>
        private float inputUsageTimeout = 10;

        Notifier parameterInputFoundNotifier;
        Notifier toolReleasedNotifier;

        #endregion Fields

        #region Methods

        #region Monobehaviour Life Cycle

        protected virtual void Awake()
        {
            if (!VRDrawingManager.Exists)
                new VRDrawingManager();

            UnityEngine.Physics.queriesHitBackfaces = true;

            foreach (AbstractUMI3DInput input in manipulationInputs)
                input.Init(this);
            foreach (AbstractUMI3DInput input in booleanInputs)
                input.Init(this);

            parameterInputFoundNotifier = NotificationHub.Default.GetNotifier(
                this,
                ID.FromType<InteractionNotificationKeys.ParameterInputFound>()
            );

            toolReleasedNotifier = NotificationHub.Default.GetNotifier(
                this,
                ID.FromType<InteractionNotificationKeys.ToolReleased>());
        }

        private void Start()
        {
            NotificationHub.Default.Unsubscribe(this);
            (VRDrawingManager.Instance as VRDrawingManager).Declare(this);
        }

        protected virtual void Update()
        {
            if (timeSinceLastInput <= inputUsageTimeout)
                timeSinceLastInput += Time.deltaTime;
        }

        #endregion

        #region Tool : projection and release

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="tool"></param>
        /// <param name="releasable"></param>
        /// <param name="reason"></param>
        /// <param name="hoveredObjectId"></param>
        public override void Project(AbstractTool tool, bool releasable, InteractionMappingReason reason, ulong hoveredObjectId)
        {
            base.Project(tool, releasable, reason, hoveredObjectId);

            if (currentTool == tool) // It means projection succeeded
            {
                tool.onProjected(bone.BoneType);
            }
        }

        /// <summary>
        /// Check if a tool can be projected on this controller.
        /// </summary>
        /// <param name="tool"> The tool to be projected.</param>
        /// <returns></returns>
        public override bool IsCompatibleWith(AbstractTool tool)
        {
            return tool.interactionsLoaded.TrueForAll(inter =>
                (inter is ManipulationDto ) ?
                (inter as ManipulationDto).dofSeparationOptions.Exists(
                    group => !group.separations.Exists(
                        dof => (dof.dofs == DofGroupEnum.X_RX) || (dof.dofs == DofGroupEnum.Y_RY) || (dof.dofs == DofGroupEnum.Z_RZ)))
                : true);
        }

        /// <summary>
        /// If current tool is not null, releases it.
        /// </summary>
        public void ReleaseCurrentTool()
        {
            if (currentTool != null)
            {
                InteractionMapper.Instance.ReleaseTool(UMI3DGlobalID.EnvironmentId, currentTool.id);
            }
        }

        /// <summary>
        /// Projects all parameters on this tool.
        /// </summary>
        /// <param name="tool"></param>
        /// <param name="interactions"></param>
        /// <param name="hoveredObjectId"></param>
        private void ProjectParameters(AbstractTool tool, List<AbstractInteractionDto> interactions, ulong hoveredObjectId)
        {
            AbstractUMI3DInput[] inputs = projectionMemory.Project(this, UMI3DGlobalID.EnvironmentId, interactions.FindAll(inter => inter is AbstractParameterDto).ToArray(), tool.id, hoveredObjectId);
            var toolInputs = new List<AbstractUMI3DInput>();

            if (associatedInputs.TryGetValue((tool.id, tool.environmentId), out AbstractUMI3DInput[] buffer))
            {
                toolInputs = new List<AbstractUMI3DInput>(buffer);
                associatedInputs.Remove((tool.id, tool.environmentId));
            }
            toolInputs.AddRange(inputs);
            associatedInputs.Add((tool.id, tool.environmentId), toolInputs.ToArray());
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="tool"></param>
        /// <param name="reason"></param>
        public override void Release(AbstractTool tool, InteractionMappingReason reason)
        {
            base.Release(tool, reason);
            tool.onReleased(bone.BoneType);

            toolReleasedNotifier[InteractionNotificationKeys.ToolReleased.tool] = tool;
            toolReleasedNotifier.Notify();
        }

        #endregion

        #region Status

        /// <summary>
        /// Check if the user is currently using the selected Tool.
        /// </summary>
        /// <returns>returns true if the user is currently interacting with the tool.</returns>
        protected override bool isInteracting()
        {
            return (currentTool != null) && (timeSinceLastInput <= inputUsageTimeout);
        }

        /// <summary>
        /// Check if the user is currently manipulating the tools menu.
        /// </summary>
        /// <returns>returns true if the user is currently manipulating the tools menu.</returns>
        protected override bool isNavigating()
        {
            throw new System.NotImplementedException();
        }

        #endregion Status

        #region Change mapping

        [ContextMenu("SWIPE")]
        private void Swipe()
        {
            ChangeInputMapping(booleanInputs[0], booleanInputs[1]);
        }

        /// <summary>
        /// Swipes the inputs of two interactions.
        /// </summary>
        /// <param name="previousInput"></param>
        /// <param name="targetInput"></param>
        public void ChangeInputMapping(AbstractVRInput previousInput, AbstractVRInput targetInput)
        {
            AbstractInteractionDto inter = null;
            ulong toolId = 0, objectId = 0;

            if (!targetInput.IsAvailable())
            {
                inter = targetInput.CurrentInteraction();
                toolId = targetInput.GetToolId();
                objectId = targetInput.GetHoveredObjectId();

                targetInput.Dissociate();
            }

            targetInput.Associate(UMI3DGlobalID.EnvironmentId, previousInput.CurrentInteraction(), previousInput.GetToolId(), previousInput.GetHoveredObjectId());
            previousInput.Dissociate();

            if (inter != null)
            {
                previousInput.Associate(UMI3DGlobalID.EnvironmentId, inter, toolId, objectId);
            }
        }

        #endregion Change mapping

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        protected override ulong GetCurrentHoveredId()
        {
            if (tool == null)
                return 0;

            return tool.id;
        }

        #endregion Methods
    }


    public class VRDrawingManager : DrawingManager
    {
        List<(VRController,RayCursor)> vRControllers = new();

        public void Declare(VRController controller)
        {
           vRControllers.Add((controller, controller.gameObject.GetComponentInChildren<RayCursor>()));
        }

        public float distance = 1.5f;
        public float objectDistance = 50f;
        public float offset = 0.01f;
        public float handOffset = 0f;

        public override (Vector3,ulong)? GetDrawingWorldPoint(DrawingInteractionDto drawing, List<UMI3DNodeInstance> nodes, AbstractUMI3DInput input)
        {
            var cursor = vRControllers
                .FirstOrDefault(c =>
                        c.Item1.HoldInput == input
                        || c.Item1.booleanInputs
                                .Any(b => b == input)
                ).Item2;

            if (nodes != null && nodes.Count > 0)
            {
                var zone = new RaySelectionZone<NodeContainer>(cursor.transform.position, cursor.transform.up);
                foreach(var nodeAndRay in zone.GetObjectsOnRayWithRayCastHits())
                    if(nodeAndRay.Value.distance <= distance &&  nodes.Contains(nodeAndRay.Key.instance))
                        return (nodeAndRay.Value.point + nodeAndRay.Value.normal * offset, nodeAndRay.Key.instance.Id);
            }

            if (drawing.canDrawInSpace)
                return (cursor.transform.position + cursor.transform.up * handOffset,0);
            
            return null;
        }

        //protected override void StartDrawingMode(DrawingInteractionDto drawing)
        //{
        //    base.StartDrawingMode(drawing);
        //    BaseCursor.SetMovement(this, BaseCursor.CursorMovement.Drawing);
        //}

        //protected override void StopDrawingMode(DrawingInteractionDto drawing)
        //{
        //    base.StopDrawingMode(drawing);
        //    BaseCursor.UnSetMovement(this);
        //}
    }
}