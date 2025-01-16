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

using System.Collections.Generic;
using UnityEngine;

namespace umi3d.browserRuntime.ui.dropdown
{
    /// <summary>
    /// Factory managing a pool of dropdown.
    /// </summary>

    public class DropdownFactory : MonoBehaviour
    {
        [SerializeField] DropdownModelContainer _dropdownPrefab;

        Queue<DropdownModelContainer> _lstDropdownsAvaible = new();

        public int AvailableDropdownCount => _lstDropdownsAvaible.Count;

        /// <summary>
        /// This method retrieves an available dropdown from the pool or creates a new one if none are available.<br/>
        /// It then sets the parent, label, options, and value for the dropdown.<br/>
        /// <br/>
        /// <example>
        /// <code>
        /// var dropdown = dropdownFactory.GetOrCreateDropdown(parentTransform, "TestLabel", new List&lt;string> { "Option1", "Option2" }, "Option1");
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="parent">The parent transform to which the dropdown will be attached.</param>
        /// <param name="label">The label to set for the dropdown. Default is an empty string.</param>
        /// <param name="options">The list of options to set for the dropdown. Default is null.</param>
        /// <param name="value">The value to set for the dropdown. Default is an empty string.</param>
        /// <returns>The created or retrieved dropdown GameObject.</returns>
        public GameObject GetOrCreateDropdown(Transform parent, string label = "", List<string> options = null, string value = "")
        {
            if (!_lstDropdownsAvaible.TryDequeue(out var dropdownModelContainer))
                dropdownModelContainer = GameObject.Instantiate(_dropdownPrefab);

            dropdownModelContainer.gameObject.SetActive(true);
            dropdownModelContainer.transform.SetParent(parent, false);

            if (!string.IsNullOrEmpty(label))
                dropdownModelContainer.model.SetLabel(label);
            dropdownModelContainer.model.SetOptions(options ?? new List<string>());
            dropdownModelContainer.model.SetValue(value);

            return dropdownModelContainer.gameObject;
        }

        /// <summary>
        /// This method returns a dropdown GameObject to the pool of available dropdowns.<br/>
        /// It deactivates the dropdown and sets its parent to the factory's transform.<br/>
        /// <br/>
        /// <example>
        /// <code>
        /// _dropdownFactory.Return(invalidGameObject);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="toggleGameObject">The dropdown GameObject to be returned to the pool.</param>
        public void Return(GameObject toggleGameObject)
        {
            var dropdownModelContainer = toggleGameObject.GetComponent<DropdownModelContainer>();
            if (!dropdownModelContainer)
                return;

            _lstDropdownsAvaible.Enqueue(dropdownModelContainer);

            dropdownModelContainer.gameObject.SetActive(false);
            dropdownModelContainer.transform.SetParent(transform, false);
        }
    }
}