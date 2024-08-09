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

namespace umi3d.browserRuntime.ui
{
    public class PointerBehaviour : IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        /// <summary>
        /// Notification key for the count of click (down or up).<br/>
        /// Value is int.<br/>
        /// </summary>
        public const string NKCount = "Count";

        /// <summary>
        /// Notification key the long press.<br/>
        /// Value is bool.<br/>
        /// True means this is a long press.
        /// </summary>
        public const string NKIsLongPress = "IsLongPress";

        public const string NKIsImmediate = "NKIsImmediate";

        /// <summary>
        /// Event raised when the pointer is down.
        /// </summary>
        public event Action<Notification> pointerDown;

        /// <summary>
        /// Event raised when the pointer is up.
        /// </summary>
        public event Action<Notification> pointerUp;

        public float timeOut;
        public MonoBehaviour monoBehaviour;
        
        int numberOfDown = 0;
        int numberOfUp = 0;
        bool? isLongPress = null;
        float currentTimeOut;
        bool isTimeOut = false;

        Coroutine coroutine;

        public void OnPointerDown(PointerEventData eventData)
        {
            // Add 'timeOut' each time the pointer is down.
            // This way the user can do more than a double click.
            currentTimeOut += timeOut;

            if (coroutine == null || isTimeOut)
            {
                // If no update is being made yet start the coroutine.
                coroutine = monoBehaviour.StartCoroutine(Update());
            }

            // Increase the number of down.
            numberOfDown++;

            // Raise the 'pointerDown' event each time the pointer is down.
            pointerDown?.Invoke(new Notification(
                "PointerBehaviour",
                this,
                new()
                {
                    { NKCount, numberOfDown },
                    { NKIsLongPress, isLongPress.HasValue ? isLongPress.Value : false },
                    { NKIsImmediate, true }
                })
            );
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            //if (coroutine == null || isTimeOut)
            //{
            //    // If the time is out or the coroutine is null
            //    // then the number of up must be 1.
            //    numberOfUp = 1;
            //}
            //else
            //{
            //}
            // Increase the number of up.
            numberOfUp++;

            // Raise the 'pointerUp' event each time the pointer is up.
            pointerUp?.Invoke(new Notification(
                "PointerBehaviour",
                this,
                new()
                {
                    { NKCount, numberOfUp },
                    { NKIsLongPress, isLongPress.HasValue ? isLongPress.Value : false },
                    { NKIsImmediate, true }
                })
            );
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (coroutine != null)
            {
                monoBehaviour.StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        IEnumerator Update()
        {
            isTimeOut = false;

            // Reset all the fields.
            numberOfDown = 0;
            numberOfUp = 0;
            isLongPress = null;
            currentTimeOut = timeOut;

            float time = 0f;
            while (time < currentTimeOut)
            {
                yield return null;
                time += Time.deltaTime;
            }

            isLongPress = numberOfUp > 0;

            // After the 'timeOut' is reached a 'pointerDown' event is raised.
            // If only one down has been made it is a single click else it is a multi click.
            pointerDown?.Invoke(new Notification(
                "PointerBehaviour",
                this,
                new()
                {
                    { NKCount, numberOfDown },
                    { NKIsLongPress, isLongPress.Value },
                    { NKIsImmediate, false }
                })
            );

            if (numberOfUp > 0)
            {
                pointerUp?.Invoke(new Notification(
                    "PointerBehaviour",
                    this,
                    new()
                    {
                        { NKCount, numberOfUp },
                        { NKIsLongPress, isLongPress.Value },
                        { NKIsImmediate, false }
                    })
                );
            }

            isTimeOut = true;
        }
    }

    public class Key : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        Button button;

        public bool buttonPressed;
        public PointerBehaviour pointerBehaviour;
        public event Action PointerDown;
        public event Action PointerUp;
        public event Action PointerDoubleUp;

        void Awake()
        {
            button = GetComponent<Button>();
            if (button == null) 
            {
                button = gameObject.AddComponent<Button>();
            }

            pointerBehaviour = new()
            {
                monoBehaviour = this,
                timeOut = .5f
            };
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
            pointerBehaviour.OnPointerDown(eventData);
            PointerDown?.Invoke();
            NotificationHub.Default.Notify(this, KeyboardNotificationKeys.AskPreviewFocus);
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
            pointerBehaviour.OnPointerUp(eventData);
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
            pointerBehaviour.OnPointerExit(eventData);
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