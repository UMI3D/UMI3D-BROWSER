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

namespace umi3d.browserRuntime.ui.settings
{
    [RequireComponent(typeof(Toggle))]
    public class SettingsDropdownControlItem : MonoBehaviour
    {
        [Header("Even Color")]
        [SerializeField] Color EvenItemColor;
        [SerializeField] Color EvenItemHighlightedColor;
        [SerializeField] Color EvenItemPressedColor;

        [Header("Odd Color")]
        [SerializeField] Color oddItemColor;
        [SerializeField] Color oddItemHighlightedColor;
        [SerializeField] Color oddItemPressedColor;

        Toggle toggle;

        void Awake()
        {
            toggle = GetComponent<Toggle>();
        }

        void Start()
        {
            int index = transform.GetSiblingIndex();
            ColorBlock colorBlock = new ColorBlock();
            colorBlock.normalColor = index % 2 == 0 ? oddItemColor : EvenItemColor;
            colorBlock.highlightedColor = index % 2 == 0 ? oddItemHighlightedColor : EvenItemHighlightedColor;
            colorBlock.pressedColor = index % 2 == 0 ? oddItemPressedColor : EvenItemPressedColor;
            colorBlock.selectedColor = index % 2 == 0 ? oddItemHighlightedColor : EvenItemHighlightedColor;
            colorBlock.colorMultiplier = 1;
            colorBlock.fadeDuration = 0.1f;
            toggle.colors = colorBlock;
        }
    }
}