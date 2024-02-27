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


        int _panelStartId;
        UMI3DUI_Button _selected;

        private void Start()
        {
            int i = 0;
            foreach (var radio in buttons)
            {
                radio.onClick.AddListener(() =>
                {
                    if (_selected != radio)
                    {
                        _selected?.SubDisplayer.Disable();
                        _selected = radio;
                    }
                    OnSelectedButtonChanged?.Invoke(radio, radio.ID);
                });

                radio.SubDisplayer.Init(NormalColor, HoverColor, SelectedColor);
                radio.SetID(i++);
            }

            buttons[_panelStartId].SubDisplayer.Click();
        }

        public void ActivateButtonWithId(int id)
        {
            _panelStartId = id;
        }
    }
}

