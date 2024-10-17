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
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace umi3dBrowsers.displayer
{
    public class LoadingBarDisplayer : MonoBehaviour
    {

        [Header("Elements")]
        [SerializeField] private Image loadingBarFill;
        [SerializeField] TextMeshProUGUI primaryDownloadText;
        [SerializeField] TextMeshProUGUI secondaryDownloadText;

        [SerializeField] private RectTransform thisRect;

        [Header("Data")]
        [SerializeField] private string primaryText = "Total";

        [Header("Debug")]
        [SerializeField, Range(0, 1)] private float value; 

        private void Awake()
        {
            SetProgressBarValue(0);  
        }

        internal void SetLoadingState(string currentState)
        {
            secondaryDownloadText.text = currentState;
        }

        public void SetProgressBarValue(float value)
        {
            this.value = value;
            float rightValue = thisRect.rect.width * (value - 1);
            loadingBarFill.rectTransform.sizeDelta = new Vector2(rightValue, 0);
            primaryDownloadText.text = primaryText + (value * 100).ToString("0.0") + "%";
        }

        private void OnValidate()
        {
            SetProgressBarValue(value);
        }
    }
}

