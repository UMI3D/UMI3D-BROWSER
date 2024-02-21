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

using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace umi3d.cdk.interaction
{
    /// <summary>
    /// Projection tree node for projection memory.
    /// </summary>
    [System.Serializable]
    public class ProjectionTreeNode
    {
        public ProjectionTreeNodeModel model;
        /// <summary>
        /// UMI3D Input projected to this node. 
        /// </summary>
        [SerializeField]
        public AbstractUMI3DInput projectedInput;

        public ProjectionTreeNode(
            string treeId, 
            ulong nodeId, 
            IProjectionTreeNodeDto projectionTreeNodeDto,
            AbstractUMI3DInput projectedInput, 
            ProjectionTree_SO projectionTree_SO
        )
        {
            this.model = new(
                projectionTree_SO, 
                treeId,
                nodeId,
                projectionTreeNodeDto
            );
            this.projectedInput = projectedInput;
        }
        public void AddChild(ProjectionTreeNode child) 
        {
            model.AddChild(child.model.Node);
        }









        /// <summary>
        /// Nodes collection by tree id.
        /// </summary>
        [SerializeField]
        protected static Dictionary<string, Dictionary<ulong, ProjectionTreeNode>> nodesByTree = new();

        /// <summary>
        /// Node's children. Please avoid calling this field too often as it is slow to compute.
        /// </summary>
        public List<ProjectionTreeNode> children
        {
            get
            {

                //var buffer = new List<ProjectionTreeNode>();
                //foreach (ulong id in childrensId)
                //{
                //    if (nodesByTree.TryGetValue(treeId, out Dictionary<ulong, ProjectionTreeNode> nodes))
                //    {
                //        if (nodes.TryGetValue(id, out ProjectionTreeNode child))
                //            buffer.Add(child);
                //    }
                //}
                //return buffer;
                return null;
            }
        }
    }
}