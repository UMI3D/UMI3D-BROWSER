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

using umi3d.cdk.interaction;
using UnityEngine;

namespace umi3d.browserRuntime.ui.contextualMenu
{
    internal class ContextualMenuView : MonoBehaviour, IInteractableHoverStateDelegate, IContextualMenuActivationObserver
    {
        [SerializeField] float _offset = 10.0f;

        Canvas _canvas;
        Vector3 _interactablePosition;

        ContextualMenuController _controller;
        ContextualMenuModel _model;

        void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();

            _controller = GetComponentInParent<ContextualMenuController>();
            _model = _controller.model;
            _model.Subscribe(this);

            InteractableHoverStateListener.delegates.Add(this);
        }

        void Start()
        {
            _controller.ActivationUpdated(false);
        }

        void OnDestroy()
        {
            _model?.Unsubscribe(this);
        }

        public void OnHover(Collider collider, InteractableContainer interactableContainer, InteractableHoverStateListener hoverStateListener)
        {
        }

        public void OnHoverEnter(Collider collider, InteractableContainer interactableContainer, InteractableHoverStateListener hoverStateListener)
        {
            _interactablePosition = interactableContainer.transform.position;
        }

        public void OnHoverExit(Collider collider, InteractableContainer interactableContainer, InteractableHoverStateListener hoverStateListener) { }

        public void UpdateActivation(bool isActive)
        {
#if UMI3D_XR
            if (isActive)
            {
                var direction = (Camera.main.transform.position - _interactablePosition).normalized;
                _canvas.transform.position = _interactablePosition + direction * _offset;
                _canvas.transform.LookAt(_interactablePosition);
            }
#endif
            gameObject.SetActive(isActive);
        }
    }
}