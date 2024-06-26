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

namespace umi3d.browserRuntime.player
{
    public static class PlayerTransformUtils 
    {
        /// <summary>
        /// Center the <paramref name="camera"/>.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="camera"></param>
        public static void CenterCamera(Transform offset, Transform camera)
        {
            // Get the height of the camera.
            float height = camera.localPosition.y;

            // Center the camera.
            MoveAndRotateOffsetToCenterObject(offset, camera);

            // Reapply the camera height.
            offset.localPosition = new()
            {
                x = offset.localPosition.x,
                y = offset.localPosition.y + height,
                z = offset.localPosition.z
            };
        }

        /// <summary>
        /// Move the <paramref name="player"/> by placing the <paramref name="camera"/> at the position <paramref name="p"/>.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="camera"></param>
        /// <param name="p"></param>
        public static void MovePlayerAndCenterCamera(Transform player, Transform camera, Vector3 p)
        {
            // Move player to position.
            player.localPosition = p;

            // Get the offset of the camera compared to the player.
            Vector3 positionOffset = player.GetPositionOffset(camera);

            // Move the player to center the camera at the position p.
            player.localPosition -= new Vector3()
            {
                x = positionOffset.x,
                y = 0,
                z = positionOffset.z
            };
        }

        /// <summary>
        /// Move and rotate <paramref name="offsetToMove"/> so that <paramref name="objectToCenter"/> is center in position and rotation compared to <paramref name="offsetToMove"/>'s parent.
        /// </summary>
        /// <param name="offsetToMove"></param>
        /// <param name="objectToCenter"></param>
        public static void MoveAndRotateOffsetToCenterObject(Transform offsetToMove, Transform objectToCenter)
        {
            // === Move the offsetToMove. ===

            // Reset the local position of offsetToMove.
            offsetToMove.localPosition = Vector3.zero;

            // Get the local position of objectToCenter
            Vector3 localPosition = offsetToMove.GetPositionOffset(objectToCenter);

            // Move offsetToMove so that the world position of objectToCenter is equal to offsetToMove's parent position.
            offsetToMove.localPosition -= localPosition;

            // === Rotate the offsetToMove. ===

            // Reset the local rotation of offsetToMove.
            offsetToMove.localRotation = Quaternion.identity;

            // Get the world position of objectToCenter.
            Vector3 worldPosition = objectToCenter.transform.position;

            // Get the rotation of objectToCenter
            Quaternion localRotation = offsetToMove.GetRotationOffset(objectToCenter);

            // Rotate offsetToMove so that the rotation of the objectToCenter is identity.
            offsetToMove.RotateAround(worldPosition, Vector3.up, -localRotation.eulerAngles.y);
        }

        /// <summary>
        /// Turn the <paramref name="player"/> around <paramref name="camera"/> by <paramref name="angle"/> degrees.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="camera"></param>
        /// <param name="angle"></param>
        public static void SnapTurn(Transform player, Transform camera, float angle)
        {
            player.RotateAround(camera.position, Vector3.up, angle);
        }

        /// <summary>
        /// Get the position offset from <paramref name="source"/> to <paramref name="objectWithOffset"/>.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="objectWithOffset"></param>
        /// <returns></returns>
        public static Vector3 GetPositionOffset(this Transform source, Transform objectWithOffset)
        {
            return objectWithOffset.position - source.position;
        }

        /// <summary>
        /// Get the rotation offset from <paramref name="source"/> to <paramref name="objectWithOffset"/>.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="objectWithOffset"></param>
        /// <returns></returns>
        public static Quaternion GetRotationOffset(this Transform source, Transform objectWithOffset)
        {
            return Quaternion.Inverse(source.rotation) * objectWithOffset.rotation;
        }
    }
}