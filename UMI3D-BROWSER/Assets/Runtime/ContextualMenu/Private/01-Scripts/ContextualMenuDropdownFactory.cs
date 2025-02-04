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
using umi3d.browserRuntime.ui.dropdown;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.browserRuntime.ui.contextualMenu
{
    [RequireComponent(typeof(DropdownFactory))]
    public class ContextualMenuDropdownFactory : MonoBehaviour
    {
        DropdownFactory _dropdownFactory;

        private void Awake()
        {
            _dropdownFactory = GetComponent<DropdownFactory>();
        }

        public GameObject GetOrCreate(Transform parent, EnumParameterDto<string> dto)
        {
            var dropdownGameobject = _dropdownFactory.GetOrCreateDropdown(parent);
            var model = dropdownGameobject.GetComponent<DropdownParameterModelContainer>().parameterModel;
            model.SetDto(dto);

            NotificationHub.Default.Subscribe(dropdownGameobject,
                ID.FromType<ContextualMenuNotificationKeys.Submit>(),
                (Callback)model.Submit);

            return dropdownGameobject;
        }

        public void Return(GameObject toggleModelContainer)
        {
            NotificationHub.Default.Unsubscribe(toggleModelContainer);
            _dropdownFactory.Return(toggleModelContainer);
        }
    }
}