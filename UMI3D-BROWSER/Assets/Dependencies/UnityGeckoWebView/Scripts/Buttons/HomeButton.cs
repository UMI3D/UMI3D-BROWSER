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
    public class HomeButton : MonoBehaviour
    {
        [SerializeField] string homeURL;

        Button button;
        RectTransform rectTransform;
        Notifier homeNotifier;

        /// <summary>
        /// Initial local scale of the object.
        /// </summary>
        Vector3 localScale;

        void Awake()
        {
            button = GetComponent<Button>();
            rectTransform = GetComponent<RectTransform>();

            homeNotifier = NotificationHub.Default.GetNotifier(
                this,
                GeckoWebViewNotificationKeys.Search
            );

            localScale = rectTransform.localScale;
        }

        void OnEnable()
        {
            button.onClick.AddListener(Click);

            NotificationHub.Default.Subscribe<GeckoWebViewNotificationKeys.WebViewSizeChanged>(
               this,
               SizeChanged
           );

            NotificationHub.Default.Subscribe(
               this,
               GeckoWebViewNotificationKeys.InteractibilityChanged,
               InteractibilityChanged
           );
        }

        void OnDisable()
        {
            button.onClick.RemoveListener(Click);

            NotificationHub.Default.Unsubscribe<GeckoWebViewNotificationKeys.WebViewSizeChanged>(this);

            NotificationHub.Default.Unsubscribe(this, GeckoWebViewNotificationKeys.InteractibilityChanged);
        }

        void Click()
        {
            homeNotifier[GeckoWebViewNotificationKeys.Info.URL] = homeURL;
            homeNotifier.Notify();
        }

        void SizeChanged(Notification notification)
        {
            if (!notification.TryGetInfoT(GeckoWebViewNotificationKeys.WebViewSizeChanged.Scale, out Vector2 size))
            {
                return;
            }

            float ratio = size.x / size.y;

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

            button.interactable = interactable;
        }
    }
}