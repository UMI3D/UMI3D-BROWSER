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

using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace umi3dBrowsers
{
    public class MainMessageContainer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI prefixText;
        [SerializeField] private TextMeshProUGUI suffixText;
        [Space]
        [SerializeField] private float prefixCharSize = 10f;
        [SerializeField] private float suffixCharSize = 12f;

        private LocalizeStringEvent prefixEvent;
        private LocalizeStringEvent suffixEvent;

        private void Awake()
        {
            prefixEvent = prefixText.GetComponent<LocalizeStringEvent>();
            suffixEvent = suffixText?.GetComponent<LocalizeStringEvent>();
        }

        /// <summary>
        /// Sets the page title
        /// </summary>
        /// <param name="prefix">The first part of the title</param>
        /// <param name="suffix">The second part of the title</param>
        public void SetTitle(string prefix, string suffix, bool prefixOverride = false, bool suffixOverride = false)
        {
            Rect suffixRectTransform = suffixText.rectTransform.rect;
            Rect prefixRectTransform = prefixText.rectTransform.rect;

            if (prefixOverride) prefixText.text = prefix;
            else prefixEvent.SetEntry(prefix);

            if (suffixOverride) suffixText.text = suffix;
            else suffixEvent.SetEntry(suffix);

            float prefixLength = prefixText.text?.Length ?? 0f;
            float suffixLength = suffixText.text?.Length ?? 0f;

            suffixRectTransform.width = suffixLength * suffixCharSize;
            prefixRectTransform.width = prefixLength * prefixCharSize;
            suffixText.rectTransform.sizeDelta = new Vector2(suffixRectTransform.width, suffixRectTransform.height);
            prefixText.rectTransform.sizeDelta = new Vector2(prefixRectTransform.width, prefixRectTransform.height);
        }

        public void SetPrefix(string prefix, bool prefixOverride = false)
        {
            if (prefixOverride) prefixText.text = prefix;
            else prefixEvent.SetEntry(prefix);
        }
    }
}
