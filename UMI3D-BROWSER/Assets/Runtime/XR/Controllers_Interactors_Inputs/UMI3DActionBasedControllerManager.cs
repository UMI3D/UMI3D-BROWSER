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

using System.Collections;
using System.Collections.Generic;
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

        [SerializeField]
        [Tooltip("The reference to the action to start the teleport aiming mode for this controller.")]
        InputActionReference m_TeleportModeActivate;

        [SerializeField]
        [Tooltip("The reference to the action to cancel the teleport aiming mode for this controller.")]
        InputActionReference m_TeleportModeCancel;

        //[SerializeField]
        //[Tooltip("The reference to the action of moving the XR Origin with this controller.")]
        //InputActionReference m_Move;

        [Space]

        //[SerializeField]
        //[Tooltip("If true, continuous turn will be enabled. If false, snap turn will be enabled. Note: If smooth motion is enabled and enable strafe is enabled on the continuous move provider, turn will be overriden in favor of strafe.")]
        //bool m_SmoothTurnEnabled;

        [SerializeField]
        [Tooltip("The reference to the action of snap turning the XR Origin with this controller.")]
        InputActionReference m_SnapTurn;

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

        void Awake()
        {
            
        }

        void EnableOrDisableNavigation()
        {

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