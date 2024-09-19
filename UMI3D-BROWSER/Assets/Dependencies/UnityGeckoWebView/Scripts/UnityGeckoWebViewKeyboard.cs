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
using UnityEngine.Events;

namespace com.inetum.unitygeckowebview
{
    public class UnityGeckoWebViewKeyboard : MonoBehaviour
    {
        AndroidJavaWebview javaWebview;

        bool isWebViewTextFieldSelected = false;

        public UnityEvent TextFieldSelected = new();

        void OnEnable()
        {
            NotificationHub.Default.Subscribe(
                this,
                GeckoWebViewNotificationKeys.WebViewTextFieldSelected,
                WebViewTextFieldSelected
            );
        }

        void OnDisable()
        {
            NotificationHub.Default.Unsubscribe(this, GeckoWebViewNotificationKeys.WebViewTextFieldSelected);
        }

        public void EnterText(string text)
        {
            if (!isWebViewTextFieldSelected)
            {
                return;
            }

            javaWebview.EnterText(text);
        }

        public void DeleteCharacter()
        {
            if (!isWebViewTextFieldSelected)
            {
                return;
            }

            javaWebview.DeleteCharacter();
        }

        public void EnterOrSubmit()
        {
            if (!isWebViewTextFieldSelected)
            {
                return;
            }

            javaWebview.EnterCharacter();
            isWebViewTextFieldSelected = false;
        }

        void WebViewTextFieldSelected()
        {
            isWebViewTextFieldSelected = true;
        }
    }
}