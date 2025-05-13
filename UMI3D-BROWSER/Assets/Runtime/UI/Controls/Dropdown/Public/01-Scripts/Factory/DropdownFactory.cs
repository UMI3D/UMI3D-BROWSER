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
    /// Factory managing a pool of dropdown.
    /// </summary>
    public class DropdownFactory : MonoBehaviour
    {
        [SerializeField] 
        internal DropdownController _dropdownPrefab;

        internal Queue<DropdownController> _lstDropdownsAvailable = new();

        public int AvailableDropdownCount => _lstDropdownsAvailable.Count;

        public bool TryToGetOrCreate(out GameObject control, Transform parent)
        {
            if (!_lstDropdownsAvailable.TryDequeue(out DropdownController controller))
            {
                controller = GameObject.Instantiate(_dropdownPrefab);
            }
            control = controller.gameObject;
            controller.gameObject.SetActive(true);
            controller.transform.SetParent(parent, false);

            return true;
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
        /// <param name="control">The dropdown GameObject to be returned to the pool.</param>
        public void Return(GameObject control)
        {
            var controller = control.GetComponent<DropdownController>();
            if (!controller) { return; }

            _lstDropdownsAvailable.Enqueue(controller);

            controller.gameObject.SetActive(false);
            controller.transform.SetParent(transform, false);
            controller.Clear();
        }
    }
}