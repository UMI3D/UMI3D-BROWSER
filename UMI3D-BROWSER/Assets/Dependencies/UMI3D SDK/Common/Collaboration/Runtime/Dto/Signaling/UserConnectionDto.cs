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
using umi3d.common.interaction;

namespace umi3d.common.collaboration.dto.signaling
{
    /// <summary>
    /// DTO describing user requirements for a connection.
    /// </summary>
    /// Typically sent by a server during the connection process.
    [Serializable]
    public class UserConnectionDto : UserDto
    {
        /// <summary>
        /// Connection form as a set of parameters.
        /// </summary>
        public ConnectionFormDto parameters { get; set; }
        public interaction.form.ConnectionFormDto divForm { get; set; }

        /// <summary>
        /// State if the libraries have been updated
        /// </summary>
        public bool librariesUpdated { get; set; } = false;

        /// <summary>
        /// Password to use to connect to the vocal server.
        /// </summary>
        public string audioPassword { get; set; }
    }
}