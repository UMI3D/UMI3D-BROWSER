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
using System.Collections.Generic;
using umi3d.browserRuntime.ui.popup;

namespace umi3d.browserRuntime.notificationKeys
{
    public struct PopupInfo 
    {
        /// <summary>
        /// ID of the popup.
        /// </summary>
        public System.Guid? id;

        /// <summary>
        /// Type of the popup.
        /// </summary>
        /// <remarks>
        /// REQUIRED.
        /// </remarks>
        public PopupType type;

        /// <summary>
        /// The title of the popup.
        /// </summary>
        /// <remarks>
        /// Value type : string <see cref="string"/> or localized string (<see cref="string"/> Table, <see cref="string"/> Key)
        /// </remarks>
        public System.Object title;

        /// <summary>
        /// The description of the popup.
        /// </summary>
        /// <remarks>
        /// Value type : string <see cref="string"/> or localized string (<see cref="string"/> Table, <see cref="string"/> Key)
        /// </remarks>
        public System.Object description;

        /// <summary>
        /// A list of buttons to show under the text of the popup.
        /// </summary>
        /// <remarks>
        /// Value type : List(<see cref="System.Object"/>) where object can be string <see cref="string"/> or localized string (<see cref="string"/> Table, <see cref="string"/> Key)
        /// </remarks>
        public List<System.Object> buttons;

        /// <summary>
        /// The action trigger when a button is clicked.
        /// </summary>
        /// <remarks>
        /// -1 means that the close button of the popup has been pressed.<br/>
        /// 0 means that the first button has been pressed, and so on.
        /// </remarks>
        public Action<int> buttonActions;

        /// <summary>
        /// Arguments used by localisation in <see cref="Title"/> and <see cref="Description"/>
        /// </summary>
        public Dictionary<string, System.Object> arguments;

        /// <summary>
        /// Whether the close button has to be hidden.
        /// </summary>
        public bool hideCloseButton;
    }
}