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

using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.thumbnails
{
    [RequireComponent(typeof(Button))]
    internal class ParallaxNavigationButton : MonoBehaviour
    {
        enum Direction
        {
            Minus,
            Plus
        }

        [SerializeField] Direction _direction;

        Button _button;
        Scrollbar _scrollbar;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _scrollbar = GetComponentInParent<Scrollbar>();

            _button.onClick.AddListener(Click);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(Click);
        }

        private void Click()
        {
            if (_direction == Direction.Minus)
                _scrollbar.value -= 0.1f;
            if (_direction == Direction.Plus)
                _scrollbar.value += 0.1f;
        }
    }
}