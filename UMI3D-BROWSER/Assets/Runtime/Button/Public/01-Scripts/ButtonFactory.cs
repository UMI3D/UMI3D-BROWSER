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

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.button
{
    public class ButtonFactory : MonoBehaviour
    {
        [SerializeField] internal ButtonModelContainer _buttonPrefab;

        internal Queue<ButtonModelContainer> _pool = new();

        public GameObject GetOrCreateButton(Transform parent, string label = "", Sprite image = null, ColorBlock? colors = null, Action callback = null)
        {
            ButtonModelContainer button;
            if (!_pool.TryDequeue(out button))
                button = Instantiate(_buttonPrefab);

            button.gameObject.SetActive(true);
            button.transform.SetParent(parent, false);

            button.Model.SetLabel(label);
            button.Model.SetCallback(callback);
            button.Model.SetImage(colors, image);

            return button.gameObject;
        }

        public void ReturnButton(GameObject gameObject)
        {
            if (!gameObject)
                return;
            var modelContainer = gameObject.GetComponent<ButtonModelContainer>();
            if (!modelContainer)
                return;

            modelContainer.gameObject.SetActive(false);
            modelContainer.transform.SetParent(transform, false);
            modelContainer.Model.SetCallback(null);

            _pool.Enqueue(modelContainer);
        }
    }
}