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
using UnityEngine.UI;

namespace umi3dBrowsers.displayer
{
    public class VignetteInputField : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] GameObject pen;
        TMP_InputField inputField;
        Image background;

        public string Text { get => inputField.text; set => inputField.text = value; }
        public TMP_InputField InputField => inputField;

        public event Action OnDisabled;
        public event Action OnHover;

        private void Awake()
        {
            inputField = GetComponent<TMP_InputField>();
            background = GetComponent<Image>();

            pen.SetActive(false);
            background.enabled = false;
        }

        void OnDisable()
        {
            OnDisabled?.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnHover?.Invoke();
            pen.SetActive(true);
            background.enabled = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            pen.SetActive(false);
            background.enabled = false;
        }
    }
}

