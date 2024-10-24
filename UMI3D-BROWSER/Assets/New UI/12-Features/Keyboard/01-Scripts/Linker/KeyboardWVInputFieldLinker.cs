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
using UnityEngine.Events;

namespace umi3d.browserRuntime.ui.keyboard
{
    public class KeyboardWVInputFieldLinker : MonoBehaviour
    {
        [Tooltip("Event raised when text has been added.")]
        public UnityEvent<string> addText = new();
        [Tooltip("Event raised when the delete button is pressed.")]
        public UnityEvent deleteCharacter = new();
        [Tooltip("Event raised when the enter or submit button is pressed.")]
        public UnityEvent enterOrSubmit = new();

        bool isSelected = false;
        Notifier selectionNotifier;
        Notifier deselectionNotifier;

        void Awake()
        {
            selectionNotifier = NotificationHub.Default
                .GetNotifier<KeyboardNotificationKeys.TextFieldSelected>(this);
            deselectionNotifier = NotificationHub.Default
                .GetNotifier<KeyboardNotificationKeys.TextFieldDeselected>(this);
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

        public void WebViewTextFieldSelected()
        {
            if (isSelected)
            {
                return;
            }
            isSelected = true;

            selectionNotifier[KeyboardNotificationKeys.TextFieldSelected.IsPreviewBar] = false;
            selectionNotifier[KeyboardNotificationKeys.TextFieldSelected.SelectionPositions] = 0;
            selectionNotifier[KeyboardNotificationKeys.TextFieldSelected.InputFieldText] = null;
            selectionNotifier.Notify();
        }

        public void WebViewTextFieldUnselected()
        {
            if (!isSelected)
            {
                return;
            }
            isSelected = false;

            deselectionNotifier.Notify();
        }

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
                    AddCharacters(notification);
                    break;
                case TextFieldTextUpdate.RemoveCharacters:
                    RemoveCharacters(notification);
                    break;
                case TextFieldTextUpdate.SubmitText:
                    SubmitText(notification);
                    break;
                default:
                    UnityEngine.Debug.LogError($"Unhandled case.");
                    break;
            }
        }

        void AddCharacters(Notification notification)
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

            addText.Invoke(characters);
        }

        void RemoveCharacters(Notification notification)
        {
            deleteCharacter.Invoke();
        }

        void SubmitText(Notification notification)
        {
            enterOrSubmit.Invoke();
        }
    }
}