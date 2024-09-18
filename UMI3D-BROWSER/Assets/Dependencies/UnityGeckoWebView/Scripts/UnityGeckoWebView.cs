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
using System.Text.RegularExpressions;
using UnityEngine;

namespace com.inetum.unitygeckowebview
{
    [RequireComponent(typeof(AndroidJavaWebview))]
    public class UnityGeckoWebView : MonoBehaviour
    {
        public static System.Collections.Generic.Queue<Action> actionsToRunOnMainThread = new();

        /// <summary>
        /// Regex to check if a string is an url or not.
        /// </summary>
        Regex validateURLRegex = new("^https?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$");

        /// <summary>
        /// The android webview.
        /// </summary>
        AndroidJavaWebview webview;

        void Awake()
        {
            webview = GetComponent<AndroidJavaWebview>();
        }

        void OnEnable()
        {
            NotificationHub.Default.Subscribe(
                this,
                GeckoWebViewNotificationKeys.Search,
                Search
            );
        }

        void OnDisable()
        {
            NotificationHub.Default.Unsubscribe(this, GeckoWebViewNotificationKeys.Search);
        }

        void OnApplicationPause(bool pause)
        {
            if (!pause && !webview.isNull)
            {
                webview.NotifyOnResume();
            }
        }

        void Update()
        {
            if (actionsToRunOnMainThread.Count > 0)
            {
                while(actionsToRunOnMainThread.Count > 0)
                {
                    actionsToRunOnMainThread.Dequeue().Invoke();
                }
            }
        }

        void Search(Notification notification)
        {
            if (!notification.TryGetInfoT(GeckoWebViewNotificationKeys.Info.URL, out string url))
            {
                return;
            }

            LoadURLInternal(url);
        }

        void LoadURLInternal(string url)
        {
            if (validateURLRegex.IsMatch(url))
            {
                // Nothing to do.
            }
            else if (url.EndsWith(".com") || url.EndsWith(".net") || url.EndsWith(".fr") || url.EndsWith(".org"))
            {
                url = "http://" + url;
            }
            else
            {
                url = "https://www.google.com/search?q=" + url;
            }

            webview.LoadUrl(url);
        }
    }
}

