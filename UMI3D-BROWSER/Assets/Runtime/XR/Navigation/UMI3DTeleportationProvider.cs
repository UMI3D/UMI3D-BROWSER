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

using inetum.unityUtils;
using System.Collections;
using System.Collections.Generic;
using umi3d.browserRuntime.NotificationKeys;
using UnityEngine;
using UnityEngine.InputSystem;


namespace umi3d.browserRuntime.navigation
{
    /// <summary>
    /// Teleports users where they want. 
    /// </summary>
    public class UMI3DTeleportationProvider : MonoBehaviour
    {
        [Header("Controller Actions")]
        [Tooltip("The reference to the action to start the teleport aiming mode for this controller.")]
        [SerializeField] InputActionReference leftHandTeleportModeActivate;
        [Tooltip("The reference to the action to cancel the teleport aiming mode for this controller.")]
        [SerializeField] InputActionReference leftHandTeleportModeCancel;
        [Space]
        [Tooltip("The reference to the action to start the teleport aiming mode for this controller.")]
        [SerializeField] InputActionReference rightHandTeleportModeActivate;
        [Tooltip("The reference to the action to cancel the teleport aiming mode for this controller.")]
        [SerializeField] InputActionReference rightHandTeleportModeCancel;

        [Header("Settings")]
        [Tooltip("The amount of time that locomotion waits before performing another action.")]
        [SerializeField] float debounceTime = .3f;

        ActionMovementTiming actionMovementTiming;

        Coroutine coroutine;
        bool isPerforming;

        Dictionary<string, System.Object> info = new();

        void Awake()
        {
            actionMovementTiming = new();
            actionMovementTiming.DebounceTime = () => debounceTime;
        }

        void OnEnable()
        {
            leftHandTeleportModeActivate.action.performed += ActiveLeftTeleportation;
            rightHandTeleportModeActivate.action.performed += ActiveRightTeleportation;
            leftHandTeleportModeActivate.action.canceled += StopLeftTeleportation;
            rightHandTeleportModeActivate.action.canceled += StopRightTeleportation;

            leftHandTeleportModeCancel.action.performed += CancelLeftTeleportation;
            rightHandTeleportModeCancel.action.performed += CancelRightTeleportation;
        }

        void OnDisable()
        {
            leftHandTeleportModeActivate.action.performed -= ActiveLeftTeleportation;
            rightHandTeleportModeActivate.action.performed -= ActiveRightTeleportation;
            leftHandTeleportModeActivate.action.canceled -= StopLeftTeleportation;
            rightHandTeleportModeActivate.action.canceled -= StopRightTeleportation;

            leftHandTeleportModeCancel.action.performed -= CancelLeftTeleportation;
            rightHandTeleportModeCancel.action.performed -= CancelRightTeleportation;
        }

        void ActiveLeftTeleportation(InputAction.CallbackContext context)
        {
            if (debounceTime == 0f)
            {
                ActiveTeleportation(Controller.LeftHand);
            }
            else
            {
                if (coroutine != null)
                {
                    return;
                }

                coroutine = StartCoroutine(ActiveTeleportationCoroutine(Controller.LeftHand));
            }
        }
        void ActiveRightTeleportation(InputAction.CallbackContext context)
        {
            if (debounceTime == 0f)
            {
                ActiveTeleportation(Controller.RightHand);
            }
            else
            {
                if (coroutine != null)
                {
                    return;
                }

                coroutine = StartCoroutine(ActiveTeleportationCoroutine(Controller.RightHand));
            }
        }
        void ActiveTeleportation(Controller controller)
        {
            isPerforming = true;
            info[LocomotionNotificationKeys.Info.Controller] = controller;
            info[LocomotionNotificationKeys.Info.ActionPhase] = InputActionPhase.Started;
            NotificationHub.Default.Notify(
                this,
                LocomotionNotificationKeys.Teleportation,
                info
            );
        }
        IEnumerator ActiveTeleportationCoroutine(Controller controller)
        {
            ActiveTeleportation(controller);

            actionMovementTiming.ResetDebounceTime();

            while (isPerforming)
            {
                yield return null;
            }
            while (!actionMovementTiming.CanPerformAction())
            {
                yield return null;
            }

            coroutine = null;
        }

        void StopLeftTeleportation(InputAction.CallbackContext context)
        {
            StopTeleportation(Controller.LeftHand);
        }
        void StopRightTeleportation(InputAction.CallbackContext context)
        {
            StopTeleportation(Controller.RightHand);
        }
        void StopTeleportation(Controller controller)
        {
            info[LocomotionNotificationKeys.Info.Controller] = controller;
            info[LocomotionNotificationKeys.Info.ActionPhase] = InputActionPhase.Performed;
            NotificationHub.Default.Notify(
                this,
                LocomotionNotificationKeys.Teleportation,
                info
            );
            isPerforming = false;
        }

        void CancelLeftTeleportation(InputAction.CallbackContext context)
        {
            CancelTeleportation(Controller.LeftHand);
        }
        void CancelRightTeleportation(InputAction.CallbackContext context)
        {
            CancelTeleportation(Controller.RightHand);
        }
        void CancelTeleportation(Controller controller)
        {
            isPerforming = false;
            info[LocomotionNotificationKeys.Info.Controller] = controller;
            info[LocomotionNotificationKeys.Info.ActionPhase] = InputActionPhase.Canceled;
            NotificationHub.Default.Notify(
                this,
                LocomotionNotificationKeys.Teleportation,
                info
            );
        }

#if UNITY_EDITOR

        [ContextMenu("Test Active Left")]
        void TestActiveLeftController()
        {
            UnityEngine.Debug.Log($"Test Active left controller");
            ActiveTeleportation(Controller.LeftHand);
        }

        [ContextMenu("Test Stop Left")]
        void TestStopLeftController()
        {
            UnityEngine.Debug.Log($"Test Stop left controller");
            StopTeleportation(Controller.LeftHand);
        }

        [ContextMenu("Test Cancel Left")]
        void TestCancelLeftController()
        {
            UnityEngine.Debug.Log($"Test Cancel left controller");
            CancelTeleportation(Controller.LeftHand);
        }
#endif
    }
}
