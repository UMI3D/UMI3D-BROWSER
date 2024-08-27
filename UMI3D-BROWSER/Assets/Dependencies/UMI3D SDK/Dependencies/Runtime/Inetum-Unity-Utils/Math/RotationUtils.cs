﻿/*
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

using UnityEngine;

namespace inetum.unityUtils.math
{
    public static class RotationUtils
    {
        /// <summary>
        /// Project an angle in degrees to [-180°;180°[ range.
        /// </summary>
        /// <param name="angle">Angle in degrees</param>
        /// <returns></returns>
        public static float ProjectAngleIn180Range(float angle)
        {
            return (angle % 360f) < 180f ? angle % 360f : ((angle % 360f) - 360f);
        }

        /// <summary>
        /// Project angles of a rotation in degrees to [-180°;180°[ range.
        /// </summary>
        /// <param name="angle">Angle in degrees</param>
        /// <returns></returns>
        public static Vector3 ProjectAngleIn180Range(Vector3 eulerAngles)
        {
            float xAngle = ProjectAngleIn180Range(eulerAngles.x);
            float yAngle = ProjectAngleIn180Range(eulerAngles.y);
            float zAngle = ProjectAngleIn180Range(eulerAngles.z);

            return new Vector3(xAngle, yAngle, zAngle);
        }

        public static void ToYawPitchRoll(Quaternion rotation, out float yawAngle, out float pitchAngle, out float rollAngle)
        {
            rotation.ToAngleAxis(out rollAngle, out Vector3 zAxis);
            yawAngle = Vector3.Angle(Vector3.forward, Vector3.ProjectOnPlane(zAxis, Vector3.up));
            pitchAngle = Vector3.Angle(zAxis, Vector3.ProjectOnPlane(zAxis, Vector3.up));
        }

        /// <summary>
        /// Transform a rotation to a direction.
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static Vector3 RotationToDirection(Quaternion rotation)
        {
            return RotationToDirection(rotation.eulerAngles);
        }

        /// <summary>
        /// Transform a rotation to a direction.
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static Vector3 RotationToDirection(Vector3 rotation)
        {
            // Convert the rotation angles to radians
            float angleRadX = rotation.x * Mathf.Deg2Rad;
            float angleRadY = rotation.y * Mathf.Deg2Rad;
            float angleRadZ = rotation.z * Mathf.Deg2Rad;

            // Calculate the X, Y, and Z components of the direction
            float dirX = Mathf.Sin(angleRadY) * Mathf.Cos(angleRadX);
            float dirY = Mathf.Sin(angleRadX);
            float dirZ = Mathf.Cos(angleRadY) * Mathf.Cos(angleRadX);

            // Normalize the direction
            Vector3 direction = new Vector3(dirX, dirY, dirZ).normalized;

            return direction;
        }

        /// <summary>
        /// Whether <paramref name="angle"/> is between (strictly) <paramref name="min"/> and <paramref name="max"/>.
        /// </summary>
        /// <param name="angle">Angle in degree.</param>
        /// <param name="min">Angle in degree.</param>
        /// <param name="max">Angle in degree.</param>
        /// <returns></returns>
        public static bool IsBetween(this float angle, float min, float max)
        {
            ZeroTo360(ref min);
            ZeroTo360(ref max);
            ZeroTo360(ref angle);

            if (min < max)
            {
                return min < angle && angle < max;
            }
            else
            {
                return min < angle || angle < max;
            }
        }

        /// <summary>
        /// Restrict <paramref name="angle"/> to [0, 360].
        /// </summary>
        /// <param name="angle">The angle in degree.</param>
        public static void ZeroTo360(ref float angle)
        {
            if (angle > 360f)
            {
                // Restrict the angle to [0, 360].
                angle %= 360f;
            }
            else if (angle < 0)
            {
                // Restrict the angle to [-360, 360].
                angle %= 360f;
                // Restrict the angle to [0, 360].
                angle += 360f;
            }
        }
    }
}
