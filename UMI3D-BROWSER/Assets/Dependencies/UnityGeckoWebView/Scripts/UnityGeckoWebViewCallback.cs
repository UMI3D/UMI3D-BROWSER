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

namespace com.inetum.unitygeckowebview
{
    /// <summary>
    /// Class which represents com.inetum.unitygeckowebview.UnityGeckoWebViewCallback class of the Android plugin.
    /// So this object can be sent directly to the native plugin.
    /// </summary>
    public class UnityGeckoWebViewCallback : AndroidJavaProxy
    {
        AndroidJavaWebview webView;

        Notifier loadingNotifier;

        public UnityGeckoWebViewCallback(AndroidJavaWebview webView) : base("com.inetum.unitygeckowebview.UnityGeckoWebViewCallback")
        {
            this.webView = webView;

            loadingNotifier = NotificationHub.Default.GetNotifier(
                this,
                GeckoWebViewNotificationKeys.Loading
            );
        }

        /// <summary>
        /// Notifies that a text input has been selected within the webview.
        /// </summary>
        public void onTextInputSelected()
        {
            if (webView is null)
                return;

            UnityGeckoWebView.actionsToRunOnMainThread.Enqueue(TextInputSelected);
        }

        /// <summary>
        /// Notifies that a web page has started loading.
        /// </summary>
        public void onUrlStartedLoading(string url)
        {
            if (webView is null)
                return;

            UnityGeckoWebView.actionsToRunOnMainThread.Enqueue(() => LoadingHasStarted(url));
        }

        /// <summary>
        /// Notify all subscribers that a text field has been selected.
        /// </summary>
        public void TextInputSelected()
        {
            NotificationHub.Default.Notify(
                this,
                GeckoWebViewNotificationKeys.WebViewTextFieldSelected
            );
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
    }
}