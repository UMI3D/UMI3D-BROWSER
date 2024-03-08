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
    public partial class VRController : MonoBehaviour
    {
        [HideInInspector]
        public MenuAsset ObjectMenu;

        /// <summary>
        /// Type of this controller
        /// </summary>
        public ControllerType type;

        protected virtual void Awake()
        {
            ObjectMenu = Resources.Load<MenuAsset>("ParametersMenu");

            UnityEngine.Physics.queriesHitBackfaces = true;
        }

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
    }
}