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
using System.Collections.Generic;

namespace umi3d.common
{
    /// <summary>
    /// DTO to describe a group of entities to load on a browser.
    /// </summary>
    /// An EntityRequest is sent when a user gets the whole enviornment while joining.
    public class EntityRequestDto : UMI3DDto
    {
        public ulong environmentId { get; set; }
        /// <summary>
        /// Entities to load id.
        /// </summary>
        public List<ulong> entitiesId { get; set; }
    }

    public class UserActionRequestDto : AbstractBrowserRequestDto
    {

        /// <summary>
        /// Entities to load id.
        /// </summary>
        public ulong actionId { get; set; }
    }
}