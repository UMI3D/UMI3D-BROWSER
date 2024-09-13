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
using System;
using TMPro;
using umi3d.browserRuntime.NotificationKeys;
using UnityEngine;

namespace umi3d.browserRuntime.ui.keyboard
{
    public abstract class BaseInputFieldSelection 
    {
        protected MonoBehaviour context;
        protected TMP_InputField inputField;

        Notifier selectionNotifier;

        /// <summary>
        /// Is text selected.
        /// </summary>
        public bool isTextSelected => startPosition < endPosition;

        /// <summary>
        /// Start position of the selection.
        /// </summary>
        public abstract int startPosition { get; set; }

        /// <summary>
        /// End position of the selection.
        /// </summary>
        public abstract int endPosition { get; set; }

        /// <summary>
        /// Position of the caret.
        /// </summary>
        public abstract int stringPosition { get; set; }

        public BaseInputFieldSelection(MonoBehaviour context) 
        {
            this.context = context;

            selectionNotifier = NotificationHub.Default.GetNotifier(
                this,
                KeyboardNotificationKeys.InputFieldSelection,
                null,
                null
            );
        }

        public virtual void OnEnable()
        {
            NotificationHub.Default.Subscribe(
                this,
                KeyboardNotificationKeys.InputFieldSelection,
                new FilterByRef(FilterType.AcceptAllExcept, this),
                PreviewSelection
            );
        }

        public virtual void OnDisable()
        {
            NotificationHub.Default.Unsubscribe(
                this,
                KeyboardNotificationKeys.InputFieldSelection
            );
        }

        /// <summary>
        /// Focus the preview bar.<br/>
        /// <br/>
        /// Display the caret and hide selection.
        /// </summary>
        public abstract void Focus();

        /// <summary>
        /// Update the selection position and width.
        /// </summary>
        public abstract void UpdateSelection();

        /// <summary>
        /// Update the caret position.
        /// </summary>
        public abstract void UpdateCaret();

        /// <summary>
        /// Make a selection.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void Select(int start, int end)
        {
            SelectWithoutNotify(start, end);

            selectionNotifier[KeyboardNotificationKeys.Info.SelectionPositions] = (start, end);
            selectionNotifier.Notify();
        }

        /// <summary>
        /// Make a selection without notifying.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void SelectWithoutNotify(int start, int end)
        {
            if (inputField.text.Length == 0)
            {
                // No selection allowed if there is no text.
                return;
            }

            if (start < 0 || end < 0)
            {
                UnityEngine.Debug.LogError($"[Keyboard selection] Start or end selection cannot be negative position.");
                return;
            }
            else if (start > end)
            {
                UnityEngine.Debug.LogError($"[Keyboard selection] Start > end position.");
                return;
            }

            startPosition = start;
            endPosition = end;

            UpdateSelection();
            UpdateCaret();
        }

        /// <summary>
        /// Deselect and place the caret at <paramref name="newCaretPosition"/>.
        /// </summary>
        /// <param name="newCaretPosition"></param>
        public void Deselect(int newCaretPosition)
        {
            DeselectWithoutNotify(newCaretPosition);

            selectionNotifier[KeyboardNotificationKeys.Info.SelectionPositions] = newCaretPosition;
            selectionNotifier.Notify();
        }

        /// <summary>
        /// Deselect and place the caret at <paramref name="newCaretPosition"/> without notifying.
        /// </summary>
        /// <param name="newCaretPosition"></param>
        public void DeselectWithoutNotify(int newCaretPosition)
        {
            newCaretPosition = newCaretPosition < 0 ? 0 : newCaretPosition;

            stringPosition = newCaretPosition;

            if (inputField.text.Length != 0 || !isTextSelected)
            {
                // No deselection allowed if there is no text.
                if (newCaretPosition < endPosition)
                {
                    startPosition = newCaretPosition;
                    endPosition = newCaretPosition;
                }
                else
                {
                    endPosition = newCaretPosition;
                    startPosition = newCaretPosition;
                }
            }

            UpdateSelection();
            UpdateCaret();
        }

        void PreviewSelection(Notification notification)
        {
            if (notification.TryGetInfoT(KeyboardNotificationKeys.Info.SelectionPositions, out int caretPosition, false))
            {
                DeselectWithoutNotify(caretPosition);
                return;
            }

            if (notification.TryGetInfoT(KeyboardNotificationKeys.Info.SelectionPositions, out (int, int) selectionPositions, false))
            {
                SelectWithoutNotify(selectionPositions.Item1, selectionPositions.Item2);
                return;
            }

            notification.LogError(nameof(KeyboardTMPInputFieldLinker), KeyboardNotificationKeys.Info.SelectionPositions, $"Selection positions is neither int nor (int, int).");
        }
    }
}