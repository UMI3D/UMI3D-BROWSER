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
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.elements.dropdown
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleDropdownItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private LocalizeStringEvent text;
        [SerializeField] private Image tic;
        [SerializeField] private Image background;
        [SerializeField] private Color color;
        [SerializeField] private Color hoverColor;
        [SerializeField] private Color activeColor;

        private Toggle toggle;

        public bool IsOn => toggle?.isOn ?? false;

        public event Action<bool> OnToggle;

        private void Awake()
        {
            toggle = GetComponent<Toggle>();

            toggle.onValueChanged.AddListener(Click);
        }

        private void Start()
        {
            tic.gameObject.SetActive(toggle.isOn);
        }

        private void Click(bool value)
        {
            tic.gameObject.SetActive(value);
            OnToggle?.Invoke(value);
            background.color = value ? activeColor : color;
        }

        public void SetLocalizedText(string entry)
        {
            text.SetEntry(entry);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            background.color = hoverColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!IsOn)
                background.color = color;
        }
    }
}