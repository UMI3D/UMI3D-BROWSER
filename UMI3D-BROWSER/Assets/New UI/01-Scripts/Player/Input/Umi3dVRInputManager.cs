/*
Copyright 2019 - 2023 Inetum

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
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

namespace umi3dBrowsers.input
{
    public class Umi3dVRInputManager : AbstractControllerInputManager
    {
        public Dictionary<ControllerType, bool> isTeleportDown = new Dictionary<ControllerType, bool>();

        [HideInInspector]
        public UnityEngine.XR.InputDevice LeftController;
        [HideInInspector]
        public UnityEngine.XR.InputDevice RightController;

        public static Dictionary<ControllerType, Dictionary<ActionType, InputAction>> ActionMap = new();
        
        [Header("GrabInput")]
        [SerializeField] private InputAction m_leftGrab;
        [SerializeField] private InputAction m_rightGrab;
        public event Action OnLeftGrabPressed;
        public event Action OnRightGrabPressed;

        [Header("Primary")]
        [SerializeField] private InputAction m_primaryLeft;
        [SerializeField] private InputAction m_primaryRight;
        public event Action OnPrimaryLeftPressed;
        public event Action OnPrimaryRightPressed;

        [Header("Secondary")]
        [SerializeField] private InputAction m_secondaryLeft;
        [SerializeField] private InputAction m_secondaryRight;
        public event Action OnSecondaryLeftPressed;
        public event Action OnSecondaryRightPressed;

        [Header("Trigger")]
        [SerializeField] private InputAction m_triggerLeft;
        [SerializeField] private InputAction m_triggerRight;
        public event Action OnTriggerLeftPressed;
        public event Action OnTriggerRightPressed;

        //[HideInInspector]
        //public PressState JoystickLeftButtonState;
        //[HideInInspector]
        //public PressState JoystickRightButtonState;

        protected override void Awake()
        {
            // For pico preview
            Application.targetFrameRate = 72;

            base.Awake();

            foreach (ControllerType ctrl in Enum.GetValues(typeof(ControllerType)))
            {
                isTeleportDown.Add(ctrl, false);
            }

            ActionMap.TryAdd(ControllerType.LeftHandController, new Dictionary<ActionType, InputAction>
            {
                { ActionType.Grab, m_leftGrab },
                { ActionType.PrimaryButton, m_primaryLeft },
                { ActionType.SecondaryButton, m_secondaryLeft},
                { ActionType.Trigger, m_triggerLeft },              
            });
            ActionMap.TryAdd(ControllerType.RightHandController, new Dictionary<ActionType, InputAction>
            {
                { ActionType.Grab, m_rightGrab },
                { ActionType.PrimaryButton, m_primaryRight },
                { ActionType.SecondaryButton, m_secondaryRight},
                { ActionType.Trigger, m_secondaryLeft },
            });
        }

        private void Start()
        {
            LeftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
            RightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        }

        public Vector2 GetJoystickAxis(ControllerType controller)
        {
            Vector2 value;
            switch (controller)
            {
                case ControllerType.LeftHandController:
                    LeftController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out value);
                    break;
                case ControllerType.RightHandController:
                    RightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out value);
                    break;
                default:
                    return Vector2.zero;
            }

            return value;
        }


        public bool GetRightSnapTurn(ControllerType controller)
        {
            var res = GetJoystick(controller);

            if (res)
            {
                (float pole, float magnitude) = GetJoystickPoleAndMagnitude(controller);

                if ((pole >= 0 && pole < 20) || (pole > 340 && pole <= 360))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return res;
        }

        public bool GetLeftSnapTurn(ControllerType controller)
        {
            var res = GetJoystick(controller);

            if (res)
            {
                (float pole, float magnitude) = GetJoystickPoleAndMagnitude(controller);

                if (pole > 160 && pole <= 200)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return res;
        }

        private (float, float) GetJoystickPoleAndMagnitude(ControllerType controller)
        {
            var getAxis = GetJoystickAxis(controller);

            Vector2 axis = getAxis.normalized;
            float pole = 0.0f;

            if (axis.x != 0)
                pole = Mathf.Atan(axis.y / axis.x);
            else
                if (axis.y == 0)
                pole = 0;
            else if (axis.y > 0)
                pole = Mathf.PI / 2;
            else
                pole = -Mathf.PI / 2;

            pole *= Mathf.Rad2Deg;

            if (axis.x < 0)
                if (axis.y >= 0)
                    pole = 180 - Mathf.Abs(pole);
                else
                    pole = 180 + Mathf.Abs(pole);
            else if (axis.y < 0)
                pole = 360 + pole;

            return (pole, getAxis.magnitude);
        }


        public bool GetTeleportDown(ControllerType controller)
        {
            var res = GetJoystickDown(controller);

            if (res)
            {

                (float pole, float magnitude) = GetJoystickPoleAndMagnitude(controller);

                if ((pole > 20 && pole < 160))
                {
                    isTeleportDown[controller] = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return res;
        }

        public bool GetTeleportUp(ControllerType controller)
        {
            var res = GetJoystickUp(controller) && isTeleportDown[controller];

            if (res)
            {
                isTeleportDown[controller] = false;
            }

            return res;
        }
    }
}
