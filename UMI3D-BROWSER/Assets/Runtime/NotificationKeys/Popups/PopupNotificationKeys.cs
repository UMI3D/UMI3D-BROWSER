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
            /// The popup info.
            /// </summary>
            /// <remarks>
            /// Value type : <see cref="PopupInfo"/>
            /// </remarks>
            public const string PopupInfo = "PopupInfo";
        }

        /// <summary>
        /// Dequeue a popup.
        /// </summary>
        /// <remarks>
        /// If the popup that will be removed is currently displayed then close the popup.
        /// </remarks>
        public class DequeuePopup
        {
            /// <summary>
            /// ID of the popup that will be removed.
            /// </summary>
            /// <remarks>
            /// Value type : <see cref="System.Guid"/>
            /// </remarks>
            public const string ID = "ID";

            /// <summary>
            /// The index of the buttonsAction.<br/>
            /// <br/>
            /// In the case where this popup is currently displaying use this index to trigger the buttons action.
            /// </summary>
            /// <remarks>
            /// Value type : nullable <see cref="int"/>
            /// </remarks>
            public const string ActionIndex = "ActionIndex";
        }

        /// <summary>
        /// Notification sent when a popup has closed.
        /// </summary>
        public class PopupClosed 
        {
            /// <summary>
            /// ID of the popup that has been closed.
            /// </summary>
            /// <remarks>
            /// Value type : nullable <see cref="System.Guid"/>
            /// </remarks>
            public const string ID = "ID";
        }

        /// <summary>
        /// Event raised when all the pop up have been closed.
        /// </summary>
        public class AllPopupAreClosed { }

        /// <summary>
        /// Close the current opened popup;
        /// </summary>
        public class CloseCurrentOpenedPopup 
        {
            /// <summary>
            /// The index of the buttonsAction.
            /// </summary>
            /// <remarks>
            /// Value type : nullable <see cref="int"/>
            /// </remarks>
            public const string ActionIndex = "ActionIndex";
        }

        /// <summary>
        /// Replace current opened popup.<br/>
        /// <br/>
        /// To replace a popup you first need to <see cref="EnqueuePopup"/> the new popup.
        /// </summary>
        /// <remarks>
        /// If the replacement popup is already the current popup then nothing happen.
        /// </remarks>
        public class ReplaceCurrentOpenedPopup 
        {
            /// <summary>
            /// ID of the popup that will replace the current popup.
            /// </summary>
            /// <remarks>
            /// Value type : <see cref="System.Guid"/>
            /// </remarks>
            public const string ID = "ID";

            /// <summary>
            /// The index of the buttonsAction.<br/>
            /// <br/>
            /// Use this index to trigger the buttons action of the current popup that will be replaced.
            /// </summary>
            /// <remarks>
            /// Value type : nullable <see cref="int"/>
            /// </remarks>
            public const string ActionIndex = "ActionIndex";
        }

        /// <summary>
        /// Display a popup.
        /// </summary>
        /// <remarks>
        /// Do not use that directly. Use <see cref="EnqueuePopup"/>.
        /// </remarks>
        public class DisplayPopup
        {
            /// <summary>
            /// The popup info.
            /// </summary>
            /// <remarks>
            /// Value type : <see cref="PopupInfo"/>
            /// </remarks>
            public const string PopupInfo = "PopupInfo";
        }
    }
}