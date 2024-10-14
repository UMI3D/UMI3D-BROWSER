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
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace com.inetum.unitygeckowebview
{
    /// <summary>
    /// Manages screen touches for a java web view.
    /// </summary>
    [RequireComponent(typeof(RawImage))]
    [RequireComponent(typeof(AndroidJavaWebview))]
    public class UnityGeckoWebViewInput : Selectable, IPointerMoveHandler
    {
        [Space]

        [Tooltip("Event raised when the pointer is down. Parameter is coordinates in pixels.")]
        public UnityEvent<Vector2> pointerDown = new();

        [Tooltip("Event raised when a text field has been selected in the web view.")]
        public UnityEvent textFieldSelected = new();
        [Tooltip("Event raised when the pointer is down without selected a text field in the web view.")]
        public UnityEvent textFieldUnSelected = new();

        [SerializeField, Tooltip("Simulate a click when a short pointer down is detected ?")]
        bool simulateClick = false;

        RectTransform rawImageRectTransform;
        AndroidJavaWebview javaWebView;

        int TextureWidth, TextureHeight;

        bool textSelectedLastFrame = false;

        #region Data

        /// <summary>
        /// World coordinates of raw image corners.
        /// </summary>
        Vector3[] coordinates = new Vector3[4];
        Vector3 up, right;

        /// <summary>
        /// Last time a up trigger was performed.
        /// </summary>
        float lastUp = 0;
        /// <summary>
        /// Time to consider that a trigger is a click.
        /// </summary>
        const int clickTime = 200;

        #endregion

        protected override void Awake()
        {
            if (Application.isEditor)
            {
                UnityEngine.Debug.LogError($"The Gecko Web View only works in android.");
                Destroy(this);
                return;
            }

            rawImageRectTransform = GetComponent<RectTransform>();
            javaWebView = GetComponent<AndroidJavaWebview>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            NotificationHub.Default.Subscribe(
                this,
                GeckoWebViewNotificationKeys.InteractibilityChanged,
                InteractibilityChanged
            );

            NotificationHub.Default.Subscribe(
                this,
                GeckoWebViewNotificationKeys.WebViewTextFieldSelected,
                WebViewTextFieldSelected
            );

            NotificationHub.Default.Subscribe<GeckoWebViewNotificationKeys.TextureSizeChanged>(
                this,
                TextureSizeChanged
            );
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            NotificationHub.Default.Unsubscribe(this, GeckoWebViewNotificationKeys.InteractibilityChanged);

            NotificationHub.Default.Unsubscribe(this, GeckoWebViewNotificationKeys.WebViewTextFieldSelected);

            NotificationHub.Default.Unsubscribe<GeckoWebViewNotificationKeys.TextureSizeChanged>(this);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            rawImageRectTransform.GetWorldCorners(coordinates);

            up = (coordinates[1] - coordinates[0]);
            right = (coordinates[3] - coordinates[0]);

            if (simulateClick)
            {
                SimulateClick(eventData);
                return;
            }

            Vector3 localPosition = WorldToLocal(eventData.pointerCurrentRaycast.worldPosition);
            pointerDown.Invoke(localPosition);
            javaWebView.PointerDown(localPosition.x, localPosition.y, eventData.pointerId);
            RaiseSelectionEvent();
        }

        async void SimulateClick(PointerEventData eventData)
        {
            await Task.Delay(clickTime);

            Vector3 localPosition = WorldToLocal(eventData.pointerCurrentRaycast.worldPosition);
            pointerDown.Invoke(localPosition);
            javaWebView.PointerDown(localPosition.x, localPosition.y, eventData.pointerId);
            UnityEngine.Debug.Log($"SimulateClick");
            RaiseSelectionEvent();

            if (Time.time - lastUp < clickTime / 1000f)
            {
                await Task.Delay(40);
                javaWebView.PointerUp(localPosition.x, localPosition.y, eventData.pointerId);
            }
        }

        async void RaiseSelectionEvent()
        {
            textSelectedLastFrame = false;

            await Task.Delay(1000);

            if (textSelectedLastFrame)
            {
                textFieldSelected.Invoke();
            }
            else
            {
                textFieldUnSelected.Invoke();
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            lastUp = Time.time;

            Vector3 localPosition = WorldToLocal(eventData.pointerCurrentRaycast.worldPosition);
            javaWebView.PointerUp(localPosition.x, localPosition.y, eventData.pointerId);
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            Vector3 localPosition = WorldToLocal(eventData.pointerCurrentRaycast.worldPosition);
            javaWebView.PointerMove(localPosition.x, localPosition.y, eventData.pointerId);
        }

        Vector3 WorldToLocal(Vector3 worldPosition)
        {
            Vector3 localPosition = worldPosition - coordinates[0];

            localPosition.x = Vector3.Dot(localPosition, right.normalized) / right.magnitude * TextureWidth;
            localPosition.y = Vector3.Dot(localPosition, up.normalized) / up.magnitude * TextureHeight;

            return localPosition;
        }

        void TextureSizeChanged(Notification notification)
        {
            if (!notification.TryGetInfoT(GeckoWebViewNotificationKeys.TextureSizeChanged.Size, out Vector2 size))
            {
                return;
            }

            TextureWidth = (int)size.x;
            TextureHeight = (int)size.y;
        }

        void InteractibilityChanged(Notification notification)
        {
            if (!notification.TryGetInfoT(GeckoWebViewNotificationKeys.Info.Interactable, out bool interactable))
            {
                return;
            }

            this.interactable = interactable;
        }

        void WebViewTextFieldSelected()
        {
            textSelectedLastFrame = true;
        }
    }
}