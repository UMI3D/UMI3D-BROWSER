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
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui
{
    [RequireComponent(typeof(Key))]
    public class KeyShift : MonoBehaviour
    {
        [SerializeField] bool isLowerCase = true;
        Key key;
        Button button;

        void Awake()
        {
            key = GetComponent<Key>();
            button = GetComponent<Button>();

            key.PointerDown += PointerDown;
        }

        private void OnEnable()
        {
            NotificationHub.Default.Subscribe(
               this,
               KeyboardNotificationKeys.ChangeMode,
               null,
               ABCOrSymbol
           );
        }

        private void OnDisable()
        {
            NotificationHub.Default.Unsubscribe(this, KeyboardNotificationKeys.ChangeMode);
        }

        void PointerDown()
        {
            isLowerCase = !isLowerCase;

            NotificationHub.Default.Notify(
                this,
                KeyboardNotificationKeys.ChangeMode,
                new()
                {
                    { KeyboardNotificationKeys.ModeInfo.IsABC, true },
                    { KeyboardNotificationKeys.ModeInfo.IsLowerCase, isLowerCase }
                }
            );
        }

        void ABCOrSymbol(Notification notification)
        {
            if (!notification.TryGetInfoT(KeyboardNotificationKeys.ModeInfo.IsABC, out bool isABC))
            {
                UnityEngine.Debug.LogError($"[KeyShift] No ModeInfo.IsABC keys.");
                return;
            }

            button.interactable = isABC;

            if (notification.TryGetInfoT(KeyboardNotificationKeys.ModeInfo.IsLowerCase, out bool isLowerCase))
            {
                this.isLowerCase = isLowerCase;
            }
        }
    }
}