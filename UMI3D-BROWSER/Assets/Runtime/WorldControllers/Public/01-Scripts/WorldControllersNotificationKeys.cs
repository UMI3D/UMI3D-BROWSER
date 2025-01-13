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

namespace umi3d.browserRuntime.worldController
{
    public static class WorldControllersNotificationKeys 
    {
        /// <summary>
        /// Event raised when a world controller has been added.
        /// </summary>
        public class Added
        {
            /// <summary>
            /// The world controller that has been added.
            /// </summary>
            /// <remarks>
            /// Value is <see cref="WorldController"/>.
            /// </remarks>
            public const string WorldController = "WorldController";
        }

        /// <summary>
        /// Event raised when a world controller has been updated.
        /// </summary>
        public class Updated
        {
            /// <summary>
            /// The world controller that has been updated.
            /// </summary>
            /// <remarks>
            /// Value is <see cref="WorldController"/>.
            /// </remarks>
            public const string WorldController = "WorldController";
        }

        /// <summary>
        /// Event raised when a world controller has been removed.
        /// </summary>
        public class Removed
        {
            /// <summary>
            /// The world controller that has been removed.
            /// </summary>
            /// <remarks>
            /// Value is <see cref="WorldController"/>.
            /// </remarks>
            public const string WorldController = "WorldController";
        }
    }
}