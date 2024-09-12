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
using umi3d.browserRuntime.NotificationKeys;
using Unity.Collections;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace com.inetum.unitygeckowebview
{
    [RequireComponent(typeof(AndroidJavaWebview))]
    public class UnityGeckoWebView : MonoBehaviour
    {
        /// <summary>
        /// The android webview.
        /// </summary>
        AndroidJavaWebview webview;

        public static System.Collections.Generic.Queue<Action> actionsToRunOnMainThread = new();

       
        [Tooltip("Notifies that a web page has started loading.")]
        public UnityEvent<string> OnUrlStartedLoading = new();


        void Awake()
        {
            webview = GetComponent<AndroidJavaWebview>();
        }

        void OnEnable()
        {
            NotificationHub.Default.Subscribe(
                this,
                "Search",
                LoadUrl
            );
        }

        void OnDisable()
        {
            NotificationHub.Default.Unsubscribe(this, "Search");
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
            NotificationHub.Default.Notify(
                this,
                KeyboardNotificationKeys.TextFieldSelected
            );
        }

        void LoadUrl(Notification notification)
        {
            if (!notification.TryGetInfoT("URL", out string url))
            {
                return;
            }

            webview.LoadUrl(url);
        }
    }
}

