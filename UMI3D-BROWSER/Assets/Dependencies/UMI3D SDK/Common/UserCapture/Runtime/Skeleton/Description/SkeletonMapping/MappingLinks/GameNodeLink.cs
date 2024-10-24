﻿/*
Copyright 2019 - 2023 Inetum

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

using UnityEngine;

namespace umi3d.common.userCapture.description
{
    /// <summary>
    /// Link based on the position and rotation of a Unity gameobject.
    /// </summary>
    public class GameNodeLink : ISkeletonMappingLink
    {
        /// <summary>
        /// Game node from which to generate the link
        /// </summary>
        public Transform transform;

        private bool IsDestroyed;

        public GameNodeLink(Transform transform)
        {
            this.transform = transform;
        }

        /// <inheritdoc/>
        public virtual (Vector3 position, Quaternion rotation) Compute()
        {
            if (IsDestroyed)
                return (Vector3.zero, Quaternion.identity);

            return (transform.position, transform.rotation);
        }

        public void MarkAsDrestroyed()
        {
            IsDestroyed = true;
        }
    }
}