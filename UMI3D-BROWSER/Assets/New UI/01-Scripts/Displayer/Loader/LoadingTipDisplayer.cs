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
    public class LoadingTipDisplayer : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private float tipDuration;
        [SerializeField] private List<LoadingTipData> loadingTipDatas;

        [Header("Elements")]
        [SerializeField] private Image tipImage;
        [SerializeField] private TextMeshProUGUI tipText;

        private Coroutine _coroutine;
        private LoadingTipData _selectedLoadingTipData;

        private void Start()
        {
            int random = UnityEngine.Random.Range(0, loadingTipDatas.Count);
            _selectedLoadingTipData = loadingTipDatas[random];
            SetTip();
        }

        [ContextMenu("Display tips")]
        internal void DisplayTips()
        {
            tipImage.enabled = true;
            tipText.enabled = true;
            _coroutine = StartCoroutine(RandomTip());
        }

        [ContextMenu("Hide Tips")]
        internal void StopDisplayTips()
        {
            tipImage.enabled = false;
            tipText.enabled = false;
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
        }

        private IEnumerator RandomTip()
        {
            float time = 0;

            while (true)
            {
                if (time > tipDuration)
                {
                    time = 0;
                    ChangeTip();
                    SetTip();
                }

                time += Time.deltaTime;
                yield return null;
            }
        }

        private void ChangeTip()
        {   
            if (loadingTipDatas.Count == 0) return;
            if (loadingTipDatas.Count == 1)
            {
                _selectedLoadingTipData = loadingTipDatas[0];
                return;
            }

            int random = UnityEngine.Random.Range(0, loadingTipDatas.Count);
            _selectedLoadingTipData = loadingTipDatas[random];
        }
        private void SetTip()
        {
            tipImage.sprite = _selectedLoadingTipData.Sprite;
            tipText.text = _selectedLoadingTipData.Tip;
        }

        [Serializable]
        public class LoadingTipData
        {
            [SerializeField, TextArea] private string tip;
            [SerializeField] private Sprite sprite;

            public string Tip => tip;
            public Sprite Sprite => sprite;
        }
    }
}

