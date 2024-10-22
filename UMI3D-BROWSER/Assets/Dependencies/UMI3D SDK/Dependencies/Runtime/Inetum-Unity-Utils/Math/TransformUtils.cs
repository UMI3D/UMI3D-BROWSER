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
using UnityEngine;

namespace inetum.unityUtils.math
{
    public static class TransformUtils 
    {
        /// <summary>
        /// Translate <paramref name="parent"/> so that <paramref name="child"/> ends up at <paramref name="worldPosition"/>.<br/>
        /// <br/>
        /// <remarks>WARNING: <paramref name="parent"/> has to be a parent but not necessary the parent of <paramref name="child"/>.</remarks>
        /// </summary>
        /// <param name="parent">The object to move.</param>
        /// <param name="child">The object to center.</param>
        /// <param name="worldPosition">The world position where child will end up.</param>
        /// <param name="filter"></param>
        public static void TranslateParentToCenterChild(
            this Transform parent, 
            Transform child, 
            Vector3 worldPosition, 
            Vector3? filter = null
        )
        {
            // Get the local position of 'child' with respect to 'parent'.
            Vector3 localPosition_otc = parent.GetRelativeTranslationOfAToB(child);

            Vector3 delta = worldPosition - parent.position;

            if (filter.HasValue)
            {
                localPosition_otc = Vector3.Scale(localPosition_otc, filter.Value);

                delta = Vector3.Scale(delta, filter.Value);
            }

            // Move 'parent' so that the world position of 'child' is equal to 'worldPosition'.
            parent.localPosition -= delta + localPosition_otc;
        }

        /// <summary>
        /// Rotate <paramref name="offsetToMove"/> so that <paramref name="objectToCenter"/> is center in rotation compared to <paramref name="offsetToMove"/>'s parent.
        /// </summary>
        /// <param name="offsetToMove"></param>
        /// <param name="objectToCenter"></param>
        public static void RotateOffsetToCenterObject(this Transform offsetToMove, Transform objectToCenter, Vector3? filter = null)
        {
            // Get the local rotation of objectToCenter with respect to offsetToMove.
            Quaternion localRotation_otc = offsetToMove.GetRelativeRotationOfAToB(objectToCenter);

            Quaternion localRotation_otm = offsetToMove.localRotation;

            if (filter.HasValue)
            {
                Vector3 euler_otc = localRotation_otc.eulerAngles;
                localRotation_otc.eulerAngles = Vector3.Scale(euler_otc, filter.Value);

                Vector3 euler_otm = localRotation_otm.eulerAngles;
                localRotation_otm.eulerAngles = Vector3.Scale(euler_otm, filter.Value);
            }

            // Rotate offsetToMove so that the world rotation of the objectToCenter is equal to offsetToMove's parent rotation.
            offsetToMove.localRotation *= Quaternion.Inverse(localRotation_otm) * Quaternion.Inverse(localRotation_otc);
        }

        /// <summary>
        /// Translate and rotate <paramref name="offsetToMove"/> so that <paramref name="objectToCenter"/> is center in position and rotation compared to <paramref name="offsetToMove"/>'s parent.
        /// </summary>
        /// <param name="offsetToMove"></param>
        /// <param name="objectToCenter"></param>
        public static void TranslateAndRotateOffsetToCenterObject(
            this Transform offsetToMove, 
            Transform objectToCenter, 
            Vector3 worldPosition, 
            Quaternion worldRotation, 
            Vector3? translationFilter = null, 
            Vector3? rotationFilter = null
        )
        {
            // === Rotate the offsetToMove. ===
            offsetToMove.RotateOffsetToCenterObject(objectToCenter, rotationFilter);

            // === Move the offsetToMove. ===
            offsetToMove.TranslateParentToCenterChild(objectToCenter, worldPosition, translationFilter);
        }
    }
}