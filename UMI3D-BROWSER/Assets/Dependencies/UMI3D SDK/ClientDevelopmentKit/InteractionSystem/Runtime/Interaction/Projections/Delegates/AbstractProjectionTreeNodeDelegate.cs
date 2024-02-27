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
        public UMI3DInputManager controlManager;

        /// <summary>
        /// Init this object with dependency injection.
        /// </summary>
        /// <param name="projectionTree_SO"></param>
        /// <param name="treeId"></param>
        public virtual void Init(
            ProjectionTree_SO projectionTree_SO,
            string treeId,
            UMI3DInputManager controlManager
        )
        {
            this.projectionTree_SO = projectionTree_SO;
            this.treeId = treeId;
            this.controlManager = controlManager;
        }

        /// <summary>
        /// Return a predicate that is true if the node is compatible with the interaction.
        /// </summary>
        /// <param name="interaction"></param>
        /// <returns></returns>
        public abstract Predicate<ProjectionTreeNodeData> IsNodeCompatible(Dto interaction);

        /// <summary>
        /// Return a <see cref="Func{ProjectionTreeNode}"/> that will create a tree node for this input found by <paramref name="getControlId"/>.<br/>
        /// This <see cref="Func{ProjectionTreeNode}"/> must throw a <see cref="NoInputFoundException"/> if no controlId is found.
        /// </summary>
        /// <param name="interaction"></param>
        /// <param name="getControlId"></param>
        /// <returns></returns>
        public abstract Func<ProjectionTreeNodeData> CreateNodeForControl(
            Dto interaction,
            Func<Guid?> getControlId
        );

        /// <summary>
        /// Return a delegate that<br/>
        /// - if <paramref name="environmentId"/> and <paramref name="toolId"/> and <paramref name="hoveredObjectId"/> are not null then associated the interaction with its input.<br/>
        /// - if <paramref name="selectedInputs"/> is not null select the node's input.
        /// </summary>
        /// <param name="environmentId"></param>
        /// <param name="toolId"></param>
        /// <param name="hoveredObjectId"></param>
        /// <returns></returns>
        public abstract Action<ProjectionTreeNodeData> ChooseProjection(
            ulong? environmentId = null,
            ulong? toolId = null,
            ulong? hoveredObjectId = null
        );
    }
}