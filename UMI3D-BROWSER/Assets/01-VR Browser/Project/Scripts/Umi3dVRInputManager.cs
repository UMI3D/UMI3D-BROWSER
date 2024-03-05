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
using System.Linq;

using umi3dVRBrowsersBase.interactions;

using UnityEngine;
using UnityEngine.XR;

namespace umi3d.picoBrowser
{
    public class Umi3dVRInputManager : AbstractControllerInputManager
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

        private Dictionary<ControllerType, bool> isTeleporting = new Dictionary<ControllerType, bool>();
        private Dictionary<ControllerType, bool> isHandTeleporting = new Dictionary<ControllerType, bool>();
        private Dictionary<ControllerType, bool> isUsingHandTeleportation = new Dictionary<ControllerType, bool>();

        public UnityEngine.XR.InputDevice LeftController => leftControllersGroup.controllerDevice;
        public UnityEngine.XR.InputDevice RightController => rightControllersGroup.controllerDevice;

        public UnityEngine.XR.InputDevice LeftHandTrackingController => leftControllersGroup.handTrackingDevice;
        public UnityEngine.XR.InputDevice RightHandTrackingController => rightControllersGroup.handTrackingDevice;

        protected override void Awake()
        {
            // For pico preview
            Application.targetFrameRate = 72;

            base.Awake();

            foreach (ControllerType ctrl in Enum.GetValues(typeof(ControllerType)))
            {
                isTeleporting.Add(ctrl, false);
                isHandTeleporting.Add(ctrl, false);
                isUsingHandTeleportation.Add(ctrl, false);
            }
        }

        private Dictionary<ActionType, PressStateCoordinator> pressStateCoordinators;

        private class ControllerGroup
        {
            public InputDevice controllerDevice;
            public InputDevice handTrackingDevice;
            public VRGestureDevice gestureDevice;
        }

        private ControllerGroup leftControllersGroup = new();
        private ControllerGroup rightControllersGroup = new();

        private class PressStateCoordinator
        {
            public ActionType actionType;
            public InputFeatureUsage<bool> xrInputFeatureUsage;

            public bool isLeftHandTrackingInput;
            public bool isRightHandTrackingInput;

            public PressState leftState;
            public PressState rightState;

            public void UpdateInputState(ControllerGroup left, ControllerGroup right)
            {
                leftState.IncrementFrame();
                left.controllerDevice.TryGetFeatureValue(xrInputFeatureUsage, out bool value);
                if (left.gestureDevice != null)
                    value = value || isLeftHandTrackingInput;
                leftState.SetPressState(value);

                rightState.IncrementFrame();
                right.controllerDevice.TryGetFeatureValue(xrInputFeatureUsage, out value);
                if (right.gestureDevice != null)
                    value = value || isRightHandTrackingInput;
                rightState.SetPressState(value);
            }

            public bool GetButton(ControllerType controller)
            {
                switch (controller)
                {
                    case ControllerType.LeftHandController:
                        return leftState.IsDown;
                    case ControllerType.RightHandController:
                        return rightState.IsDown;
                    default:
                        return false;
                }
            }

            public bool GetButtonDown(ControllerType controller)
            {
                switch (controller)
                {
                    case ControllerType.LeftHandController:
                        return leftState.IsDownThisFrame;
                    case ControllerType.RightHandController:
                        return rightState.IsDownThisFrame;
                    default:
                        return false;
                }
            }

            public bool GetButtonUp(ControllerType controller)
            {
                switch (controller)
                {
                    case ControllerType.LeftHandController:
                        return leftState.IsUpThisFrame;
                    case ControllerType.RightHandController:
                        return rightState.IsUpThisFrame;
                    default:
                        return false;
                }
            }
        }

        #region Lifecycle

        private void Start()
        {
            List<InputDevice> queryResult = new(); //maybe remove held in hand
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.HeldInHand, queryResult);
            leftControllersGroup.controllerDevice = queryResult.Count > 0 ? queryResult[0] : default;
            queryResult.Clear();

            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.HeldInHand, queryResult);
            rightControllersGroup.controllerDevice = queryResult.Count > 0 ? queryResult[0] : default;
            queryResult.Clear();

            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left | InputDeviceCharacteristics.HandTracking, queryResult);
            leftControllersGroup.handTrackingDevice = queryResult.Count > 0 ? queryResult[0] : default;
            queryResult.Clear();

            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right | InputDeviceCharacteristics.HandTracking, queryResult);
            rightControllersGroup.handTrackingDevice = queryResult.Count > 0 ? queryResult[0] : default;
            queryResult.Clear();

            UnityEngine.Debug.Log("Controllers found :\n" +
                $"Left Controller {LeftController != default}. Name: {LeftController.name}. IsValid: {LeftController.isValid}. Charac: {LeftController.characteristics}\n" +
                $"Right Controller {RightController != default}. Name: {RightController.name}. IsValid: {RightController.isValid}. Charac: {RightController.characteristics}\n" +
                $"Left Hand Controller {LeftHandTrackingController != default}. Name: {LeftHandTrackingController.name}. IsValid: {LeftHandTrackingController.isValid}. Charac: {LeftHandTrackingController.characteristics}\n");

            InputDevices.GetDevices(queryResult);
            Debug.Log(queryResult.Count);
            Debug.Log(queryResult.Aggregate("", (x, y) => x + " " + y.name + " : " + y.characteristics.ToString() + " | "));

            pressStateCoordinators = new()
            {
                { ActionType.Trigger, new() { actionType = ActionType.Trigger, xrInputFeatureUsage = UnityEngine.XR.CommonUsages.triggerButton } },
                { ActionType.Grab, new() { actionType = ActionType.Grab, xrInputFeatureUsage = UnityEngine.XR.CommonUsages.gripButton } },
                { ActionType.PrimaryButton, new() { actionType = ActionType.PrimaryButton, xrInputFeatureUsage = UnityEngine.XR.CommonUsages.primaryButton } },
                { ActionType.SecondaryButton, new() { actionType = ActionType.SecondaryButton, xrInputFeatureUsage = UnityEngine.XR.CommonUsages.secondaryButton } },
                { ActionType.JoystickButton, new() { actionType = ActionType.JoystickButton, xrInputFeatureUsage = UnityEngine.XR.CommonUsages.primary2DAxisClick } },
            };
        }

        protected override void Update()
        {
            base.Update();

            foreach (PressStateCoordinator pressStateCoordinator in pressStateCoordinators.Values)
            {
                pressStateCoordinator.UpdateInputState(leftControllersGroup, rightControllersGroup);
            }
        }

        #endregion Lifecycle

        public void AddPhysicalDevice(ControllerType controllerType, InputDevice device)
        {
            if (controllerType == ControllerType.LeftHandController && LeftController == default)
            {
                leftControllersGroup.controllerDevice = device;
            }
            else if (controllerType == ControllerType.RightHandController && RightController == default)
            {
                rightControllersGroup.controllerDevice = device;
            }
        }

        #region Hand Tracking

        public void AddHandTrackedDevice(ControllerType controllerType, InputDevice device)
        {
            Debug.Log($"Add Hand Tracked Device Controller {controllerType}");
            if (controllerType == ControllerType.LeftHandController && LeftHandTrackingController == default)
            {
                leftControllersGroup.handTrackingDevice = device;
            }
            else if (RightHandTrackingController == default)
            {
                rightControllersGroup.handTrackingDevice = device;
            }
        }

        public void AddHandTrackedGestureDevice(VRGestureDevice device)
        {
            if (device.ControllerType is ControllerType.LeftHandController && leftControllersGroup.gestureDevice != device)
            {
                leftControllersGroup.gestureDevice = device;
            }
            else if (device.ControllerType is ControllerType.RightHandController && rightControllersGroup.gestureDevice != device)
            {
                rightControllersGroup.gestureDevice = device;
            }
            else
                return;

            foreach (VRGestureObserver observer in device.GestureInputs.Where(x => x != null))
            {
                observer.GestureStarted += () => SetHandTrackingInputAction(device.ControllerType, observer.ActionType, true);
                observer.GestureStopped += () => SetHandTrackingInputAction(device.ControllerType, observer.ActionType, false);
            }

            foreach (VRPokeInputObserver observer in device.PokeInputs.Where(x => x != null))
            {
                observer.Poked += () => SetHandTrackingInputAction(device.ControllerType, observer.ActionType, true);
                observer.Unpoked += () => SetHandTrackingInputAction(device.ControllerType, observer.ActionType, false);
            }
        }

        private void SetHandTrackingInputAction(ControllerType controllerType, ActionType actionType, bool value)
        {
            Debug.Log($"Action {actionType} on {controllerType} to {value}");
            if (actionType == ActionType.Teleport)
            {
                isHandTeleporting[controllerType] = value;
                return;
            }

            if (!pressStateCoordinators.ContainsKey(actionType))
                return;

            if (controllerType == ControllerType.LeftHandController)
                pressStateCoordinators[actionType].isLeftHandTrackingInput = value;
            else if (controllerType == ControllerType.RightHandController)
                pressStateCoordinators[actionType].isRightHandTrackingInput = value;
        }

        #endregion Hand Tracking

        #region Inputs

        #region Grab


        public PressState GrabLeftState => pressStateCoordinators[ActionType.Grab].leftState;

        public PressState GrabRightState => pressStateCoordinators[ActionType.Grab].rightState;
        public override bool GetGrab(ControllerType controller)
        {
            return pressStateCoordinators[ActionType.Grab].GetButton(controller);
        }

        public override bool GetGrabDown(ControllerType controller)
        {
            return pressStateCoordinators[ActionType.Grab].GetButtonDown(controller);
        }

        public override bool GetGrabUp(ControllerType controller)
        {
            return pressStateCoordinators[ActionType.Grab].GetButtonUp(controller);
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

        public PressState JoystickLeftButtonState => pressStateCoordinators[ActionType.JoystickButton].leftState;

        public PressState JoystickRightButtonState => pressStateCoordinators[ActionType.JoystickButton].rightState;
        public override bool GetJoystickButton(ControllerType controller)
        {
            return pressStateCoordinators[ActionType.JoystickButton].GetButton(controller);
        }

        public override bool GetJoystickButtonDown(ControllerType controller)
        {
            return pressStateCoordinators[ActionType.JoystickButton].GetButtonDown(controller);
        }

        public override bool GetJoystickButtonUp(ControllerType controller)
        {
            return pressStateCoordinators[ActionType.JoystickButton].GetButtonUp(controller);
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


        public PressState PrimaryLeftState => pressStateCoordinators[ActionType.PrimaryButton].leftState;

        public PressState PrimaryRightState => pressStateCoordinators[ActionType.PrimaryButton].rightState;
        public override bool GetPrimaryButton(ControllerType controller)
        {
            return pressStateCoordinators[ActionType.PrimaryButton].GetButton(controller);
        }

        public override bool GetPrimaryButtonDown(ControllerType controller)
        {
            return pressStateCoordinators[ActionType.PrimaryButton].GetButtonDown(controller);
        }

        public override bool GetPrimaryButtonUp(ControllerType controller)
        {
            return pressStateCoordinators[ActionType.PrimaryButton].GetButtonUp(controller);
        }

        #endregion

        #region Secondary Button

        [HideInInspector]
        public PressState SecondaryLeftState => pressStateCoordinators[ActionType.SecondaryButton].leftState;
        [HideInInspector]
        public PressState SecondaryRightState => pressStateCoordinators[ActionType.SecondaryButton].rightState;
        public override bool GetSecondaryButton(ControllerType controller)
        {
            return pressStateCoordinators[ActionType.SecondaryButton].GetButton(controller);
        }

        public override bool GetSecondaryButtonDown(ControllerType controller)
        {
            return pressStateCoordinators[ActionType.SecondaryButton].GetButtonDown(controller);
        }

        public override bool GetSecondaryButtonUp(ControllerType controller)
        {
            return pressStateCoordinators[ActionType.SecondaryButton].GetButtonUp(controller);
        }

        #endregion

        #region Trigger

        [HideInInspector]
        public PressState TriggerLeftState => pressStateCoordinators[ActionType.Trigger].leftState;
        [HideInInspector]
        public PressState TriggerRightState => pressStateCoordinators[ActionType.Trigger].rightState;
        public override bool GetTrigger(ControllerType controller)
        {
            return pressStateCoordinators[ActionType.Trigger].GetButton(controller);
        }

        public override bool GetTriggerDown(ControllerType controller)
        {
            return pressStateCoordinators[ActionType.Trigger].GetButtonDown(controller);
        }

        public override bool GetTriggerUp(ControllerType controller)
        {
            return pressStateCoordinators[ActionType.Trigger].GetButtonUp(controller);
        }

        #endregion

        public override bool GetTeleportDown(ControllerType controller)
        {
            if (isHandTeleporting[controller] && !isUsingHandTeleportation[controller] && !isTeleporting[controller])
            {
                isTeleporting[controller] = true;
                isUsingHandTeleportation[controller] = true;
                return isTeleporting[controller];
            }

            if (!GetJoystickDown(controller))
                return false;
            
            (float pole, float magnitude) = GetJoystickPoleAndMagnitude(controller);

            if ((pole > 20 && pole < 160))
            {
                isTeleporting[controller] = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool GetTeleportUp(ControllerType controller)
        {
            if (!isTeleporting[controller])
                return false;
            
            if (!isHandTeleporting[controller] && isUsingHandTeleportation[controller])
            {   
                isTeleporting[controller] = false;
                isUsingHandTeleportation[controller] = false;
                return true;
            }
            else if (GetJoystickUp(controller))
            {
                isTeleporting[controller] = false;
                return true;
            }
            else
                return false;
        }

        #endregion Inputs

        public override void VibrateController(ControllerType controller, float vibrationDuration, float vibrationFrequency, float vibrationAmplitude)
        {
            //PXR_Input.SendHapticImpulse(controller == ControllerType.LeftHandController ? PXR_Input.VibrateType.LeftController : PXR_Input.VibrateType.RightController, vibrationAmplitude, (int)vibrationDuration, (int)vibrationFrequency);
        }
    }
}
