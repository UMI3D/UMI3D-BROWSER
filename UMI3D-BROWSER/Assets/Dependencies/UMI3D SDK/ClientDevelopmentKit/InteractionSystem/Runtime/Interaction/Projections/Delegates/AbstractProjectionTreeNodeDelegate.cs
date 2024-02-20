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

using inetum.unityUtils.saveSystem;
using System;
using System.Collections.Generic;
using umi3d.common.interaction;

namespace umi3d.cdk.interaction
{
    public abstract class AbstractProjectionTreeNodeDelegate<Dto> : SerializableScriptableObject
    where Dto : AbstractInteractionDto
    {
        public ProjectionTree_SO projectionTree_SO;
        public string treeId;

        /// <summary>
        /// Init this object with dependency injection.
        /// </summary>
        /// <param name="projectionTree_SO"></param>
        /// <param name="treeId"></param>
        public virtual void Init(ProjectionTree_SO projectionTree_SO, string treeId)
        {
            this.projectionTree_SO = projectionTree_SO;
            this.treeId = treeId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="findInput">Find the input that match this interaction.</param>
        /// <param name="adequation">Is the [node] in adequation with this <paramref name="dto"/>.</param>
        /// <param name="deepProjectionCreation">Create a tree node with the corresponding to a concreate node type.</param>
        /// <param name="chooseProjection">Select this input.</param>
        /// <param name="selectedInputs"></param>
        public abstract void PrepareForNodeFactory(
            Dto dto,
            Func<AbstractUMI3DInput> findInput,
            out Predicate<ProjectionTreeNode> adequation,
            out Func<ProjectionTreeNode> deepProjectionCreation,
            out Action<ProjectionTreeNode> chooseProjection,
            List<AbstractUMI3DInput> selectedInputs
        );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="environmentId"></param>
        /// <param name="toolId"></param>
        /// <param name="hoveredObjectId"></param>
        /// <param name="findInput">Find the input that match this interaction.</param>
        /// <param name="adequation">Is the [node] in adequation with this <paramref name="dto"/>.</param>
        /// <param name="deepProjectionCreation">Create a tree node with the corresponding to a concreate node type.</param>
        /// <param name="chooseProjection">Select this input and associate the dto with this input.</param>
        /// <param name="selectedInputs"></param>
        public abstract void PrepareForNodeFactory(
            Dto dto,
            ulong environmentId,
            ulong toolId,
            ulong hoveredObjectId,
            Func<AbstractUMI3DInput> findInput,
            out Predicate<ProjectionTreeNode> adequation,
            out Func<ProjectionTreeNode> deepProjectionCreation,
            out Action<ProjectionTreeNode> chooseProjection,
            List<AbstractUMI3DInput> selectedInputs
        );

        /// <summary>
        /// Project a dto on a controller and return associated input.
        /// </summary>
        /// <param name="dto">dto to project</param>
        /// <param name="controller">Controller to project on</param>
        /// <param name="environmentId"></param>
        /// <param name="toolId"></param>
        /// <param name="hoveredObjectId"></param>
        /// <param name="unusedInputsOnly">Project on unused inputs only</param>
        public abstract void PartialProject(
            Dto dto,
            ulong environmentId,
            ulong toolId,
            ulong hoveredObjectId,
            bool unusedInputsOnly,
            Func<AbstractUMI3DInput> findInput,
            out Predicate<ProjectionTreeNode> adequation,
            out Func<ProjectionTreeNode> deepProjectionCreation,
            out Action<ProjectionTreeNode> chooseProjection
        );

        /// <summary>
        /// Exception thrown when not associated input has been found for an interaction.
        /// </summary>
        public class NoInputFoundException : System.Exception
        {
            public NoInputFoundException() { }
            public NoInputFoundException(string message) : base(message) { }
            public NoInputFoundException(string message, System.Exception inner) : base(message, inner) { }
        }
    }
}