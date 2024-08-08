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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui
{
    public class Key : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        KeyType type;
        Button button;
        bool buttonPressed;

        public event Action PointerDown;
        public event Action PointerUp;

        void Awake()
        {
            button = GetComponent<Button>();
            if (button == null) 
            {
                button = gameObject.AddComponent<Button>();
            }
            button.onClick.AddListener(OnPress);
        }

        private void OnEnable()
        {

        }

        private void OnDestroy()
        {

        }

        public virtual void OnPress()
        {
            if (!button.IsInteractable())
            {
                return;
            }

            UnityEngine.Debug.Log($"[Key] press");
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!button.IsInteractable())
            {
                return;
            }

            UnityEngine.Debug.Log($"[Key] down");
            buttonPressed = true;
            PointerDown?.Invoke();
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
            PointerUp?.Invoke();
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!button.IsInteractable())
            {
                return;
            }

            // Don't work yet
            UnityEngine.Debug.Log($"[Key] enter");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!button.IsInteractable())
            {
                return;
            }

            // Don't work yet
            UnityEngine.Debug.Log($"[Key] exit");
        }

        public void Enter()
        {
            if (!button.IsInteractable())
            {
                return;
            }

            // Don't work yet
            UnityEngine.Debug.Log($"[Key] enter 2");
        }

        public void Exit()
        {
            if (!button.IsInteractable())
            {
                return;
            }

            // Don't work yet
            UnityEngine.Debug.Log($"[Key] exit 2");
        }
    }
}