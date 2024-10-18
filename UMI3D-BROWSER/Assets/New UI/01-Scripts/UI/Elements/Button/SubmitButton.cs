using System;
using System.Collections;
using System.Collections.Generic;
using umi3dBrowsers.utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
namespace umi3dBrowsers.displayer
{
    [RequireComponent(typeof(UMI3DUI_Button))]
    public class SubmitButton : MonoBehaviour, ISubDisplayer
    {
        public event Action OnClick;
        public event Action OnDisabled;
        public event Action OnHover;

        [SerializeField] private Image image;

        [Header("Sprites")]
        [SerializeField] private Sprite clickedSprite;
        [SerializeField] private Sprite normalSprite;

        [Header("color")]
        [SerializeField] private Color hoverColor;
        [SerializeField] private Color pressedColor;
        [SerializeField] private Color normalColor;

        private bool _isPressed;

        public void Click()
        {
            image.sprite = clickedSprite;
            image.color = pressedColor;
            _isPressed = true;
        }

        public void HoverEnter(PointerEventData eventData)
        {
            if (_isPressed) return;
            image.sprite = clickedSprite;
            image.color = hoverColor;
        }

        public void HoverExit(PointerEventData eventData)
        {
            if (_isPressed) return;
            Disable();
        }

        public void Disable()
        {
            image.sprite = normalSprite;
            image.color = normalColor;
        }

        public void Init(Color normalColor, Color hoverColor, Color selectedColor)
        {
            throw new NotImplementedException();
        }
    }
}

