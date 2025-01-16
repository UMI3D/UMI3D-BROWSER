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

using UnityEngine;

namespace umi3d.browserRuntime.ui.dropdown
{
    /// <summary>
    /// Container of an <see cref="DropdownParameterModel"/>. 
    /// Used with an <see cref="DropdownModelContainer"/>.
    /// </summary>
    [RequireComponent(typeof(DropdownModelContainer))]

    public class DropdownParameterModelContainer : MonoBehaviour
    {
        public DropdownParameterModel parameterModel;

        DropdownModelContainer _modelContainer;

        private void Awake()
        {
            _modelContainer = GetComponent<DropdownModelContainer>();

            parameterModel = new DropdownParameterModel(_modelContainer.model);
        }

    }
}