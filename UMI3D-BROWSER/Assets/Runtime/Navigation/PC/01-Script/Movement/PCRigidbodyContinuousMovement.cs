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
            jumpInputAction.action.started += JumpStarted; ;
            jumpInputAction.action.canceled += JumpCanceled; ;

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
            isJumping = true;
        }

        void JumpCanceled(InputAction.CallbackContext obj)
        {
            isJumping = false;
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

        public override void HandleGroundCollision(float RideHeight, float RideSpringStrength, float RideSpringDamper)
        {
            Vector3 DownDir = Vector3.down;

            bool _rayDidHit = UnityEngine.Physics.Raycast(
                personalSkeletonContainer.position, // Starting position of the rayn
                personalSkeletonContainer.TransformDirection(DownDir), // Direction of the ray
                out RaycastHit _rayHit, // Result of the Raycast
                RideHeight + 1.0f // Maximum distance of the Raycast
            );

            if (_rayDidHit)
            {
                // Get the current velocity of the Rigidbody
                Vector3 vel = rigidbody.velocity;

                // Direction of the ray (downward, transformed into local space)
                Vector3 rayDir = personalSkeletonContainer.TransformDirection(DownDir);

                // Calculate the velocity component in the direction of the ray
                float rayDirVel = Vector3.Dot(rayDir, vel);

                // Calculate the spring compression (distance between the player and the ground)
                float x = _rayHit.distance - RideHeight;

                // Calculate the spring force
                float springForce = (x * RideSpringStrength) - (rayDirVel * RideSpringDamper);

                rigidbody.AddForce(rayDir * springForce);
            }
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