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

namespace umi3d.browserRuntime.navigation
{
    public abstract class UMI3DCamera : MonoBehaviour
    {
        protected CameraMovement movement;
        public Vector2 angularViewSpeed = Vector2.one * 5f;
        public Vector2 maxCameraAngle = Vector2.one * 90f;
        public Vector2 maxHeadXAngle = new(-60f, 70f);
        public float maxNeckXAngle = 50f;
        
        protected CameraProperties properties;

        protected abstract bool IsInitialized();

        public void HandleView()
        {
            if (!movement.CanCameraMove()) { return; }

            movement.ComputeRotationInput(angularViewSpeed);
            movement.ComputeCameraAndBodyRotation(maxCameraAngle, maxHeadXAngle, maxNeckXAngle);
            movement.MoveCameraAndBody();
        }

        void Update()
        {
            if (!IsInitialized()) { return; }

            HandleView();
        }
    }
}