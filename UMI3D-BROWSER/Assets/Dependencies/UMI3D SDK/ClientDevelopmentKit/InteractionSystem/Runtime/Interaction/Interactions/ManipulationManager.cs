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
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.cdk.interaction
{
    public class ManipulationManager : IProjectionTreeNodeDelegate<ManipulationDto>
    {
        public DofGroupDto dofGroup;

        public Predicate<ProjectionTreeNodeData> IsNodeCompatible(ManipulationDto interaction)
        {
            return node =>
            {
                return node.interactionData is ProjectionTreeManipulationNodeData nodeData
                && nodeData.interaction is ManipulationDto
                && nodeData.dofGroup.dofs == dofGroup.dofs;
            };
        }

        public Func<ProjectionTreeNodeData> CreateNodeForControl(
            string treeId,
            ManipulationDto interaction,
            Func<AbstractControlEntity> getControl
        )
        {
            return () =>
            {
                AbstractControlEntity control = getControl?.Invoke();

                if (control == null)
                {
                    throw new NoInputFoundException($"For {nameof(ManipulationDto)}: {interaction.name}");
                }

                return new ProjectionTreeNodeData()
                {
                    treeId = treeId,
                    id = interaction.id,
                    children = new(),
                    interactionData = new ProjectionTreeManipulationNodeData()
                    {
                        interaction = interaction,
                        dofGroup = dofGroup
                    },
                    control = control
                };
            };
        }

        public Action<ProjectionTreeNodeData> ChooseProjection(
            UMI3DControlManager controlManager,
            ulong? environmentId = null,
            ulong? toolId = null,
            ulong? hoveredObjectId = null
        )
        {
            return node =>
            {
                if (environmentId.HasValue && toolId.HasValue && hoveredObjectId.HasValue)
                {
                    controlManager.Associate(
                        node.control,
                        environmentId.Value,
                        toolId.Value,
                        node.interactionData.Interaction,
                        hoveredObjectId.Value,
                        dofGroup
                    );
                }
            };
        }
    }
}