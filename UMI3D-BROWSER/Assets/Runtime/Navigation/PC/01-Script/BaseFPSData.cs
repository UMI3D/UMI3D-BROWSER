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
using umi3d.cdk.navigation;
using UnityEngine;

namespace umi3d.baseBrowser.Navigation
{
    [CreateAssetMenu(fileName = "FPSData", menuName = "UMI3D/FPS Data", order = 1)]
    public class BaseFPSData : ScriptableObject
    {
        [Header("Crouch")]
        [Tooltip("player height while crouching")]
        public float crouchYAxis = -.35f;
        [Tooltip("Time to switch between standing up and crouching (both ways)")]
        public float crouchSpeed = 0.2f;

        [Header("Input")]
        [Tooltip("Whether or not the player is crouching.")]
        public bool IsCrouching;
    }
}
