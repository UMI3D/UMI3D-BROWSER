/*
Copyright 2019 - 2022 Inetum

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
using umi3d.browserRuntime.navigation;
using umi3d.cdk;
using UnityEngine;

namespace umi3d.baseBrowser.navigation.pc
{

    /// <summary>
    /// This class handles the navigation possibility. It is based on Unity navmesh.
    /// </summary>
    public class PCNavmeshController : NavmeshController
    {
        protected override void AddNavmeshArea(GameObject nodeGameObject)
        {
            nodeGameObject.layer = ToLayer(navmeshLayer);
        }

        protected override void RemoveNavmeshArea(UMI3DNodeInstance node)
        {
            SetLayer(node, defaultLayer);
        }

        protected override void SetTraversable(UMI3DNodeInstance node)
        {
            if (node.isNavmesh) { return; }

            SetLayer(node, node.isTraversable ? defaultLayer : obstacleLayer);
        }

        protected override void SwitchHierarchyToObstacleLayer(GameObject go)
        {
            go.layer = obstacleLayer;

            foreach (Transform t in go.transform)
            {
                SwitchHierarchyToObstacleLayer(t.gameObject);
            }
        }
    }
}