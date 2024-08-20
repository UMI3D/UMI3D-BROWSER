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
using UnityEngine;
using UnityEngine.EventSystems;

namespace umi3d.browserRuntime.ui
{
    public class PointerBehaviour : MonoBehaviour, IPointerExitHandler
    {
        /// <summary>
        /// Notification key for the count of click.<br/>
        /// Value is int.<br/>
        /// </summary>
        public const string NKCount = "Count";

        /// <summary>
        /// Notification key for whether this event has been send immediately or after a delay.
        /// Value is bool.<br/>
        /// </summary>
        public const string NKIsImmediate = "NKIsImmediate";

        /// <summary>
        /// Event raised when the pointer is down if this is <see cref="PointerDownBehaviour"/> or up if this is <see cref="PointerUpBehaviour"/>.
        /// </summary>
        public event Action<Notification> pointerClicked;
        /// <summary>
        /// Event raised when the pointer is down if this is <see cref="PointerDownBehaviour"/> or up if this is <see cref="PointerUpBehaviour"/>.
        /// </summary>
        public event Action pointerClickedSimple;

        [SerializeField] protected bool isSimpleClick = true;
        [SerializeField] protected float timeOut = .5f;

        protected int numberOfClick = 0;
        protected float currentTimeOut;
        protected bool isTimeOut = false;

        protected Coroutine coroutine;
        protected Dictionary<string, object> info;

        protected virtual void OnEnable()
        {
            
        }

        protected virtual void OnDisable()
        {
            
        }

        protected void OnPointerClicked(Notification notification)
        {
            pointerClicked?.Invoke(notification);
        }
        protected void OnPointerClicked()
        {
            pointerClickedSimple?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }
    }
}