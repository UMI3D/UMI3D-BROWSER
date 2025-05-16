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

using TMPro;
using UnityEngine;

namespace umi3d.browserRuntime.ui
{
    [RequireComponent(typeof(TMP_Text))]
    public class InputFieldLabelView : MonoBehaviour, ILabelObserver
    {
        TMP_Text _text;

        InputFieldController _controller;
        InputFieldModel _model;

        void Awake()
        {
            _text = GetComponent<TMP_Text>();
            _controller = GetComponentInParent<InputFieldController>();
            _model = _controller.model;
            _model.Subscribe(this);
        }

        void OnDestroy()
        {
            _model.Unsubscribe(this);
        }

        public void UpdateLabel(string label, bool isVisible)
        {
            gameObject.SetActive(isVisible);
            if (!isVisible) { return; }

            _text.text = label;
        }
    }
}