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

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace umi3d.browserRuntime.inputField
{
    [Serializable]
    public class InputFieldFactory : MonoBehaviour
    {
        [SerializeField] InputFieldModelContainer _singleLinePrefab;
        [SerializeField] InputFieldModelContainer _multiLinePrefab;

        Queue<InputFieldModelContainer> _lstInputFieldsAvailable = new();

        public GameObject GetOrCreateInputField(Transform parent, bool isMultiline)
        {
            InputFieldModelContainer inputFieldModelContainer;
            if (!_lstInputFieldsAvailable.TryDequeue(out inputFieldModelContainer))
                inputFieldModelContainer = GameObject.Instantiate(isMultiline ? _multiLinePrefab : _singleLinePrefab);

            inputFieldModelContainer.gameObject.SetActive(true);
            inputFieldModelContainer.transform.SetParent(parent, false);

            return inputFieldModelContainer.gameObject;
        }

        public GameObject GetOrCreateInputField(Transform parent, string title, string value, string placeholder, bool isMultiline, int nbLine)
        {
            if (nbLine < 1) nbLine = 1;

            InputFieldModelContainer inputFieldModelContainer = GetOrCreateInputField(parent, isMultiline).GetComponent<InputFieldModelContainer>();

            if (title != null || title != string.Empty)
                inputFieldModelContainer.model.SetTitle(title);
            if (value != null || value != string.Empty)
                inputFieldModelContainer.model.SetValue(value);
            if (placeholder != null || placeholder != string.Empty)
                inputFieldModelContainer.model.SetPlaceholder(placeholder);
            if (nbLine != 1)
                inputFieldModelContainer.model.SetNbrLines(isMultiline ? nbLine : 1);

            return inputFieldModelContainer.gameObject;
        }

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