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
    public class ToggleFactory : MonoBehaviour
    {
        [SerializeField] ToggleModelContainer _togglePrefab;

        Queue<ToggleModelContainer> _lstTogglesAvaible = new ();

        public int AvailableToggleCount => _lstTogglesAvaible.Count;

        public GameObject GetOrCreateToggle(Transform parent, string label = "", bool value = false)
        {
            if (!_lstTogglesAvaible.TryDequeue(out var toggleModelContainer))
                toggleModelContainer = GameObject.Instantiate(_togglePrefab);

            if (string.IsNullOrEmpty(label))
                toggleModelContainer.model.SetLabel(label);
            toggleModelContainer.model.SetValue(value);

            toggleModelContainer.gameObject.SetActive(true);

            return toggleModelContainer.gameObject;
        }

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