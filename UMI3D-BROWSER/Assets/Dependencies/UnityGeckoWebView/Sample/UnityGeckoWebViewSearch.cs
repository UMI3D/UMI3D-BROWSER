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
using System.Text.RegularExpressions;
using umi3d.browserRuntime.NotificationKeys;
using UnityEngine;
using UnityEngine.UI;

namespace com.inetum.unitygeckowebview.samples
{
    public class UnityGeckoWebViewSearch : MonoBehaviour
    {
        /// <summary>
        /// Regex to check if a string is an url or not.
        /// </summary>
        private Regex validateURLRegex = new Regex("^https?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$");

        TMPro.TMP_InputField searchField;

        Notifier searchNotifier;

        void Awake()
        {
            searchField = GetComponent<TMPro.TMP_InputField>();

            searchNotifier = NotificationHub.Default.GetNotifier(
                this,
                WebviewNotificationKeys.Search,
                null,
                new()
            );
        }

        public void Search()
        {
            string search = searchField.text;

            if (validateURLRegex.IsMatch(search))
            {
                // Nothing to do.
            }
            else if (search.EndsWith(".com") || search.EndsWith(".net") || search.EndsWith(".fr") || search.EndsWith(".org"))
            {
                search = "http://" + search;
            }
            else
            {
                search = "https://www.google.com/search?q=" + search;
            }

            searchNotifier[WebviewNotificationKeys.Info.URL] = search;
            searchNotifier.Notify();
        }

        public void OnUrlLoaded(string url)
        {
            if (searchField != null)
            {
                searchField.SetTextWithoutNotify(url);
            }
        }
    }
}