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
    public class PointerUpBehaviour : PointerBehaviour, IPointerUpHandler
    {
        public void OnPointerUp(PointerEventData eventData)
        {
            if (isSimpleClick)
            {
                OnPointerClicked();
            }
            else
            {
                this.eventData = eventData;
                OnMultiClick();
            }
        }

        void OnMultiClick()
        {
            // Add 'timeOut' each time the pointer is up.
            // This way the user can do more than a double click.
            currentTimeOut += timeOut;

            if (coroutine == null || isTimeOut)
            {
                // If the count down has not started yet, start the coroutine.
                coroutine = StartCoroutine(ClickCountDown());
            }

            // Increase the number of click.
            numberOfClick++;

            // Raise the 'pointerClick' event each time the pointer is up.
            info[NKPointerEvent] = eventData;
            info[NKCount] = numberOfClick;
            info[NKIsImmediate] = true;
            OnPointerClicked(new Notification(
                "PointerUpBehaviour",
                this,
                info)
            );
        }

        IEnumerator ClickCountDown()
        {
            isTimeOut = false;

            // Reset all the fields.
            numberOfClick = 0;
            currentTimeOut = timeOut;

            float time = 0f;
            while (time < currentTimeOut)
            {
                yield return null;
                time += Time.deltaTime;
            }

            // After the 'timeOut' is reached a 'pointerClick' event is raised.
            // If only one down has been made it is a single click else it is a multi click.
            info[NKPointerEvent] = eventData;
            info[NKCount] = numberOfClick;
            info[NKIsImmediate] = false;
            OnPointerClicked(new Notification(
                "PointerUpBehaviour",
                this,
                info)
            );

            isTimeOut = true;
        }
    }
}