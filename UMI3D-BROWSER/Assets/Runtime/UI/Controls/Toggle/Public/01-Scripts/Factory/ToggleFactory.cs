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

namespace umi3d.browserRuntime.ui
{
    /// <summary>
     /// Factory managing a pool of toggle.
     /// </summary>
    public class ToggleFactory : MonoBehaviour
    {
        [SerializeField] ToggleController _togglePrefab;

        Queue<ToggleController> _lstTogglesAvailable = new ();

        public int AvailableToggleCount => _lstTogglesAvailable.Count;

        public bool TryToGetOrCreate(out GameObject control, out ToggleModel model, Transform parent)
        {
            if (!_lstTogglesAvailable.TryDequeue(out var controller))
            {
                controller = GameObject.Instantiate(_togglePrefab);
            }

            controller.transform.SetParent(parent, false);
            control = controller.gameObject;
            model = controller.model;

            return true;
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
            var controller = toggleGameObject.GetComponent<ToggleController>();
            if (!controller) { return; }

            _lstTogglesAvailable.Enqueue(controller);

            controller.gameObject.SetActive(false);
            controller.transform.SetParent(transform, false);
            controller.Clear();
        }
    }
}