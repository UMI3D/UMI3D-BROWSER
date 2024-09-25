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

using inetum.unityUtils;
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
        /// Notification sent when a textfield in the web view has been selected.
        /// </summary>
        public const string WebViewTextFieldSelected = "GeckoWebViewNotificationKeysWebViewTextFieldSelected";

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
        /// Notification sent when the size of the web view change.
        /// </summary>
        public class WebViewSizeChanged
        {
            /// <summary>
            /// The new scale of the web view.<br/>
            /// Value is <see cref="Vector2"/>.
            /// </summary>
            public const string Scale = "Scale";
        }


        /// <summary>
        /// Notification sent when the size of the texture change.
        /// </summary>
        public class TextureSizeChanged
        {
            /// <summary>
            /// The new size of the texture.<br/>
            /// Value is <see cref="Vector2"/>.
            /// </summary>
            public const string Size = "Size";
        }


        /// <summary>
        /// Notification sent when the scroll value changed from the server.
        /// </summary>
        public class ScrollChanged
        {
            /// <summary>
            /// The scrolling value of the web view.<br/>
            /// Value is <see cref="Vector2"/>.
            /// </summary>
            public const string Scroll = "Scroll";
        }


        /// <summary>
        /// Notification sent when the interactibility will changed.
        /// </summary>
        public const string InteractibilityChanged = "GeckoWebViewNotificationKeysInteractibilityChanged";


        /// <summary>
        /// Notification sent when the synchronization change.
        /// </summary>
        public class SynchronizationChanged
        {
            /// <summary>
            /// Whether the web view is not synchronized with the server.<br/>
            /// Value is <see cref="bool"/>.
            /// </summary>
            public const string IsDesynchronized = "IsDesynchronized";

            /// <summary>
            /// Whether the web view is synchronizing (the user pressed the synchronized button).<br/>
            /// Value is <see cref="bool"/>.
            /// </summary>
            public const string IsSynchronizing = "IsSynchronizing";

            /// <summary>
            /// Whether the user is admin of the web view.<br/>
            /// Value is <see cref="bool"/>.
            /// </summary>
            public const string IsAdmin = "IsAdmin";

            /// <summary>
            /// The delta scrolling value of the web view.<br/>
            /// Value is <see cref="Vector2"/>.
            /// </summary>
            public const string Scroll = "Scroll";
        }


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
            /// Whether the web view is interactable.<br/>
            /// Value is <see cref="bool"/>.<br/>
            /// <br/>
            /// See Notification key: <see cref="InteractibilityChanged"/>
            /// </summary>
            public const string Interactable = "Interactable";

            
        }
    }
}