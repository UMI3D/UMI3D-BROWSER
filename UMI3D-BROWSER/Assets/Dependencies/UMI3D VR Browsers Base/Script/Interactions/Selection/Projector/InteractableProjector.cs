/*
Copyright 2019 - 2021 Inetum
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

using umi3d.cdk.interaction;
using umi3d.common;
using umi3d.common.interaction;
using umi3dVRBrowsersBase.ui;
using umi3dVRBrowsersBase.ui.playerMenu;

namespace umi3dBrowsers.interaction.selection.projector
{
    /// <summary>
    /// Projector for Interactable
    /// </summary>
    public class InteractableProjector : IProjector<InteractableContainer>
    {
        ParameterGear parameterGear;

        public InteractableProjector(ParameterGear parameterGear)
        {
            this.parameterGear = parameterGear;
        }

        /// <summary>
        /// Checks whether an interctable has already projected tools
        /// </summary>
        /// <param name="interactable"></param>
        /// <returns></returns>
        public bool IsProjected(InteractableContainer interactable, UMI3DController controller)
        {
            throw new System.NotImplementedException();
            //return InteractionMapper.Instance.IsToolSelected(UMI3DGlobalID.EnvironmentId, interactable.Interactable.dto.id);
        }

        /// <summary>
        /// Project an interactable that possesses a tool on a controller
        /// </summary>
        /// <param name="interactable"></param>
        /// <param name="controller"></param>
        public void Project(InteractableContainer interactable, UMI3DController controller)
        {
            throw new System.NotImplementedException();
            //var interactionTool = AbstractInteractionMapper.Instance.GetTool(UMI3DGlobalID.EnvironmentId, interactable.Interactable.dto.id);
            //Project(interactionTool, interactable.Interactable.dto.nodeId, controller);

            //// display the parameter gear if it is required to access the interactions
            //if (interactable.Interactable.interactions.FindAll(i => i.Result is AbstractParameterDto).Count > 0)
            //{
            //    parameterGear.Display(interactable, controller.transform.position);
            //    PlayerMenuManager.Instance.CtrlToolMenu.RememberParameters();
            //}
        }

        /// <summary>
        /// Project a given tool on a controller
        /// </summary>
        /// <param name="interactionTool"></param>
        /// <param name="selectedObjectId"></param>
        /// <param name="controller"></param>
        public void Project(AbstractTool interactionTool, ulong selectedObjectId, UMI3DController controller)
        {
            throw new System.NotImplementedException();
            //controller.projectionManager.Project(interactionTool, true, new RequestedUsingSelector<AbstractSelector>() { controller = controller }, selectedObjectId);
        }

        /// <summary>
        /// Release (deproject) a tool from a controller
        /// </summary>
        /// <param name="interactionTool"></param>
        /// <param name="controller"></param>
        public void Release(AbstractTool interactionTool, UMI3DController controller)
        {
            throw new System.NotImplementedException();
            //controller.projectionManager.Release(interactionTool, new RequestedUsingSelector<AbstractSelector>() { controller = controller });

            parameterGear.HideWithDelay();
        }

        /// <inheritdoc/>
        public void Release(InteractableContainer interactable, UMI3DController controller)
        {
            throw new System.NotImplementedException();
            //controller.projectionManager.Release(AbstractInteractionMapper.Instance.GetTool(UMI3DGlobalID.EnvironmentId, interactable.Interactable.dto.id), new RequestedUsingSelector<AbstractSelector>() { controller = controller });

            //parameterGear.HideWithDelay();
        }
    }
}