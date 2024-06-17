/*
Copyright 2019 - 2022 Inetum

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

using inetum.unityUtils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace umi3dBrowsers.input
{
    /// <summary>
    /// This class enables scripts to get input from VR devices. This class must be implemenented on each browser depending on the device.
    /// </summary>
    public abstract class AbstractControllerInputManager : SingleBehaviour<AbstractControllerInputManager>
    {
        /// <summary>
        /// Duration (in seconds) of a controller vibration.
        /// </summary>
        public float vibrationDuration;

        /// <summary>
        /// Frequency used for a controller vibration.
        /// </summary>
        public float vibrationFrequency;

        /// <summary>
        /// Amplitude used for a controller vibration.
        /// </summary>
        public float vibrationAmplitude;

        Array ControllerTypes = Enum.GetValues(typeof(ControllerType));

        protected override void Awake()
        {
            base.Awake();

            foreach (ControllerType ctrl in ControllerTypes)
            {
                isJoystick.Add(ctrl, false);
                isJoystickDown.Add(ctrl, false);
                isJoystickUp.Add(ctrl, false);
            }
        }

        protected Dictionary<ControllerType, bool> isJoystick = new Dictionary<ControllerType, bool>();

        protected Dictionary<ControllerType, bool> isJoystickDown = new Dictionary<ControllerType, bool>();

        protected Dictionary<ControllerType, bool> isJoystickUp = new Dictionary<ControllerType, bool>();

        /// <summary>
        /// Returns true if <paramref name="controller"/> joystick starts being used.
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public bool GetJoystickDown(ControllerType controller)
        {
            if (isJoystickDown.ContainsKey(controller))
                return isJoystickDown[controller];

            Debug.LogError("Internal error, unkown controller " + controller);
            return false;
        }

        /// <summary>
        /// Returns true if <paramref name="controller"/> joystick is being used.
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public bool GetJoystick(ControllerType controller)
        {
            if (isJoystick.ContainsKey(controller))
                return isJoystickDown[controller];

            Debug.LogError("Internal error, unkown controller " + controller);
            return false;
        }

        /// <summary>
        /// Returns true if <paramref name="controller"/> joystick is being used.
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public bool GetJoystickUp(ControllerType controller)
        {
            if (isJoystickUp.ContainsKey(controller))
                return isJoystickUp[controller];

            Debug.LogError("Internal error, unkown controller " + controller);
            return false;
        }
    }

    /// <summary>
    /// Lists all type of controllers.
    /// </summary>
    public enum ControllerType
    {
        LeftHandController,
        RightHandController
    }

    /// <summary>
    /// Lists all types of action.
    /// </summary>
    public enum ActionType
    {
        Trigger,
        Grab,
        PrimaryButton,
        SecondaryButton,
        JoystickButton,
        Teleport,
        RightSnapTurn,
        LeftSnapTurn
    }
}