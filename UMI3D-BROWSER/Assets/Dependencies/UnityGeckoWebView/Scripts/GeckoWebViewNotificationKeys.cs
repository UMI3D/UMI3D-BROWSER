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

namespace com.inetum.unitygeckowebview
{
    public static class GeckoWebViewNotificationKeys 
    {
        /// <summary>
        /// Notification sent when one of the history button is pressed.
        /// </summary>
        public const string History = "GeckoWebViewNotificationKeysHistory";

        /// <summary>
        /// Notification sent when a search will be initiated.
        /// </summary>
        public const string Search = "GeckoWebViewNotificationKeysSearch";

        /// <summary>
        /// Notification sent when the loading of a web page has been made.
        /// </summary>
        public const string Loading = "GeckoWebViewNotificationKeysLoading";

        /// <summary>
        /// Notification sent when a rendering process will be call.
        /// </summary>
        public const string Rendering = "GeckoWebViewNotificationKeysRendering";

        /// <summary>
        /// Notification sent when the size of the web view will change.
        /// </summary>
        public const string SizeChanged = "GeckoWebViewNotificationKeysSizeChanged";

        /// <summary>
        /// Notification sent when the size of the texture will change.
        /// </summary>
        public const string TextureSizeChanged = "GeckoWebViewNotificationKeysTextureSizeChanged";

        /// <summary>
        /// Notification sent when the scroll value has changed from the server.
        /// </summary>
        public const string ScrollChanged = "GeckoWebViewNotificationKeysScrollChanged";

        /// <summary>
        /// Notification sent when the interactibility will changed.
        /// </summary>
        public const string InteractibilityChanged = "GeckoWebViewNotificationKeysInteractibilityChanged";

        /// <summary>
        /// Notification sent when the synchronization has changed.
        /// </summary>
        public const string SynchronizationChanged = "GeckoWebViewNotificationKeysSynchronizationChanged";

        public static class Info
        {
            /// <summary>
            /// The type of history button pressed.<br/>
            /// Value is <see cref="com.inetum.unitygeckowebview.History"/>.<br/>
            /// <br/>
            /// See Notification key: <see cref="History"/>
            /// </summary>
            public const string BackwardOrForward = "BackwardOrForward";

            /// <summary>
            /// The url used for a web search.<br/>
            /// Value is <see cref="string"/>.<br/>
            /// <br/>
            /// See Notification key: <see cref="Search"/>, <see cref="Loading"/>
            /// </summary>
            public const string URL = "URL";

            /// <summary>
            /// The rendering process called.<br/>
            /// Value is <see cref="com.inetum.unitygeckowebview.Rendering"/>.<br/>
            /// <br/>
            /// See Notification key: <see cref="Rendering"/>
            /// </summary>
            public const string RenderingProcess = "RenderingProcess";

            /// <summary>
            /// A <see cref="Vector2"/>.<br/>
            /// Could be the new size or the scrolling value of the web view.<br/>
            /// Value is <see cref="Vector2"/>.<br/>
            /// <br/>
            /// See Notification key: <see cref="SizeChanged"/>, <see cref="TextureSizeChanged"/>, <see cref="ScrollChanged"/>
            /// </summary>
            public const string Vector2 = "Vector2";

            /// <summary>
            /// The position of the corner of the web view.<br/>
            /// Value is <see cref="Vector3"/>[].<br/>
            /// <br/>
            /// See Notification key: <see cref="SizeChanged"/>
            /// </summary>
            public const string CornersPosition = "CornerPositions";

            /// <summary>
            /// Whether the web view is interactable.<br/>
            /// Value is <see cref="bool"/>.<br/>
            /// <br/>
            /// See Notification key: <see cref="InteractibilityChanged"/>
            /// </summary>
            public const string Interactable = "Interactable";

            /// <summary>
            /// Whether the web view is not synchronized with the server.<br/>
            /// Value is <see cref="bool"/>.<br/>
            /// <br/>
            /// See Notification key: <see cref="SynchronizationChanged"/>
            /// </summary>
            public const string IsDesynchronized = "IsDesynchronized";

            /// <summary>
            /// Whether the web view is synchronizing (the user pressed the synchronized button).<br/>
            /// Value is <see cref="bool"/>.<br/>
            /// <br/>
            /// See Notification key: <see cref="SynchronizationChanged"/>
            /// </summary>
            public const string IsSynchronizing = "IsSynchronizing";

            /// <summary>
            /// Whether the user is admin of the web view.<br/>
            /// Value is <see cref="bool"/>.<br/>
            /// <br/>
            /// See Notification key: <see cref="SynchronizationChanged"/>
            /// </summary>
            public const string IsAdmin = "IsAdmin";
        }
    }
}