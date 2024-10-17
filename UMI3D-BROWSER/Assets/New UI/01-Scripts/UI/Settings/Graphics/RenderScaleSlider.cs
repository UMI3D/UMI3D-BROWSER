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
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.settings.graphics
{
    public class RenderScaleSlider : MonoBehaviour
    {
        [SerializeField] private UniversalRenderPipelineAsset pipelineAsset;
        [SerializeField] private Slider slider;

        private void Start()
        {
            slider.onValueChanged.AddListener(newValue => {
                pipelineAsset.renderScale = (float)Math.Round(newValue, 2);
            });

            slider.value = pipelineAsset.renderScale;
        }
    }
}