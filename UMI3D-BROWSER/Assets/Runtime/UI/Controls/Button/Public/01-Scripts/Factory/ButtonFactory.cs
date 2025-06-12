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

using System.Collections.Generic;
using UnityEngine;

namespace umi3d.browserRuntime.ui
{
    public class ButtonFactory : MonoBehaviour
    {
        [SerializeField] internal ButtonController _buttonPrefab;

        internal Queue<ButtonController> _pool = new();

        public bool TryToGetOrCreate(out GameObject control, out ButtonModel model, Transform parent)
        {
            if (!_pool.TryDequeue(out ButtonController controller))
            {
                controller = Instantiate(_buttonPrefab);
            }
            controller.transform.SetParent(parent, false);
            control = controller.gameObject;
            model = controller.model;

            return true;
        }

        public void Return(GameObject gameObject)
        {
            if (!gameObject)
                return;
            var modelContainer = gameObject.GetComponent<ButtonController>();
            if (!modelContainer)
                return;

            modelContainer.gameObject.SetActive(false);
            modelContainer.transform.SetParent(transform, false);
            modelContainer.model.SetCallback(null);

            _pool.Enqueue(modelContainer);
        }
    }
}