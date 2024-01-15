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
using umi3dVRBrowsersBase.interactions;
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.XR;

namespace umi3d.picoBrowser
{
    public class Umi3dPicoInputManager : AbstractControllerInputManager
    {
        public struct PressState
        {
            /// <summary>
            /// The number of frames since the state changed.
            /// </summary>
            public int Frame;
            /// <summary>
            /// Whether or not this button is down.
            /// </summary>
            public bool IsDown => m_isDown;
            /// <summary>
            /// Whether or not this button is up.
            /// </summary>
            public bool IsUp => !m_isDown;
            /// <summary>
            /// Whether or not this button has been pressed this frame.
            /// </summary>
            public bool IsDownThisFrame => m_isDown && Frame == 0;
            /// <summary>
            /// Whether or not this button has been released this frame.
            /// </summary>
            public bool IsUpThisFrame => !m_isDown && Frame == 0;

            public bool m_isDown;

            /// <summary>
            /// Change the state of this button.
            /// </summary>
            /// <param name="isDown"></param>
            public void SetPressState(bool isDown)
            {
                if (m_isDown == isDown) return;

                Frame = 0;
                m_isDown = isDown;
            }

            /// <summary>
            /// Increments the frame counter.
            /// </summary>
            public void IncrementFrame() => Frame++;
        }

        public Dictionary<ControllerType, bool> isTeleportDown = new Dictionary<ControllerType, bool>();

        [HideInInspector]
        public UnityEngine.XR.InputDevice LeftController;
        [HideInInspector]
        public UnityEngine.XR.InputDevice RightController;

        protected override void Awake()
        {
            // For pico preview
            Application.targetFrameRate = 72;

            base.Awake();

            foreach (ControllerType ctrl in Enum.GetValues(typeof(ControllerType)))
            {
                isTeleportDown.Add(ctrl, false);
            }
        }

        private void Start()
        {
            LeftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
            RightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        }

        protected override void Update()
        {
            base.Update();

            bool value;

            GrabLeftState.IncrementFrame();
            LeftController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out value);
            GrabLeftState.SetPressState(value);
            GrabRightState.IncrementFrame();
            RightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out value);
            GrabRightState.SetPressState(value);

            JoystickLeftButtonState.IncrementFrame();
            LeftController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxisClick, out value);
            JoystickLeftButtonState.SetPressState(value);
            JoystickRightButtonState.IncrementFrame();
            RightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxisClick, out value);
            JoystickRightButtonState.SetPressState(value);

            PrimaryLeftState.IncrementFrame();
            LeftController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out value);
            PrimaryLeftState.SetPressState(value);
            PrimaryRightState.IncrementFrame();
            RightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out value);
            PrimaryRightState.SetPressState(value);

            SecondaryLeftState.IncrementFrame();
            LeftController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.secondaryButton, out value);
            SecondaryLeftState.SetPressState(value);
            SecondaryRightState.IncrementFrame();
            RightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.secondaryButton, out value);
            SecondaryRightState.SetPressState(value);

            TriggerLeftState.IncrementFrame();
            LeftController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out value);
            TriggerLeftState.SetPressState(value);
            TriggerRightState.IncrementFrame();
            RightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out value);
            TriggerRightState.SetPressState(value);
        }

        #region Grab

        [HideInInspector]
        public PressState GrabLeftState;
        [HideInInspector]
        public PressState GrabRightState;
        public override bool GetGrab(ControllerType controller)
        {
            switch (controller)
            {
                case ControllerType.LeftHandController:
                    return GrabLeftState.IsDown;
                case ControllerType.RightHandController:
                    return GrabRightState.IsDown;
                default:
                    return false;
            }
        }

        public override bool GetGrabDown(ControllerType controller)
        {
            switch (controller)
            {
                case ControllerType.LeftHandController:
                    return GrabLeftState.IsDownThisFrame;
                case ControllerType.RightHandController:
                    return GrabRightState.IsDownThisFrame;
                default:
                    return false;
            }
        }

        public override bool GetGrabUp(ControllerType controller)
        {
            switch (controller)
            {
                case ControllerType.LeftHandController:
                    return GrabLeftState.IsUpThisFrame;
                case ControllerType.RightHandController:
                    return GrabRightState.IsUpThisFrame;
                default:
                    return false;
            }
        }

        #endregion

        #region Joystick

        public override Vector2 GetJoystickAxis(ControllerType controller)
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

        [HideInInspector]
        public PressState JoystickLeftButtonState;
        [HideInInspector]
        public PressState JoystickRightButtonState;
        public override bool GetJoystickButton(ControllerType controller)
        {
            switch (controller)
            {
                case ControllerType.LeftHandController:
                    return JoystickLeftButtonState.IsDown;
                case ControllerType.RightHandController:
                    return JoystickRightButtonState.IsDown;
                default:
                    return false;
            }
        }

        public override bool GetJoystickButtonDown(ControllerType controller)
        {
            switch (controller)
            {
                case ControllerType.LeftHandController:
                    return JoystickLeftButtonState.IsDownThisFrame;
                case ControllerType.RightHandController:
                    return JoystickRightButtonState.IsDownThisFrame;
                default:
                    return false;
            }

        }

        public override bool GetJoystickButtonUp(ControllerType controller)
        {
            switch (controller)
            {
                case ControllerType.LeftHandController:
                    return JoystickLeftButtonState.IsUpThisFrame;
                case ControllerType.RightHandController:
                    return JoystickRightButtonState.IsUpThisFrame;
                default:
                    return false;
            }
        }

        public override bool GetRightSnapTurn(ControllerType controller)
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

        public override bool GetLeftSnapTurn(ControllerType controller)
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

        #endregion

        #region Primary Button

        [HideInInspector]
        public PressState PrimaryLeftState;
        [HideInInspector]
        public PressState PrimaryRightState;
        public override bool GetPrimaryButton(ControllerType controller)
        {
            switch (controller)
            {
                case ControllerType.LeftHandController:
                    return PrimaryLeftState.IsDown;
                case ControllerType.RightHandController:
                    return PrimaryRightState.IsDown;
                default:
                    return false;
            }
        }

        public override bool GetPrimaryButtonDown(ControllerType controller)
        {
            switch (controller)
            {
                case ControllerType.LeftHandController:
                    return PrimaryLeftState.IsDownThisFrame;
                case ControllerType.RightHandController:
                    return PrimaryRightState.IsDownThisFrame;
                default:
                    return false;
            }
        }

        public override bool GetPrimaryButtonUp(ControllerType controller)
        {
            switch (controller)
            {
                case ControllerType.LeftHandController:
                    return PrimaryLeftState.IsUpThisFrame;
                case ControllerType.RightHandController:
                    return PrimaryRightState.IsUpThisFrame;
                default:
                    return false;
            }
        }

        #endregion

        #region Secondary Button

        [HideInInspector]
        public PressState SecondaryLeftState;
        [HideInInspector]
        public PressState SecondaryRightState;
        public override bool GetSecondaryButton(ControllerType controller)
        {
            switch (controller)
            {
                case ControllerType.LeftHandController:
                    return SecondaryLeftState.IsDown;
                case ControllerType.RightHandController:
                    return SecondaryRightState.IsDown;
                default:
                    return false;
            }
        }

        public override bool GetSecondaryButtonDown(ControllerType controller)
        {
            switch (controller)
            {
                case ControllerType.LeftHandController:
                    return SecondaryLeftState.IsDownThisFrame;
                case ControllerType.RightHandController:
                    return SecondaryRightState.IsDownThisFrame;
                default:
                    return false;
            }
        }

        public override bool GetSecondaryButtonUp(ControllerType controller)
        {
            switch (controller)
            {
                case ControllerType.LeftHandController:
                    return SecondaryLeftState.IsUpThisFrame;
                case ControllerType.RightHandController:
                    return SecondaryRightState.IsUpThisFrame;
                default:
                    return false;
            }
        }

        #endregion

        #region Trigger

        [HideInInspector]
        public PressState TriggerLeftState;
        [HideInInspector]
        public PressState TriggerRightState;
        public override bool GetTrigger(ControllerType controller)
        {
            switch (controller)
            {
                case ControllerType.LeftHandController:
                    return TriggerLeftState.IsDown;
                case ControllerType.RightHandController:
                    return TriggerRightState.IsDown;
                default:
                    return false;
            }
        }

        public override bool GetTriggerDown(ControllerType controller)
        {
            switch (controller)
            {
                case ControllerType.LeftHandController:
                    return TriggerLeftState.IsDownThisFrame;
                case ControllerType.RightHandController:
                    return TriggerRightState.IsDownThisFrame;
                default:
                    return false;
            }
        }

        public override bool GetTriggerUp(ControllerType controller)
        {
            switch (controller)
            {
                case ControllerType.LeftHandController:
                    return TriggerLeftState.IsUpThisFrame;
                case ControllerType.RightHandController:
                    return TriggerRightState.IsUpThisFrame;
                default:
                    return false;
            }
        }

        #endregion

        public override bool GetTeleportDown(ControllerType controller)
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

        public override bool GetTeleportUp(ControllerType controller)
        {
            var res = GetJoystickUp(controller) && isTeleportDown[controller];

            if (res)
            {
                isTeleportDown[controller] = false;
            }

            return res;
        }

        public override void VibrateController(ControllerType controller, float vibrationDuration, float vibrationFrequency, float vibrationAmplitude)
        {
            PXR_Input.SendHapticImpulse(controller == ControllerType.LeftHandController ? PXR_Input.VibrateType.LeftController : PXR_Input.VibrateType.RightController, vibrationAmplitude, (int)vibrationDuration, (int)vibrationFrequency);
        }
    }
}
