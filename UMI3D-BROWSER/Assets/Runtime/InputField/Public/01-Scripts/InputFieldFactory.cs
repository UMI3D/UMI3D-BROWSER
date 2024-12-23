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

        /// <summary>
        /// Create or get an input field from the pool and set it up.
        /// </summary>
        /// <param name="parent">Transform where the input field will be attached.</param>
        /// <param name="label">Label of the input field.</param>
        /// <param name="value">Value of the input field.</param>
        /// <param name="placeholder">Placeholder of the input field.</param>
        /// <param name="isMultiline">Is singel or multi line.</param>
        /// <param name="nbLine">The number of line wanted. If isMultiline is false, it is automaticly 1.</param>
        /// <returns></returns>
        public GameObject GetOrCreateInputField(Transform parent, bool isMultiline, string label = "", string value = "", string placeholder = "", int nbLine = 1)
        {
            if (nbLine < 1) nbLine = 1;

            InputFieldModelContainer inputFieldModelContainer = GameObject.Instantiate(isMultiline ? _multiLinePrefab : _singleLinePrefab);

            if (label != null || label != string.Empty)
                inputFieldModelContainer.model.SetLabel(label);
            if (value != null || value != string.Empty)
                inputFieldModelContainer.model.SetValue(value);
            if (placeholder != null || placeholder != string.Empty)
                inputFieldModelContainer.model.SetPlaceholder(placeholder);
            if (nbLine != 1)
                inputFieldModelContainer.model.SetNbrLines(isMultiline ? nbLine : 1);

            return inputFieldModelContainer.gameObject;
        }

        /// <summary>
        /// Return an input field to the pool to be used later.
        /// </summary>
        /// <param name="inputFieldGameobject"></param>
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