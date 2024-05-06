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

using System.Text.RegularExpressions;
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

        [SerializeField]
        private UnityGeckoWebView webView;

        [SerializeField]
        private InputField searchField;

        public void Search()
        {
            Debug.Assert(searchField != null);
            Debug.Assert(webView != null);

            if (validateURLRegex.IsMatch(searchField.text))
                webView.LoadUrl(searchField.text);
            else if (searchField.text.EndsWith(".com") || searchField.text.EndsWith(".net") || searchField.text.EndsWith(".fr") || searchField.text.EndsWith(".org"))
                webView.LoadUrl("http://" + searchField.text);
            else
                webView.LoadUrl("https://www.google.com/search?q=" + searchField.text);
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