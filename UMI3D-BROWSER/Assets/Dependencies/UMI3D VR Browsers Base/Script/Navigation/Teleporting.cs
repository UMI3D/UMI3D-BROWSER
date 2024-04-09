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

namespace umi3dVRBrowsersBase.navigation
{
    /// <summary>
    /// Teleports users where they want. 
    /// </summary>
    public class Teleporting : MonoBehaviour
    {
        /// <summary>
        /// Player object.
        /// </summary>
        public GameObject teleportingObject;

        /// <summary>
        /// Player camera object.
        /// </summary>
        public GameObject centerEyeAnchor;

        /// <summary>
        /// Teleportation preview.
        /// </summary>
        public TeleportArc arc;

        /// <summary>
        /// manage teleportation of group.
        /// </summary>
        public GroupTeleportation groupTeleportation = new GroupTeleportation();

        //public List<GameObject> otherPlayers; // Liste des autres joueurs pour la téléportation de groupe //////// a recup coté serveur

        bool isLoadingScreenDisplayed = false;

        protected virtual void Awake()
        {
            LoadingScreenDisplayer.OnLoadingScreenDislayed.AddListener(() => isLoadingScreenDisplayed = true);
            LoadingScreenDisplayer.OnLoadingScreenHidden.AddListener(() => isLoadingScreenDisplayed = false);
        }

        // Téléportation individuelle ou de groupe basée sur le flag isGroupTeleport
        [ContextMenu("Teleport")]
        public void Teleport()
        {
            if (isLoadingScreenDisplayed)
            {
                Debug.Log("Teleporting.Teleport.isLoadingScreenDisplayed=false");
                return;
            }

            Vector3? position = arc.GetPointedPoint();
            if (position.HasValue)
            {
                if (GroupTeleportation.isGroupTeleport)
                {
                    Debug.Log("Teleporting.Teleport.isGroupTeleport=true");
                    groupTeleportation.TeleportGroup(position.Value, teleportingObject.transform, centerEyeAnchor.transform);
                }
                else
                {
                    Debug.Log("Teleporting.Teleport.isGroupTeleport=false");
                    TeleportIndividual(position.Value);
                }
            }
        }

        // Méthode de téléportation individuelle
        private void TeleportIndividual(Vector3 position)
        {
            Vector3 offset = teleportingObject.transform.rotation * centerEyeAnchor.transform.localPosition;
            teleportingObject.transform.position = new Vector3(position.x - offset.x,
                                                                   position.y,
                                                                   position.z - offset.z);
        }

        

    }
}
