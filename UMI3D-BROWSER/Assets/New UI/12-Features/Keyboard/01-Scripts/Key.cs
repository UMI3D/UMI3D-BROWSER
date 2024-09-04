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
using System;
using System.Collections;
using System.Collections.Generic;
using umi3d.browserRuntime.NotificationKeys;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.keyboard
{
    public class Key : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        Button button;

        public bool buttonPressed;
        public event Action PointerUp;

        PointerDownBehaviour pointerDown;

        void Awake()
        {
            button = GetComponent<Button>();
            if (button == null) 
            {
                button = gameObject.AddComponent<Button>();
            }

            pointerDown = GetComponent<PointerDownBehaviour>();
            if (pointerDown == null)
            {
                pointerDown = gameObject.AddComponent<PointerDownBehaviour>();
            }
            // Disable to avoid pointer event to be trigger directly from the pointerDownBehaviour class.
            pointerDown.enabled = false;
        }

        private void OnEnable()
        {

        }

        private void OnDestroy()
        {

        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!button.IsInteractable())
            {
                return;
            }

            UnityEngine.Debug.Log($"[Key] down");
            buttonPressed = true;
            pointerDown.OnPointerDown(eventData);
            //NotificationHub.Default.Notify(this, KeyboardNotificationKeys.AskPreviewFocus);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!button.IsInteractable())
            {
                return;
            }

            // Don't work yet
            UnityEngine.Debug.Log($"[Key] up");
            buttonPressed = false;
            pointerDown.OnPointerUp(eventData);
            PointerUp?.Invoke();

            NotificationHub.Default.Notify(
              this,
              KeyboardNotificationKeys.KeyClicked,
              null
          );
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!button.IsInteractable())
            {
                return;
            }

            NotificationHub.Default.Notify(
                this,
                KeyboardNotificationKeys.KeyHovered,
                null
            );

            // Don't work yet
            //UnityEngine.Debug.Log($"[Key] enter");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!button.IsInteractable())
            {
                return;
            }

            // Don't work yet
            //UnityEngine.Debug.Log($"[Key] exit");
            pointerDown.OnPointerExit(eventData);
        }
    }
}