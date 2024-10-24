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

using inetum.unityUtils.debug;
using inetum.unityUtils.math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace umi3d.browserRuntime.UX
{
    public class SphericalGuardian : Guardian
    {
        [Tooltip("Radius of the sphere (the safe zone).")]
        public float radius;

        public override bool DoesNotContain(Transform transform)
        {
            return !transform.position.IsInsideSphere(this.transform.position, radius);
        }

#if UNITY_EDITOR

        [Header("Debug")]
        [SerializeReference] bool debug = true;
        [SerializeField] Color debugColor = Color.cyan;

        void OnDrawGizmos()
        {
            if (!debug)
            {
                return;
            }

            GizmosDrawer.DrawWireSphere(transform.position, radius, debugColor);
        }

#endif
    }
}