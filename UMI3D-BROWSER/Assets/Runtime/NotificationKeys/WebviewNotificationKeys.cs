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
    public static class WebviewNotificationKeys 
    {
        /// <summary>
        /// Notification sent when a search is in progress.
        /// </summary>
        public const string Search = "WebviewNotificationKeysSearch";

        public static class Info
        {
            /// <summary>
            /// The url used for a web search.<br/>
            /// Value is <see cref="string"/>.<br/>
            /// <br/>
            /// See Notification key: <see cref="Search"/>
            /// </summary>
            public const string URL = "url";
        }
    }
}