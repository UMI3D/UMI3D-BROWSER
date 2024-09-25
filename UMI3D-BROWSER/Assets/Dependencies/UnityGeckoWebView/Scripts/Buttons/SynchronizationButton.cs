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
    /// <summary>
    /// The purpose of the button is to send the url and the scroll position of the web page to the others users.<br/>
    /// <br/>
    /// When the admin click that button the record of the page start. The red dot appears to tell the user that its web page is shared to others.<br/>
    /// When another admin try to synch its page or if the user click one more time on the sync button then the synchronization stopped. The red button disappears.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class SynchronizationButton : MonoBehaviour
    {
        [SerializeField] AndroidJavaWebview webview;

        [Tooltip("Delay to send current url and scroll offset when user synchronizes his content. In seconds")]
        [SerializeField] float synchronizationDelay = 2f;

        RectTransform rectTransform;

        Button button;
        /// <summary>
        /// A feedback to show user he's currently sharing its content.
        /// </summary>
        Image recordPageFeedback;

        /// <summary>
        /// Whether the interaction with the synch button is allowed.
        /// </summary>
        bool isInteractable = true;

        /// <summary>
        /// Whether the web page is recorded.
        /// </summary>
        bool isRecording = false;
        bool IsRecording
        {
            get => isRecording;
            set
            {
                isRecording = value;
                recordPageFeedback.gameObject.SetActive(value);
                synchronizationNotifier[GeckoWebViewNotificationKeys.SynchronizationChanged.IsRecording] = value;
                synchronizationNotifier[GeckoWebViewNotificationKeys.SynchronizationChanged.Scroll] = new Vector2(currentScrollXPosition, currentScrollYPosition);
                synchronizationNotifier.Notify();
            }
        }

        /// <summary>
        /// Whether the user is a web view administrator.
        /// </summary>
        bool isAdmin = false;

        Notifier synchronizationNotifier;

        int currentScrollXPosition, currentScrollYPosition;

        /// <summary>
        /// Initial local scale of the object.
        /// </summary>
        Vector3 localScale;

        void Awake()
        {
            UnityEngine.Debug.Assert(webview != null, $"Java web view is not referenced.");

            button = GetComponent<Button>();
            recordPageFeedback = transform.GetChild(0).GetComponent<Image>();

            rectTransform = GetComponent<RectTransform>();

            localScale = rectTransform.localScale;

            synchronizationNotifier = NotificationHub.Default
                .GetNotifier<GeckoWebViewNotificationKeys.SynchronizationChanged>(this);

            button.interactable = false;
            IsRecording = false;
        }

        void Start()
        {
            StartCoroutine(SynchronizationCoroutine());
        }

        void OnEnable()
        {
            NotificationHub.Default.Subscribe<GeckoWebViewNotificationKeys.WebViewSizeChanged>(
                this, 
                WebViewSizeChanged
            );

            NotificationHub.Default.Subscribe(
               this,
               GeckoWebViewNotificationKeys.InteractibilityChanged,
               InteractibilityChanged
           );

            NotificationHub.Default.Subscribe<GeckoWebViewNotificationKeys.SynchronizationAdministrationChanged>(
               this,
               SynchronizationAdministrationChanged
           );

            NotificationHub.Default.Subscribe<GeckoWebViewNotificationKeys.Desynchronization>(
               this,
               Desynchronization
           );

            button.onClick.AddListener(ToggleSynchronization);
        }

        void OnDisable()
        {
            NotificationHub.Default.Unsubscribe<GeckoWebViewNotificationKeys.WebViewSizeChanged>(this);

            NotificationHub.Default.Unsubscribe(this, GeckoWebViewNotificationKeys.InteractibilityChanged);

            NotificationHub.Default.Unsubscribe<GeckoWebViewNotificationKeys.SynchronizationAdministrationChanged>(this);

            NotificationHub.Default.Unsubscribe<GeckoWebViewNotificationKeys.Desynchronization>(this);

            button.onClick.RemoveListener(ToggleSynchronization);
        }

        public void ToggleSynchronization()
        {
            IsRecording = !IsRecording;
        }

        void WebViewSizeChanged(Notification notification)
        {
            if (!notification.TryGetInfoT(GeckoWebViewNotificationKeys.WebViewSizeChanged.Scale, out Vector2 scale))
            {
                return;
            }

            float ratio = scale.x / scale.y;

            rectTransform.localScale = new Vector3(
                localScale.x / ratio,
                localScale.y,
                localScale.z
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

        void SynchronizationAdministrationChanged(Notification notification)
        {
            if (!notification.TryGetInfoT(GeckoWebViewNotificationKeys.SynchronizationAdministrationChanged.IsAdmin, out bool isAdmin))
            {
                return;
            }

            this.isAdmin = isAdmin;

            button.targetGraphic.enabled = isAdmin;
            if (!isAdmin)
            {
                IsRecording = false;
            }

            button.interactable = isInteractable && isAdmin;
        }

        void Desynchronization(Notification notification)
        {
            IsRecording = false;
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

                    if (IsRecording)
                    {
                        synchronizationNotifier[GeckoWebViewNotificationKeys.SynchronizationChanged.IsRecording] = true;
                        synchronizationNotifier[GeckoWebViewNotificationKeys.SynchronizationChanged.Scroll] = new Vector2(scrollX, scrollY);
                        synchronizationNotifier.Notify();
                    }
                }

                yield return wait;
            }
        }
    }
}