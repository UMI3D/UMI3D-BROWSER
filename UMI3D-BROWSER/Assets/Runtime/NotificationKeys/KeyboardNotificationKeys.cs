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
        /// Notification sent when a text field has been selected.
        /// </summary>
        public const string TextFieldSelected = "KeyboardNotificationKeysTextFieldSelected";

        /// <summary>
        /// Notification sent when a special key is pressed.
        /// </summary>
        public const string SpecialKeyPressed = "KeyboardNotificationKeysSpecialKeyPressed";

        /// <summary>
        /// Notification sent when an object ask for the focus of the preview bar.
        /// </summary>
        public const string AskPreviewFocus = "KeyboardAskPreviewFocus";

        /// <summary>
        /// Notification sent when characters will be added or removed.
        /// </summary>
        public const string AddOrRemoveCharacters = "KeyboardAddOrRemoveCharacters";

        /// <summary>
        /// Notification sent when the keyboard mode (abc / symbol / lower case / upper case) will change.
        /// </summary>
        public const string ChangeMode = "KeyboardChangeMode";

        /// <summary>
        /// Notification sent when the version of the keyboard will change.
        /// </summary>
        public const string ChangeVersion = "KeyboardChangeVersion";

        /// <summary>
        /// Notification sent when the key is hovered.
        /// </summary>
        public const string KeyHovered = "KeyboardKeyHovered";

        /// <summary>
        /// Notification sent when the key is clicked.
        /// </summary>
        public const string KeyClicked = "KeyboardKeyClicked";

        /// <summary>
        /// Notification sent when the keyboard will be opened or closed.
        /// </summary>
        public const string OpenOrClose = "KeyboardOpenOrClose";

        /// <summary>
        /// Notification sent when the animations settings will change.
        /// </summary>
        public const string AnimationSettings = "KeyboardAnimationSettings";

        public static class Info
        {
            /// <summary>
            /// Whether the selection of the input field active it.<br/>
            /// Value is <see cref="bool"/>.<br/>
            /// <br/>
            /// See Notification key: <see cref="TextFieldSelected"/>
            /// </summary>
            public const string IsActivation = "IsActivation";

            /// <summary>
            /// Whether the selection is the preview bar.<br/>
            /// Value is <see cref="bool"/>.<br/>
            /// <br/>
            /// See Notification key: <see cref="TextFieldSelected"/>
            /// </summary>
            public const string IsPreviewBar = "IsPreviewBar";

            /// <summary>
            /// The position of the caret if value is int or the selection if value is (int, int).<br/>
            /// Value is <see cref="int"/> or (<see cref="int"/>, <see cref="int"/>).<br/>
            /// <br/>
            /// See Notification key: <see cref="TextFieldSelected"/>
            /// </summary>
            public const string SelectionPositions = "SelectionPositions";

            /// <summary>
            /// The text of the input field.<br/>
            /// Value is <see cref="string"/>.<br/>
            /// <br/>
            /// See Notification key: <see cref="TextFieldSelected"/>
            /// </summary>
            public const string InputFieldText = "InputFieldText";

            /// <summary>
            /// The special key pressed.<br/>
            /// Value is <see cref="umi3d.browserRuntime.NotificationKeys.SpecialKey"/>.<br/>
            /// <br/>
            /// See Notification key: <see cref="SpecialKeyPressed"/>
            /// </summary>
            public const string SpecialKey = "SpecialKey";

            /// <summary>
            /// How the text of a text field will be updated.<br/>
            /// Value is <see cref="umi3d.browserRuntime.NotificationKeys.TextFieldTextUpdate"/>.<br/>
            /// <br/>
            /// See Notification key: <see cref="AddOrRemoveCharacters"/>
            /// </summary>
            public const string TextFieldTextUpdate = "TextFieldTextUpdate";

            /// <summary>
            /// The characters added.<br/>
            /// Value is <see cref="string"/> or <see cref="char"/>.<br/>
            /// <br/>
            /// See Notification key: <see cref="AddOrRemoveCharacters"/>
            /// </summary>
            public const string Characters = "Characters";

            /// <summary>
            /// The deletion phase.<br/>
            /// Value is <see cref="int"/>.
            /// 
            /// <list type="number">
            /// <item>Character by character, or the current selected text.</item>
            /// <item>Word / spaces by word / spaces.</item>
            /// </list>
            /// 
            /// See Notification key: <see cref="AddOrRemoveCharacters"/>
            /// </summary>
            public const string DeletionPhase = "DeletionPhase";

            /// <summary>
            /// Whether the letter case is lower case.<br/>
            /// Value is <see cref="bool"/>. <br/>
            /// <br/>
            /// See Notification key: <see cref="ChangeMode"/>
            /// </summary>
            public const string IsLowerCase = "IsLowerCase";

            /// <summary>
            /// Whether the upper case is locked.<br/>
            /// Value is <see cref="bool"/>.<br/>
            /// <br/>
            /// See Notification key: <see cref="ChangeMode"/>
            /// </summary>
            public const string IsUpperCaseLocked = "IsUpperCaseLocked";

            /// <summary>
            /// Whether the characters mode is selected.<br/>
            /// Value is <see cref="bool"/>.<br/>
            /// <br/>
            /// See Notification key: <see cref="ChangeMode"/>
            /// </summary>
            public const string IsABC = "IsABC";

            /// <summary>
            /// The version of the keyboard (AZERTY, QWERTY, ...).<br/>
            /// Value is <see cref="string"/>.<br/>
            /// <br/>
            /// See Notification key: <see cref="ChangeVersion"/>
            /// </summary>
            public const string Version = "Version";

            /// <summary>
            /// The pointer event data related of the interaction.<br/>
            /// Value is <see cref="PointerEventData"/>.<br/>
            /// <br/>
            /// See Notification key: <see cref="KeyHovered"/>
            /// </summary>
            public const string PointerEventData = "PointerEventData";

            /// <summary>
            /// Whether the opening or closing is animated.<br/>
            /// Value is <see cref="bool"/>.<br/>
            /// <br/>
            /// See Notification key: <see cref="OpenOrClose"/><br/>
            /// See Notification key: <see cref="AnimationSettings"/><br/>
            /// </summary>
            public const string WithAnimation = "WithAnimation";

            /// <summary>
            /// The animation time.<br/>
            /// Value is <see cref="float"/>.<br/>
            /// <br/>
            /// See Notification key: <see cref="OpenOrClose"/>
            /// </summary>
            public const string AnimationTime = "AnimationTime";

            /// <summary>
            /// Whether the keyboard is opening or closing.<br/>
            /// Value is <see cref="bool"/>.<br/>
            /// <br/>
            /// See Notification key: <see cref="OpenOrClose"/>
            /// </summary>
            public const string IsOpening = "IsOpening";

            /// <summary>
            /// The percentage [0,1] of the <see cref="AnimationTime"/> when the phase 1 start.<br/>
            /// Value is <see cref="float"/>.<br/>
            /// <br/>
            /// See Notification key: <see cref="OpenOrClose"/>
            /// </summary>
            public const string PhaseOneStartTimePercentage = "PhaseOneStartTimePercentage";

            /// <summary>
            /// The type of animation.<br/>
            /// Value is <see cref="NotificationKeys.KeyboardAnimationType"/>.<br/>
            /// <br/>
            /// See Notification key: <see cref="AnimationSettings"/><br/>
            /// </summary>
            public const string AnimationType = "AnimationType";
        }
    }
}