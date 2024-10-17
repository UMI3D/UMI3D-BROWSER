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
using System.Diagnostics;
using umi3d.browserRuntime.NotificationKeys;
using UnityEngine;
using UnityEngine.Events;

namespace umi3d.browserRuntime.ui.keyboard
{
    [RequireComponent(typeof(TMPro.TMP_InputField))]
    public class KeyboardTMPInputFieldLinker : MonoBehaviour
    {
        [Tooltip("Close the keyboard when the submit button is pressed.")]
        public bool closeOnSubmit = true;

        [Tooltip("Whether the input field wait for the submit but to be pressed to update the text.")]
        [SerializeField] bool waitForSubmit = false;

        [Tooltip("Event raised when the enter or submit button is pressed.")]
        public UnityEvent enterOrSubmit = new();

        TMPro.TMP_InputField inputField;
        UMI3DInputFieldSelection selection;
        Notifier deselectionNotifier;

#if UMI3D_XR
        void Awake()
        {
            enabled = false;
            return;
            inputField = GetComponent<TMPro.TMP_InputField>();

            selection = new(this);
            selection.Blur();
            selection.allowTextModification = !waitForSubmit;
            selection.allowSelection = !waitForSubmit;

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

            selection.OnEnable();
        }

        void OnDisable()
        {
            NotificationHub.Default.Unsubscribe(this, KeyboardNotificationKeys.AddOrRemoveCharacters);

            selection.OnDisable();
        }

        void AddOrRemoveCharacters(Notification notification)
        {
            if (!selection.isActive)
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
                    if (!waitForSubmit)
                    {
                        AddCharacters(notification);
                    }
                    break;
                case TextFieldTextUpdate.RemoveCharacters:
                    if (!waitForSubmit)
                    {
                        RemoveCharacters(notification);
                    }
                    break;
                case TextFieldTextUpdate.SubmitText:
                    if (waitForSubmit)
                    {
                        SubmitText(notification);
                    }

                    if (closeOnSubmit)
                    {
                        deselectionNotifier.Notify();
                    }

                    enterOrSubmit.Invoke();
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

            string text = inputField.text;

            if (!selection.isTextSelected)
            {
                int caretPosition = selection.stringPosition;

                inputField.text = text.Insert(caretPosition, characters);
                selection.DeselectWithoutNotify(caretPosition + characters.Length);
            }
            else
            {
                int start = selection.startPosition;
                int end = selection.endPosition;

                text = text.Remove(start, end - start);
                text = text.Insert(start, characters);
                inputField.text = text;
                selection.DeselectWithoutNotify(start + 1);
            }
        }

        void RemoveCharacters(Notification notification)
        {
            if (selection.stringPosition == 0 && !selection.isTextSelected)
            {
                return;
            }

            if (!notification.TryGetInfoT(KeyboardNotificationKeys.Info.DeletionPhase, out int deletionPhase))
            {
                UnityEngine.Debug.LogWarning($"[KeyboardPreviewBar] No deletion phase.");
                return;
            }

            string text = inputField.text;

            // In phase 0: delete only one character or the selected text.
            if (deletionPhase == 0)
            {
                if (!selection.isTextSelected)
                {
                    int caretPosition = selection.stringPosition;

                    inputField.text = text.Remove(caretPosition - 1, 1);
                    selection.DeselectWithoutNotify(caretPosition - 1);
                }
                else
                {
                    int start = selection.startPosition;
                    int end = selection.endPosition;

                    text = text.Remove(start, end - start);
                    selection.DeselectWithoutNotify(start);
                    inputField.text = text;
                }
            }
            // In phase 1: delete world by world.
            else if (deletionPhase == 1)
            {
                int caretPosition = selection.stringPosition;

                // The part that will be partially deleted.
                string left = text.Substring(0, caretPosition);
                // The part that will not be deleted.
                string right = text.Substring(caretPosition, text.Length - caretPosition);

                // Remove the trailing spaces.
                string trimmedLeft = left.TrimEnd();
                if (trimmedLeft.Length < left.Length)
                {
                    inputField.text = trimmedLeft + right;
                    selection.DeselectWithoutNotify(trimmedLeft.Length);
                    return;
                }

                // Remove the last word.
                int lastIdxOfSpace = left.LastIndexOf(' ');

                if (lastIdxOfSpace == -1)
                {
                    inputField.text = right;
                    selection.DeselectWithoutNotify(0);
                }
                else
                {
                    inputField.text = left.Substring(0, lastIdxOfSpace + 1) + right;
                    selection.DeselectWithoutNotify(lastIdxOfSpace + 1);
                }
            }
            else
            {
                UnityEngine.Debug.LogError($"[KeyboardPreviewBar] Deletion phase case unhandled.");
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

            inputField.text = characters;
        }
#else
        private void Awake()
        {
            enabled = false;
        }
#endif
    }
}