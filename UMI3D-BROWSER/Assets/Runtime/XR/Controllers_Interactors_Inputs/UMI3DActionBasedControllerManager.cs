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

namespace umi3d.browserRuntime.xr
{
    /// <summary>
    /// Use this class to mediate the controllers and their associated interactors and input actions under different interaction states.
    /// </summary>
    public class UMI3DActionBasedControllerManager : MonoBehaviour
    {
        [Space]
        [Header("Controller Actions")]

        //[SerializeField]
        //[Tooltip("If true, continuous movement will be enabled. If false, teleport will enabled.")]
        //bool m_SmoothMotionEnabled;

        [Tooltip("The reference to the action to start the teleport aiming mode for this controller.")]
        [SerializeField] InputActionReference m_TeleportModeActivate;

        [Tooltip("The reference to the action to cancel the teleport aiming mode for this controller.")]
        [SerializeField] InputActionReference m_TeleportModeCancel;

        [Tooltip("Whether the teleportation is enabled with this controller.")]
        [SerializeField] bool enableTeleportation;

        //[SerializeField]
        //[Tooltip("The reference to the action of moving the XR Origin with this controller.")]
        //InputActionReference m_Move;

        [Space]

        //[SerializeField]
        //[Tooltip("If true, continuous turn will be enabled. If false, snap turn will be enabled. Note: If smooth motion is enabled and enable strafe is enabled on the continuous move provider, turn will be overriden in favor of strafe.")]
        //bool m_SmoothTurnEnabled;

        [Tooltip("The reference to the action of snap turning the XR Origin with this controller.")]
        [SerializeField] InputActionReference m_SnapTurn;

        [Tooltip("Whether the snap turn is enabled with this controller.")]
        [SerializeField] bool enableSnapTurn;

        //[SerializeField]
        //[Tooltip("The reference to the action of continuous turning the XR Origin with this controller.")]
        //InputActionReference m_Turn;

        //[Space]

        //[SerializeField]
        //[Tooltip("If true, UI scrolling will be enabled.")]
        //bool m_UIScrollingEnabled;

        //[SerializeField]
        //[Tooltip("The reference to the action of scrolling UI with this controller.")]
        //InputActionReference m_UIScroll;

        ControllerType controllerType;

        public bool EnableTeleportation
        {
            get => enableTeleportation;
            set
            {
                SetEnabled(m_TeleportModeActivate, value);
                SetEnabled(m_TeleportModeCancel, value);
                enableTeleportation = value;
            }
        }

        public bool EnableSnapTurn
        {
            get => enableSnapTurn;
            set
            {
                SetEnabled(m_SnapTurn, value);
                enableSnapTurn = value;
            }
        }

        private void Awake()
        {
            controllerType = GetComponent<ControllerType>();
        }

        void OnEnable()
        {
            NotificationHub.Default.Subscribe(
                this,
                LocomotionNotificationKeys.System,
                null,
                EnableOrDisableNavigation
            );

            if (enableTeleportation)
            {
                EnableAction(m_TeleportModeActivate);
                EnableAction(m_TeleportModeCancel);
            }

            if (enableSnapTurn)
            {
                EnableAction(m_SnapTurn);
            }
        }

        void OnDisable()
        {
            NotificationHub.Default.Unsubscribe(this, LocomotionNotificationKeys.System);

            DisableAction(m_TeleportModeActivate);
            DisableAction(m_TeleportModeCancel);
            DisableAction(m_SnapTurn);
        }

        void EnableOrDisableNavigation(Notification notification)
        {
            if (!notification.TryGetInfoT(LocomotionNotificationKeys.Info.Controller, out Controller controller))
            {
                return;
            }

            if (controller != controllerType.controller && controller != Controller.LeftAndRight)
            {
                // This controller is not the target of the notification.
                return;
            }

            if (notification.TryGetInfoT(LocomotionNotificationKeys.Info.SnapTurnActiveState, out ActiveState snapTurnState, false))
            {
                EnableSnapTurn = snapTurnState == ActiveState.Enable;
            }

            if (notification.TryGetInfoT(LocomotionNotificationKeys.Info.TeleportationActiveState, out ActiveState teleportationState, false))
            {
                EnableTeleportation = teleportationState == ActiveState.Enable;
            }
        }

        #region Actions

        static void SetEnabled(InputActionReference actionReference, bool enabled)
        {
            if (enabled)
                EnableAction(actionReference);
            else
                DisableAction(actionReference);
        }

        static void EnableAction(InputActionReference actionReference)
        {
            var action = GetInputAction(actionReference);
            if (action != null && !action.enabled)
                action.Enable();
        }

        static void DisableAction(InputActionReference actionReference)
        {
            var action = GetInputAction(actionReference);
            if (action != null && action.enabled)
                action.Disable();
        }

        static InputAction GetInputAction(InputActionReference actionReference)
        {
            return actionReference != null ? actionReference.action : null;
        }

        #endregion
    }
}