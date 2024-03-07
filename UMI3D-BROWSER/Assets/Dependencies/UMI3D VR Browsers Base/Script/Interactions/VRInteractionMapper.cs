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
using umi3d.cdk.interaction;
using umi3d.cdk.menu;
using umi3dVRBrowsersBase.ui.playerMenu;
using UnityEngine;

namespace umi3dVRBrowsersBase.interactions
{
    /// <summary>
    /// Custom InteractionMapper for the VR UMI3D Browsers.
    /// </summary>
    public class VRInteractionMapper : InteractionMapper
    {
        #region Fields

        /// <summary>
        /// Last controller used by users in a menu to trigger a UI.
        /// </summary>
        public static ControllerType lastControllerUsedToClick = ControllerType.RightHandController;

        #region Data

        /// <summary>
        /// Get the tool associated to an interaction.
        /// </summary>
        private Dictionary<ulong, GlobalTool> interactionsIdToTool = new Dictionary<ulong, GlobalTool>();

        public UMI3DController lastControllerUsedInMenu;

        /// <summary>
        /// Associate a tool id and if it is releasable or not.
        /// </summary>

        private Dictionary<ulong, bool> releasableTools = new Dictionary<ulong, bool>();

        #endregion

        #endregion

        #region Methods

        private void Start()
        {
            toolboxMenu = new Menu { Name = "Toolbox" };
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void ResetModule()
        {
            base.ResetModule();

            releasableTools.Clear();
        }

        ///// <summary>
        ///// <inheritdoc/>
        ///// </summary>
        ///// <param name="toolId"></param>
        ///// <param name="reason"></param>
        //public override void ReleaseTool(ulong environmentId, ulong toolId, InteractionMappingReason reason = null)
        //{
        //    base.ReleaseTool(environmentId, toolId, reason);
        //    lastReason = null;
        //}


        /// <summary>
        /// Select the best compatible controller for a given tool (not necessarily available).
        /// </summary>
        /// <param name="tool"></param>
        /// <returns></returns>
        protected UMI3DController GetController(AbstractTool tool, InteractionMappingReason reason)
        {
            if (reason is RequestedFromMenu)
            {
                //Make sure to project on the controller which was used to select the tool via menu
                if (lastControllerUsedInMenu == null)
                {
                    Debug.LogError("GetController requested from menu but lastControllerUsedInMenu is null");
                    return null;
                }
                return lastControllerUsedInMenu;
            }
            else if (reason is RequestedUsingSelector requestedUsingSelectionReason)
            {
                return requestedUsingSelectionReason.controller;
            }
            else
            {
                return GetController(tool);
            }
        }

        /// <summary>
        /// Returns true if users can release the tool associated to toolId
        /// </summary>
        public bool IsToolReleasable(ulong toolId)
        {
            bool res = true;

            if (releasableTools.ContainsKey(toolId))
                res = releasableTools[toolId];

            return res;
        }


        #endregion
    }
}
