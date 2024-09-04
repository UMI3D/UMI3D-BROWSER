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
    public class ArcGuardian : Guardian
    {
        [Tooltip("The percentage [0, 1] of a cercle that will be consider the safe zone.")]
        public float percentage;

        public override bool DoesNotContain(Transform transform)
        {
            // Length of the arc.
            float arcLength = 360f * percentage;
            // Half of the length of the arcLength.
            float halfArcLength = arcLength / 2f;

            // The minimum rotation of 'transform' allowed.
            Vector3 min = this.transform.rotation.eulerAngles - halfArcLength * Vector3.one;
            // The maximum rotation of 'transform' allowed.
            Vector3 max = this.transform.rotation.eulerAngles + halfArcLength * Vector3.one;
            for (int i = 0; i < 3; i++)
            {
                // Restrict the angle to [-360, 360].
                min[i] %= 360f;
                // Restrict the angle to [0, 360]
                if (min[i] < 0)
                {
                    min[i] += 360f;
                }
                // Restrict the angle to [0, 360]. Max angle is always > 0.
                max[i] %= 360f;
            }

            Vector3 rotation = transform.rotation.eulerAngles;

            for (int i = 0; i < 3; i++)
            {
                if (!rotation[i].IsBetween(min[i], max[i]))
                {
                    return true;
                }
            }

            return false;
        }

#if UNITY_EDITOR

        [Header("Debug")]
        [SerializeReference] bool debug = true;
        [SerializeField] float debugRadius = 1f;
        [SerializeField] Color debugColorXZ = Color.green;
        [SerializeField] Color debugColorYZ = Color.green;

        void OnDrawGizmos()
        {
            if (!debug)
            {
                return;
            }

            Vector3 direction = RotationUtils.RotationToDirection(transform.rotation);
            GizmosDrawer.DrawWireArc(
                transform.position,
                direction,
                new(1f, 0f, 1f),
                percentage,
                debugRadius,
                debugColorXZ
            );
            GizmosDrawer.DrawWireArc(
                transform.position,
                direction,
                new(0f, 1f, 1f),
                percentage,
                debugRadius,
                debugColorYZ
            );
        }

#endif
    }
}