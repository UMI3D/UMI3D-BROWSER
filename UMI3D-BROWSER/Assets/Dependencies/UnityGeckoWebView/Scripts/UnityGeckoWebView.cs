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

        Notifier loadingNotifier;

        void Awake()
        {
            webview = GetComponent<AndroidJavaWebview>();

            loadingNotifier = NotificationHub.Default.GetNotifier(
                this,
                GeckoWebViewNotificationKeys.Loading
            );
        }

        void OnEnable()
        {
            NotificationHub.Default.Subscribe(
                this,
                GeckoWebViewNotificationKeys.History,
                HistoryButtonPressed
            );

            NotificationHub.Default.Subscribe(
                this,
                GeckoWebViewNotificationKeys.Search,
                Search
            );
        }

        void OnDisable()
        {
            NotificationHub.Default.Unsubscribe(this, GeckoWebViewNotificationKeys.History);

            NotificationHub.Default.Unsubscribe(this, GeckoWebViewNotificationKeys.Search);
        }

        void OnApplicationPause(bool pause)
        {
            if (!pause && webview.webView != null)
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

        /// <summary>
        /// Notify all subscribers that a text field has been selected.
        /// </summary>
        public void TextInputSelected()
        {
            //NotificationHub.Default.Notify(
            //    this,
            //    KeyboardNotificationKeys.TextFieldSelected
            //);
        }

        /// <summary>
        /// Notify all subscribers that a web page has started loading.
        /// </summary>
        /// <param name="url"></param>
        public void LoadingHasStarted(string url)
        {
            loadingNotifier[GeckoWebViewNotificationKeys.Info.URL] = url;
            loadingNotifier.Notify();
        }

        void HistoryButtonPressed(Notification notification)
        {
            if (!notification.TryGetInfoT(GeckoWebViewNotificationKeys.Info.BackwardOrForward, out History historyType))
            {
                return;
            }

            switch (historyType)
            {
                case History.Backward:
                    webview.GoBack();
                    break;
                case History.Forward:
                    webview.GoForward();
                    break;
                default:
                    UnityEngine.Debug.LogError($"Unhandled case.");
                    break;
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

