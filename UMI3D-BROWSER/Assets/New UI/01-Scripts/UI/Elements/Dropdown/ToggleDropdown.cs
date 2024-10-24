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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.elements.dropdown
{
    [RequireComponent(typeof(Button))]
    public class ToggleDropdown : MonoBehaviour
    {
        [SerializeField] private Transform content;
        [SerializeField] private ToggleDropdownItem template;
        [SerializeField] private Button openButton;

        private List<ToggleDropdownItem> options;

        public List<ToggleDropdownItem> Options => options;

        public event Action OnValueChanged;

        private void Awake()
        {
            options = new();
            template.gameObject.SetActive(false);

            openButton.onClick.AddListener(() => {
                content.gameObject.SetActive(!content.gameObject.activeSelf);
            });
        }

        private void OnEnable()
        {
            content.gameObject.SetActive(false);
        }

        public ToggleDropdownItem AddOption(string text)
        {
            var item = Instantiate(template, content);
            item.gameObject.SetActive(true);

            item.SetLocalizedText(text);
            item.OnToggle += b => OnValueChanged?.Invoke();

            options.Add(item);
            return item;
        }

        public void ClearOptions()
        {
            foreach(var option in options)
                Destroy(option.gameObject);
        }
    }
}