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
        public SelectedInteractableManager selectedInteractableManager;

        /// <summary>
        /// Checks whether an interctable has already projected tools
        /// </summary>
        /// <param name="interactable"></param>
        /// <returns></returns>
        public bool IsProjected(InteractableContainer interactable, AbstractController controller)
        {
            return InteractionMapper.Instance.IsToolSelected(UMI3DGlobalID.EnvironmentId, interactable.Interactable.dto.id);
        }

        /// <summary>
        /// Project an interactable that possesses a tool on a controller
        /// </summary>
        /// <param name="interactable"></param>
        /// <param name="controller"></param>
        public void Project(InteractableContainer interactable, AbstractController controller)
        {
            var interactionTool = AbstractInteractionMapper.Instance.GetTool(UMI3DGlobalID.EnvironmentId, interactable.Interactable.dto.id);
            Project(interactionTool, interactable.Interactable.dto.nodeId, controller);

            selectedInteractableManager.Display(interactable, controller.transform.position);
        }

        /// <summary>
        /// Project a given tool on a controller
        /// </summary>
        /// <param name="interactionTool"></param>
        /// <param name="selectedObjectId"></param>
        /// <param name="controller"></param>
        public void Project(AbstractTool interactionTool, ulong selectedObjectId, AbstractController controller)
        {
            // This method doesn't use InteractionMapper.SelectTool so we need to listen tool events.
            interactionTool.OnUpdated.AddListener(() => UpdateTools(interactionTool, controller));
            interactionTool.OnAdded.AddListener(abstractInteractionDto => { UpdateAddOnTools(interactionTool, controller, abstractInteractionDto); });
            interactionTool.OnRemoved.AddListener(abstractInteractionDto => { UpdateRemoveOnTools(interactionTool, controller, abstractInteractionDto); });

            controller.Project(interactionTool, true, new RequestedUsingSelector<AbstractSelector>() { controller = controller }, selectedObjectId);
        }

        /// <summary>
        /// Release (deproject) a tool from a controller
        /// </summary>
        /// <param name="interactionTool"></param>
        /// <param name="controller"></param>
        public void Release(AbstractTool interactionTool, AbstractController controller)
        {
            UnregisterToToolUpdate(interactionTool);

            controller.Release(interactionTool, new RequestedUsingSelector<AbstractSelector>() { controller = controller });

            selectedInteractableManager.HideWithDelay();
        }

        /// <inheritdoc/>
        public void Release(InteractableContainer interactable, AbstractController controller)
        {
            AbstractTool tool = AbstractInteractionMapper.Instance.GetTool(UMI3DGlobalID.EnvironmentId, interactable.Interactable.dto.id);
            UnregisterToToolUpdate(tool);

            controller.Release(tool, new RequestedUsingSelector<AbstractSelector>() { controller = controller });

            selectedInteractableManager.HideWithDelay();
        }

        private void UnregisterToToolUpdate(AbstractTool interactionTool)
        {
            interactionTool.OnAdded.RemoveAllListeners();
            interactionTool.OnUpdated.RemoveAllListeners();
            interactionTool.OnRemoved.RemoveAllListeners();
        }

        private void UpdateTools(AbstractTool tool, AbstractController controller)
        {
            if (tool.interactionsId.Count <= 0)
                Release(tool, controller);
            else
                controller.Update(tool, true, null);
        }

        private void UpdateAddOnTools(AbstractTool tool, AbstractController controller, AbstractInteractionDto abstractInteractionDto)
        {
            controller.AddUpdate(tool, true, abstractInteractionDto, null);
        }

        private void UpdateRemoveOnTools(AbstractTool tool, AbstractController controller, AbstractInteractionDto abstractInteractionDto)
        {
            foreach (AbstractUMI3DInput input in controller.inputs)
            {
                if (input != null && !input.IsAvailable() && input.CurrentInteraction().id == abstractInteractionDto.id)
                {
                    input.Dissociate();
                }
            }
        }
    }
}