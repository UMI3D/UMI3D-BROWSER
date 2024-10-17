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
using UnityEngine;

namespace com.inetum.unitygeckowebview.samples
{
    [RequireComponent(typeof(TMPro.TMP_InputField))]
    public class UnityGeckoWebViewSearch : MonoBehaviour
    {
        RectTransform rectTransform;
        RectTransform parentRectTransform;
        TMPro.TMP_InputField searchField;

        Notifier searchNotifier;

        /// <summary>
        /// Initial local scale of the object.
        /// </summary>
        Vector3 localScale;
        /// <summary>
        /// Width ratio compared to its parent.
        /// </summary>
        float widthRatio;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            searchField = GetComponent<TMPro.TMP_InputField>();
            parentRectTransform = transform.parent.GetComponent<RectTransform>();

            searchNotifier = NotificationHub.Default.GetNotifier(
                this,
                GeckoWebViewNotificationKeys.Search
            );

            localScale = rectTransform.localScale;

            widthRatio = rectTransform.rect.width / parentRectTransform.rect.width;
        }

        void OnEnable()
        {
            NotificationHub.Default.Subscribe(
                this,
                GeckoWebViewNotificationKeys.Loading,
                Loading
            );

            NotificationHub.Default.Subscribe<GeckoWebViewNotificationKeys.WebViewSizeChanged>(
                this,
                WebViewSizeChanged
            );
        }

        void OnDisable()
        {
            NotificationHub.Default.Unsubscribe(this, GeckoWebViewNotificationKeys.Loading);

            NotificationHub.Default.Unsubscribe<GeckoWebViewNotificationKeys.WebViewSizeChanged>(this);
        }

        void Loading(Notification notification)
        {
            if (!notification.TryGetInfoT(GeckoWebViewNotificationKeys.Info.URL, out string url))
            {
                return;
            }

            searchField.SetTextWithoutNotify(url);
        }

        public void Search()
        {
            searchNotifier[GeckoWebViewNotificationKeys.Info.URL] = searchField.text;
            searchNotifier.Notify();
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

            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parentRectTransform.rect.width * scale.y * widthRatio);
        }
    }
}