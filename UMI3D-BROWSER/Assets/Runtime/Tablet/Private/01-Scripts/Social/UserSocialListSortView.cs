/*
Copyright 2019 - 2025 Inetum

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
using System.Linq;
using TMPro;
using UnityEngine;

namespace umi3d.browserRuntime.ui.tablet.social
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class UserSocialListSortView : MonoBehaviour
    {
        UserSocialListModelContainer _modelContainer;

        TMP_Dropdown _dropdown;

        private void Awake()
        {
            _modelContainer = GetComponentInParent<UserSocialListModelContainer>();
            _dropdown = GetComponent<TMP_Dropdown>();

            foreach (var sortingMethode in Enum.GetValues(typeof(UserSocialSortingMethode)).Cast<UserSocialSortingMethode>())
                _dropdown.options.Add(new TMP_Dropdown.OptionData(sortingMethode.ToString()));

            _dropdown.onValueChanged.AddListener(OnValueChanged);
            _dropdown.value = 0;
        }

        private void OnDestroy()
        {
            _dropdown.onValueChanged.RemoveListener(OnValueChanged);
        }

        void OnValueChanged(int index) => _modelContainer.Model.SetSortingMethode((UserSocialSortingMethode)index);
    }
}