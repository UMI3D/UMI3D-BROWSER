/*
Copyright 2019 - 2021 Inetum

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
using System.Collections.Generic;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.cdk.interaction
{
    /// <summary>
    /// Abstract class for UMI3D Player.
    /// </summary>
    public abstract class AbstractInteractionMapper : MonoBehaviour
    {
        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static AbstractInteractionMapper Instance;

        /// <summary>
        /// The Interaction Controllers.
        /// Should be input devices (or groups of input devices) connectors.
        /// </summary>
        [SerializeField, Tooltip("The Interaction Controllers.\nShould be input devices (or groups of input devices) connectors")]
        protected List<AbstractController> Controllers = new List<AbstractController>();

        /// <summary>
        /// If true, when a tool with holdable events is projected, InteractionMapper will
        /// ask to selected AbstractController to project this event on a specific input if
        /// it can.
        /// </summary>
        [Tooltip("If true, when a tool with holdable events is projected, " +
            "InteractionMapper will ask to selected AbstractController " +
            "to project this event on a specific input if it can")]
        public bool shouldProjectHoldableEventOnSpecificInput = false;


        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }


        /// <summary>
        /// Reset the InteractionMapper module.
        /// </summary>
        public abstract void ResetModule();



        /// <summary>
        /// Check if an interaction with the given id exists.
        /// </summary>
        public abstract bool InteractionExists(ulong environmentId, ulong id);

        /// <summary>
        /// Get the interaction with the given id (if any).
        /// </summary>
        public abstract AbstractInteractionDto GetInteraction(ulong environmentId, ulong id);

        /// <summary>
        /// Return the interactions matching a given condition.
        /// </summary>
        public abstract IEnumerable<AbstractInteractionDto> GetInteractions( Predicate<AbstractInteractionDto> condition);

        /// <summary>
        /// Return all known interactions.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<AbstractInteractionDto> GetInteractions() { return GetInteractions(t => true); }

        /// <summary>
        /// Get the controller onto a given tool has been projected.
        /// </summary>
        /// <param name="projectedToolId">Tool's id</param>
        /// <returns></returns>
        public abstract AbstractController GetController(ulong environmentId, ulong projectedToolId);
    }
}
