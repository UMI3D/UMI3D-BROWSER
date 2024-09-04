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

        /// <summary>
        /// Notification sent when a teleportation will be performed.
        /// </summary>
        public const string Teleportation = "LocomotionNotificationKeysTeleportation";

        /// <summary>
        /// Notification sent when the locomotion system will change.
        /// </summary>
        public const string System = "LocomotionNotificationKeysSystem";

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

            /// <summary>
            /// The controller responsible of the locomotion.<br/>
            /// Value is <see cref="umi3d.browserRuntime.NotificationKeys.Controller"/>.<br/>
            /// <br/>
            /// See Notification key: <see cref="Teleportation"/><br/>
            /// See Notification key: <see cref="System"/>
            /// </summary>
            public const string Controller = "LocomotionNotificationKeysController";

            /// <summary>
            /// The phase of the input responsible of the locomotion.<br/>
            /// <br/>
            /// <list type="bullet">
            /// <item>InputActionPhase.Started: Initiate the teleportation - Active the teleporting arc.</item>
            /// <item>InputActionPhase.Performed: Retrieve the targeted position - Get the targeted area and hide the teleporting arc.</item>
            /// <item>InputActionPhase.Waiting: Perform the teleportation - Teleport the target at a given position.</item>
            /// <item>InputActionPhase.Canceled: Cancel the teleportation - Hide the teleporting arc and do not move.</item>
            /// </list>
            /// Value is <see cref="InputActionPhase"/>.<br/>
            /// <br/>
            /// See Notification key: <see cref="Teleportation"/>
            /// </summary>
            public const string ActionPhase = "LocomotionNotificationKeysActionPhase";

            /// <summary>
            /// The position where the player will be moved.<br/>
            /// Value is <see cref="Vector3"/>.<br/>
            /// <br/>
            /// See Notification key: <see cref="Teleportation"/>
            /// </summary>
            public const string Position = "LocomotionNotificationKeysPosition";

            /// <summary>
            /// The active state of the snap turn.<br/>
            /// Value is <see cref="umi3d.browserRuntime.NotificationKeys.ActiveState"/>.<br/>
            /// <br/>
            /// See Notification key: <see cref="System"/>
            /// </summary>
            public const string SnapTurnActiveState = "LocomotionNotificationKeysSnapTurnActiveState";

            /// <summary>
            /// The active state of the teleportation.<br/>
            /// Value is <see cref="umi3d.browserRuntime.NotificationKeys.ActiveState"/>.<br/>
            /// <br/>
            /// See Notification key: <see cref="System"/>
            /// </summary>
            public const string TeleportationActiveState = "LocomotionNotificationKeysTeleportationActiveState";
        }
    }
}