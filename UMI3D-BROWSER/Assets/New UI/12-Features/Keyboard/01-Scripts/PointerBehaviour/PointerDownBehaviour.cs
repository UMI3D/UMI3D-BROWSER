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
using UnityEngine;
using UnityEngine.EventSystems;

namespace umi3d.browserRuntime.ui
{
    public class PointerDownBehaviour : PointerBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        /// <summary>
        /// Notification key for whether this is a long press or not.<br/>
        /// Value is bool.<br/>
        /// </summary>
        public const string NKIsLongPress = "IsLongPress";

        protected bool? isLongPress = null;

        void Awake()
        {
            if (!isSimpleClick)
            {
                info = new()
                {
                    { NKCount, 1 },
                    { NKIsLongPress, false },
                    { NKIsImmediate, true }
                };
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (isSimpleClick)
            {
                OnPointerClicked();
            }
            else
            {
                OnMultiClick();
            }
        }

        void OnMultiClick()
        {
            // Add 'timeOut' each time the pointer is down.
            // This way the user can do more than a double click.
            currentTimeOut += timeOut;

            if (coroutine == null || isTimeOut)
            {
                // If the count down has not started yet, start the coroutine.
                coroutine = StartCoroutine(ClickCountDown());
            }

            // Increase the number of click.
            numberOfClick++;

            // Raise the 'pointerClick' event each time the pointer is down.
            info[NKCount] = numberOfClick;
            info[NKIsLongPress] = false;
            info[NKIsImmediate] = true;
            OnPointerClicked(new Notification(
                "PointerDownBehaviour",
                this,
                info)
            );
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isLongPress = false;
        }

        IEnumerator ClickCountDown()
        {
            isTimeOut = false;

            // Reset all the fields.
            numberOfClick = 0;
            currentTimeOut = timeOut;
            isLongPress = null;

            float time = 0f;
            while (time < currentTimeOut)
            {
                yield return null;
                time += Time.deltaTime;
            }

            isLongPress = isLongPress.HasValue ? isLongPress.Value : true;

            // After the 'timeOut' is reached a 'pointerClick' event is raised.
            // If only one down has been made it is a single click else it is a multi click.
            info[NKCount] = numberOfClick;
            info[NKIsLongPress] = isLongPress.Value;
            info[NKIsImmediate] = false;
            OnPointerClicked(new Notification(
                "PointerDownBehaviour",
                this,
                info)
            );

            isTimeOut = true;
        }
    }
}