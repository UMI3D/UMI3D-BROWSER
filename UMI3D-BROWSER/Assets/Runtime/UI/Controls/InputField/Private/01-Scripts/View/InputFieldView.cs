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

using inetum.unityUtils.observation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.inputField
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(TMP_InputField))]
    [RequireComponent(typeof(LayoutElement))]
    public class InputFieldView : MonoBehaviour, IValueObserver<string>, IInputFieldContentTypeObserver, IInputFieldNbLineObserver
    {
        [SerializeField] RectTransform _viewport;

        TMP_InputField _inputField;
        LayoutElement _layoutElement;

        InputFieldController _controller;
        InputFieldModel _model;

        void Awake()
        {
            _inputField = GetComponent<TMP_InputField>();
            _layoutElement = GetComponent<LayoutElement>();
			_inputField.onValueChanged.AddListener(OnValueChanged);
            _inputField.onSelect.AddListener(OnSelect);
            _inputField.onDeselect.AddListener(OnDeselect);

            _controller = GetComponentInParent<InputFieldController>();
        }

        void Start()
        {
            _model = _controller.model;
            _model.Subscribe(this as IValueObserver<string>);
            _model.Subscribe(this as IInputFieldContentTypeObserver);
            _model.Subscribe(this as IInputFieldNbLineObserver);
        }

        private void OnEnable()
        {
            _inputField.Select();
        }

        private void OnDestroy()
        {
            _inputField.onValueChanged.RemoveListener(OnValueChanged);
            _inputField.onSelect.RemoveListener(OnSelect);
            _inputField.onDeselect.RemoveListener(OnDeselect);
            
            _model?.Unsubscribe(this as IValueObserver<string>);
            _model?.Unsubscribe(this as IInputFieldContentTypeObserver);
            _model?.Unsubscribe(this as IInputFieldNbLineObserver);
        }

        void OnSelect(string s)
        {
            NotificationHub.Default.Notify(this, ID.FromType<InputFieldNotificationsKeys.Selected>());
        }

        void OnDeselect(string s)
        {
            NotificationHub.Default.Notify(this, ID.FromType<InputFieldNotificationsKeys.Deselected>());
        }

        void OnValueChanged(string newValue)
        {
            _controller.ValueUpdated(newValue);
        }

        public void updateValue(string value)
        {
            _inputField.SetTextWithoutNotify(value);
        }

        public void UpdateContentType(TMP_InputField.ContentType contentType)
        {
            _inputField.contentType = contentType;
            _inputField.ForceLabelUpdate();
        }

        public void UpdateNbLine(int nbLine)
        {
            if (!_viewport) { return; }

            RectTransform textAreaTransform = _inputField.textViewport.GetComponent<RectTransform>();
            float padding = textAreaTransform.offsetMin.y + textAreaTransform.offsetMax.y;

            TMP_Text textComponent = _inputField.textComponent;
            float desiredHeight = textComponent.GetPreferredValues(new string('\n', nbLine)).y + padding;

            _layoutElement.minHeight = desiredHeight;

            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, desiredHeight);

            _viewport.sizeDelta = new Vector2(_viewport.sizeDelta.x, desiredHeight);
        }
    }
}