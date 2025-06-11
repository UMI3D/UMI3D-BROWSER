/*
Copyright 2019 - 2025 Inetum

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
using umi3d.common;
using UnityEngine;

namespace umi3d.cdk.navigation
{
    public abstract class ContinuousMovement 
    {
        public abstract bool CanMove();

        public abstract void HandleInput(Vector2 speed, float runCoef);

        public abstract bool CheckForGroundHit(float RideHeight, out RaycastHit rayHit);
        public abstract bool IsGrounded(float rideHeight);

        public abstract void HandleGroundCollision(float rideHeight, float rideSpringStrength, float rideSpringDamper, RaycastHit rayHit);

        public abstract void HandleJump(float jumpForce, bool isGrounded);

        /// <summary>
        /// Apply continuous navigation request from server.
        /// </summary>
        /// <param name="data"></param>
        /// <seealso cref="Teleport(TeleportDto)"/>
        public abstract void Move(ulong environmentId, NavigateDto data);

        public abstract void Move();
    }
}