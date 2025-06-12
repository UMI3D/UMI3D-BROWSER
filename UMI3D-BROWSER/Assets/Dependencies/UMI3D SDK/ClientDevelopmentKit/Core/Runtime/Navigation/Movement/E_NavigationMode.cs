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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace umi3d.cdk.navigation
{
    public enum E_NavigationMode
    {
        /// <summary>
        /// Moving like a PC FPS or TPS.
        /// </summary>
        Continuous,
        /// <summary>
        /// Moving like a VR FPS
        /// </summary>
        Teleportation,
        /// <summary>
        /// Moving like in the editor scene view but with some constraints.
        /// </summary>
        Fly,
        /// <summary>
        /// No collision, can fly.
        /// </summary>
        Debug
    }
}
