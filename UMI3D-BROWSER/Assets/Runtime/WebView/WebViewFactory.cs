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
using umi3d.cdk;
using System.Threading.Tasks;
using inetum.unityUtils.multiTarget;

namespace umi3d.runtimeBrowser.webView
{
    public class WebViewFactory : AbstractWebViewFactory
    {
        public MultiTargetReference<GameObject> template;

        public override async Task<AbstractUMI3DWebView> CreateWebView()
        {
            if (template.Reference == null)
            {
                return null;
            }

            GameObject go = Instantiate(template.Reference);
            AbstractUMI3DWebView view = go.GetComponent<AbstractUMI3DWebView>();

            await UMI3DAsyncManager.Yield();

            return view;
        }
    }
}