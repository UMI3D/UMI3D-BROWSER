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
using UnityEngine;

namespace umi3d.cdk.navigation
{
    public abstract class UMI3DCamera : MonoBehaviour
    {
        public E_CameraMode cameraMode;
        [Space]
        [SerializeField] protected float horizontalSensibility = 20f;
        [SerializeField] protected float verticalSensibility = 20f;
        [Space]
        [SerializeField] protected Vector2 maxCameraAngle = Vector2.one * 90f;
        [SerializeField] protected Vector2 maxHeadXAngle = new(-60f, 70f);
        [SerializeField] protected float maxNeckXAngle = 50f;
        
        protected CameraMovement movement;
        protected CameraProperties properties;

        public new Camera camera { get; protected set; }

        protected abstract bool IsInitialized();

        protected void HandleView()
        {
            if (!movement.CanMove()) { return; }

            movement.HandleInput(new Vector2(verticalSensibility, horizontalSensibility));
            movement.Move(maxCameraAngle, maxHeadXAngle, maxNeckXAngle);
        }
    }
}