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
using UnityEngine.UI;

namespace umi3dBrowsers.displayer
{
    public class RadioButtonGroup : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private List<UMI3DUI_Button> buttons = new();
        public event Action<UMI3DUI_Button, int> OnSelectedButtonChanged;

        [Header("Style")]
        [SerializeField] private Color NormalColor;
        [SerializeField] private Color HoverColor;
        [SerializeField] private Color SelectedColor;

        private UMI3DUI_Button _selectedButton;

        private void Awake()
        {
            int i = 0;
            foreach (var radio in buttons)
            {
                radio.onClick.AddListener(() =>
                {
                    OnSelectedButtonChanged(radio, radio.ID);
                    foreach (var rad in buttons)
                    {
                        rad.Image.color = NormalColor;
                    }
                    radio.Image.color = SelectedColor;
                    _selectedButton = radio;
                });
                radio.OnHoverEnter += () =>
                {
                    radio.Image.color = HoverColor;
                };
                radio.OnHoverExit += () =>
                {
                    if (radio == _selectedButton) return;
                    radio.Image.color = NormalColor;
                };
                radio.SetID(i++);
            }
        }
    }
}

