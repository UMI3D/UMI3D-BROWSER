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

using inetum.unityUtils.observation;
using System.Collections;
using System.Collections.Generic;
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
        public UMI3DInputFieldSelection Selection { get; private set; }
        Notifier deselectionNotifier;

#if UMI3D_XR
        void Awake()
        {
            inputField = GetComponent<TMPro.TMP_InputField>();

            Selection = new(this);
            Selection.Blur();
            Selection.allowTextModification = !waitForSubmit;
            Selection.allowSelection = !waitForSubmit;

            deselectionNotifier = NotificationHub.Default.GetNotifier(
                this,
                ID.FromType<KeyboardNotificationKeys.TextFieldDeselected>()
            );
        }

        void OnEnable()
        {
            NotificationHub.Default.Subscribe(
                this,
                KeyboardNotificationKeys.AddOrRemoveCharacters,
                (Callback)AddOrRemoveCharacters
            );

            Selection.OnEnable();
        }

        void OnDisable()
        {
            NotificationHub.Default.Unsubscribe(this, KeyboardNotificationKeys.AddOrRemoveCharacters);

            Selection.OnDisable();
        }

        void AddOrRemoveCharacters(Notification notification)
        {
            if (!Selection.isActive)
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
                    else
                    {
                        Dictionary<string, object> info = new()
                        {
                            { KeyboardNotificationKeys.Info.TextFieldTextUpdate, TextFieldTextUpdate.AddCharacters },
                            { KeyboardNotificationKeys.Info.Characters, '\n' }
                        };
                        NotificationHub.Default.Notify(
                            this,
                            KeyboardNotificationKeys.AddOrRemoveCharacters,
                            info
                        );
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

            if (!Selection.isTextSelected)
            {
                int caretPosition = Selection.stringPosition;

                inputField.text = text.Insert(caretPosition, characters);
                Selection.DeselectWithoutNotify(caretPosition + characters.Length);
            }
            else
            {
                var start = Mathf.Min(Selection.startPosition, Selection.endPosition);
                var end = Mathf.Max(Selection.startPosition, Selection.endPosition);

                text = text.Remove(start, end - start);
                text = text.Insert(start, characters);
                inputField.text = text;
                Selection.DeselectWithoutNotify(start + 1);
            }
        }

        void RemoveCharacters(Notification notification)
        {
            if (Selection.stringPosition == 0 && !Selection.isTextSelected)
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
                if (!Selection.isTextSelected)
                {
                    int caretPosition = Selection.stringPosition;

                    inputField.text = text.Remove(caretPosition - 1, 1);
                    Selection.DeselectWithoutNotify(caretPosition - 1);
                }
                else
                {
                    var start = Mathf.Min(Selection.startPosition, Selection.endPosition);
                    var end = Mathf.Max(Selection.startPosition, Selection.endPosition);

                    text = text.Remove(start, end - start);
                    Selection.DeselectWithoutNotify(start);
                    inputField.text = text;
                }
            }
            // In phase 1: delete world by world.
            else if (deletionPhase == 1)
            {
                int caretPosition = Selection.stringPosition;

                // The part that will be partially deleted.
                string left = text.Substring(0, caretPosition);
                // The part that will not be deleted.
                string right = text.Substring(caretPosition, text.Length - caretPosition);

                // Remove the trailing spaces.
                string trimmedLeft = left.TrimEnd();
                if (trimmedLeft.Length < left.Length)
                {
                    inputField.text = trimmedLeft + right;
                    Selection.DeselectWithoutNotify(trimmedLeft.Length);
                    return;
                }

                // Remove the last word.
                int lastIdxOfSpace = left.LastIndexOf(' ');

                if (lastIdxOfSpace == -1)
                {
                    inputField.text = right;
                    Selection.DeselectWithoutNotify(0);
                }
                else
                {
                    inputField.text = left.Substring(0, lastIdxOfSpace + 1) + right;
                    Selection.DeselectWithoutNotify(lastIdxOfSpace + 1);
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