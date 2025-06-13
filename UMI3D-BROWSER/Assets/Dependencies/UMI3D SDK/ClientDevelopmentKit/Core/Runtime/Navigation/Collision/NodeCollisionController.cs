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

using inetum.unityUtils.observation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace umi3d.cdk.navigation
{
    public class NodeCollisionController : IEnumerable<Collider>
    {
        bool _isNavmesh = false;
        /// <summary>
        /// Is this node part of the navmesh ?
        /// </summary>
        public bool isNavmesh
        {
            get => _isNavmesh;

            set
            {
                if (_isNavmesh != value)
                {
                    _isNavmesh = value;
                    notifier.Notify();
                }
            }
        }

        bool _isTraversable = true;
        /// <summary>
        /// Is this node traversable ?
        /// </summary>
        public bool isTraversable
        {
            get => _isTraversable;

            set
            {
                if (_isTraversable != value)
                {
                    _isTraversable = value;
                    notifier.Notify();
                }
            }
        }

        List<Collider> colliders = new();

        Notifier notifier;

        public NodeCollisionController()
        {
            notifier = NotificationHub.Default.GetNotifier(
                this,
                ID.FromType<CollisionNotificationKeys.NodeCollisionModeChanged>()
            );
        }

        public IEnumerator<Collider> GetEnumerator()
        {
            return colliders.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return colliders.GetEnumerator();
        }

        public bool Any(Func<Collider, bool> predicate)
        {
            return colliders.Any(predicate);
        }

        public void Add(Collider collider)
        {
            if (!colliders.Contains(collider)) { return; }
            colliders.Add(collider);
        }

        public void Clear()
        {
            colliders.Clear();
        }
    }
}