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

using umi3d.browserRuntime.notificationKeys;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.settings
{
    [RequireComponent(typeof(Slider))]
    [RequireComponent(typeof(SettingsSliderControl))]
    internal class GraphicsRenderScaleSettings : MonoBehaviour
    {
        Slider slider;
        SettingsSliderControl sliderControl;

        GraphicsSettings graphicsSettings;

        private const float MIN_SLIDER_VALUE = 10;

        private const float MAX_SLIDER_VALUE = 200;

        private const float SLIDER_VALUE_TO_URP_RENDER_SCALE = 1 / 100f;

        private const float URP_RENDER_SCALE_TO_SLIDER_VALUE = 1 / SLIDER_VALUE_TO_URP_RENDER_SCALE;

        void Awake()
        {
            slider = GetComponent<Slider>();
            slider.minValue = MIN_SLIDER_VALUE;
            slider.maxValue = MAX_SLIDER_VALUE;

            sliderControl = GetComponent<SettingsSliderControl>();
            sliderControl.valueChanged += ValueChanged;

            graphicsSettings = GetComponentInParent<GraphicsSettings>();
        }

        void OnEnable()
        {
            if (graphicsSettings.model.quality != BrowserQualitySettings.Custom)
            {
                return;
            }

            slider.SetValueWithoutNotify(graphicsSettings.model.renderScale * URP_RENDER_SCALE_TO_SLIDER_VALUE);
        }

        void ValueChanged(float newValue)
        {
            newValue = Mathf.Clamp(newValue, MIN_SLIDER_VALUE, MAX_SLIDER_VALUE);
            newValue *= SLIDER_VALUE_TO_URP_RENDER_SCALE;

            UniversalRenderPipelineAsset urp = QualitySettings.renderPipeline as UniversalRenderPipelineAsset;
            urp.renderScale = newValue;
            graphicsSettings.model.renderScale = newValue;
        }
    }
}