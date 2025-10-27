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
using UnityEngine.Localization.Components;

namespace umi3d.browserRuntime.ui
{
    internal class MumbleIndicatorTextView : MonoBehaviour, IMumbleIndicatorConnectedObserver
    {
        [SerializeField] string _connectedTextKey;
        [SerializeField] string _notConnectedTextKey;

        LocalizeStringEvent _localizeStringEvent;

        private void Awake()
        {
            _localizeStringEvent = GetComponent<LocalizeStringEvent>();
            UpdateConnected(false);
        }

        private void Start()
        {
            var controller = GetComponentInParent<MumbleIndicatorController>();
            controller.Model.Subscribe(this);
        }

        public void UpdateConnected(bool isConnected)
        {
            _localizeStringEvent.SetEntry(isConnected ? _connectedTextKey : _notConnectedTextKey);
        }
    }
}