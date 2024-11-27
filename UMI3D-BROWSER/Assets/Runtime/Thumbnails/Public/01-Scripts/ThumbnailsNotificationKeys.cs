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
using UnityEngine;

namespace umi3d.browserRuntime.ui.thumbnails
{
    public static class ThumbnailsNotificationKeys 
    {
        /// <summary>
        /// Event raised when the content mode of the thumbnails changed.
        /// </summary>
        public class ContentModeChanged
        {
            /// <summary>
            /// The new content mode.
            /// </summary>
            /// <remarks>
            /// Value is <see cref="ThumbnailContentMode"/>.
            /// </remarks>
            public const string ContentMode = "ContentMode";
        }

        /// <summary>
        /// Event raised when the slider value will changed.
        /// </summary>
        public class SliderValueWillChanged
        {
            /// <summary>
            /// The new value.
            /// </summary>
            /// <remarks>
            /// Value is <see cref="float"/>.
            /// </remarks>
            public const string Value = "Value";
        }

        /// <summary>
        /// Event raised when the visibility of the slider buttons will changed.
        /// </summary>
        public class SliderButtonVisibilityChanged
        {
            /// <summary>
            /// Whether the slider buttons are visible.
            /// </summary>
            /// <remarks>
            /// Value is <see cref="bool"/>.
            /// </remarks>
            public const string IsVisible = "IsVisible";
        }

        /// <summary>
        /// Event raised when the properties of the grid will changed.
        /// </summary>
        public class GridPropertiesWillChanged
        {
            /// <summary>
            /// The size of the grid.
            /// </summary>
            /// <remarks>
            /// Value is <see cref="Vector2"/>.
            /// </remarks>
            public const string Size = "Size";

            /// <summary>
            /// The row count of the grid.
            /// </summary>
            /// <remarks>
            /// Value is <see cref="int"/>.
            /// </remarks>
            public const string RowCount = "RowCount";

            /// <summary>
            /// The spacing of the grid.
            /// </summary>
            /// <remarks>
            /// Value is <see cref="Vector2"/>.
            /// </remarks>
            public const string Spacing = "Spacing";
        }
    }
}