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
    /// <summary>
    /// The types of controller.
    /// </summary>
    public enum Controller
    {
        LeftHand,
        RightHand, 
        LeftAndRight
    }

    /// <summary>
    /// Active state of an object.
    /// </summary>
    public enum ActiveState
    {
        Enable,
        Disable
    }

    /// <summary>
    /// Opening state of an object.
    /// </summary>
    public enum OpeningState
    {
        Open,
        Close
    }

    /// <summary>
    /// The animation settings for the keyboard.
    /// </summary>
    public enum KeyboardAnimationType
    {
        OpenOrClose,
        KeyPress
    }

    /// <summary>
    /// The locomotion types.
    /// </summary>
    [Flags]
    public enum LocomotionType
    {
        None = 0,
        SnapTurn = 1,
        ContinuousTurn = 1 << 1,
        Teleportation = 1 << 2,
        Move = 1 << 3,
    }
}