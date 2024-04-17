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
using umi3d.cdk;
using umi3d.common;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.browserRuntime.navigation
{
    public class GroupTeleportation 
    {
        /// <summary>
        /// Flag for groupTeleportation
        /// </summary>
        public static bool isGroupTeleport = true; //TODO: Remettre à false une fois initialisé au lancement selon si arène en AR et déplacement de groupe activé A RETIRER AVANT MERGE DE LA BRANCHE SINON AR POUR TOUT LE MONDE

        /*/// <summary>
        /// Flag for using common guardian
        /// </summary>
        public static bool isUsingCommonGuardian = true;*/

        /// <summary>
        /// Function of group teleportation
        /// </summary>
        public void TeleportGroup(Vector3 newPosition, Transform transformPlayer, Transform transformCamera)
        {
            // Calculate the offset of the leader
            Vector3 leaderOffset = transformPlayer.rotation * transformCamera.localPosition;
            Vector3Dto teleportLeaderPosition = new Vector3Dto() { X = newPosition.x - leaderOffset.x, Y = newPosition.y, Z = newPosition.z - leaderOffset.z }; // Appliquez cet offset à la nouvelle position pour le leader

            // Position of leader before teleportation
            Vector3 currentLeaderPosition = transformPlayer.position;

            var tGroupRequest = new TeleportGroupRequestDto()
            {
                teleportLeaderPosition = teleportLeaderPosition,
                currentLeaderPosition = new Vector3Dto() { X = currentLeaderPosition.x, Y = currentLeaderPosition.y, Z = currentLeaderPosition.z }
            };

            UMI3DClientServer.SendRequest(tGroupRequest, true);

            // Move the leader to the new position
            transformPlayer.position = new Vector3(teleportLeaderPosition.X, teleportLeaderPosition.Y, teleportLeaderPosition.Z);
        }
    }
}