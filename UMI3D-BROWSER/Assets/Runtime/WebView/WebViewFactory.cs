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
using BrowserDesktop;
using inetum.unityUtils.multiTarget;
using System.Threading.Tasks;
using umi3d.cdk;
using UnityEngine;
using Process = System.Diagnostics.Process;

namespace umi3d.runtimeBrowser.webView
{
    public class WebViewFactory : AbstractWebViewFactory
    {
        public MultiTargetReference<GameObject> template;

#if UNITY_STANDALONE_WIN
        /// <summary>
        /// Last time a webview was created.
        /// </summary>
        static float lastTimeWebViewCreated = 0;

        /// <summary>
        /// Delay in seconds between web view creation.
        /// </summary>
        readonly float creationDelay = 3f;

        protected override void OnDestroy()
        {
            base.OnDestroy();

            // Name of process launched in background by webviews.
            const string webEngineProcessName = "UnityWebBrowser.Engine.Cef";
            Process[] processes = Process.GetProcessesByName(webEngineProcessName);

            Debug.Log($"{nameof(RuntimeWebBrowserBasic)} : process to kill " + processes.Length);

#if UNITY_EDITOR
            foreach (Process process in Process.GetProcessesByName(webEngineProcessName))
            {
                process.Kill();
            }
#endif
        }
#endif

        public override async Task<AbstractUMI3DWebView> CreateWebView()
        {
            GameObject template = this.template.Reference;
            if (template == null)
            {
                return null;
            }

#if UNITY_STANDALONE_WIN
            while (lastTimeWebViewCreated != 0 && lastTimeWebViewCreated + creationDelay > Time.time)
            {
                await UMI3DAsyncManager.Yield();
            }

            lock (this)
            {
                lastTimeWebViewCreated = Time.time;

                GameObject go = Instantiate(template);
                return go.GetComponent<AbstractUMI3DWebView>();
            }
#elif UNITY_ANDROID
            GameObject go = Instantiate(template);
            AbstractUMI3DWebView view = go.GetComponent<AbstractUMI3DWebView>();

            await UMI3DAsyncManager.Yield();

            return view;
#endif
        }
    }
}