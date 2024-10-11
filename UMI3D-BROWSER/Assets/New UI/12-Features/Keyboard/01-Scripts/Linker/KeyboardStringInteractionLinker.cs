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

namespace umi3d.browserRuntime.ui.keyboard
{
    public class KeyboardStringInteractionLinker : MonoBehaviour
    {
        [Tooltip("Event raised when the enter or submit button is pressed.")]
        public event System.Action<string> enterOrSubmit;

        bool isSelected = false;
        Notifier selectionNotifier;

        void Awake()
        {
            selectionNotifier = NotificationHub.Default
                .GetNotifier<KeyboardNotificationKeys.TextFieldSelected>(this);
        }

        void OnEnable()
        {
            NotificationHub.Default.Subscribe(
                this,
                KeyboardNotificationKeys.AddOrRemoveCharacters,
                AddOrRemoveCharacters
            );

            NotificationHub.Default.Subscribe<KeyboardNotificationKeys.TextFieldSelected>(
                this,
                new FilterByRef(FilterType.AcceptAllExcept, this),
                OtherTextFieldSelected
            );

            NotificationHub.Default.Subscribe<KeyboardNotificationKeys.TextFieldDeselected>(
                this,
                new FilterByRef(FilterType.AcceptAllExcept, this),
                TextFieldDeselected
            );
        }

        void OnDisable()
        {
            NotificationHub.Default.Unsubscribe(this, KeyboardNotificationKeys.AddOrRemoveCharacters);

            NotificationHub.Default.Unsubscribe<KeyboardNotificationKeys.TextFieldSelected>(this);

            NotificationHub.Default.Unsubscribe<KeyboardNotificationKeys.TextFieldDeselected>(this);
        }

        public void TextFieldSelected(string text)
        {
            isSelected = true;

            selectionNotifier[KeyboardNotificationKeys.TextFieldSelected.IsPreviewBar] = false;
            selectionNotifier[KeyboardNotificationKeys.TextFieldSelected.SelectionPositions] = text.Length;
            selectionNotifier[KeyboardNotificationKeys.TextFieldSelected.InputFieldText] = text;
            selectionNotifier.Notify();
        }

        /// <summary>
        /// Method called when another textfield has been selected.
        /// </summary>
        /// <param name="notification"></param>
        void OtherTextFieldSelected()
        {
            isSelected = false;
        }

        void TextFieldDeselected()
        {
            isSelected = false; 
        }

        void AddOrRemoveCharacters(Notification notification)
        {
            if (!isSelected)
            {
                return;
            }

            if (!notification.TryGetInfoT(KeyboardNotificationKeys.Info.TextFieldTextUpdate, out TextFieldTextUpdate textUpdate))
            {
                return;
            }

            switch (textUpdate)
            {
                case TextFieldTextUpdate.AddCharacters:
                    break;
                case TextFieldTextUpdate.RemoveCharacters:
                    break;
                case TextFieldTextUpdate.SubmitText:
                    SubmitText(notification);
                    break;
                default:
                    UnityEngine.Debug.LogError($"Unhandled case.");
                    break;
            }
        }

        void SubmitText(Notification notification)
        {
            if (!notification.TryGetInfoT(KeyboardNotificationKeys.Info.Characters, out string characters, false))
            {
                if (!notification.TryGetInfoT(KeyboardNotificationKeys.Info.Characters, out char character, false))
                {
                    notification.LogError(
                        nameof(KeyboardPreviewBar),
                        KeyboardNotificationKeys.Info.Characters,
                        "Character added is neither string not char."
                    );
                    return;
                }

                characters = character.ToString();
            }

            enterOrSubmit?.Invoke(characters);
        }
    }
}