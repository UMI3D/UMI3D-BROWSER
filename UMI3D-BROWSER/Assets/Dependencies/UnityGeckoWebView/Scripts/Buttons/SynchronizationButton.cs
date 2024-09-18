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
using UnityEngine.UI;

namespace com.inetum.unitygeckowebview
{
    [RequireComponent(typeof(Button))]
    public class SynchronizationButton : MonoBehaviour
    {
        [SerializeField] AndroidJavaWebview webview;

        [Tooltip("Delay to send current url and scroll offset when user synchronizes his content. In seconds")]
        [SerializeField] float synchronizationDelay = 2f;

        Button button;
        /// <summary>
        /// A feedback to show user he's currently sharing its content.
        /// </summary>
        Image feedbackImage;

        RectTransform rectTransform;

        bool isInteractable;

        bool isDesynchronized;
        bool IsDesynchronized
        {
            get => isDesynchronized;
            set
            {
                isDesynchronized = value;
                feedbackImage.gameObject.SetActive(value);
            }
        }
        bool isAdmin;

        Notifier synchronizationNotifier;

        int currentScrollXPosition, currentScrollYPosition;

        void Awake()
        {
            UnityEngine.Debug.Assert(webview != null, $"Java web view is not referenced.");

            button = GetComponent<Button>();
            feedbackImage = transform.GetChild(0).GetComponent<Image>();

            rectTransform = GetComponent<RectTransform>();

            button.interactable = false;
            IsDesynchronized = false;

            synchronizationNotifier = NotificationHub.Default.GetNotifier(
                this,
                GeckoWebViewNotificationKeys.SynchronizationChanged
            );
        }

        void Start()
        {
            StartCoroutine(SynchronizationCoroutine());
        }

        void OnEnable()
        {
            NotificationHub.Default.Subscribe(
                this,
                GeckoWebViewNotificationKeys.SizeChanged,
                SizeChanged
            );

            NotificationHub.Default.Subscribe(
               this,
               GeckoWebViewNotificationKeys.InteractibilityChanged,
               InteractibilityChanged
           );

            NotificationHub.Default.Subscribe(
               this,
               GeckoWebViewNotificationKeys.SynchronizationChanged,
               SynchronizationChanged
           );
        }

        void OnDisable()
        {
            NotificationHub.Default.Unsubscribe(this, GeckoWebViewNotificationKeys.SizeChanged);

            NotificationHub.Default.Unsubscribe(this, GeckoWebViewNotificationKeys.InteractibilityChanged);

            NotificationHub.Default.Unsubscribe(this, GeckoWebViewNotificationKeys.SynchronizationChanged);
        }

        public void ToggleSynchronization()
        {
            IsDesynchronized = !IsDesynchronized;

            synchronizationNotifier[GeckoWebViewNotificationKeys.Info.IsSynchronizing] = true;
            synchronizationNotifier[GeckoWebViewNotificationKeys.Info.Vector2] = new Vector2(float.NaN, float.NaN);
            synchronizationNotifier.Notify();
        }

        void SizeChanged(Notification notification)
        {
            if (!notification.TryGetInfoT(GeckoWebViewNotificationKeys.Info.Vector2, out Vector2 size))
            {
                return;
            }

            rectTransform.localScale = new Vector3(
                rectTransform.localScale.x / size.x,
                rectTransform.localScale.y,
                rectTransform.localScale.z
            );
        }

        void InteractibilityChanged(Notification notification)
        {
            if (!notification.TryGetInfoT(GeckoWebViewNotificationKeys.Info.Interactable, out bool interactable))
            {
                return;
            }

            isInteractable = interactable;

            button.interactable = isInteractable && isAdmin;
        }

        void SynchronizationChanged(Notification notification)
        {
            if (!notification.TryGetInfoT(GeckoWebViewNotificationKeys.Info.IsAdmin, out bool isAdmin))
            {
                return;
            }

            if (!notification.TryGetInfoT(GeckoWebViewNotificationKeys.Info.IsDesynchronized, out bool isDesynchronized))
            {
                return;
            }

            this.isAdmin = isAdmin;
            IsDesynchronized = this.isDesynchronized;

            button.interactable = isInteractable && isAdmin;
        }

        /// <summary>
        /// If user is synchronizing his view, send his current url and scroll offset.
        /// </summary>
        /// <returns></returns>
        IEnumerator SynchronizationCoroutine()
        {
            var wait = new WaitForSeconds(synchronizationDelay);

            while (true)
            {
                int scrollY = webview.GetScrollY();
                int scrollX = webview.GetScrollX();

                if (currentScrollXPosition != scrollX || currentScrollYPosition != scrollY)
                {
                    currentScrollXPosition = scrollX;
                    currentScrollYPosition = scrollY;

                    synchronizationNotifier[GeckoWebViewNotificationKeys.Info.IsSynchronizing] = false;
                    synchronizationNotifier[GeckoWebViewNotificationKeys.Info.Vector2] = new Vector2(scrollX, scrollY);
                    synchronizationNotifier.Notify();
                }

                yield return wait;
            }
        }
    }
}