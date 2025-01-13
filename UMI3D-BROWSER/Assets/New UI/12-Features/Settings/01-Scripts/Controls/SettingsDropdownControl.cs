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
using TMPro;
using UnityEngine;

namespace umi3d.browserRuntime.ui.settings
{
    [RequireComponent(typeof(TMP_Dropdown))]
    internal class SettingsDropdownControl : MonoBehaviour
    {
        TMP_Dropdown dropdown;
        TMP_Text label;

        List<TMP_Dropdown.OptionData> options = new();

        public int optionsCount;
        public int selectedIndex;

        public event Action<int> valueChanged;
        public Func<int, string> indexToItem;

        void Awake()
        {
            dropdown = GetComponent<TMP_Dropdown>();
            label = GetComponentInChildren<TMP_Text>();

            dropdown.options = options;

            dropdown.onValueChanged.AddListener(ValueChanged);
        }

        public void SetOptions()
        {
            options.Clear();
            for (int i = 0; i < optionsCount; i++)
            {
                if (i == selectedIndex)
                {
                    continue;
                }

                string item = indexToItem?.Invoke(i) ?? "Error: [indexToItem] not set";
                options.Add(new(item));
            }
            dropdown.SetValueWithoutNotify(-1);
            label.text = indexToItem?.Invoke(selectedIndex) ?? "Error: [indexToItem] not set";
        }

        void UpdateOptions()
        {
            int index = 0;
            for (int i = 0; i < options.Count; i++)
            {
                index = i < selectedIndex ? i : i + 1;

                options[i].text = indexToItem?.Invoke(index) ?? "Error: [indexToItem] not set";
            }
            dropdown.SetValueWithoutNotify(-1);
            label.text = indexToItem?.Invoke(selectedIndex) ?? "Error: [indexToItem] not set";
        }

        void ValueChanged(int index)
        {
            selectedIndex = index < selectedIndex ? index : index + 1;

            valueChanged?.Invoke(selectedIndex);

            UpdateOptions();
        }
    }
}