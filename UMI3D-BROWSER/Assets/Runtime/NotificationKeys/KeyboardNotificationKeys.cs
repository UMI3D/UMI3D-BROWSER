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

namespace umi3d.browserRuntime.NotificationKeys
{
    public static class KeyboardNotificationKeys 
    {
        /// <summary>
        /// Notification sent when characters will be added.
        /// </summary>
        public const string AddCharacters = "AddCharacters";

        public static class AddCharactersInfo
        {
            /// <summary>
            /// The characters added. Can be <see cref="string"/> or <see cref="char"/>.
            /// </summary>
            public const string Characters = "Characters";
        }

        /// <summary>
        /// Notification sent when characters will be removed.
        /// </summary>
        public const string RemoveCharacters = "RemoveCharacters";

        public static class RemoveCharactersInfo
        {
            /// <summary>
            /// The deletion phase.
            /// 
            /// <list type="number">
            /// <item>Character by character, or the current selected text.</item>
            /// <item>Word / spaces by word / spaces.</item>
            /// </list>
            /// </summary>
            public const string DeletionPhase = "DeletionPhase";
        }

        /// <summary>
        /// Notification sent when the letter case will change.
        /// </summary>
        public const string LetterCase = "LetterCase";

        public static class LetterCaseInfo
        {
            /// <summary>
            /// Whether the letter case is lower case.
            /// </summary>
            public const string IsLowerCase = "IsLowerCase";
        }

        /// <summary>
        /// Notification sent when the keyboard mode will change (characters or symbols).
        /// </summary>
        public const string ABCOrSymbol = "ABCOrSymbol";

        public static class ABCOrSymbolInfo
        {
            /// <summary>
            /// Whether the characters mode is selected.
            /// </summary>
            public const string IsABC = "IsABC";
        }

        /// <summary>
        /// Notification sent when the version of the keyboard will change.
        /// </summary>
        public const string ChangeVersion = "ChangeVersion";

        public static class VersionInfo
        {
            /// <summary>
            /// The version of the keyboard (AZERTY, QWERTY, ...).<br/>
            /// This should be a string.
            /// </summary>
            public const string Version = "Version";
        }
    }
}