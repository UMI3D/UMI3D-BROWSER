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
using UnityEngine.UI;

namespace umi3d.browserRuntime.settings
{
    public class SettingsTabBadge : MonoBehaviour
    {
        [SerializeField] float padding = 10f;
        [SerializeField] bool isHorizontal = true;
        [SerializeField] bool isVertical = true;

        TMP_Text text;
        RectTransform textTransform;
        Image background;
        RectTransform backgroundTransform;

        void Awake()
        {
            text = GetComponentInChildren<TMP_Text>();
            textTransform = text.rectTransform;
            background = GetComponentInChildren<Image>();
            backgroundTransform = background.rectTransform;
        }

        [ContextMenu("UpdateUI")]
        void OnGUI()
        {
            backgroundTransform.sizeDelta = new Vector2(
                isHorizontal ? textTransform.sizeDelta.x + 2f * padding : backgroundTransform.sizeDelta.x,
                isVertical ? textTransform.sizeDelta.y + 2f * padding : backgroundTransform.sizeDelta.y
            );
        }
    }
}