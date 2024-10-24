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
using umi3d.browserRuntime.NotificationKeys;
using UnityEngine;

namespace umi3d.browserRuntime.ui.keyboard
{
    public class KeyboardPreviewBar : MonoBehaviour
    {
        TMPro.TMP_InputField inputField;
        KeyboardPreviewBarSelection selection;

        Notifier submitTextNotifier;

        void Awake()
        {
            inputField = GetComponentInChildren<TMPro.TMP_InputField>();
            UnityEngine.Debug.Assert(inputField != null, $"inputField is null");
            selection = GetComponentInChildren<KeyboardPreviewBarSelection>();
            if (selection == null)
            {
                inputField.gameObject.AddComponent<KeyboardPreviewBarSelection>();
            }

            submitTextNotifier = NotificationHub.Default.GetNotifier(
                this,
                KeyboardNotificationKeys.AddOrRemoveCharacters
            );
        }

        void Start()
        {
            selection.Blur();
        }

        void OnEnable()
        {
            NotificationHub.Default.Subscribe(
                this,
                KeyboardNotificationKeys.AddOrRemoveCharacters,
                new FilterByRef(FilterType.AcceptAllExcept, this),
                AddOrRemoveCharacters
            );

            NotificationHub.Default.Subscribe(
                this,
                KeyboardNotificationKeys.SpecialKeyPressed,
                SpecialKeyPressed
            );
        }

        void OnDisable()
        {
            NotificationHub.Default.Unsubscribe(this, KeyboardNotificationKeys.AddOrRemoveCharacters);
        }

        void AddOrRemoveCharacters(Notification notification)
        {
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
                    UnityEngine.Debug.LogError($"Unhandled case.");
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
            
            var text = inputField.text;

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
            if (!notification.TryGetInfoT(KeyboardNotificationKeys.Info.DeletionPhase, out int deletionPhase))
            {
                UnityEngine.Debug.LogWarning($"[KeyboardPreviewBar] No deletion phase.");
                return;
            }

            if (selection.stringPosition == 0 && !selection.isTextSelected)
            {
                return;
            }

            var text = inputField.text;

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

        void SpecialKeyPressed(Notification notification)
        {
            if (!notification.TryGetInfoT(KeyboardNotificationKeys.Info.SpecialKey, out SpecialKey specialKey) || specialKey != SpecialKey.Enter)
            {
                return;
            }
            
            submitTextNotifier[KeyboardNotificationKeys.Info.TextFieldTextUpdate] = TextFieldTextUpdate.SubmitText;
            submitTextNotifier[KeyboardNotificationKeys.Info.Characters] = inputField.text;
            submitTextNotifier.Notify();
        }

#if UNITY_EDITOR
        [ContextMenu("TestAddSimple")]
        void TestAddSimple()
        {
            char a = 'a';
            char z = 'z';
            char random = 'a';

            random = (char)Random.Range(a, z);
            UnityEngine.Debug.Log($"Test add {random}");
            AddCharacters(new Notification("", null, new() { 
                { "Characters", random.ToString() } }));
        }

        [ContextMenu("TestAddSpace")]
        void TestAddSpace()
        {
            UnityEngine.Debug.Log($"Test add space");
            AddCharacters(new Notification("", null, new() {
                { "Characters", ' ' } }));
        }

        [ContextMenu("TestAddMultiple")]
        void TestAddMultiple()
        {
            char a = 'a';
            char z = 'z';
            char random = 'a';

            random = (char)Random.Range(a, z);
            UnityEngine.Debug.Log($"Test add {random}");
            AddCharacters(new Notification("", null, new() {
                { "Characters", random.ToString() } }));

            random = (char)Random.Range(a, z);
            UnityEngine.Debug.Log($"Test add {random}");
            AddCharacters(new Notification("", null, new() {
                { "Characters", random.ToString() } }));

            random = (char)Random.Range(a, z);
            UnityEngine.Debug.Log($"Test add {random}");
            AddCharacters(new Notification("", null, new() {
                { "Characters", random } }));

            random = (char)Random.Range(a, z);
            UnityEngine.Debug.Log($"Test add {random}");
            AddCharacters(new Notification("", null, new() {
                { "Characters", random.ToString() } }));

            random = (char)Random.Range(a, z);
            UnityEngine.Debug.Log($"Test add {random}");
            AddCharacters(new Notification("", null, new() {
                { "Characters", random } }));
        }

        [ContextMenu("TestRemovePhase0")]
        void TestRemovePhase0()
        {
            RemoveCharacters(new Notification("", null, new() {
                { "DeletionPhase", 0 } }));
        }

        [ContextMenu("TestRemovePhase1")]
        void TestRemovePhase1()
        {
            RemoveCharacters(new Notification("", null, new() {
                { "DeletionPhase", 1 } }));
        }
#endif
    }
}