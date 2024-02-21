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
    public class ProjectionTreeModel 
    {
        debug.UMI3DLogger logger = new(mainTag: nameof(ProjectionTreeModel));

        public ProjectionTree_SO projectionTree_SO;
        /// <summary>
        /// Tree's id the node belongs to.
        /// </summary>
        public string treeId;

        public ProjectionTreeModel(
            ProjectionTree_SO projectionTree_SO, 
            string treeId
        )
        {
            this.projectionTree_SO = projectionTree_SO;
            this.treeId = treeId;
        }

        public List<ProjectionTreeNodeDto> Nodes
        {
            get
            {
                return projectionTree_SO.trees.Find(tree =>
                {
                    return tree.treeId == treeId;
                }).nodes;
            }
        }

        public int IndexOf(ulong nodeId)
        {
            return Nodes.FindIndex(node =>
            {
                return node.id == nodeId;
            });
        }

        public int IndexOfChildInParent(ProjectionTreeNodeDto parent, ulong child)
        {
            return parent.children?.FindIndex(node =>
            {
                return node.id == child;
            }) ?? -1;
        }

        public bool IsChildOf(ulong parent, ulong child)
        {
            var parentIndex = IndexOf(parent);
            if (parentIndex == -1)
            {
                return false;
            }

            return IndexOfChildInParent(Nodes[parentIndex], child) != -1;
        }

        public bool AddRoot(ProjectionTreeNodeDto root)
        {
            if (projectionTree_SO.trees.FindIndex(tree =>
            {
                return tree.treeId == root.treeId;
            }) == -1)
            {
                projectionTree_SO.trees.Add(
                    new ProjectionTreeDto()
                    {
                        treeId = root.treeId,
                        root = root,
                        nodes = new()
                    }
                );
                return true;
            }
            return false;
        }

        /// <summary>
        /// Add a child to this parent.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child">Node to add</param>
        public bool AddChild(ulong parent, ProjectionTreeNodeDto child)
        {
            if (IsChildOf(parent, child.id))
            {
                return true;
            }

            var parentIndex = IndexOf(parent);
            if (parentIndex != -1)
            {
                return false;
            }

            var parentNode = Nodes[parentIndex];
            if (parentNode.children == null)
            {
                parentNode.children = new();
            }

            if (IndexOfChildInParent(parentNode, child.id) == -1)
            {
                parentNode.children.Add(child);
            }

            if (IndexOf(child.id) == -1)
            {
                Nodes.Add(child);
            }
            Nodes[parentIndex] = parentNode;
            return true;
        }
    }
}