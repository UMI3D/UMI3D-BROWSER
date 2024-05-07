/*
Copyright 2019 - 2023 Inetum

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
using UnityEngine.UI;

namespace umi3dBrowsers.displayer
{
    public class ProgressBar : MonoBehaviour
    {
        [Header("Elements")]
        [SerializeField] private Image loadingBarFill;
        [SerializeField] private RectTransform thisRect;

        [Header("Values")]
        [SerializeField] private float maxValue = 1;
        [SerializeField] private float value = 0.5f;

        public void SetProgressBarMaxValue(float value)
        {
            if (value <= 0)
                value = float.Epsilon;
            maxValue = value;
        }

        public void SetProgressBarValue(float value)
        {
            if (value < 0)
                value = 0;
            if (value > maxValue)
                value = maxValue;
            this.value = value;
            float rightValue = thisRect.rect.width * (value / maxValue);
            loadingBarFill.rectTransform.sizeDelta = new Vector2(rightValue, thisRect.rect.height);
        }

        private void OnValidate()
        {
            SetProgressBarMaxValue(maxValue);
            SetProgressBarValue(value);
        }
    }
}

