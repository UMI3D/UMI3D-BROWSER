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

using inetum.unityUtils.math;
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
            // Filter the translation to keep the height of the camera.
            Vector3 translationFilter = new Vector3(1f, 0f, 1f);

            // Filter the rotation to avoid rotate the x and z axis.
            Vector3 rotationFilter = Vector3.up;

            Transform parent = offset.transform.parent;

            // Center the camera.
            offset.TranslateAndRotateParentToCenterChild(
                camera,
                parent.position,
                parent.rotation,
                translationFilter,
                rotationFilter
            );
        }

        /// <summary>
        /// Translate the <paramref name="player"/> by placing the <paramref name="camera"/> at the position <paramref name="p"/>.<br/>
        /// <br/>
        /// At the end of this method the camera x and z coordinates will be respectively px and pz. The y position of the camera will not change.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="camera"></param>
        /// <param name="p"></param>
        public static void TranslatePlayerAndCenterCamera(Transform player, Transform camera, Vector3 p)
        {
            // Move player to position to set the camera y coordinate.
            player.localPosition = p;

            // Center the camera at p but not the y coordinate.
            player.TranslateParentToCenterChild(camera, p, new Vector3(1f, 0f, 1f));
        }

        /// <summary>
        /// Rotate the <paramref name="player"/> by placing the <paramref name="camera"/> at the rotation <paramref name="r"/>.<br/>
        /// <br/>
        /// At the end of this method the camera y rotation will be py. The x and z rotations of the camera will not change.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="camera"></param>
        /// <param name="r"></param>
        public static void RotatePlayerAndCenterCamera(Transform player, Transform camera, Quaternion r)
        {
            // Rotate player to r.
            player.rotation = r;

            player.RotateParentToCenterChild(camera, r, Vector3.up);
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
    }
}