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
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace umi3dBrowsers.displayer
{
    public class VignetteInputField : MonoBehaviour
    {

        [Header("obj")]
        [SerializeField] private TMP_InputField inputField;

        public string Text { get => inputField.text; set => inputField.text = value; }
        public TMP_InputField InputField => inputField;

        public event Action OnClick;
        public event Action OnDisabled;
        public event Action OnHover;
        public event Action OnHoverExit;

        private void Awake()
        {
            inputField.onSelect.AddListener((a) => Click());
            inputField.onDeselect.AddListener(InputFieldDeselected);
            
        }



        public void Click()
        {
            OnClick?.Invoke();
            //backGroundCanvasGroup.alpha = 0;
        }


        public void HoverEnter(PointerEventData eventData)
        {
            OnHover?.Invoke();
        }

        public void HoverExit(PointerEventData eventData)
        {
            OnHoverExit?.Invoke();
        }

        private void InputFieldDeselected(string arg0)
        {
            //backGroundCanvasGroup.alpha = 1f;
        }

        public void Init(Color normalColor, Color hoverColor, Color selectedColor)
        {

        }
    }
}

