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
        /// Event raised when the slider value will change.
        /// </summary>
        public class SliderValueSet
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
        /// Event raised when the visibility of the slider buttons will change.
        /// </summary>
        public class SliderButtonVisibilityWillChange
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
        /// Event raised when the properties of the grid will change.
        /// </summary>
        public class GridPropertiesWillChange
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

        /// <summary>
        /// Event raised when a thumbnail has been added.
        /// </summary>
        public class Added
        {
            /// <summary>
            /// The thumbnail.
            /// </summary>
            /// <remarks>
            /// Value is <see cref="ThumbnailModel"/>.
            /// </remarks>
            public const string Thumbnail = "Thumbnail";
        }

        /// <summary>
        /// Event raised when a thumbnail has been deleted.
        /// </summary>
        public class Deleted
        {
            /// <summary>
            /// The thumbnail.
            /// </summary>
            /// <remarks>
            /// Value is <see cref="ThumbnailModel"/>.
            /// </remarks>
            public const string Thumbnail = "Thumbnail";
        }

        #region Thumbnail

        /// <summary>
        /// Event raised when a thumbnail will be selected.
        /// </summary>
        public class Select
        {
        }

        /// <summary>
        /// Event raised when the sprite of a thumbnail will change.
        /// </summary>
        public class ImageSpriteWillChange
        {
            /// <summary>
            /// The sprite.
            /// </summary>
            /// <remarks>
            /// Value is <see cref="Sprite"/>.
            /// </remarks>
            public const string Sprite = "Sprite";
        }

        /// <summary>
        /// Event raised when the favorite status of a thumbnail has been modify by the view.
        /// </summary>
        public class FavoriteStatusUpdated
        {
            /// <summary>
            /// The favorite status.
            /// </summary>
            /// <remarks>
            /// Value is <see cref="bool"/>.
            /// </remarks>
            public const string IsFavorite = "IsFavorite";
        }

        /// <summary>
        /// Event raised when the favorite status of a thumbnail has been modify by the model and the view need to change.
        /// </summary>
        public class FavoriteStatusSet
        {
            /// <summary>
            /// The favorite status.
            /// </summary>
            /// <remarks>
            /// Value is <see cref="bool"/>.
            /// </remarks>
            public const string IsFavorite = "IsFavorite";
        }

        /// <summary>
        /// Event raised when the sub inputs visibility of a thumbnail will change.
        /// </summary>
        public class SubInputsVisibilityWillChange
        {
            /// <summary>
            /// Whether the sub inputs are visible.
            /// </summary>
            /// <remarks>
            /// Value is <see cref="bool"/>.
            /// </remarks>
            public const string IsVisible = "IsVisible";
        }

        /// <summary>
        /// Event raised when the name of the thumbnail is set by the model.
        /// </summary>
        public class NameSet
        {
            /// <summary>
            /// The name of the thumbnail.
            /// </summary>
            /// <remarks>
            /// Value is <see cref="string"/>.
            /// </remarks>
            public const string Name = "Name";
        }

        /// <summary>
        /// Event raised when the name of the thumbnail is updated by the view.
        /// </summary>
        public class NameUpdated
        {
            /// <summary>
            /// The name of the thumbnail.
            /// </summary>
            /// <remarks>
            /// Value is <see cref="string"/>.
            /// </remarks>
            public const string Name = "Name";
        }

        #endregion
    }
}