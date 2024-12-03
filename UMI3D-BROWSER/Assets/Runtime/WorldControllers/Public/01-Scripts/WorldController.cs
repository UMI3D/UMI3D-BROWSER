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

using System;
using System.Collections.Generic;

namespace umi3d.browserRuntime.worldController
{
    public struct WorldController 
    {
        /// <summary>
        /// The url targeting the world controller.
        /// </summary>
        /// <remarks>
        /// This field uniquely identify a <see cref="WorldController"/>.
        /// </remarks>
        public string url;
        /// <summary>
        /// The name of the world controller. This field will be displayed under a thumbnail in the connection page.
        /// </summary>
        public string name;

        public bool isFavorite;

        public DateTime firstConnection;
        public DateTime lastConnection;

        public struct ComparerOnUrl : IEqualityComparer<WorldController>
        {
            public bool Equals(WorldController wc1, WorldController wc2)
            {
                return wc1.url == wc2.url;
            }

            public int GetHashCode(WorldController wc)
            {
                return wc.url.GetHashCode();
            }
        }
    }
}