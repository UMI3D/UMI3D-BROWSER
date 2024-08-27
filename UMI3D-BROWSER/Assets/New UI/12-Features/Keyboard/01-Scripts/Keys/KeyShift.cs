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
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.keyboard
{
    public class KeyShift : MonoBehaviour
    {
        [SerializeField] Sprite iconDefault;
        [SerializeField] Sprite iconLock;

        PointerDownBehaviour pointerDown;
        Button button;
        Image icon;

        bool isLowerCase = true;
        bool isUpperCaseLocked = false;

        Dictionary<string, object> info = new()
        {
            { KeyboardNotificationKeys.Info.IsABC, true },
            { KeyboardNotificationKeys.Info.IsLowerCase, true },
            { KeyboardNotificationKeys.Info.IsUpperCaseLocked, false }
        };

        void Awake()
        {
            pointerDown = GetComponent<PointerDownBehaviour>();
            button = GetComponent<Button>();
            icon = transform
                .GetChild(0)
                .GetChild(0)
                .GetComponent<Image>();
        }

        void OnEnable()
        {
            pointerDown.pointerClicked += PointerDown;

            NotificationHub.Default.Subscribe(
               this,
               KeyboardNotificationKeys.ChangeMode,
               null,
               ABCOrSymbol
           );

            NotificationHub.Default.Subscribe(
               this,
               KeyboardNotificationKeys.AddOrRemoveCharacters,
               null,
               AddOrRemoveCharacters
           );
        }

        void OnDisable()
        {
            pointerDown.pointerClicked -= PointerDown;

            NotificationHub.Default.Unsubscribe(this, KeyboardNotificationKeys.ChangeMode);
        }

        void PointerDown(Notification notification)
        {
            if (!notification.TryGetInfoT(PointerBehaviour.NKIsImmediate, out bool isImmediate) || !isImmediate)
            {
                return;
            }
            if (!notification.TryGetInfoT(PointerBehaviour.NKCount, out int count))
            {
                UnityEngine.Debug.LogError($"[KeyShift] No PointerBehaviour.NKCount keys.");
                return;
            }

            if (count == 1)
            {
                // When simple click :
                // switch between lower case and upper case.
                isLowerCase = !isLowerCase;
                // unlock the upper case.
                isUpperCaseLocked = false;
                // change the sprite to default.
                icon.sprite = iconDefault;
            }
            else if (count == 2)
            {
                // When double click :
                // set to upper case.
                isLowerCase = false;
                // lock the upper case.
                isUpperCaseLocked = true;
                // change the sprite to icon lock.
                icon.sprite = iconLock;
            }

            info[KeyboardNotificationKeys.Info.IsABC] = true;
            info[KeyboardNotificationKeys.Info.IsLowerCase] = isLowerCase;
            info[KeyboardNotificationKeys.Info.IsUpperCaseLocked] = isUpperCaseLocked;
            NotificationHub.Default.Notify(
                this,
                KeyboardNotificationKeys.ChangeMode,
                info
            );
        }

        void ABCOrSymbol(Notification notification)
        {
            // If the notification is raised by this class then return.
            if (notification.Publisher is KeyShift keyShift)
            {
                return;
            }

            // change the sprite to default.
            icon.sprite = iconDefault;

            if (!notification.TryGetInfoT(KeyboardNotificationKeys.Info.IsABC, out bool isABC))
            {
                UnityEngine.Debug.LogError($"[KeyShift] No KeyboardNotificationKeys.Info.IsABC keys.");
                return;
            }

            // Activate or deactivate this key.
            button.interactable = isABC;
            // Set the case to lower.
            isLowerCase = true;
            // Unlock the upper case.
            isUpperCaseLocked = false;
        }

        void AddOrRemoveCharacters(Notification notification)
        {
            if (isUpperCaseLocked || !button.IsInteractable())
            {
                return;
            }

            if (!notification.TryGetInfoT(KeyboardNotificationKeys.Info.IsAddingCharacters, out bool isAdding))
            {
                UnityEngine.Debug.LogError($"[KeyShift] No KeyboardNotificationKeys.Info.IsAddingCharacters keys.");
                return;
            }

            if (isAdding && notification.TryGetInfoT(KeyboardNotificationKeys.Info.Characters, out char character) && character == ' ')
            {
                return;
            }

            isLowerCase = true;
            isUpperCaseLocked = false;

            info[KeyboardNotificationKeys.Info.IsABC] = true;
            info[KeyboardNotificationKeys.Info.IsLowerCase] = isLowerCase;
            info[KeyboardNotificationKeys.Info.IsUpperCaseLocked] = isUpperCaseLocked;
            NotificationHub.Default.Notify(
                this,
                KeyboardNotificationKeys.ChangeMode,
                info
            );
        }
    }
}