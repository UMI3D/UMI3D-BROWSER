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

namespace umi3d.browserRuntime.ui.inputField
{
    /// <summary>
    /// Factory managing a pool of input field.
    /// Can get or create an input field single or multi line.
    /// </summary>
    public class InputFieldFactory : MonoBehaviour
    {
        [SerializeField] InputFieldModelContainer _singleLinePrefab;
        [SerializeField] InputFieldModelContainer _multiLinePrefab;

        Queue<InputFieldModelContainer> _lstInputFieldsAvailable = new();

        public int AvailableInputFieldCount => _lstInputFieldsAvailable.Count;

        /// <summary>
        /// This method retrieves an available input field from the pool or creates a new one if none are available.<br/>
        /// It then sets up the input field with the provided parameters and returns the GameObject.<br/>
        /// <br/>
        /// <example>
        /// Given valid parameters for a single line input field:
        /// <code>
        /// Transform parent = new GameObject().transform;
        /// bool isMultiline = false;
        /// string label = "Test Label";
        /// string value = "Test Value";
        /// string placeholder = "Test Placeholder";
        /// int nbLine = 1;
        /// bool isPrivate = false;
        /// GameObject inputField = _inputFieldFactory.GetOrCreateInputField(parent, isMultiline, label, value, placeholder, nbLine, isPrivate);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="parent">The parent transform to which the input field will be attached.</param>
        /// <param name="isMultiline">Indicates whether the input field should be multiline.</param>
        /// <param name="label">The label text for the input field.</param>
        /// <param name="value">The initial value of the input field.</param>
        /// <param name="placeholder">The placeholder text for the input field.</param>
        /// <param name="nbLine">The number of lines for the input field. Defaults to 1.</param>
        /// <param name="isPrivate">Indicates whethre the input field should be private.</param>
        /// <returns>The created or retrieved input field GameObject.</returns>
        public GameObject GetOrCreateInputField(Transform parent, bool isMultiline, string label = "", string value = "", string placeholder = "", int nbLine = 1, bool isPrivate = false)
        {
            if (nbLine < 1) nbLine = 1;

            if (!_lstInputFieldsAvailable.TryDequeue(out var inputFieldModelContainer))
                inputFieldModelContainer = GameObject.Instantiate(isMultiline ? _multiLinePrefab : _singleLinePrefab);

            inputFieldModelContainer.gameObject.SetActive(true);
            inputFieldModelContainer.transform.SetParent(parent, false);

            if (label != null || label != string.Empty)
                inputFieldModelContainer.model.SetLabel(label);
            if (value != null || value != string.Empty)
                inputFieldModelContainer.model.SetValue(value);
            if (placeholder != null || placeholder != string.Empty)
                inputFieldModelContainer.model.SetPlaceholder(placeholder);
            if (nbLine != 1)
                inputFieldModelContainer.model.SetNbrLines(isMultiline ? nbLine : 1);
            inputFieldModelContainer.model.SetPrivate(isPrivate);

            return inputFieldModelContainer.gameObject;
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
        /// <param name="inputFieldGameobject">The input field GameObject to be returned to the pool.</param>
        public void Return(GameObject inputFieldGameobject)
        {
            var inputFieldModelContainer = inputFieldGameobject.GetComponent<InputFieldModelContainer>();
            if (!inputFieldModelContainer)
                return;

            _lstInputFieldsAvailable.Enqueue(inputFieldModelContainer);

            inputFieldModelContainer.gameObject.SetActive(false);
            inputFieldModelContainer.transform.SetParent(transform, false);
        }
    }
}