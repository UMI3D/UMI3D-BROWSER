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
    /// Factory managing a pool of input field.
    /// Can get or create an input field single or multi line.
    /// </summary>
    public class InputFieldFactory : MonoBehaviour
    {
        [SerializeField] internal InputFieldController _singleLinePrefab;
        [SerializeField] internal InputFieldController _multiLinePrefab;

        internal Queue<InputFieldController> _lstInputFieldsSingleAvailable = new();
        internal Queue<InputFieldController> _lstInputFieldsMultiAvailable = new();

        public int AvailableInputFieldSingleCount => _lstInputFieldsSingleAvailable.Count;
        public int AvailableInputFieldMultiCount => _lstInputFieldsMultiAvailable.Count;

        public bool TryToGetOrCreateSingleLine(out GameObject control, out InputFieldModel model, Transform parent)
        {
            InputFieldController controller = null;
            if (!_lstInputFieldsSingleAvailable.TryDequeue(out controller))
            {
                controller = GameObject.Instantiate(_singleLinePrefab);
            }
            control = controller.gameObject;
            model = controller.model;
            control.transform.SetParent(parent, false);

            return true;
        }

        public bool TryToGetOrCreateMultiLine(out GameObject control, out InputFieldModel model, Transform parent)
        {
            InputFieldController controller = null;
            if (!_lstInputFieldsMultiAvailable.TryDequeue(out controller))
            {
                controller = GameObject.Instantiate(_multiLinePrefab);
            }
            control = controller.gameObject;
            model = controller.model;
            control.transform.SetParent(parent, false);

            return true;
        }

        /// <summary>
        /// This method returns an input field GameObject to the pool, deactivates it, and reassigns its parent.<br/>
        /// <br/>
        /// <example>
        /// Given an input field created by the factory:
        /// <code>
        /// Transform parent = new GameObject().transform;
        /// bool isMultiline = false;
        /// GameObject inputField = _inputFieldFactory.GetOrCreateInputField(parent, isMultiline);
        /// _inputFieldFactory.Return(inputField);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="control">The input field GameObject to be returned to the pool.</param>
        public void Return(GameObject control)
        {
            var controller = control.GetComponent<InputFieldController>();
            if (!controller) { return; }

            if (controller.isMultiline)
                _lstInputFieldsMultiAvailable.Enqueue(controller);
            else
                _lstInputFieldsSingleAvailable.Enqueue(controller);

            controller.gameObject.SetActive(false);
            controller.transform.SetParent(transform, false);
            controller.Clear();
        }
    }
}