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

        public static Dictionary<ControllerType, Dictionary<ActionType, InputActionReference>> ActionMap = new();
        
        [Header("GrabInput")]
        [SerializeField] private InputActionReference m_leftGrab;
        [SerializeField] private InputActionReference m_rightGrab;

        [Header("Primary")]
        [SerializeField] private InputActionReference m_primaryLeft;
        [SerializeField] private InputActionReference m_primaryRight;

        [Header("Secondary")]
        [SerializeField] private InputActionReference m_secondaryLeft;
        [SerializeField] private InputActionReference m_secondaryRight;

        [Header("Trigger")]
        [SerializeField] private InputActionReference m_triggerLeft;
        [SerializeField] private InputActionReference m_triggerRight;

        [Header("Teleport")]
        [SerializeField] private InputActionReference m_rightJoystick;
        [SerializeField] private InputActionReference m_rightSnapTurn;

        protected override void Awake()
        {
            // For pico preview
            Application.targetFrameRate = 72;

            base.Awake();

            foreach (ControllerType ctrl in Enum.GetValues(typeof(ControllerType)))
            {
                isTeleportDown.Add(ctrl, false);
            }

            ActionMap.TryAdd(ControllerType.LeftHandController, new Dictionary<ActionType, InputActionReference>
            {
                { ActionType.Grab, m_leftGrab },
                { ActionType.PrimaryButton, m_primaryLeft },
                { ActionType.SecondaryButton, m_secondaryLeft},
                { ActionType.Trigger, m_triggerLeft },
                
            });
            ActionMap.TryAdd(ControllerType.RightHandController, new Dictionary<ActionType, InputActionReference>
            {
                { ActionType.Grab, m_rightGrab },
                { ActionType.PrimaryButton, m_primaryRight },
                { ActionType.SecondaryButton, m_secondaryRight},
                { ActionType.Trigger, m_secondaryLeft },
                { ActionType.Teleport, m_rightJoystick },
                { ActionType.PrimaryButton, m_primaryRight },
                { ActionType.SecondaryButton, m_secondaryRight },
                { ActionType.RightSnapTurn, m_rightJoystick },
                { ActionType.LeftSnapTurn, m_rightJoystick }
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
