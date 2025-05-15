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

namespace umi3d.browserRuntime.ui.toggle
{
    /// <summary>
    /// Container of an <see cref="ToggleParameterModel"/>. 
    /// Used with an <see cref="ToggleModelContainer"/>.
    /// </summary>
    [RequireComponent(typeof(ToggleModelContainer))]
    public class ToggleParameterModelContainer : MonoBehaviour
    {
        public ToggleParameterModel parameterModel;

        ToggleModelContainer _modelContainer;

        private void Awake()
        {
            _modelContainer = GetComponent<ToggleModelContainer>();

            parameterModel = new ToggleParameterModel(_modelContainer.model);
        }
    }
}