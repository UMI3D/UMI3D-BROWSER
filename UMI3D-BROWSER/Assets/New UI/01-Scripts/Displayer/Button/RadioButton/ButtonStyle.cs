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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace umi3dBrowsers.displayer
{
    public class ButtonStyle : MonoBehaviour, ISubDisplayer
    {
        public event Action OnClick;
        public event Action OnDisabled;
        public event Action OnHover;

        [Header("Style")]
        [SerializeField] private Image image;
        [SerializeField] private Color normalColor;
        public Color NormalColor => normalColor;
        [SerializeField] private Color hoverColor;
        public Color HoverColor => hoverColor;
        [SerializeField] private Color selectedColor;
        public Color SelectedColor => selectedColor;

        private bool _isSelected;
        private bool _hovered;

        public void Init(Color normalColor, Color hoverColor, Color selectedColor)
        {
            this.normalColor = normalColor;
            this.hoverColor = hoverColor;
            this.selectedColor = selectedColor;

            image.color = normalColor;
        }

        public void Init(ButtonStyle buttonStyle)
        {
            if (buttonStyle != null)
                Init(buttonStyle.normalColor, buttonStyle.hoverColor, buttonStyle.selectedColor);
        }

        public void Click()
        {
            OnClick?.Invoke();
            if (_isSelected)
            {
                _isSelected = false;
                image.color = normalColor;
            }
            else
            {
                image.color = selectedColor;
                _isSelected = true;
            }
        }

        public void Disable()
        {
            _isSelected = false;
            image.color = normalColor;
        }

        public void HoverEnter(PointerEventData eventData)
        {
            if(!_isSelected)
                image.color = hoverColor;
            OnHover?.Invoke();

            _hovered = true;
        }

        public void HoverExit(PointerEventData eventData)
        {
            if (!_isSelected)
                image.color = normalColor;

            _hovered = false;
        }

        internal void SetNormalColor(Color overrideColor)
        {
            normalColor = overrideColor;
        }

        internal void SetHoverColor(Color overrideColor)
        {
            hoverColor = overrideColor;
        }

        internal void SetSelectedColor(Color overrideColor)
        {
            selectedColor = overrideColor;
        }

        internal void applyColor()
        {
            if(_isSelected)
                image.color= selectedColor;
            else if(_hovered)
                image.color = hoverColor;
            else
                image.color = normalColor;
        }
    }
}
