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
using TMPro;
using UnityEngine;

namespace umi3d.browserRuntime.ui
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class DropdownView : MonoBehaviour, IValueObserver<string>, IDropdownOptionsObserver
    {
        TMP_Dropdown _dropdown;

        [Header("Dropdown height")]
        [SerializeField] int _maxCharactersPerLine = 27;
        RectTransform _rectTransform;
        float _initialHeight;
        [SerializeField] RectTransform _templateItemRectTransform;
        float _templateItemInitialHeight;

        DropdownController _controller;
        DropdownModel _model;

        void Awake()
        {
            _dropdown = GetComponent<TMP_Dropdown>();
            _dropdown.onValueChanged.AddListener(OnValueChanged);

            _rectTransform = GetComponent<RectTransform>();
            _initialHeight = _rectTransform.sizeDelta.y;
            _templateItemInitialHeight = _templateItemRectTransform.sizeDelta.y;

            _controller = GetComponentInParent<DropdownController>();
            _model = _controller.model;
            _model.Subscribe((IValueObserver<string>)this);
            _model.Subscribe((IDropdownOptionsObserver)this);
        }

        void Start()
        {
            updateOptions(_model.options);
            _dropdown.RefreshShownValue();
        }

        void OnEnable()
        {
            _dropdown.Select();
        }

        void OnDestroy()
        {
            _model.Unsubscribe((IValueObserver<string>)this);
            _model.Unsubscribe((IDropdownOptionsObserver)this);
        }

        void OnValueChanged(int newIndex)
        {
            _controller.ValueUpdated(newIndex);
        }

        public void updateValue(string value)
        {
            _dropdown.value = _model.IndexOf(value);

            // Update the height of the dropdown if the label has to display more than one line.
            if (string.IsNullOrEmpty(value)) { return; }
            _rectTransform.sizeDelta = new Vector2(
                _rectTransform.sizeDelta.x, 
                _initialHeight * (value.Length / _maxCharactersPerLine + 1)
            );
        }

        public void updateOptions(List<string> options)
        {
            updateOptions(options.GetEnumerator());
        }

        void updateOptions(IEnumerator<string> options)
        {
            _dropdown.ClearOptions();

            // Update the height of the dropdown if the label has to display more than one line.
            int numberOfCharacters = 0;
            while (options.MoveNext())
            {
                _dropdown.options.Add(new TMP_Dropdown.OptionData(options.Current));

                if (options.Current.Length > numberOfCharacters)
                {
                    numberOfCharacters = options.Current.Length;
                }
            }

            _templateItemRectTransform.sizeDelta = new Vector2(
                _templateItemRectTransform.sizeDelta.x, 
                _initialHeight * (numberOfCharacters / _maxCharactersPerLine + 1)
            );
        }
    }
}