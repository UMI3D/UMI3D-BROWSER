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

using System.Collections.Generic;
using umi3d.cdk.interaction;
using umi3d.cdk.menu;
using umi3d.cdk.menu.interaction;
using umi3d.cdk.userCapture.tracking;
using umi3d.common;
using umi3d.common.interaction;
using umi3dVRBrowsersBase.interactions.input;
using umi3dVRBrowsersBase.ui.playerMenu;
using Unity.VisualScripting;
using UnityEngine;

namespace umi3dVRBrowsersBase.interactions
{
    public partial class VRController : AbstractController
    {
        #region Fields

        [HideInInspector]
        public MenuAsset ObjectMenu;

        /// <summary>
        /// Type of this controller
        /// </summary>
        public ControllerType type;

        #endregion Fields

        #region Methods

        #region Monobehaviour Life Cycle

        protected virtual void Awake()
        {
            ObjectMenu = Resources.Load<MenuAsset>("ParametersMenu");

            UnityEngine.Physics.queriesHitBackfaces = true;

            foreach (AbstractUMI3DInput input in manipulationInputs)
                input.Init(this);
            foreach (AbstractUMI3DInput input in booleanInputs)
                input.Init(this);
        }

        #endregion

        #region Tool : projection and release

        /// <summary>
        /// Projects all parameters on this tool.
        /// </summary>
        /// <param name="tool"></param>
        /// <param name="interactions"></param>
        /// <param name="hoveredObjectId"></param>
        private void ProjectParameters(AbstractTool tool, List<AbstractInteractionDto> interactions, ulong hoveredObjectId)
        {
            throw new System.NotImplementedException();
            //AbstractUMI3DInput[] inputs = projectionManager.Project(
            //    interactions.FindAll(inter => inter is AbstractParameterDto).ToArray(), 
            //    UMI3DGlobalID.EnvironmentId, 
            //    tool.id, 
            //    hoveredObjectId
            //);
            //var toolInputs = new List<AbstractUMI3DInput>();

            //if (associatedInputs.TryGetValue(tool.id, out AbstractUMI3DInput[] buffer))
            //{
            //    toolInputs = new List<AbstractUMI3DInput>(buffer);
            //    associatedInputs.Remove(tool.id);
            //}
            //toolInputs.AddRange(inputs);
            //associatedInputs.Add(tool.id, toolInputs.ToArray());
        }

        ///// <summary>
        ///// <inheritdoc/>
        ///// </summary>
        ///// <param name="tool"></param>
        ///// <param name="reason"></param>
        //public override void Release(AbstractTool tool, InteractionMappingReason reason)
        //{
        //    base.Release(tool, reason);
        //    tool.onReleased(bone.BoneType);

        //    PlayerMenuManager.Instance.CtrlToolMenu.ClearBindingList(type);
        //    PlayerMenuManager.Instance.MenuHeader.DisplayControllerButton(false, type, string.Empty);
        //}

        #endregion

        public virtual void Update()
        {

        }

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

        #endregion Methods
    }
}