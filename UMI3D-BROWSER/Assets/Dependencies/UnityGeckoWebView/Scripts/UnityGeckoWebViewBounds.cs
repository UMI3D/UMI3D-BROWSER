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
using UnityEngine;

namespace com.inetum.unitygeckowebview
{
    /// <summary>
    /// Represents <see cref="UnityGeckoWebView"/> bounds to optimize its rendering (only renderer it if visible). 
    /// </summary>
    public class UnityGeckoWebViewBounds : MonoBehaviour
    {
        [SerializeField]
        private UnityGeckoWebView webview;

        private void Start()
        {
            Debug.Assert(webview != null, "Web view should not be null");
        }

        private void OnBecameVisible()
        {
            webview?.StartRendering();
        }

        private void OnBecameInvisible()
        {
            webview?.StopRendering();
        }
    }
}