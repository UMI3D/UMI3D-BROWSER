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
    public class WebViewSizeTest : MonoBehaviour
    {
        Notifier sizeChangedNotifier;

        [SerializeField] TMPro.TMP_InputField width;
        [SerializeField] TMPro.TMP_InputField height;
        [SerializeField] Button resize;

        void Awake()
        {
            sizeChangedNotifier = NotificationHub.Default
                .GetNotifier<GeckoWebViewNotificationKeys.WebViewSizeChanged>(this);
        }

        void Start()
        {
            Notify();
        }

        void OnEnable()
        {
            resize.onClick.AddListener(Resize);

            NotificationHub.Default.Subscribe<GeckoWebViewNotificationKeys.WebViewSizeChanged>(
                this,
                new FilterByRef(FilterType.AcceptAllExcept, this),
                SizeChanged
            );
        }

        void OnDisable()
        {
            resize.onClick.RemoveListener(Resize);

            NotificationHub.Default.Unsubscribe<GeckoWebViewNotificationKeys.WebViewSizeChanged>(this);
        }

        void Notify()
        {
            if (!float.TryParse(this.width.text, out float width))
            {
                UnityEngine.Debug.LogError($"Cannot get size of web view.");
                return;
            }

            if (!float.TryParse(this.height.text, out float height))
            {
                UnityEngine.Debug.LogError($"Cannot get size of web view.");
                return;
            }

            sizeChangedNotifier[GeckoWebViewNotificationKeys.WebViewSizeChanged.Scale] = new Vector2(width, height);
            sizeChangedNotifier.Notify();
        }

        void Resize()
        {
            Notify();
        }

        void SizeChanged(Notification notification)
        {
            if (!notification.TryGetInfoT(GeckoWebViewNotificationKeys.WebViewSizeChanged.Scale, out Vector2 size))
            {
                return;
            }

            width.SetTextWithoutNotify(size.x.ToString());
            height.SetTextWithoutNotify(size.y.ToString());
        }
    }
}