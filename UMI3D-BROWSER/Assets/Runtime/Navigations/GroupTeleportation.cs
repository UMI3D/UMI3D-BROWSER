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
        /// Flag pour la téléportation de groupe
        /// </summary>
        public static bool isGroupTeleport = true; //TODO: Remettre à false une fois initialisé au lancement selon si arène en AR et déplacement de groupe activé A RETIRER AVANT MERGE DE LA BRANCHE SINON AR POUR TOUT LE MONDE

        /*/// <summary>
        /// Flag pour l'utilisation d'un guardian commun
        /// </summary>
        public static bool isUsingCommonGuardian = true;*/

        /// <summary>
        /// Méthode de téléportation de groupe avec chaque user qui utilise un guardian différent
        /// </summary>
        public void TeleportGroup(Vector3 newPosition, Transform transformPlayer, Transform transformCamera)
        {
            Debug.Log("GroupTeleportation.TeleportGroup.begin");
            // Calculez d'abord l'offset pour le joueur principal (leader)
            Vector3 leaderOffset = transformPlayer.rotation * transformCamera.localPosition;
            Vector3Dto teleportLeaderPosition = new Vector3Dto() { X = newPosition.x - leaderOffset.x, Y = newPosition.y, Z = newPosition.z - leaderOffset.z }; // Appliquez cet offset à la nouvelle position pour le leader

            // Position actuelle du leader avant la téléportation
            Vector3 currentLeaderPosition = transformPlayer.position;

            var tGroupRequest = new TeleportGroupRequestDto()
            {
                userId = UMI3DClientServer.Instance.GetUserId(),
                teleportLeaderPosition = teleportLeaderPosition,
                currentLeaderPosition = new Vector3Dto() { X = currentLeaderPosition.x, Y = currentLeaderPosition.y, Z = currentLeaderPosition.z }
            };

            Debug.Log("GroupTeleportation.TeleportGroup.readyToSend");

            UMI3DClientServer.SendRequest(tGroupRequest, true);

            Debug.Log("GroupTeleportation.TeleportGroup.Sended");

            // Déplacez le leader à la nouvelle position
            transformPlayer.position = new Vector3(teleportLeaderPosition.X, teleportLeaderPosition.Y, teleportLeaderPosition.Z);
        }

        /*/// <summary>
        /// Méthode de téléportation de groupe avec un guardian commun entre les users
        /// </summary>
        public void TeleportGroupCommonGuardian(Vector3 newPosition, Transform transformPlayer, Transform transformCamera)
        {
            // Calculez d'abord l'offset pour le joueur principal (leader)
            Vector3 leaderOffset = transformPlayer.rotation * transformCamera.localPosition;
            //newPosition -= leaderOffset; // Appliquez cet offset à la nouvelle position pour le leader

            // Position actuelle du leader avant la téléportation
            Vector3 currentLeaderPosition = transformPlayer.position;

            var tGroupRequest = new TeleportGroupRequestDto()
            {
                userId = UMI3DClientServer.Instance.GetUserId(),
                teleportLeaderPosition = new Vector3Dto() { X = newPosition.x, Y = newPosition.y, Z = newPosition.z },
                currentLeaderPosition = new Vector3Dto() { X = currentLeaderPosition.x, Y = currentLeaderPosition.y, Z = currentLeaderPosition.z }
                //ajouter param dans dto pour dire si oui ou non on a un guardian commun
            };

            UMI3DClientServer.SendRequest(tGroupRequest, true);

            // Déplacez le leader à la nouvelle position
            transformPlayer.position = new Vector3(newPosition.x - leaderOffset.x,
                                                                   newPosition.y,
                                                                   newPosition.z - leaderOffset.z);
        }*/
    }
}