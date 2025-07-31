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

using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace umi3d.browserRuntime.ui.keyboard
{
    public class InputFieldSelectionIndicator : MonoBehaviour, IInputFieldSelectionIndicator, IDragHandler, IPointerDownHandler
    {
        [SerializeField] bool _isStart = true;

        RectTransform _rectTransform;
        bool _isMouseDown = false;

        public Action<Vector2> OnMove { get; set; }

        void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            Assert.IsNotNull(_rectTransform);
        }

        void Start()
        {
            var inputFieldLinker = GetComponentInParent<KeyboardTMPInputFieldLinker>();
            Assert.IsNotNull(inputFieldLinker);
            Assert.IsNotNull(inputFieldLinker.Selection);

            if (_isStart)
                inputFieldLinker.Selection.StartIndicator = this;
            else
                inputFieldLinker.Selection.EndIndicator = this;

            gameObject.SetActive(false);
        }

        public void ShowAt(Vector2 anchoredPosition)
        {
            _rectTransform.anchoredPosition = anchoredPosition;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void OnDrag(PointerEventData eventData)
        {
            OnMove?.Invoke(eventData.position);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // Just to capture the click
        }
    }
}