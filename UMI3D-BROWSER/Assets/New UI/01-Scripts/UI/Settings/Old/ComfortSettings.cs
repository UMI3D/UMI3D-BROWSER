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
using System.Collections;
using System.Collections.Generic;
using umi3dBrowsers.displayer;
using umi3dVRBrowsersBase.tutorial.fakeServer;
using UnityEngine;
using UnityEngine.UI;

namespace umi3dBrowsers.containere
{
    public class ComfortSettings : MonoBehaviour
    {
        [Header("TP fade")]
        [SerializeField] private bool isFadingWhenTP;
        [SerializeField] private RadioButtonGroup fadingTP_RB;
        [Space]
        [SerializeField] private float fadeSpeed;
        [SerializeField] private Slider fadeSlider;

        [Header("reorient")]
        [SerializeField] private bool isFadingWhenRiorient;
        [SerializeField] private RadioButtonGroup fadingRiorient_RB;

        public event Action<bool> OnTpFadingChanged;
        public event Action<bool> OnRiorientFadingChanged;
        public event Action<float> OnFadingValueChanged;

        private void Awake()
        {
            fadingTP_RB.OnSelectedButtonChanged += (sender, value) =>
            {
                if (value == 0)
                    isFadingWhenTP = false;
                else if (value == 1)
                    isFadingWhenTP = true;

                OnTpFadingChanged?.Invoke(isFadingWhenTP);
            };

            fadingRiorient_RB.OnSelectedButtonChanged += (sender, value) =>
            {
                if (value == 0)
                    isFadingWhenRiorient = false;
                else if (value == 1)
                    isFadingWhenRiorient = true;

                OnRiorientFadingChanged?.Invoke(isFadingWhenRiorient);
            };

            fadeSlider.onValueChanged.AddListener((value) =>
            {
                fadeSpeed = value;  
                OnFadingValueChanged?.Invoke(fadeSpeed);
            });
        }
    }
}
