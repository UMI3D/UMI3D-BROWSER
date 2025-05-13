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

using System.Collections;
using TMPro;
using UnityEngine;

namespace umi3d.browserRuntime.ui.inputField
{
    [RequireComponent(typeof(TMP_InputField))]
    public class InputFieldPinFormatter : MonoBehaviour, IInputFieldPinObserver
    {
        TMP_InputField _inputField;

        InputFieldController _controller;
        InputFieldModel _model;

        private void Awake()
        {
            _inputField = GetComponent<TMP_InputField>();
            _controller = GetComponentInParent<InputFieldController>();
            _model = _controller.model;
            _model.Subscribe(this);
        }

        private void OnDestroy()
        {
            _model.Unsubscribe(this);
        }

        public void UpdatePin(bool isPin)
        {
            if (isPin)
                _inputField.onValueChanged.AddListener(OnValueChanged);
            else
                _inputField.onValueChanged.RemoveListener(OnValueChanged);
        }

        void OnValueChanged(string newValue)
        {
            newValue = newValue.Replace(" ", "");

            if (newValue.Length > 6)
                newValue = newValue.Substring(0, 6);

            if (newValue.Length > 3)
                newValue = newValue.Insert(3, " ");

            _inputField.text = newValue;
            StartCoroutine(SetCaretPosition());
        }

        IEnumerator SetCaretPosition()
        {
            yield return new WaitForEndOfFrame();
            _inputField.MoveToEndOfLine(false, false);
        }
    }
}
