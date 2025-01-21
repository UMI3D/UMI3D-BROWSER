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

using inetum.unityUtils.observation;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace umi3d.browserRuntime.ui.dropdown
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class DropdownView : MonoBehaviour
    {
        DropdownModelContainer _modelContainer;
        TMP_Dropdown _dropdown;
        List<string> _options;

        private void Awake()
        {
            _modelContainer = GetComponentInParent<DropdownModelContainer>();
            _dropdown = GetComponent<TMP_Dropdown>();

            _dropdown.onValueChanged.AddListener(OnValueChanged);

            NotificationHub.Default.Subscribe(this,
                ID.FromType<DropdownNotificationKeys.DropdownSet>(),
                (Callback)DropdownSet,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _modelContainer.model));
        }

        void DropdownSet(Notification notification)
        {
            if (notification.TryGetInfoT(DropdownNotificationKeys.DropdownSet.Options, out _options))
            {
                _dropdown.ClearOptions();
                foreach (var option in _options)
                    _dropdown.options.Add(new TMP_Dropdown.OptionData(option));
            }

            if (_options != null && notification.TryGetInfoT(DropdownNotificationKeys.DropdownSet.Value, out string value))
                _dropdown.value = _options.IndexOf(value);
        }

        void OnValueChanged(int newIndex)
        {
            if (_options != null)
                _modelContainer.model.UpdateValue(_options[newIndex]);
        }
    }
}