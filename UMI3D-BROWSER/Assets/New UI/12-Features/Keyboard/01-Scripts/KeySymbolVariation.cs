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

using inetum.unityUtils;
using System.Collections;
using System.Collections.Generic;
using umi3d.browserRuntime.NotificationKeys;
using UnityEngine;

namespace umi3d.browserRuntime.ui
{
    [RequireComponent(typeof(KeyMainCharacter))]
    public class KeySymbolVariation : MonoBehaviour
    {
        [SerializeField] string variation;
        TMPro.TMP_Text text;

        void Awake()
        {
            text = GetComponent<TMPro.TMP_Text>();
            UnityEngine.Debug.Assert(text != null, $"[KeySymbolVariation] No TMP_Text");
        }

        private void OnEnable()
        {
            NotificationHub.Default.Subscribe(
                this,
                KeyboardNotificationKeys.ABCOrSymbol, 
                null, 
                SwitchToSymbol
            );
        }

        private void OnDisable()
        {
            NotificationHub.Default.Unsubscribe(this, KeyboardNotificationKeys.ABCOrSymbol);
        }

        void SwitchToSymbol(Notification notification)
        {
            if (!notification.TryGetInfoT(KeyboardNotificationKeys.ABCOrSymbolInfo.IsABC, out bool isABC) || isABC)
            {
                return;
            }

            text.text = variation;
        }
    }
}