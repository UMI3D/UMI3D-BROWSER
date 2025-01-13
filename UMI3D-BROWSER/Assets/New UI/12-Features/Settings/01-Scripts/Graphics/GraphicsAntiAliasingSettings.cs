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
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.settings
{
    [RequireComponent(typeof(Button))]
    internal class GraphicsAntiAliasingSettings : MonoBehaviour
    {
        [SerializeField] int msaa;

        Button button;

        GraphicsSettings graphicsSettings;

        void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(Click);

            graphicsSettings = GetComponentInParent<GraphicsSettings>();
        }

        void OnEnable()
        {
            if (graphicsSettings.model == null || graphicsSettings.model.quality != BrowserQualitySettings.Custom)
            {
                return;
            }

            if (msaa == graphicsSettings.model.msaa)
            {
                button.onClick?.Invoke();
            }
        }

        private void Start()
        {
            if (graphicsSettings.model == null || graphicsSettings.model.quality != BrowserQualitySettings.Custom)
            {
                return;
            }

            if (msaa == graphicsSettings.model.msaa)
            {
                button.onClick?.Invoke();
            }
        }

        void Click()
        {
            UniversalRenderPipelineAsset urp = QualitySettings.renderPipeline as UniversalRenderPipelineAsset;
            urp.msaaSampleCount = msaa;
            graphicsSettings.model.msaa = msaa;
        }
    }
}