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

using umi3d.browserRuntime.ui.elements.dropdown;
using UnityEngine;

namespace umi3d.browserRuntime.ui.tablet.social
{
    [RequireComponent(typeof(ToggleDropdown))]
    public class UserSocialListFilterView : MonoBehaviour
    {
        UserSocialListModelContainer _listModelContainer;

        ToggleDropdown _filterDropdown;

        ToggleDropdownItem _muteFilter;
        ToggleDropdownItem _unmuteFilter;

        private void Awake()
        {
            _listModelContainer = GetComponentInParent<UserSocialListModelContainer>();

            _filterDropdown = GetComponent<ToggleDropdown>();
        }

        private void Start()
        {
            _muteFilter = _filterDropdown.AddOption("Mute");
            _muteFilter.OnToggle += _listModelContainer.Model.SetMuteFilter;
            _unmuteFilter = _filterDropdown.AddOption("Unmute");
            _unmuteFilter.OnToggle += _listModelContainer.Model.SetUnmuteFilter;
        }

        private void OnDestroy()
        {
            _muteFilter.OnToggle -= _listModelContainer.Model.SetMuteFilter;
            _unmuteFilter.OnToggle -= _listModelContainer.Model.SetUnmuteFilter;
        }
    }
}