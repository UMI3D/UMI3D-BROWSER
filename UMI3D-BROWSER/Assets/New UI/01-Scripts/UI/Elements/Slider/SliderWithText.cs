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


using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace umi3dBrowsers.displayer
{
    public class SliderWithText : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;
        [SerializeField] Slider slider;

        private void Start()
        {
            SetText(slider.value);

            slider.onValueChanged.AddListener(value => {
                SetText(value);
            });
        }

        /// <summary>
        /// Call it before start
        /// </summary>
        public void Init(float value)
        {
            SetText(value);
            slider.value = value;
        }

        public void SetText(float value)
        {
            text.SetText(value.ToString());
        }
    }
}

