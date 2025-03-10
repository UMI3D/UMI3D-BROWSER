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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using umi3d.cdk.interaction;
using umi3d.common.interaction;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace umi3d.browserRuntime.interactions
{
    public class BrowserInteractableManager : IInteractableVisibilityDelegate, IInteractableUIControllerDataDelegate, IInteractableHoverStateDelegate
    {
        #region Initialization

        public static BrowserInteractableManager @default => _default.Value;
        static Lazy<BrowserInteractableManager> _default = new(() => new BrowserInteractableManager());
        BrowserInteractableManager()
        {
            visibilityListeners = _visibilityListeners.AsReadOnly();
            InteractableVisibilityListener.delegates.Add(this);
            InteractableUIController.dataDelegate = this;

            InteractableHoverStateListener.delegates.Add(this);
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void OnBeforeSceneLoadRuntimeMethod()
        {
            _ = @default;

            var op = Addressables.LoadAssetAsync<GameObject>("InteractableUI");
            @default.interactableUIPrefab = op.WaitForCompletion(); //Forces synchronous load so that we can return immediately
        }

        #endregion

        #region IInteractableUIControllerDataDelegate

        GameObject interactableUIPrefab;

        public bool TryGetInteractableUI(out GameObject gameObject, out object viewController, InteractableContainer interactableContainer, InteractableUIController uiController)
        {
            gameObject = GameObject.Instantiate(interactableUIPrefab);
            InteractableUIVC vc = gameObject.GetComponent<InteractableUIVC>();
            viewController = vc;
            vc.interactable = interactableContainer.Interactable;
            vc.renderer = uiController.visibilityListener.renderer;

            vc.SetInteractableName(interactableContainer.Interactable.name);
            return true;
        }

        #endregion

        #region IInteractableVisibilityDelegate

        List<InteractableVisibilityListener> _visibilityListeners = new();
        public readonly ReadOnlyCollection<InteractableVisibilityListener> visibilityListeners;

        public void OnBecameVisible(Renderer renderer, InteractableContainer interactableContainer, InteractableVisibilityListener visibilityListener)
        {
            _visibilityListeners.Add(visibilityListener);
            InteractableUIController uiController = visibilityListener.uiController;

            if (!uiController.TryCastViewController(out InteractableUIVC viewController))
            {
                return;
            }

            viewController.OnInteractableBecameVisible();
        }

        public void OnBecameInvisible(Renderer renderer, InteractableContainer interactableContainer, InteractableVisibilityListener visibilityListener)
        {
            _visibilityListeners.Remove(visibilityListener);
            InteractableUIController uiController = visibilityListener.uiController;

            if (!uiController.TryCastViewController(out InteractableUIVC viewController))
            {
                return;
            }

            viewController.OnInteractableBecameInvisible();
        }

        #endregion

        #region IInteractableHoverStateDelegate

        List<InteractableHoverStateListener> _hoverStateListener = new();
        public readonly ReadOnlyCollection<InteractableHoverStateListener> hoverStateListener;

        public void OnHoverEnter(Collider collider, InteractableContainer interactableContainer, InteractableHoverStateListener hoverStateListener)
        {
            _hoverStateListener.Add(hoverStateListener);
            InteractableUIController uiController = hoverStateListener.uiController;

            if (!uiController.TryCastViewController(out InteractableUIVC viewController))
            {
                return;
            }

            viewController.OnInteractableBecameHovered();
        }

        public void OnHoverExit(Collider collider, InteractableContainer interactableContainer, InteractableHoverStateListener hoverStateListener)
        {
            _hoverStateListener.Remove(hoverStateListener);
            InteractableUIController uiController = hoverStateListener.uiController;

            if (!uiController.TryCastViewController(out InteractableUIVC viewController))
            {
                return;
            }

            viewController.OnInteractableBecameNotHovered();
        }

        public void OnHover(Collider collider, InteractableContainer interactableContainer, InteractableHoverStateListener hoverStateListener) {}

        #endregion
    }
}