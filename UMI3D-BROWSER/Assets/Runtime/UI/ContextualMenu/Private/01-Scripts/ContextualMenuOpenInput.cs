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

using UnityEngine;
using UnityEngine.InputSystem;

namespace umi3d.browserRuntime.ui.contextualMenu
{
    internal class ContextualMenuOpenInput : MonoBehaviour
    {
        [SerializeField] InputActionReference _openInputAction;

        ContextualMenuController _controller;
        ContextualMenuModel _model;

        void Awake()
        {
            _openInputAction.action.started += OnClick;
            _openInputAction.action.Enable();

            _controller = GetComponentInParent<ContextualMenuController>();
        }

        void Start()
        {
            _model = _controller.model;
        }

        void OnDestroy()
        {
            _openInputAction.action.started -= OnClick;
        }

        void OnClick(InputAction.CallbackContext context)
        {
            if (_model.parameters.Count == 0) { return; }

            _controller.OpenInputPressed();
        }
    }
}