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
using Unity.VisualScripting;
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

            //keyboard.DeactivateShift();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            UnityEngine.Debug.Log($"down");
            buttonPressed = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            // Don't work yet
            UnityEngine.Debug.Log($"up");
            buttonPressed = false;
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            // Don't work yet
            UnityEngine.Debug.Log($"enter");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // Don't work yet
            UnityEngine.Debug.Log($"exit");
        }

        public void Enter()
        {
            // Don't work yet
            UnityEngine.Debug.Log($"enter 2");
        }

        public void Exit()
        {
            // Don't work yet
            UnityEngine.Debug.Log($"exit 2");
        }
    }
}