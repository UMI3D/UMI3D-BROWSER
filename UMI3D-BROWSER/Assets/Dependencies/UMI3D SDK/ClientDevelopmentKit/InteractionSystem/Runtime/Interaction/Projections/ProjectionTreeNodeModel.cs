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
using umi3d.common;
using umi3d.debug;
using UnityEngine;

namespace umi3d.cdk.interaction
{
    public class ProjectionTreeNodeModel 
    {
        debug.UMI3DLogger logger = new(mainTag: nameof(ProjectionTreeNodeModel));

        public ProjectionTree_SO projectionTree_SO;
        /// <summary>
        /// Tree's id the node belongs to.
        /// </summary>
        public string treeId;
        /// <summary>
        /// Node id.
        /// </summary>
        public ulong nodeId;

        public ProjectionTreeNodeModel(
            ProjectionTree_SO projectionTree_SO, 
            string treeId, 
            ulong nodeId,
            IProjectionTreeNodeDto projectionTreeNodeDto
        )
        {
            this.projectionTree_SO = projectionTree_SO;
            this.treeId = treeId;
            this.nodeId = nodeId;

            if (Index < 0)
            {
                nodes.Add(new()
                {
                    treeId = treeId,
                    id = nodeId,
                    childrenId = new(),
                    nodeDto = projectionTreeNodeDto
                });
            }
        }

        List<ProjectionTreeNodeDto> nodes
        {
            get
            {
                return projectionTree_SO.trees.Find(tree =>
                {
                    return tree.treeId == treeId;
                }).nodes;
            }
        }

        public int Index
        {
            get 
            {
                return nodes.FindIndex(node => node.id == nodeId); 
            }
        }

        public ProjectionTreeNodeDto Node
        {
            get
            {
                return nodes[Index];
            }
            set
            {
                if (value.id !=  nodeId)
                {
                    logger.Error(nameof(Node), $"Try to replace projection node {nodeId} by {value.id}");
                    return;
                }

                nodes[Index] = value;
            }
        }

        public bool IsChild(ulong id)
        {
            var node = nodes.Find(node =>  node.id == id);
            return node.childrenId?.Contains(id) ?? false;
        }

        /// <summary>
        /// Add a child to this node.
        /// </summary>
        /// <param name="child">Node to add</param>
        public void AddChild(ProjectionTreeNodeDto child)
        {
            if (IsChild(child.id))
            {
                return;
            }

            var node = nodes[Index];
            if (node.childrenId == null)
            {
                node.childrenId = new();
            }
            node.childrenId.Add(child.id);
            nodes[Index] = node;
        }
    }
}