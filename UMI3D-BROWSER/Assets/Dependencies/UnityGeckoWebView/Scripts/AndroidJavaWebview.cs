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

using System;
using UnityEngine;

namespace com.inetum.unitygeckowebview
{
    public class AndroidJavaWebview : MonoBehaviour
    {
        /// <summary>
        /// Webview handled natively.
        /// </summary>
        public AndroidJavaObject webView;

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
        }

        public void Dispose()
        {
            webView?.Dispose();
        }
    }
}