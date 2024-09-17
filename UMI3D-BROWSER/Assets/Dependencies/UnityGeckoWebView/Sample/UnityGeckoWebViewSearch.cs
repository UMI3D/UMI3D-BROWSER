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
        TMPro.TMP_InputField searchField;

        Notifier searchNotifier;

        void Awake()
        {
            searchField = GetComponent<TMPro.TMP_InputField>();

            searchNotifier = NotificationHub.Default.GetNotifier(
                this,
                GeckoWebViewNotificationKeys.Search
            );
        }

        void OnEnable()
        {
            NotificationHub.Default.Subscribe(
                this,
                GeckoWebViewNotificationKeys.Loading,
                Loading
            );
        }

        void OnDisable()
        {
            NotificationHub.Default.Unsubscribe(this, GeckoWebViewNotificationKeys.Loading);
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
    }
}