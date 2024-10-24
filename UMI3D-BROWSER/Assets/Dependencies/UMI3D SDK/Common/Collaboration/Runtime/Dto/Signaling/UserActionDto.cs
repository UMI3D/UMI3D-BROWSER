﻿/*
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

namespace umi3d.common.collaboration.dto.signaling
{
    [Serializable]
    public class UserActionDto
    {
        /// <summary>
        /// id
        /// </summary>
        public ulong id { get; set; }

        /// <summary>
        /// should this action be highlighted
        /// </summary>
        public bool isPrimary { get; set; }

        /// <summary>
        /// Name of the interaction
        /// </summary>
        public string name { get; set; } = null;

        /// <summary>
        /// Description of the interaction
        /// </summary>
        public string description { get; set; } = null;

        /// <summary>
        /// Path or url to a 2D icon 
        /// </summary>
        public ResourceDto icon2D { get; set; } = null;

        /// <summary>
        /// Path or url to a 3D icon 
        /// </summary>
        public ResourceDto icon3D { get; set; } = null;
    }
}