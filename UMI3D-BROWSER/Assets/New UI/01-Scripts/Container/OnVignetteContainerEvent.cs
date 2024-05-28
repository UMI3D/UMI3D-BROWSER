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
using System;
using UnityEngine;
using UnityEngine.Events;

namespace umi3dBrowsers.container
{
    public class OnVignetteContainerEvent : MonoBehaviour
    {
        [SerializeField] private VignetteContainerEvent vignetteContainerEvent;

        [SerializeField] private UnityEvent onReset;
        [SerializeField] private UnityEvent<VignetteContainer.VignetteScale> onVignetteChangeMode;

        private void OnEnable()
        {
            vignetteContainerEvent.OnVignetteReset += OnReset;
            vignetteContainerEvent.OnVignetteChangeMode += OnVignetteChangeMode;
        }

        private void OnDisable()
        {
            vignetteContainerEvent.OnVignetteReset -= OnReset;
            vignetteContainerEvent.OnVignetteChangeMode -= OnVignetteChangeMode;
        }

        private void OnReset() => onReset?.Invoke();
        private void OnVignetteChangeMode(VignetteContainer.VignetteScale scale) => onVignetteChangeMode?.Invoke(scale);
    }
}
