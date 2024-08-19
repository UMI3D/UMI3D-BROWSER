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
using System.Threading.Tasks;
using umi3d.browserRuntime.NotificationKeys;
using UnityEngine;

namespace umi3d.browserRuntime.ui
{
    public class KeyboardPreviewBar : MonoBehaviour
    {
        TMPro.TMP_InputField inputField;

        /// <summary>
        /// Start position of the selection.<br/>
        /// If there is no selection then null.
        /// </summary>
        int? SelectionStart = null;
        /// <summary>
        /// End position of the selection.<br/>
        /// If there is no selection then null.
        /// </summary>
        int? SelectionEnd = null;

        /// <summary>
        /// Is text selected.
        /// </summary>
        bool isTextSelected
        {
            get
            {
                return SelectionStart.HasValue && SelectionEnd.HasValue && SelectionStart.Value < SelectionEnd.Value;
            }
        }

        private void Awake()
        {
            inputField = GetComponentInChildren<TMPro.TMP_InputField>();
        }

        void OnEnable()
        {
            inputField.onTextSelection.AddListener(SelectText);
            inputField.onEndTextSelection.AddListener(UnSelectText);

            NotificationHub.Default.Subscribe(
                this,
                KeyboardNotificationKeys.AskPreviewFocus,
                null,
                Focus
            );

            NotificationHub.Default.Subscribe(
               this,
               KeyboardNotificationKeys.AddCharacters,
               null,
               AddCharacters
           );

            NotificationHub.Default.Subscribe(
               this,
               KeyboardNotificationKeys.RemoveCharacters,
               null,
               RemoveCharacters
           );
        }

        void OnDisable()
        {
            inputField.onTextSelection.RemoveListener(SelectText);
            inputField.onEndTextSelection.RemoveListener(UnSelectText);

            NotificationHub.Default.Unsubscribe(this, KeyboardNotificationKeys.AddCharacters);
            NotificationHub.Default.Unsubscribe(this, KeyboardNotificationKeys.RemoveCharacters);
        }

        void Focus()
        {
            bool onFocusSelectAll = inputField.onFocusSelectAll;
            inputField.onFocusSelectAll = false;
            inputField.Select();
            new Task(async () =>
            {
                await Task.Yield();
                inputField.onFocusSelectAll = onFocusSelectAll;
            }).Start(TaskScheduler.FromCurrentSynchronizationContext());
        }

        void SelectText(string str, int pos1, int pos2)
        {
            if (inputField.isFocused)
            {
                SelectionStart = Mathf.Min(pos1, pos2);
                //minus one to convert from caret pos to character pos
                SelectionEnd = Mathf.Max(pos1 - 1, pos2 - 1);
            }
        }

        void UnSelectText(string str, int pos1, int pos2)
        {
            inputField.stringPosition = pos1;
            SelectionStart = null;
            SelectionEnd = null;
        }

        void AddCharacters(Notification notification)
        {
            if (!notification.TryGetInfoT(KeyboardNotificationKeys.Info.Characters, out string characters))
            {
                if (!notification.TryGetInfoT(KeyboardNotificationKeys.Info.Characters, out char character))
                {
                    UnityEngine.Debug.LogError($"[KeyboardPreviewBar] No characters added.");
                    return;
                }

                characters = character.ToString();
            }
            
            var text = inputField.text;

            if (!isTextSelected)
            {
                inputField.text = text.Insert(inputField.stringPosition, characters);
                inputField.stringPosition += characters.Length;
            }
            else
            {
                int start = SelectionStart.Value;
                int end = SelectionEnd.Value;

                text = text.Remove(start, end - start + 1);
                text = text.Insert(start, characters);
                inputField.text = text;

                inputField.stringPosition = start + 1;
                inputField.selectionAnchorPosition = start + 1;
                inputField.selectionFocusPosition = start + 1;
                SelectionStart = null;
                SelectionEnd = null;
            }
        }

        void RemoveCharacters(Notification notification)
        {
            if (!notification.TryGetInfoT(KeyboardNotificationKeys.Info.DeletionPhase, out int deletionPhase))
            {
                UnityEngine.Debug.LogWarning($"[KeyboardPreviewBar] No deletion phase.");
                return;
            }

            if (inputField.stringPosition == 0 && !isTextSelected)
            {
                return;
            }

            var text = inputField.text;

            // In phase 0: delete only one character or the selected text.
            if (deletionPhase == 0)
            {

                if (!isTextSelected)
                {
                    inputField.stringPosition -= 1;
                    inputField.text = text.Remove(inputField.stringPosition, 1);
                }
                else
                {
                    int start = SelectionStart.Value;
                    int end = SelectionEnd.Value;

                    text = text.Remove(start, end - start + 1);
                    inputField.text = text;

                    inputField.stringPosition = start;
                    inputField.selectionAnchorPosition = start;
                    inputField.selectionFocusPosition = start;
                    SelectionStart = null;
                    SelectionEnd = null;
                }
            }
            // In phase 1: delete world by world.
            else if (deletionPhase == 1)
            {
                // The part that will be partially deleted.
                string left = text.Substring(0, inputField.stringPosition);
                // The part that will not be deleted.
                string right = text.Substring(inputField.stringPosition, text.Length - inputField.stringPosition);

                // Remove the trailing spaces.
                string trimmedLeft = left.TrimEnd();
                if (trimmedLeft.Length < left.Length)
                {
                    inputField.text = trimmedLeft + right;
                    inputField.stringPosition = trimmedLeft.Length;
                    return;
                }

                // Remove the last word.
                int lastIdxOfSpace = left.LastIndexOf(' ');

                if (lastIdxOfSpace == -1)
                {
                    inputField.text = right;
                    inputField.stringPosition = 0;
                }
                else
                {
                    inputField.text = left.Substring(0, lastIdxOfSpace + 1) + right;
                    inputField.stringPosition = lastIdxOfSpace + 1;
                }
            }
            else
            {
                UnityEngine.Debug.LogError($"[KeyboardPreviewBar] Deletion phase case unhandled.");
            }
        }

#if UNITY_EDITOR
        [ContextMenu("TestFocus")]
        void TestFocus()
        {
            UnityEngine.Debug.Log($"test focus = {inputField.stringPosition}");
            Focus();
        }

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