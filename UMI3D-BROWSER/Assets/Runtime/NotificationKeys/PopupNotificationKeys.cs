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

namespace umi3d.browserRuntime.ui.popup
{
    public static class PopupNotificationKeys
    {
        /// <summary>
        /// Show a popup. See <see cref="Popup" to/>. Need at least a Type.
        /// </summary>
        public class Show
        {
            /// <summary>
            /// Type of the popup.
            /// </summary>
            /// <remarks>
            /// Value type : <see cref="Type"/>
            /// </remarks>
            public const string Type = "popup-type";
            /// <summary>
            /// The localized key for the title of the popup
            /// </summary>
            /// <remarks>
            /// Value type : <see cref="string"/>
            /// </remarks>
            public const string Title = "popup-title";
            /// <summary>
            /// The localized key for the description of the popup
            /// </summary>
            /// <remarks>
            /// Value type : <see cref="string"/>
            /// </remarks>
            public const string Description = "popup-description";
            /// <summary>
            /// A list of buttons to show under the text of the popup
            /// </summary>
            /// <remarks>
            /// Value type : List(<see cref="string"/> "text", <see cref="System.Action"/> "onClick")
            /// </remarks>
            public const string Buttons = "popup-buttons";
            /// <summary>
            /// Arguments used by localisation in <see cref="Title"/> and <see cref="Description"/>
            /// </summary>
            /// <remarks>
            /// Value type : Dictionary(<see cref="string"/> "key", <see cref="object"/> "value")
            /// </remarks>
            public const string Arguments = "popup-arguments";
        }

        /// <summary>
        /// Close all opened popup;
        /// </summary>
        public class CloseAll { }
    }
}