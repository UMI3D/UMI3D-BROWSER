/*
Copyright 2019 - 2024 Inetum

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
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.cdk.interaction
{
    [CreateAssetMenu(fileName = "UMI3D PT Event Node Delegate", menuName = "UMI3D/Interactions/Projection Delegate/PT Event Node Delegate")]
    public class ProjectionTreeEventNodeDelegate : AbstractProjectionTreeNodeDelegate<EventDto>
    {
        public override Predicate<ProjectionTreeNodeData> IsNodeCompatible(EventDto interaction)
        {
            return node =>
            {
                var interactionDto = node.interactionData.Interaction;
                return interactionDto is EventDto && interactionDto.name.Equals(interaction.name);
            };
        }

        public override Func<ProjectionTreeNodeData> CreateNodeForControl(
            EventDto interaction,
            Func<AbstractControlData> getControl
        )
        {
            return () =>
            {
                AbstractControlData control = getControl?.Invoke();

                if (control == null)
                {
                    throw new NoInputFoundException($"For {nameof(EventDto)}: {interaction.name}");
                }

                return new ProjectionTreeNodeData()
                {
                    treeId = treeId,
                    id = interaction.id,
                    children = new(),
                    interactionData = new ProjectionTreeEventNodeData()
                    {
                        interaction = interaction
                    },
                    controlId = control
                };
            };
        }

        public override Action<ProjectionTreeNodeData> ChooseProjection(
            ulong? environmentId = null,
            ulong? toolId = null,
            ulong? hoveredObjectId = null
        )
        {
            return node =>
            {
                if (environmentId.HasValue && toolId.HasValue && hoveredObjectId.HasValue)
                {
                    controlManager.eventInputDelegate.Associate(
                        node.controlId,
                        environmentId.Value,
                        node.interactionData.Interaction,
                        toolId.Value,
                        hoveredObjectId.Value
                    );
                }
            };
        }
    }
}