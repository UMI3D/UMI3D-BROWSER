﻿/*
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

namespace umi3d.common
{
    /// <summary>
    /// DTO to teleport users in relative position next to the leader user who start a teleportation group.
    /// </summary>
    public class TeleportGroupRequestDto : AbstractBrowserRequestDto
    {
        /// <summary>
        /// Target teleportation of the leader user.
        /// </summary>
        public Vector3Dto teleportLeaderPosition { get; set; }
        /// <summary>
        /// Old position of the leader user to calculate relative position.
        /// </summary>
        public Vector3Dto currentLeaderPosition { get; set; }
    }
};