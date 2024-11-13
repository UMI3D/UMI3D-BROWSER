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
        /// Add popup to queue.<br/>
        /// <br/>
        /// If the queue is empty then display this popup.
        /// </summary>
        public class EnqueuePopup
        {
            /// <summary>
            /// ID of the popup.
            /// </summary>
            /// <remarks>
            /// Value type : <see cref="System.Guid"/>
            /// </remarks>
            public const string ID = "ID";

            /// <summary>
            /// Type of the popup. REQUIRED
            /// </summary>
            /// <remarks>
            /// Value type : <see cref="PopupType"/>
            /// </remarks>
            public const string Type = "Type";

            /// <summary>
            /// The title of the popup.
            /// </summary>
            /// <remarks>
            /// Value type : string <see cref="string"/> or localized string (<see cref="string"/> Table, <see cref="string"/> Key)
            /// </remarks>
            public const string Title = "Title";

            /// <summary>
            /// The description of the popup.
            /// </summary>
            /// <remarks>
            /// Value type : string <see cref="string"/> or localized string (<see cref="string"/> Table, <see cref="string"/> Key)
            /// </remarks>
            public const string Description = "Description";

            /// <summary>
            /// A list of buttons to show under the text of the popup.
            /// </summary>
            /// <remarks>
            /// Value type : List(<see cref="System.Object"/>) where object can be string <see cref="string"/> or localized string (<see cref="string"/> Table, <see cref="string"/> Key)
            /// </remarks>
            public const string Buttons = "Buttons";

            /// <summary>
            /// The action trigger when a button is clicked.
            /// </summary>
            /// <remarks>
            /// Value type : <see cref="System.Action"/>(<see cref="int"/>)<br/>
            /// -1 means that the close button of the popup has been pressed.<br/>
            /// 0 means that the first button has been pressed, and so on.
            /// </remarks>
            public const string ButtonActions = "ButtonActions";

            /// <summary>
            /// Arguments used by localisation in <see cref="Title"/> and <see cref="Description"/>
            /// </summary>
            /// <remarks>
            /// Value type : Dictionary(<see cref="string"/> "key", <see cref="object"/> "value")
            /// </remarks>
            public const string Arguments = "Arguments";

            /// <summary>
            /// Whether the close button has to be hidden.
            /// </summary>
            /// <remarks>
            /// Value type : <see cref="bool"/>
            /// </remarks>
            public const string HideCloseButton = "HideCloseButton";
        }

        /// <summary>
        /// Notification when a popup has closed.
        /// </summary>
        public class PopupClosed { }

        /// <summary>
        /// Event raised when all the pop up have been closed.
        /// </summary>
        public class AllPopupAreClosed { }

        /// <summary>
        /// Close all opened popup;
        /// </summary>
        public class CloseAll { }
    }
}