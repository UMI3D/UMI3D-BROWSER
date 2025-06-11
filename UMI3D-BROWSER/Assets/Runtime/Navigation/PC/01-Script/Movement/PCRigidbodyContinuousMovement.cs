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
using umi3d.cdk.navigation;
using umi3d.common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace umi3d.browserRuntime.navigation.pc
{
    public class PCRigidbodyContinuousMovement : ContinuousMovement
    {
        InputActionReference movementInputAction;
        InputActionReference runInputAction;
        InputActionReference jumpInputAction;
        Rigidbody rigidbody;
        Transform personalSkeletonContainer;

        Vector3 globalMovement;
        bool isRunning = false;

        Vector3 groundPosition;
        bool wantJumping = false;
        bool isJumping = false;

        public PCRigidbodyContinuousMovement(InputActionReference movementInputAction, Rigidbody rigidbody, Transform personalSkeletonContainer, InputActionReference runInputAction, InputActionReference jumpInputAction)
        {
            this.movementInputAction = movementInputAction;
            movementInputAction.action.Enable();
            this.runInputAction = runInputAction;
            runInputAction.action.Enable();
            runInputAction.action.started += RunStarted;
            runInputAction.action.canceled += RunCanceled;
            jumpInputAction.action.Enable();
            jumpInputAction.action.started += JumpStarted;
            jumpInputAction.action.canceled += JumpCanceled;

            this.rigidbody = rigidbody;
            this.personalSkeletonContainer = personalSkeletonContainer;
        }

        void RunStarted(InputAction.CallbackContext obj)
        {
            isRunning = true;
        }

        void RunCanceled(InputAction.CallbackContext obj)
        {
            isRunning = false;
        }

        void JumpStarted(InputAction.CallbackContext obj)
        {
            wantJumping = true;
        }

        void JumpCanceled(InputAction.CallbackContext obj)
        {
            wantJumping = false;
        }

        public override bool CanMove()
        {
            return true;
        }

        public override void HandleInput(Vector2 speed, float runCoef)
        {
            Vector2 movementInput = movementInputAction.action.ReadValue<Vector2>();
            movementInput = new Vector2(movementInput.x * speed.x, movementInput.y * speed.y) * (isRunning ? runCoef : 1);

            Vector3 localMovement = new Vector3(
                movementInput.x, 
                0f, 
                movementInput.y
            );
            globalMovement = personalSkeletonContainer.TransformDirection(localMovement);
        }

        Vector3 DownDir = Vector3.down;
        public override bool CheckForGroundHit(float RideHeight, out RaycastHit rayHit)
        {
            bool didHit = UnityEngine.Physics.Raycast(
                personalSkeletonContainer.position, // Starting position of the rayn
                personalSkeletonContainer.TransformDirection(DownDir), // Direction of the ray
                out rayHit, // Result of the Raycast
                RideHeight + 1.0f // Maximum distance of the Raycast
            );

            if (didHit)
            {
                groundPosition = rayHit.point;
            }

            return didHit;
        }

        public override bool IsGrounded(float rideHeight)
        {
            return personalSkeletonContainer.position.y > groundPosition.y + rideHeight - 0.2f
                && personalSkeletonContainer.position.y < groundPosition.y + rideHeight;
        }

        public override void HandleGroundCollision(float rideHeight, float rideSpringStrength, float rideSpringDamper, RaycastHit rayHit)
        {
            if (isJumping) { return; }

            // Get the current velocity of the Rigidbody
            Vector3 vel = rigidbody.velocity;

            // Direction of the ray (downward, transformed into local space)
            Vector3 rayDir = personalSkeletonContainer.TransformDirection(DownDir);

            // Calculate the velocity component in the direction of the ray
            float rayDirVel = Vector3.Dot(rayDir, vel);

            // Calculate the spring compression (distance between the player and the ground)
            float x = rayHit.distance - rideHeight;

            // Calculate the spring force
            float springForce = (x * rideSpringStrength) - (rayDirVel * rideSpringDamper);

            rigidbody.AddForce(rayDir * springForce);
        }

        public override void HandleJump(float jumpForce, bool isGrounded)
        {
            if (isGrounded && !isJumping && wantJumping)
            {
                StartJump(jumpForce);
            }
            else if (isGrounded && isJumping )
            {
                StopJump();
            }
        }

        void StartJump(float jumpForce)
        {
            rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            isJumping = true;
        }

        void StopJump()
        {
            rigidbody.velocity = Vector3.zero;
            isJumping = false;
        }

        public override void Move(ulong environmentId, NavigateDto data)
        {
            throw new NotImplementedException();
        }

        public override void Move()
        {
            Vector3 velocity = new Vector3(
                globalMovement.x,
                rigidbody.velocity.y,
                globalMovement.z
            );

            rigidbody.velocity = velocity;
        }
    }
}