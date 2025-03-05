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
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace umi3d.browserRuntime.interactions
{
    public class BrowserInteractableManager : IInteractableVisibilityDelegate, IInteractableVisibilityDataDelegate
    {
        #region Initialization

        public static BrowserInteractableManager @default => _default.Value;
        static Lazy<BrowserInteractableManager> _default = new(() => new BrowserInteractableManager());
        BrowserInteractableManager()
        {
            visibilityListeners = _visibilityListeners.AsReadOnly();
            InteractableVisibilityListener.delegates.Add(this);
            InteractableVisibilityListener.dataDelegate = this;
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void OnBeforeSceneLoadRuntimeMethod()
        {
            _ = @default;

            var op = Addressables.LoadAssetAsync<GameObject>("InteractableUI");
            @default.interactableUIPrefab = op.WaitForCompletion(); //Forces synchronous load so that we can return immediately
        }

        #endregion

        GameObject interactableUIPrefab;
        List<InteractableVisibilityListener> _visibilityListeners = new();
        public readonly ReadOnlyCollection<InteractableVisibilityListener> visibilityListeners;

        #region IInteractableVisibilityDelegate

        public void OnBecameVisible(Renderer renderer, InteractableContainer interactableContainer, InteractableVisibilityListener visibilityListener)
        {
            _visibilityListeners.Add(visibilityListener);
            InteractableUIVC viewController = visibilityListener.interactableUI.GetComponent<InteractableUIVC>();
            viewController.renderer = renderer;
            viewController.interactable = interactableContainer.Interactable;
            viewController.enabled = true;
            viewController.DisplayFeedback(true);
            viewController.SetInteractableName(interactableContainer.Interactable.name);
        }

        public void OnBecameInvisible(Renderer renderer, InteractableContainer interactableContainer, InteractableVisibilityListener visibilityListener)
        {
            _visibilityListeners.Remove(visibilityListener);
            InteractableUIVC viewController = visibilityListener.interactableUI.GetComponent<InteractableUIVC>();
            viewController.enabled = false;
            viewController.DisplayFeedback(false);
        }

        #endregion

        public GameObject GetInteractableUI()
        {
            GameObject _interactableUI = GameObject.Instantiate(interactableUIPrefab);
            return _interactableUI;
        }
    }
}