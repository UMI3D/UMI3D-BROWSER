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
    public static class LocomotionNotificationKeys 
    {
        /// <summary>
        /// Notification sent when a snap turn will be performed.
        /// </summary>
        public const string SnapTurn = "LocomotionNotificationKeysSnapTurn";

        public static class Info
        {
            /// <summary>
            /// The direction of the snap turn.<br/>
            /// Value is <see cref="int"/>: 0: turn around, 1: turn left, 2: turn right.<br/>
            /// <br/>
            /// See Notification key: <see cref="SnapTurn"/>
            /// </summary>
            public const string Direction = "LocomotionNotificationKeysDirection";

            /// <summary>
            /// The amount of degree that locomotion turn.<br/>
            /// Value is <see cref="float"/>.<br/>
            /// <br/>
            /// See Notification key: <see cref="SnapTurn"/>
            /// </summary>
            public const string TurnAmount = "LocomotionNotificationKeysTurnAmount";
        }
    }
}