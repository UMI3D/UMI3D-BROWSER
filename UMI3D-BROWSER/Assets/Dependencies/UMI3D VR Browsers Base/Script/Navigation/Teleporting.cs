/*
Copyright 2019 - 2022 Inetum

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

using umi3dVRBrowsersBase.connection;
using UnityEngine;
using System.Collections.Generic;
using umi3d.cdk;
using umi3d.common;
using umi3d.browserRuntime.navigation;
using umi3d.cdk.collaboration;

namespace umi3dVRBrowsersBase.navigation
{
    /// <summary>
    /// Teleports users where they want. 
    /// </summary>
    public class Teleporting : MonoBehaviour
    {
        public ClientLBE.GuardianManager guardianManager; // Assurez-vous de référencer le GuardianManager


        /// <summary>
        /// Player object.
        /// </summary>
        public GameObject teleportingObject;

        /// <summary>
        /// Player camera object.
        /// </summary>
        public GameObject centerEyeAnchor;

        //public GameObject calibrateur;

        /// <summary>
        /// Teleportation preview.
        /// </summary>
        public TeleportArc arc;

        /// <summary>
        /// manage teleportation of group.
        /// </summary>
        public GroupTeleportation groupTeleportation = new GroupTeleportation();

        bool isLoadingScreenDisplayed = false;

        protected virtual void Awake()
        {
            //LoadingScreenDisplayer.OnLoadingScreenDislayed.AddListener(() => isLoadingScreenDisplayed = true);
            //LoadingScreenDisplayer.OnLoadingScreenHidden.AddListener(() => isLoadingScreenDisplayed = false);
        }


        bool IsLeaderInGroup()
        {
            if (guardianManager == null)
            {
                Debug.LogError("REMI GuardianManagerServer non assigné dans UMI3DLBEManager !");
                return false;
            }

            // Vérifie chaque groupe pour voir si leaderId correspond à AdminUserId
            if (guardianManager.lBEGroupDto != null)
            {
                var collaborationServer = UMI3DCollaborationClientServer.Instance; // Accès à l'instance
                if (collaborationServer != null)
                {
                    ulong userId = collaborationServer.GetUserId();

                    if (guardianManager.lBEGroupDto.AdminUserId == userId)
                        return true;             
                    else
                        return false;                
                }        
                return false;
                
            }
            else
            {
                // Aucun groupe trouvé où leaderId est AdminUserId
                Debug.LogWarning($"REMY : Aucun groupe trouvé où leaderId est AdminUserId.");
                return false;
            }         
        }

        // Individual or group teleportation based on the isGroupTeleport flag
        [ContextMenu("Teleport")]
        public void Teleport()
        {

            if (isLoadingScreenDisplayed) // A supprimer ?
            {
                return;
            }

            Debug.LogError("Commented when merged");

            Vector3? position = arc.GetPointedPoint();

            if (position.HasValue)
            {
                bool isLeader = IsLeaderInGroup();

                if (/*GroupTeleportation.isGroupTeleport*/isLeader == true) // -> controler si le user qui demande la téléportation est un leader ou non
                {
                    Debug.Log("REMY -> is leader = true");

                    // Capture la position initiale
                    groupTeleportation.OnTeleportStart(teleportingObject.transform);

                    // Effectue la téléportation
                    TeleportIndividual(position.Value);

                    // Capture la position finale
                    groupTeleportation.OnTeleportEnd(teleportingObject.transform, position.Value.y);
                }
                else
                {
                    Debug.Log("REMY -> is leader = false");

                    TeleportIndividual(position.Value);
                }
            }
        }

        // Function of individual teleportation
        private void TeleportIndividual(Vector3 position)
        {
            Vector3 offset = centerEyeAnchor.transform.position - teleportingObject.transform.position;
            teleportingObject.transform.position = new Vector3(position.x - offset.x, position.y, position.z - offset.z);
        }
    }
}
