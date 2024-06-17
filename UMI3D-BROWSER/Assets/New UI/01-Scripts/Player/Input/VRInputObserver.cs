﻿/*
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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace umi3dBrowsers.input
{
    /// <summary>
    /// Observer for VR controller buttons.
    /// </summary>
    public class VRInputObserver : MonoBehaviour
    {
        #region Fields

        /// <summary>
        /// Input acyion type.
        /// </summary>
        public ActionType action;

        /// <summary>
        /// Type of controller.
        /// </summary>
        public ControllerType controller;

        /// <summary>
        /// Event raised when the input is pressed.
        /// </summary>
        public UnityEvent onActionDown;

        /// <summary>
        /// Event raised when the input is released.
        /// </summary>
        public UnityEvent onActionUp;

        /// <summary>
        /// List of suscribers to <see cref="onActionUp"/>.
        /// </summary>
        private List<System.Action> subscribersUp = new List<System.Action>();

        /// <summary>
        /// List of suscribers to <see cref="onActionDown"/>.
        /// </summary>
        private List<System.Action> subscribersDown = new List<System.Action>();


        /// <summary>
        /// Adds a suscriber to <see cref="onActionUp"/>.
        /// </summary>
        /// <param name="callback"></param>
        public void AddOnStateUpListener(System.Action callback)
        {
            subscribersUp.Add(callback);
        }

        /// <summary>
        /// Adds a suscriber to <see cref="onActionDown"/>.
        /// </summary>
        /// <param name="callback"></param>
        public void AddOnStateDownListener(System.Action callback)
        {
            subscribersDown.Add(callback);
        }


        /// <summary>
        /// Removes a suscriber from <see cref="onActionUp"/>.
        /// </summary>
        /// <param name="callback"></param>
        public void RemoveOnStateUpListener(System.Action callback)
        {
            subscribersUp.Remove(callback);
        }

        /// <summary>
        /// Removes a suscriber from <see cref="onActionDown"/>.
        /// </summary>
        /// <param name="callback"></param>
        public void RemoveOnStateDownListener(System.Action callback)
        {
            subscribersDown.Remove(callback);
        }

        private void Awake()
        {
            if (Umi3dVRInputManager.ActionMap.TryGetValue(controller, out var controllerAction))
            {
                if(controllerAction.TryGetValue(action, out var inputAction)){
                    inputAction.performed += i =>
                    {
                        onActionDown.Invoke();
                        foreach (System.Action action in subscribersDown)
                            action.Invoke();
                    };
                    inputAction.canceled += i =>
                    {
                        onActionUp.Invoke();
                        foreach (System.Action action in subscribersUp)
                            action.Invoke();
                    };
                }
            }
        }

        #endregion Methods
    }
}