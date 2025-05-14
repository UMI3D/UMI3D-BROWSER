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

using TMPro;
using UnityEngine;

namespace umi3d.browserRuntime.ui
{
    [RequireComponent(typeof(TMP_Text))]
    public class SliderValueView : MonoBehaviour, IValueObserver<float>
    {
        TMP_Text _text;

        SliderController _controller;
        SliderModel _model;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
            _controller = GetComponentInParent<SliderController>();
            _model = _controller.model;
            _model.Subscribe(this);
        }

        private void OnDestroy()
        {
            _model?.Unsubscribe(this);
        }

        public void updateValue(float value)
        {
            _text.text = value.ToString();
        }
    }
}