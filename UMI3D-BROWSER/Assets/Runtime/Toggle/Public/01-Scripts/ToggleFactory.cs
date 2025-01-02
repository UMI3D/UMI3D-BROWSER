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

namespace umi3d.browserRuntime.ui.toggle
{
    /// <summary>
     /// Factory managing a pool of toggle.
     /// </summary>
    public class ToggleFactory : MonoBehaviour
    {
        [SerializeField] ToggleModelContainer _togglePrefab;

        Queue<ToggleModelContainer> _lstTogglesAvaible = new ();

        public int AvailableToggleCount => _lstTogglesAvaible.Count;

        /// <summary>
        /// Gets an available toggle from the pool or creates a new one if none are available, sets its label and value, and activates it.<br/>
        /// <br/>
        /// <example>
        /// Given a parent transform, label, and value, when getting or creating a toggle, then the toggle is set up with the specified parameters and activated.
        /// <code>
        /// var toggle = _toggleFactory.GetOrCreateToggle(parent, "TestLabel", true);
        /// // toggle is active
        /// // toggle's label is "TestLabel"
        /// // toggle's value is true
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="parent">The parent transform to which the toggle will be attached.</param>
        /// <param name="label">The label to set for the toggle. Default is an empty string.</param>
        /// <param name="value">The value to set for the toggle. Default is false.</param>
        /// <returns>The created or retrieved toggle GameObject.</returns>
        public GameObject GetOrCreateToggle(Transform parent, string label = "", bool value = false)
        {
            if (!_lstTogglesAvaible.TryDequeue(out var toggleModelContainer))
                toggleModelContainer = GameObject.Instantiate(_togglePrefab);

            if (!string.IsNullOrEmpty(label))
                toggleModelContainer.model.SetLabel(label);
            toggleModelContainer.model.SetValue(value);

            toggleModelContainer.gameObject.SetActive(true);

            return toggleModelContainer.gameObject;
        }

        /// <summary>
        /// Returns the specified toggle GameObject to the pool, deactivates it, and reassigns its parent.<br/>
        /// <br/>
        /// <example>
        /// Given a toggle GameObject, when returning it to the pool, then the toggle is deactivated and added back to the pool.
        /// <code>
        /// _toggleFactory.Return(toggleGameObject);
        /// // toggleGameObject is inactive
        /// // toggleGameObject is added back to the pool
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="toggleGameObject">The toggle GameObject to return to the pool.</param>
        public void Return(GameObject toggleGameObject)
        {
            var toggleModelContainer = toggleGameObject.GetComponent<ToggleModelContainer>();
            if (!toggleModelContainer)
                return;

            _lstTogglesAvaible.Enqueue(toggleModelContainer);

            toggleModelContainer.gameObject.SetActive(false);
            toggleModelContainer.transform.SetParent(transform, false);
        }
    }
}