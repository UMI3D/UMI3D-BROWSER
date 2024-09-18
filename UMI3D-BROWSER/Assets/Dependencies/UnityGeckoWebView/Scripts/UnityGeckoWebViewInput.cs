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
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace com.inetum.unitygeckowebview
{
    /// <summary>
    /// Manages screen touches for a java web view.
    /// </summary>
    [RequireComponent(typeof(RawImage))]
    [RequireComponent(typeof(AndroidJavaWebview))]
    [RequireComponent(typeof(UnityGeckoWebViewRendering))]
    public class UnityGeckoWebViewInput : Selectable, IPointerMoveHandler
    {
        #region Fields

        RectTransform rawImageRectTransform;
        AndroidJavaWebview javaWebView;
        UnityGeckoWebViewRendering webviewRendering;

        [SerializeField, Tooltip("Simulate a click when a short pointer down is detected ?")]
        bool simulateClick = false;

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

        #endregion


        protected override void Awake()
        {
            rawImageRectTransform = GetComponent<RectTransform>();
            javaWebView = GetComponent<AndroidJavaWebview>();
            webviewRendering = GetComponent<UnityGeckoWebViewRendering>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            NotificationHub.Default.Subscribe(
               this,
               GeckoWebViewNotificationKeys.InteractibilityChanged,
               InteractibilityChanged
           );
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            NotificationHub.Default.Unsubscribe(this, GeckoWebViewNotificationKeys.InteractibilityChanged);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            rawImageRectTransform.GetWorldCorners(coordinates);

            up = (coordinates[1] - coordinates[0]);
            right = (coordinates[3] - coordinates[0]);

            if (simulateClick)
            {
                SimulateClick(eventData);
            }
            else
            {
                Vector3 localPosition = WorldToLocal(eventData.pointerCurrentRaycast.worldPosition);
                javaWebView.PointerDown(localPosition.x, localPosition.y, eventData.pointerId);
            }
        }

        async void SimulateClick(PointerEventData eventData)
        {
            Vector3 localPosition = WorldToLocal(eventData.pointerCurrentRaycast.worldPosition);

            await Task.Delay(clickTime);

            if (Time.time - lastUp < clickTime / 1000f)
            {
                javaWebView.PointerDown(localPosition.x, localPosition.y, eventData.pointerId);
                await Task.Delay(40);
                javaWebView.PointerUp(localPosition.x, localPosition.y, eventData.pointerId);
            }
            else
            {
                javaWebView.PointerDown(localPosition.x, localPosition.y, eventData.pointerId);
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            Vector3 localPosition = WorldToLocal(eventData.pointerCurrentRaycast.worldPosition);
            javaWebView.PointerUp(localPosition.x, localPosition.y, eventData.pointerId);

            lastUp = Time.time;
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            Vector3 localPosition = WorldToLocal(eventData.pointerCurrentRaycast.worldPosition);
            javaWebView.PointerMove(localPosition.x, localPosition.y, eventData.pointerId);
        }

        public void OnClick(PointerEventData eventData)
        {
            if (simulateClick)
            {
                SimulateClick(eventData);
            }
            else
            {
                Vector3 localPosition = WorldToLocal(eventData.pointerCurrentRaycast.worldPosition);
                javaWebView.Click(localPosition.x, localPosition.y, eventData.pointerId);
            }
        }

        Vector3 WorldToLocal(Vector3 worldPosition)
        {
            Vector3 localPosition = worldPosition - coordinates[0];

            localPosition.x = Vector3.Dot(localPosition, right.normalized) / right.magnitude * webviewRendering.width;
            localPosition.y = Vector3.Dot(localPosition, up.normalized) / up.magnitude * webviewRendering.height;

            return localPosition;
        }

        void InteractibilityChanged(Notification notification)
        {
            if (!notification.TryGetInfoT(GeckoWebViewNotificationKeys.Info.Interactable, out bool interactable))
            {
                return;
            }

            this.interactable = interactable;
        }
    }
}