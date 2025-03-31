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
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace umi3d.browserRuntime.thumbnails
{
    [RequireComponent(typeof(Button))]
    internal class ParallaxNavigationButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        enum Direction
        {
            Minus,
            Plus
        }

        [SerializeField] Direction _direction;
        [SerializeField] Image _border;
        [SerializeField] Color _borderActiveColor;
        private Color _borderBaseColor;
        [SerializeField] Image _arrow;
        [SerializeField] Color _arrowActiveColor;
        private Color _arrowBaseColor;

        Button _button;
        Scrollbar _scrollbar;

        ThumbnailListModelContainer _modelContainer;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _scrollbar = GetComponentInParent<Scrollbar>();

            _modelContainer = GetComponentInParent<ThumbnailListModelContainer>();

            _borderBaseColor = _border.color;
            _arrowBaseColor = _arrow.color;

            _button.onClick.AddListener(Click);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(Click);
        }

        private void Click()
        {
            if (_direction == Direction.Minus)
                _scrollbar.value -= 1.0f / (_modelContainer.Model.Thumbnails.Count -  (_modelContainer.Model.Mode.NbrColumn * _modelContainer.Model.Mode.NbrRow));
            if (_direction == Direction.Plus)
                _scrollbar.value += 1.0f / (_modelContainer.Model.Thumbnails.Count - (_modelContainer.Model.Mode.NbrColumn * _modelContainer.Model.Mode.NbrRow));
            _scrollbar.value = Mathf.Clamp(_scrollbar.value, 0, 1);

            EventSystem.current.SetSelectedGameObject(null);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _border.color = _borderActiveColor;
            _arrow.color = _arrowActiveColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _border.color = _borderBaseColor;
            _arrow.color = _arrowBaseColor;
        }
    }
}