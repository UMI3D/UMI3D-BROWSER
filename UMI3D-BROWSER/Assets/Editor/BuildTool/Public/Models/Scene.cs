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
using System.Collections.Generic;
using umi3d.common.core.target;
using UnityEngine;

namespace umi3d.browserEditor.BuildTool
{
    [Serializable]
    public struct Scene 
    {
        /// <summary>
        /// Whether this scene can be add.
        /// </summary>
        public readonly bool enabled;
        /// <summary>
        /// The path where the scene is stored.
        /// </summary>
        public readonly string path;
        /// <summary>
        /// The list of release cycles this scene has to be applied to.
        /// </summary>
        public readonly IReadOnlyList<ReleaseCycle> releaseCycles;
        /// <summary>
        /// The list of platforms this scene has to be applied to.
        /// </summary>
        public readonly IReadOnlyList<Platform> platforms;

        public Scene(bool enabled, string path, ReleaseCycle[] releaseCycles, Platform[] platforms)
        {
            this.enabled = enabled;
            this.path = path;
            this.releaseCycles = releaseCycles;
            this.platforms = platforms;
        }
    }
}