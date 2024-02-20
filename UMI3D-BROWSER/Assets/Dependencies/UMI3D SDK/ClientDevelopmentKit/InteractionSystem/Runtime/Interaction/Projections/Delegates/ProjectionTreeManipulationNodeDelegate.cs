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
    [CreateAssetMenu(fileName = "UMI3D PT Manipulation Node Delegate", menuName = "UMI3D/Interactions/PT Delegates/PT Manipulation Node Delegate")]
    public class ProjectionTreeManipulationNodeDelegate : AbstractProjectionTreeNodeDelegate<ManipulationDto>
    {
        public DofGroupDto sep;

        public override void PrepareForNodeFactory(
            ManipulationDto dto,
            Func<AbstractUMI3DInput> findInput,
            out Predicate<ProjectionTreeNode> adequation,
            out Func<ProjectionTreeNode> deepProjectionCreation,
            out Action<ProjectionTreeNode> chooseProjection,
            List<AbstractUMI3DInput> selectedInputs
        )
        {
            adequation = node =>
            {
                var interactionDto = node.model.Node.nodeDto.interactionDto;
                var nodeDto = (ProjectionTreeManipulationNodeDto)node.model.Node.nodeDto;
                return interactionDto is ManipulationDto
                && nodeDto.manipulationDofGroupDto.dofs == sep.dofs;
            };

            deepProjectionCreation = () =>
            {
                AbstractUMI3DInput projection = findInput?.Invoke();

                if (projection == null)
                {
                    throw new NoInputFoundException();
                }

                return new ProjectionTreeNode(
                    treeId: treeId,
                    nodeId: dto.id,
                    new ProjectionTreeManipulationNodeDto()
                    {
                        dto = dto,
                        manipulationDofGroupDto = sep
                    },
                    projection,
                    projectionTree_SO
                );
            };

            chooseProjection = node =>
            {
                selectedInputs.Add(node.projectedInput);
            };
        }

        public override void PrepareForNodeFactory(
            ManipulationDto dto,
            ulong environmentId,
            ulong toolId, 
            ulong hoveredObjectId,
            Func<AbstractUMI3DInput> findInput,
            out Predicate<ProjectionTreeNode> adequation,
            out Func<ProjectionTreeNode> deepProjectionCreation,
            out Action<ProjectionTreeNode> chooseProjection,
            List<AbstractUMI3DInput> selectedInputs
        )
        {
            adequation = node =>
            {
                var interactionDto = node.model.Node.nodeDto.interactionDto;
                var nodeDto = (ProjectionTreeManipulationNodeDto)node.model.Node.nodeDto;
                return interactionDto is ManipulationDto
                && nodeDto.manipulationDofGroupDto.dofs == sep.dofs;
            };

            deepProjectionCreation = () =>
            {
                AbstractUMI3DInput projection = findInput?.Invoke();

                if (projection == null)
                {
                    throw new NoInputFoundException();
                }

                return new ProjectionTreeNode(
                    treeId: treeId,
                    nodeId: dto.id,
                    new ProjectionTreeManipulationNodeDto()
                    {
                        dto = dto,
                        manipulationDofGroupDto = sep
                    },
                    projection,
                    projectionTree_SO
                );
            };

            chooseProjection = node =>
            {
                var interactionDto = node.model.Node.nodeDto.interactionDto;
                node.projectedInput.Associate(
                    environmentId,
                    interactionDto as ManipulationDto,
                    sep.dofs,
                    toolId,
                    hoveredObjectId
                );

                selectedInputs.Add(node.projectedInput);
            };
        }

        public override void PartialProject(
            ManipulationDto dto,
            ulong environmentId,
            ulong toolId,
            ulong hoveredObjectId,
            bool unusedInputsOnly,
            Func<AbstractUMI3DInput> findInput,
            out Predicate<ProjectionTreeNode> adequation,
            out Func<ProjectionTreeNode> deepProjectionCreation,
            out Action<ProjectionTreeNode> chooseProjection
        )
        {
            adequation = node =>
            {
                var interactionDto = node.model.Node.nodeDto.interactionDto;
                var nodeDto = (ProjectionTreeManipulationNodeDto)node.model.Node.nodeDto;
                return interactionDto is ManipulationDto
                && nodeDto.manipulationDofGroupDto.dofs == sep.dofs;
            };

            deepProjectionCreation = () =>
            {
                AbstractUMI3DInput projection = findInput?.Invoke();

                if (projection == null)
                {
                    throw new NoInputFoundException();
                }

                return new ProjectionTreeNode(
                    treeId: treeId,
                    nodeId: dto.id,
                    new ProjectionTreeManipulationNodeDto()
                    {
                        dto = dto,
                        manipulationDofGroupDto = sep
                    },
                    projection,
                    projectionTree_SO
                );
            };

            chooseProjection = node =>
            {
                var interactionDto = node.model.Node.nodeDto.interactionDto;
                node.projectedInput.Associate(
                    environmentId,
                    interactionDto as ManipulationDto,
                    sep.dofs,
                    toolId,
                    hoveredObjectId
                );
            };
        }
    }
}