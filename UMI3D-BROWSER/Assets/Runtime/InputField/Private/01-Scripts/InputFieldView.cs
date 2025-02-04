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
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.inputField
{
    [RequireComponent(typeof(TMP_InputField))]
    [RequireComponent(typeof(LayoutElement))]
    public class InputFieldView : MonoBehaviour
    {
        [SerializeField] RectTransform _viewport;

        TMP_InputField _inputField;
        LayoutElement _layoutElement;

        InputFieldModelContainer _modelContainer;

        void Awake()
        {
            _inputField = GetComponent<TMP_InputField>();
            _layoutElement = GetComponent<LayoutElement>();
            _modelContainer = GetComponentInParent<InputFieldModelContainer>();
			_inputField.onValueChanged.AddListener(OnValueChanged);
            _inputField.onSelect.AddListener(OnSelect);
            _inputField.onDeselect.AddListener(OnDeselect);

            NotificationHub.Default.Subscribe(this,
                ID.FromType<InputFieldNotificationsKeys.InputFieldSet>(), 
                (Callback)InputFieldSet,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _modelContainer.model));
        }

        private void OnEnable()
        {
            _inputField.Select();
        }

        private void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
            _inputField.onValueChanged.RemoveListener(OnValueChanged);
            _inputField.onSelect.RemoveListener(OnSelect);
            _inputField.onDeselect.RemoveListener(OnDeselect);
        }

        void OnSelect(string s)
        {
            NotificationHub.Default.Notify(this, ID.FromType<InputFieldNotificationsKeys.Selected>());
        }

        void OnDeselect(string s)
        {
            NotificationHub.Default.Notify(this, ID.FromType<InputFieldNotificationsKeys.Deselected>());
        }

        private void OnValueChanged(string newValue)
        {
            _modelContainer.model.UpdateValue(newValue);
        }

        private void InputFieldSet(Notification notification)
        {
            if (notification.TryGetInfoT(InputFieldNotificationsKeys.InputFieldSet.Value, out string value))
            {
                _inputField.text = value;
            }

            if (notification.TryGetInfoT(InputFieldNotificationsKeys.InputFieldSet.IsPrivate, out bool isPrivate))
            {
                _inputField.contentType = isPrivate ? TMP_InputField.ContentType.Password : TMP_InputField.ContentType.Standard;
            }

            if (notification.TryGetInfoT(InputFieldNotificationsKeys.InputFieldSet.NbrLine, out int nbrLine))
            {
                if (!_viewport)
                    return;

                RectTransform textAreaTransform = _inputField.textViewport.GetComponent<RectTransform>();
                float padding = textAreaTransform.offsetMin.y + textAreaTransform.offsetMax.y;

                TMP_Text textComponent = _inputField.textComponent;
                float desiredHeight = textComponent.GetPreferredValues(new string('\n', nbrLine)).y + padding;

                _layoutElement.minHeight = desiredHeight;

                RectTransform rectTransform = GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, desiredHeight);

                _viewport.sizeDelta = new Vector2(_viewport.sizeDelta.x, desiredHeight);
            }
        }
    }
}