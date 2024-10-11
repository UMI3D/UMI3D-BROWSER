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
        /// <summary>
        /// Whether this selection is the keyboard preview bar.
        /// </summary>
        public bool isPreviewBar = false;

        /// <summary>
        /// Whether the associated text field is being used.
        /// </summary>
        public bool isActive => isPreviewBar || activeSelection == this;

        static BaseInputFieldSelection activeSelection;

        /// <summary>
        /// Whether the input field text can be edited when selected.
        /// </summary>
        public bool allowTextModification { get; set; } = false;
        /// <summary>
        /// Whether the caret or the selection are available.
        /// </summary>
        public virtual bool allowSelection { get; set; } = false;

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

            selectionNotifier = NotificationHub.Default
                .GetNotifier<KeyboardNotificationKeys.TextFieldSelected>(this);
        }

        public virtual void OnEnable()
        {
            NotificationHub.Default.Subscribe<KeyboardNotificationKeys.TextFieldSelected>(
                this,
                new FilterByRef(FilterType.AcceptAllExcept, this),
                TextFieldSelected
            );

            NotificationHub.Default.Subscribe<KeyboardNotificationKeys.TextFieldDeselected>(
               this,
               new FilterByRef(FilterType.AcceptAllExcept, this),
               TextFieldDeselected
           );
        }

        public virtual void OnDisable()
        {
            NotificationHub.Default.Unsubscribe<KeyboardNotificationKeys.TextFieldSelected>(this);

            NotificationHub.Default.Unsubscribe<KeyboardNotificationKeys.TextFieldDeselected>(this);
        }

        /// <summary>
        /// Focus the input field.<br/>
        /// <br/>
        /// Display the caret and hide selection.
        /// </summary>
        public abstract void Focus();

        /// <summary>
        /// Unfocus the input field.<br/>
        /// <br/>
        /// Hide the caret and hide selection.
        /// </summary>
        public abstract void Blur();

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
            ActivateInternal();

            selectionNotifier[KeyboardNotificationKeys.TextFieldSelected.IsPreviewBar] = isPreviewBar;
            selectionNotifier[KeyboardNotificationKeys.TextFieldSelected.SelectionPositions] = allowSelection ? (start, end) : null;
            selectionNotifier[KeyboardNotificationKeys.TextFieldSelected.InputFieldText] = inputField.text;
            selectionNotifier.Notify();

            if (allowSelection)
            {
                SelectWithoutNotify(start, end);
            }
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
            ActivateInternal();

            selectionNotifier[KeyboardNotificationKeys.TextFieldSelected.IsPreviewBar] = isPreviewBar;
            selectionNotifier[KeyboardNotificationKeys.TextFieldSelected.SelectionPositions] = allowSelection ? newCaretPosition : null;
            selectionNotifier[KeyboardNotificationKeys.TextFieldSelected.InputFieldText] = inputField.text;
            selectionNotifier.Notify();

            if (allowSelection)
            {
                DeselectWithoutNotify(newCaretPosition);
            }
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

        /// <summary>
        /// Deactivate the current active selection and active this if this is not preview bar. If allow selection then display the caet at <see cref="stringPosition"/>.
        /// </summary>
        public void Activate()
        {
            Deselect(stringPosition);
        }

        /// <summary>
        /// Deactivate the current active selection and active this if this is not preview bar.
        /// </summary>
        void ActivateInternal()
        {
            if (isPreviewBar)
            {
                return;
            }

            if (!isActive && activeSelection != null)
            {
                // If this selection is not activated but an active selection is registered then deactivate it.
                activeSelection.Blur();
            }

            activeSelection = this;
        }

        public void Deactivate()
        {
            activeSelection.Blur();
            activeSelection = null;
        }

        /// <summary>
        /// Call when a text field is selected.<br/>
        /// Update the input field text.<br/>
        /// Update the position of the caret or the selection of the text.
        /// </summary>
        /// <param name="notification"></param>
        void TextFieldSelected(Notification notification)
        {
            if (!isActive)
            {
                return;
            }

            // Update text.
            if (!allowTextModification)
            {
                return;
            }
            if (!notification.TryGetInfoT(KeyboardNotificationKeys.TextFieldSelected.InputFieldText, out string text))
            {
                return;
            }
            if (isPreviewBar)
            {
                // Hide or display preview bar.
                inputField.textViewport.gameObject.SetActive(text != null);
                inputField.targetGraphic?.gameObject.SetActive(text != null);
            }
            inputField.text = text;


            // Update caret or selection.
            if (!allowSelection)
            {
                return;
            }
            if (notification.TryGetInfoT(KeyboardNotificationKeys.TextFieldSelected.SelectionPositions, out int caretPosition, false))
            {
                DeselectWithoutNotify(caretPosition);
            }
            else if (notification.TryGetInfoT(KeyboardNotificationKeys.TextFieldSelected.SelectionPositions, out (int, int) selectionPositions, false))
            {
                SelectWithoutNotify(selectionPositions.Item1, selectionPositions.Item2);
            }
            else
            {
                notification.LogError(nameof(KeyboardTMPInputFieldLinker), KeyboardNotificationKeys.TextFieldSelected.SelectionPositions, $"Selection positions is neither int nor (int, int).");
                return;
            }
        }

        void TextFieldDeselected()
        {
            if (!isActive || !allowSelection)
            {
                return;
            }

            if (!isPreviewBar)
            {
                Deactivate();
            }
        }
    }
}