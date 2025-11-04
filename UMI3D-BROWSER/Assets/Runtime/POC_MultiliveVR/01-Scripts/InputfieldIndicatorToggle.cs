/*
Copyright 2019 - 2025 Inetum

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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d
{
    public class InputfieldIndicatorToggle : MonoBehaviour
    {
        [SerializeField] Slider _slider;

        [Header("Left")]
        [SerializeField] Sprite _leftSpriteBar;
        [SerializeField] Sprite _leftSpriteBubble;
        [SerializeField] List<Image> _leftIndicators;

        [Header("Right")]
        [SerializeField] Sprite _rightSpriteBar;
        [SerializeField] Sprite _rightSpriteBubble;
        [SerializeField] List<Image> _rightIndicators;

        private void Awake()
        {
            _slider.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(float value)
        {
            foreach (Image indicator in _leftIndicators)
            {
                var sprite = value >= 0.5f ? _leftSpriteBar : _leftSpriteBubble;
                indicator.sprite = sprite;
                indicator.rectTransform.pivot = sprite.pivot;
            }

            foreach (Image indicator in _rightIndicators)
            {
                var sprite = value >= 0.5f ? _rightSpriteBar : _rightSpriteBubble;
                indicator.sprite = sprite;
                indicator.rectTransform.pivot = sprite.pivot;
            }
        }
    }
}