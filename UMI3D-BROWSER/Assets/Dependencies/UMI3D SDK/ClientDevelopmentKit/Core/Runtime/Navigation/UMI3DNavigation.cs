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

using umi3d.common;
using UnityEngine;

namespace umi3d.cdk.navigation
{
    /// <summary>
    /// Navigation manager for user displacement in the environment. 
    /// </summary>
    public abstract class UMI3DNavigation : MonoBehaviour
    {
        [Header("Continuous Movement")]
        [SerializeField] Vector2 speed = Vector2.one * 5;
        [SerializeField] float runCoef = 2;
        [Space]
        [SerializeField] float rideHeight = 1.5f; // Target height above the ground
        [SerializeField] float rideSpringStrength = 100.0f; // Spring stiffness
        [SerializeField] float rideSpringDamper = 10.0f; // Spring damping
        [Space]
        [SerializeField] float jumpForce = 30.0f; // Force applied during the jump

        /// <summary>
        /// Current navigation system.
        /// </summary>
        public static UMI3DNavigation currentNav = null;

        protected FrameController frameController;
        protected ContinuousMovement continuousMovement;

        protected void HandleContinuousMovement()
        {
            if (!continuousMovement.CanMove()) { return; }

            bool didHit = continuousMovement.CheckForGroundHit(rideHeight, out RaycastHit rayHit);
            if (didHit)
            {
                continuousMovement.HandleGroundCollision(rideHeight, rideSpringStrength, rideSpringDamper, rayHit);
            }
            continuousMovement.HandleJump(jumpForce, continuousMovement.IsGrounded(rideHeight));

            continuousMovement.HandleInput(speed, runCoef);
            continuousMovement.Move();
        }

        /// <summary>
        /// Disable this navigation system.
        /// </summary>
        public abstract void Disable();

        /// <summary>
        /// Activate this navigation system.
        /// </summary>
        public abstract void Activate();


        public static void SetFrame(ulong environmentId, FrameRequestDto frameRequest)
        {
            UnityEngine.Debug.LogError("Need to handle rescaling");

            currentNav.UpdateFrame(environmentId, frameRequest);

            var fConfirmation = new FrameConfirmationDto()
            {
                userId = UMI3DClientServer.Instance.GetUserId()
            };
            UMI3DClientServer.SendRequest(fConfirmation, true);
        }

        /// <summary>
        /// Apply FrameRequestDto request from server.
        /// </summary>
        /// <param name="data"></param>
        protected abstract void UpdateFrame(ulong environmentId, FrameRequestDto data);


        /// <summary>
        /// Move the user according to a <see cref="NavigateDto"/>.
        /// </summary>
        /// <param name="dto"></param>
        public static void Navigate(ulong environmentId, NavigateDto dto)
        {
            switch (dto)
            {
                case ViewpointTeleportDto viewpointTeleportDto:
                    currentNav.ViewpointTeleport(environmentId, viewpointTeleportDto);
                    break;

                case TeleportDto teleportDto:
                    currentNav.Teleport(environmentId, teleportDto);
                    break;

                default:
                    currentNav.MoveContinuously(environmentId, dto);
                    break;
            }
        }

        /// <summary>
        /// Apply continuous navigation request from server.
        /// </summary>
        /// <param name="data"></param>
        /// <seealso cref="Teleport(TeleportDto)"/>
        protected abstract void MoveContinuously(ulong environmentId, NavigateDto data);

        /// <summary>
        /// Apply teleport request from server.
        /// </summary>
        /// <param name="data"></param>
        /// <seealso cref="Navigate(NavigateDto)"/>
        protected abstract void Teleport(ulong environmentId, TeleportDto data);

        /// <summary>
        /// Apply viewpoint teleport request from server.
        /// </summary>
        /// <param name="data"></param>
        /// <seealso cref="Navigate(NavigateDto)"/>
        protected abstract void ViewpointTeleport(ulong environmentId, ViewpointTeleportDto data);

        /// <summary>
        /// Get data on current movements of the user.
        /// </summary>
        /// <returns></returns>
        public abstract NavigationData GetNavigationData();
    }
}