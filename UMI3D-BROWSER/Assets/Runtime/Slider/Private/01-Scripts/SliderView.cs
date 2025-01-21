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
using inetum.unityUtils.observation;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.slider
{
    [RequireComponent(typeof(Slider))]
    public class SliderView : MonoBehaviour
    {
        Slider _slider;

        SliderModelContainer _modelContainer;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
            _modelContainer = GetComponentInParent<SliderModelContainer>();

            _slider.onValueChanged.AddListener(OnValueChanged);

            NotificationHub.Default.Subscribe(this,
                ID.FromType<SliderNotifiactionKeys.SliderSet>(),
                (Callback)SliderSet,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _modelContainer.model));
        }

        private void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        private void OnValueChanged(float newValue)
        {
            _modelContainer.model.UpdateValue(newValue);
        }

        private void SliderSet(Notification notification)
        {
            if (notification.TryGetInfoT(SliderNotifiactionKeys.SliderSet.MaxValue, out float maxValue))
            {
                _slider.maxValue = maxValue;
            }

            if (notification.TryGetInfoT(SliderNotifiactionKeys.SliderSet.MinValue, out float minValue))
            {
                _slider.minValue = minValue;
            }

            if (notification.TryGetInfoT(SliderNotifiactionKeys.SliderSet.Value, out float value))
            {
                _slider.value = value;
            }
        }
    }
}