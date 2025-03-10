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
using umi3d.browserRuntime.ui.tablet;
using umi3d.cdk.interaction;
using UnityEngine;

namespace umi3d.browserRuntime.ui.contextualMenu
{
    public class ContextualMenuView : MonoBehaviour, IInteractableHoverStateDelegate
    {
        [SerializeField] float _offset = 10.0f;

        Vector3 _interactablePosition;
        Canvas _canvas;

        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();

            NotificationHub.Default.Subscribe(this,
                ID.FromType<ContextualMenuNotificationKeys.Open>(), 
                (Callback)Display);

            NotificationHub.Default.Subscribe(this,
                ID.FromType<ContextualMenuNotificationKeys.Close>(), 
                (Callback)Hide);

            NotificationHub.Default.Subscribe(this, 
                ID.FromType<TabletNotificationKeys.Opened>(), 
                (Callback)Hide);

            InteractableHoverStateListener.delegates.Add(this);
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void OnHover(Collider collider, InteractableContainer interactableContainer, InteractableHoverStateListener hoverStateListener)
        {
        }

        public void OnHoverEnter(Collider collider, InteractableContainer interactableContainer, InteractableHoverStateListener hoverStateListener)
        {
            _interactablePosition = interactableContainer.transform.position;
        }

        public void OnHoverExit(Collider collider, InteractableContainer interactableContainer, InteractableHoverStateListener hoverStateListener) { }

        private void Display()
        {
#if UMI3D_XR
            var direction = (Camera.main.transform.position - _interactablePosition).normalized;
            _canvas.transform.position = _interactablePosition + direction * _offset;
            _canvas.transform.LookAt(_interactablePosition);
#endif

            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}