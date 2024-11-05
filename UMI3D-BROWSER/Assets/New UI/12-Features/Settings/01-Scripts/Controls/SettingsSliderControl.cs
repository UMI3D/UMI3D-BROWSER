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
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.settings
{
    [RequireComponent(typeof(Slider))]
    internal class SettingsSliderControl : MonoBehaviour
    {
        public event Action<float> valueChanged;

        [SerializeField] string suffix;
        [SerializeField] bool isRounded;
        [SerializeField] int decimalPoint;

        Slider slider;
        TMP_Text text;

        void Awake()
        {
            slider = GetComponent<Slider>();
            text = GetComponentInChildren<TMP_Text>();
        }

        void Start()
        {
            SetText(slider.value);

            slider.onValueChanged.AddListener(SetText);
        }

        public void SetText(float value)
        {
            if (isRounded)
            {
                value = (float)Math.Round(value, decimalPoint);
            }

            slider.SetValueWithoutNotify(value);
            text.text = $"{value}{suffix}";
            valueChanged?.Invoke(value);
        }
    }
}