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

namespace umi3d.browserRuntime.notificationKeys
{
    public static class DialogueBoxNotificationKey
    {
        /// <summary>
        /// Display a new pop up.
        /// </summary>
        public const string NewDialogueBox = "NewDialogueBox";

        /// <summary>
        /// Information containing in a <see cref="NewDialogueBox"/> notification.
        /// </summary>
        public static class NewDialogueBoxInfo
        {
            /// <summary>
            /// Type of the pop up.
            /// </summary>
            public enum PopUpType
            {
                Info,
                Warning,
                Error
            }

            /// <summary>
            /// Type of the pop up.
            /// </summary>
            public const string Type = "Type";
            /// <summary>
            /// Title of the pop up.
            /// </summary>
            public const string Title = "Title";
            /// <summary>
            /// Message or description of the pop up.
            /// </summary>
            public const string Message = "Message";
            /// <summary>
            /// The buttons' text and callback (string, Action)[] of the pop up.
            /// </summary>
            public const string Buttons = "Buttons";
            /// <summary>
            /// Arguments for localisation purpose of the pop up.
            /// </summary>
            public const string Arguments = "Arguments";
        }

        /// <summary>
        /// Close all the opened pop up.
        /// </summary>
        public const string CloseAllPopUp = "CloseAllPopUp";

        /// <summary>
        /// A pop up will be opened.
        /// </summary>
        public const string PopUpOpen = "PopUpOpen";

        /// <summary>
        /// The pop ups will be closed.
        /// </summary>
        public const string PopUpClose = "PopUpClose";
    }
}
