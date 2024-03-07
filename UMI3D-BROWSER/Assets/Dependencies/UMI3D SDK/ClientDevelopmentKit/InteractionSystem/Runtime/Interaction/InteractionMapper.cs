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
using System;
using System.Collections.Generic;
using System.Linq;
using umi3d.cdk.menu;
using umi3d.common.interaction;

namespace umi3d.cdk.interaction
{
    /// <summary>
    /// Default implementation of <see cref="AbstractInteractionMapper"/>.
    /// </summary>
    public class InteractionMapper : AbstractInteractionMapper
    {
        public static new InteractionMapper Instance => AbstractInteractionMapper.Instance as InteractionMapper;

        #region Data

        /// <summary>
        /// Associate a toolid with the controller the tool is projected on.
        /// </summary>
        public Dictionary<(ulong, ulong), UMI3DController> toolIdToController { get; protected set; } = new Dictionary<(ulong, ulong), UMI3DController>();

        /// <summary>
        /// Id to Dto interactions map.
        /// </summary>
        public Dictionary<(ulong, ulong), AbstractInteractionDto> interactionsIdToDto { get; protected set; } = new Dictionary<(ulong, ulong), AbstractInteractionDto>();

        /// <summary>
        /// Currently projected tools.
        /// </summary>
        private readonly Dictionary<(ulong, ulong), InteractionMappingReason> projectedTools = new Dictionary<(ulong, ulong), InteractionMappingReason>();

        #endregion

        ///// <inheritdoc/>
        //public override bool UpdateTools(ulong environmentId, ulong toolId, bool releasable, InteractionMappingReason reason = null)
        //{
        //    if (toolIdToController.ContainsKey((environmentId,toolId)))
        //    {
        //        AbstractController controller = toolIdToController[(environmentId, toolId)];
        //        AbstractTool tool = GetTool(environmentId, toolId);
        //        if (tool.interactionsId.Count <= 0)
        //            ReleaseTool(environmentId, tool.id, new ToolNeedToBeUpdated());
        //        else
        //            controller.projectionManager.Project(
        //                tool, 
        //                releasable, 
        //                reason,
        //                controller.toolManager.toolDelegate.CurrentHoverTool.id
        //            );
        //        return true;
        //    }
        //    throw new Exception("no controller have this tool projected");
        //}

        ///// <inheritdoc/>
        //public override bool UpdateAddOnTools(ulong environmentId, ulong toolId, bool releasable, AbstractInteractionDto abstractInteractionDto, InteractionMappingReason reason = null)
        //{
        //    if (toolIdToController.ContainsKey((environmentId, toolId)))
        //    {
        //        AbstractController controller = toolIdToController[(environmentId, toolId)];
        //        AbstractTool tool = GetTool(environmentId, toolId);
        //        controller.projectionManager.Project(tool, abstractInteractionDto, releasable, reason);
        //        return true;
        //    }
        //    throw new Exception("no controller have this tool projected");
        //}

        ///// <inheritdoc/>
        //public override bool UpdateRemoveOnTools(ulong environmentId, ulong toolId, bool releasable, AbstractInteractionDto abstractInteractionDto, InteractionMappingReason reason = null)
        //{
        //    AbstractTool tool = GetTool(environmentId, toolId);
        //    tool.interactionsId.Remove(abstractInteractionDto.id);

        //    foreach (AbstractController item in Controllers)
        //    {
        //        foreach (AbstractUMI3DInput input in item.inputs)
        //        {
        //            if (input != null && !input.IsAvailable() && input.CurrentInteraction().id == abstractInteractionDto.id)
        //            {
        //                input.Dissociate();
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}

        #region CRUD



        /// <inheritdoc/>
        public override AbstractInteractionDto GetInteraction(ulong environmentId,ulong id)
        {
            if (!InteractionExists(environmentId, id))
                throw new KeyNotFoundException();
            interactionsIdToDto.TryGetValue((environmentId, id), out AbstractInteractionDto inter);
            return inter;
        }

        /// <inheritdoc/>
        public override IEnumerable<AbstractInteractionDto> GetInteractions(Predicate<AbstractInteractionDto> condition)
        {
            return interactionsIdToDto.Values.ToList().FindAll(condition);
        }

        /// <inheritdoc/>
        public override bool InteractionExists(ulong environmentId, ulong id)
        {
            return interactionsIdToDto.ContainsKey((environmentId, id));
        }

        #endregion
    }
}
