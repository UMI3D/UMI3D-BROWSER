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
using System.Security.Policy;
using UnityEngine;

namespace com.inetum.unitygeckowebview
{
    public class AndroidJavaWebview : MonoBehaviour
    {
        /// <summary>
        /// Webview handled natively.
        /// </summary>
        AndroidJavaObject webView;

        Notifier loadingNotifier;
        bool isInit = false;

        public bool isNull => webView == null;

        void Awake()
        {
            loadingNotifier = NotificationHub.Default.GetNotifier(
               this,
               GeckoWebViewNotificationKeys.Loading
           );
        }

        void Start()
        {
            if (Application.isEditor)
            {
                UnityEngine.Debug.LogError($"The Gecko Web View only works in android.");
                Destroy(this);
            }
        }

        void OnEnable()
        {
            NotificationHub.Default.Subscribe(
                this,
                GeckoWebViewNotificationKeys.ScrollChanged,
                ScrollChanged
            );

            NotificationHub.Default.Subscribe(
                this,
                GeckoWebViewNotificationKeys.History,
                HistoryButtonPressed
            );
        }

        void OnDisable()
        {
            NotificationHub.Default.Unsubscribe(this, GeckoWebViewNotificationKeys.ScrollChanged);

            NotificationHub.Default.Unsubscribe(this, GeckoWebViewNotificationKeys.History);
        }

        public void Init(int width, int height, bool useNativeKeyboard, AndroidJavaObject byteBufferJavaObject)
        {
            if (isInit)
            {
                return;
            }

            using (var baseClass = new AndroidJavaClass("com.inetum.unitygeckowebview.UnityGeckoWebView"))
            {
                webView = baseClass.CallStatic<AndroidJavaObject>(
                    "createWebView",
                    width,
                    height,
                    useNativeKeyboard,
                    new UnityGeckoWebViewCallback(this),
                    byteBufferJavaObject
                );
            }
        }

        public void GoBack()
        {
            webView?.Call("goBack");
        }

        public void GoForward()
        {
            webView.Call("goForward");
        }

        /// <summary>
        /// Loads an url.
        /// </summary>
        /// <param name="url"></param>
        public void LoadUrl(string url)
        {
            webView?.Call("loadUrl", url);
        }

        /// <summary>
        /// Enters text in webview.
        /// </summary>
        /// <param name="text"></param>
        public void EnterText(string text)
        {
            webView?.Call("enterText", text);
        }

        public void DeleteCharacter()
        {
            webView?.Call("deleteCharacter");
        }

        public void EnterCharacter()
        {
            webView?.Call("enterKey");
        }

        public void Click(float x, float y, int pointerId)
        {
            webView?.Call("click", x, y, pointerId);
        }

        public void PointerDown(float x, float y, int pointerId)
        {
            webView?.Call("pointerDown", x, y, pointerId);
        }

        public void PointerUp(float x, float y, int pointerId)
        {
            webView?.Call("pointerUp", x, y, pointerId);
        }

        public void PointerMove(float x, float y, int pointerId)
        {
            webView?.Call("pointerMove", x, y, pointerId);
        }

        public void Scroll(int scrollX, int scrollY)
        {
            webView?.Call("scroll", scrollX, scrollY);
        }

        public void SetScroll(int scrollX, int scrollY)
        {
            webView?.Call("setScroll", scrollX, scrollY);
        }

        public int GetScrollY()
        {
            return webView?.Call<int>("getScrollY") ?? 0;
        }

        public int GetScrollX()
        {
            return webView?.Call<int>("getScrollX") ?? 0;
        }

        public void Render()
        {
            webView?.Call("render");
        }

        public void ChangeTextureSize(int width, int height, AndroidJavaObject byteBufferJavaObject)
        {
            webView?.Call("changeTextureSize", width, height, byteBufferJavaObject);
        }

        public void NotifyOnResume()
        {
            webView.Call("notifyOnResume");
        }

        public void Destroy()
        {
            webView?.Call("destroy");
            isInit = false;
        }

        public void Dispose()
        {
            webView?.Dispose();
            isInit = false;
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

        #region Notifications

        void ScrollChanged(Notification notification)
        {
            if (!notification.TryGetInfoT(GeckoWebViewNotificationKeys.Info.Vector2, out Vector2 scroll))
            {
                return;
            }

            try
            {
                SetScroll((int)scroll.x, (int)scroll.y);
            }
            catch (Exception ex)
            {
                Debug.LogError("Impossible to set offset");
                Debug.LogException(ex);
            }
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
                    GoBack();
                    break;
                case History.Forward:
                    GoForward();
                    break;
                default:
                    UnityEngine.Debug.LogError($"Unhandled case.");
                    break;
            }
        }

        #endregion
    }
}