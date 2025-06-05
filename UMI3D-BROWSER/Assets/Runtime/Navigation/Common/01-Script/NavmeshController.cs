/*
Copyright 2019 - 2025 Inetum

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

using System.Collections;
using System.Collections.Generic;
using umi3d.cdk;
using umi3d.cdk.collaboration;
using umi3d.cdk.volumes;
using UnityEngine;

namespace umi3d.browserRuntime.navigation
{
    public abstract class NavmeshController : MonoBehaviour
    {
        [Tooltip("Default layer for traversable objets")]
        public LayerMask defaultLayer;

        [Tooltip("Layer for objects part of the navmesh")]
        public LayerMask navmeshLayer;

        [Tooltip("Layer for non traversable objects")]
        public LayerMask obstacleLayer;

        protected Dictionary<ulong, GameObject> cellIdToGameobjects = new Dictionary<ulong, GameObject>();

        void Start()
        {
            UMI3DEnvironmentLoader.Instance.onNodePartOfNavmeshSet += SetPartOfNavmesh;
            UMI3DEnvironmentLoader.Instance.onNodeTraversableSet += SetTraversable;
            UMI3DCollaborationClientServer.Instance.OnLeaving.AddListener(Reset);

            VolumePrimitiveManager.SubscribeToPrimitiveCreation(OnPrimitiveCreated, false);
            VolumePrimitiveManager.SubscribeToPrimitiveDelete(OnPrimitiveDeleted);
        }

        void OnDestroy()
        {
            if (UMI3DEnvironmentLoader.Exists)
            {
                UMI3DEnvironmentLoader.Instance.onNodePartOfNavmeshSet -= SetPartOfNavmesh;
            }
            if (UMI3DEnvironmentLoader.Exists)
            {
                UMI3DEnvironmentLoader.Instance.onNodeTraversableSet -= SetTraversable;
            }
            if (UMI3DCollaborationClientServer.Exists)
            {
                UMI3DCollaborationClientServer.Instance.OnLeaving.RemoveListener(Reset);
            }

            VolumePrimitiveManager.UnsubscribeToPrimitiveCreation(OnPrimitiveCreated);
            VolumePrimitiveManager.UnsubscribeToPrimitiveDelete(OnPrimitiveDeleted);
        }

        void Reset()
        {
            foreach (var primitive in cellIdToGameobjects)
            {
                if (primitive.Value != null) { Destroy(primitive.Value); }
            }
            cellIdToGameobjects.Clear();
        }

        void SetPartOfNavmesh(UMI3DNodeInstance node)
        {
            if (node.IsPartOfNavmesh)
            {
                if (node.GameObject.GetComponent<Collider>() != null)
                {
                    AddNavmeshArea(node.GameObject);
                }

                foreach (var renderer in node.renderers)
                {
                    if (renderer.TryGetComponent<Collider>(out Collider c))
                    {
                        AddNavmeshArea(renderer.gameObject);
                    }
                    else
                    {
                        Debug.LogWarning(renderer.gameObject.name + " is set as part of the navmesh but it does not have colliders");
                    }
                }
            }
            else if (node.IsTraversable)
            {
                SetTraversable(node);
            } else
            {
                RemoveNavmeshArea(node);
            }
        }

        protected abstract void AddNavmeshArea(GameObject nodeGameObject);

        protected abstract void RemoveNavmeshArea(UMI3DNodeInstance node);

        protected abstract void SetTraversable(UMI3DNodeInstance node);

        protected static int ToLayer(int bitmask)
        {
            int result = bitmask > 0 ? 0 : 31;
            while (bitmask > 1)
            {
                bitmask = bitmask >> 1;
                result++;
            }
            return result;
        }

        protected void SetLayer(UMI3DNodeInstance node, LayerMask mask)
        {
            if (node.GameObject.GetComponent<Collider>() != null)
            {
                node.GameObject.layer = ToLayer(mask);
            }

            foreach (var renderer in node.renderers)
            {
                renderer.gameObject.layer = ToLayer(mask);
            }
        }

        /// <summary>
        /// Creates an obstacle when a new primitive non traversable is created.
        /// </summary>
        /// <param name="primitive"></param>
        private void OnPrimitiveCreated(AbstractVolumeCell primitive)
        {
            if (primitive.isTraversable) { return; }

            GameObject go = new GameObject("Obstacle-Volume-" + primitive.Id());
            go.transform.SetParent((primitive as AbstractPrimitive)?.rootNode?.transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;

            switch (primitive)
            {
                case Box box:
                    BoxCollider boxCollider = go.AddComponent<BoxCollider>();
                    boxCollider.center = box.bounds.center;
                    boxCollider.size = box.bounds.size;
                    break;

                case Cylinder cylinder:
                    CapsuleCollider capsuleCollider = go.AddComponent<CapsuleCollider>();
                    capsuleCollider.height = cylinder.height;
                    capsuleCollider.radius = cylinder.radius;
                    break;

                default:
                    Destroy(go);
                    Debug.LogError("Primitive of type " + primitive?.GetType() + " not supported.");
                    break;
            }

            if (go != null)
            {
                SwitchHierarchyToObstacleLayer(go);
            }
        }
        protected abstract void SwitchHierarchyToObstacleLayer(GameObject go);

        /// <summary>
        /// If <paramref name="primitive"/> had a related obstacle, deletes it.
        /// </summary>
        /// <param name="primitive"></param>
        void OnPrimitiveDeleted(AbstractVolumeCell primitive)
        {
            if (cellIdToGameobjects.ContainsKey(primitive.Id()))
            {
                GameObject go = cellIdToGameobjects[primitive.Id()];
                if (go != null) { Destroy(go); }
                cellIdToGameobjects.Remove(primitive.Id());
            }
        }
    }
}