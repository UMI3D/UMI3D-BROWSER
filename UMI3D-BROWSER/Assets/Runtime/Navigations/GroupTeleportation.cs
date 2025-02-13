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

using umi3d.cdk;
using umi3d.common;
using UnityEngine;

namespace umi3d.browserRuntime.navigation
{
    public class GroupTeleportation
    {
        /// <summary>
        /// Flag for groupTeleportation
        /// </summary>
        public static bool isGroupTeleport = true; //TODO: Remettre à false une fois initialisé au lancement selon si arène en AR et déplacement de groupe activé A RETIRER AVANT MERGE DE LA BRANCHE SINON AR POUR TOUT LE MONDE

        public static void OnGroupTeleport(Vector3 initialPlayerPosition, Vector3 finalPlayerPosition)
        {
            Vector3 teleportationVector = new Vector3(finalPlayerPosition.x - initialPlayerPosition.x, finalPlayerPosition.y, finalPlayerPosition.z - initialPlayerPosition.z);
            SendTeleportationVectorToServer(teleportationVector);
        }

        private static void SendTeleportationVectorToServer(Vector3 teleportationVector)
        {
            var tGroupRequest = new TeleportGroupRequestDto()
            {
                teleportationVector = new Vector3Dto()
                {
                    X = teleportationVector.x,
                    Y = teleportationVector.y,
                    Z = teleportationVector.z
                }
            };

            UMI3DClientServer.SendRequest(tGroupRequest, true);
        }
    }
}