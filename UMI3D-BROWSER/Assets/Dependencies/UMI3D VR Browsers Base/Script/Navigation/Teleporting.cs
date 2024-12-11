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

        public GameObject calibrateur;

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
            Debug.Log("REMY : Teleporting script has been initialized.");
        }

        protected virtual void Start()
        {
            Debug.Log("REMY : Teleporting script is running.");
        }

        // Individual or group teleportation based on the isGroupTeleport flag
        [ContextMenu("Teleport")]
        public void Teleport()
        {
            Debug.Log("REMY : Go Teleport.");

            if (isLoadingScreenDisplayed)
            {
                Debug.Log("Teleporting.Teleport.isLoadingScreenDisplayed=false");
                return;
            }

            Debug.LogError("Commented when merged");

            Vector3? position = arc.GetPointedPoint();

            Debug.Log("REMY Position teleport Arc -> " + position.Value);


            if (position.HasValue)
            {
                if (GroupTeleportation.isGroupTeleport)
                {
                    Debug.Log("REMY : Teleporting.Teleport.isGroupTeleport=true");
                    // Capture la position initiale
                    groupTeleportation.OnTeleportStart(teleportingObject.transform);

                    // Effectue la téléportation
                    TeleportIndividual(position.Value);

                    // Capture la position finale
                    groupTeleportation.OnTeleportEnd(teleportingObject.transform, position.Value.y);
                }
                else
                {
                    Debug.Log("REMY : Teleporting.Teleport.isGroupTeleport=false");
                    TeleportIndividual(position.Value);
                }
            }
        }

        // Function of individual teleportation
        private void TeleportIndividual(Vector3 position)
        {
            Debug.Log("REMY : Teleport Individual + position Y -> " + position.y);

            Vector3 offset = centerEyeAnchor.transform.position - teleportingObject.transform.position;

            teleportingObject.transform.position = new Vector3(position.x - offset.x, position.y, position.z - offset.z);
        }
    }
}
