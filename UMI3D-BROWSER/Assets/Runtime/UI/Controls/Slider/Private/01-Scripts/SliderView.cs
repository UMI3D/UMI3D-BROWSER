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
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui
{
    [RequireComponent(typeof(Slider)), ExecuteInEditMode]
    public class SliderView : MonoBehaviour, IValueObserver<float>, ISliderRangeObserver, ISliderWholeNumbersObserver
    {
        Slider _slider;

        SliderController _controller;
        SliderModel _model;

        void Awake()
        {
            _slider = GetComponent<Slider>();
            _slider.onValueChanged.AddListener(OnValueChanged);

            _controller = GetComponentInParent<SliderController>();
            _model = _controller.model;
            _model.Subscribe(this as IValueObserver<float>);
            _model.Subscribe(this as ISliderRangeObserver);
            _model.Subscribe(this as ISliderWholeNumbersObserver);
        }

        void OnEnable()
        {
            _slider.Select();
        }

        void OnDestroy()
        {
            _model.Unsubscribe(this as IValueObserver<float>);
            _model.Unsubscribe(this as ISliderRangeObserver);
            _model.Unsubscribe(this as ISliderWholeNumbersObserver);
        }

        void OnValueChanged(float newValue)
        {
            _controller.ValueUpdated(newValue);
        }

        public void updateValue(float value)
        {
            _slider.SetValueWithoutNotify(value);
        }

        public void UpdateRange(float min, float max)
        {
            _slider.minValue = min;
            _slider.maxValue = max;
        }

        public void UpdateWholeNumbers(bool wholeNumbers)
        {
            _slider.wholeNumbers = wholeNumbers;
        }
    }
}