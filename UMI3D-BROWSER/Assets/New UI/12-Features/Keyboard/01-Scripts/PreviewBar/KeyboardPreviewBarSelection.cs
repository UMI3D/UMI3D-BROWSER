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
using inetum.unityUtils.math;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using umi3d.browserRuntime.NotificationKeys;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.keyboard
{
    public class KeyboardPreviewBarSelection : MonoBehaviour
    {
        TMP_InputField inputField;

        /// <summary>
        /// Is text selected.
        /// </summary>
        public bool isTextSelected => startPosition < endPosition;

        /// <summary>
        /// Start position of the selection.
        /// </summary>
        public int startPosition
        {
            get
            {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
                return Mathf.Min(
                    inputField.selectionAnchorPosition, 
                    inputField.selectionFocusPosition
                );
#else
                return startPos;
#endif
            }
            private set
            {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
                if (inputField.selectionAnchorPosition <= inputField.selectionFocusPosition)
                {
                    inputField.selectionAnchorPosition = value;
                }
                else
                {
                    inputField.selectionFocusPosition = value;
                }

                // TODO: remove those call.
                startPos = value;
#else
                startPos = value;
#endif
            }
        }

        /// <summary>
        /// End position of the selection.
        /// </summary>
        public int endPosition
        {
            get
            {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
                return Mathf.Max(
                    inputField.selectionAnchorPosition,
                    inputField.selectionFocusPosition
                );
#else
                return endPos;
#endif
            }
            private set
            {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
                if (inputField.selectionAnchorPosition <= inputField.selectionFocusPosition)
                {
                    inputField.selectionFocusPosition = value;
                }
                else
                {
                    inputField.selectionAnchorPosition = value;
                }

                // TODO: remove those call.
                endPos = value;
#else
                endPos = value;
#endif
            }
        }

        /// <summary>
        /// Position of the caret.
        /// </summary>
        public int stringPosition
        {
            get
            {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
                return inputField.stringPosition;
#else
                return caretPos;
#endif
            }
            private set
            {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
                inputField.stringPosition = value;

                // TODO: remove those call.
                caretPos = value;
#else
                caretPos = value;
#endif
            }
        }

        void Awake()
        {
            inputField = GetComponentInChildren<TMP_InputField>();

#if UNITY_ANDROID // TODO restrict to not UNITY_EDITOR!
            pointerDown = gameObject.AddComponent<PointerDownBehaviour>();
            pointerDown.isSimpleClick = false;

            textAreaRT = inputField.transform.GetChild(0).GetComponent<RectTransform>();
            textTMP = inputField.textComponent;

            GameObject caretGO = new("AndroidCaret");
            caret = caretGO.AddComponent<RawImage>();
            caretRT = caretGO.GetComponent<RectTransform>();
            caret.transform.SetParent(textAreaRT, false);
            caret.transform.SetAsFirstSibling();
            caret.color = caretColor;

            caretRT.anchorMin = Vector2.zero;
            caretRT.anchorMax = new(0, 1);
            caretRT.pivot = new(0f, 0.5f);
            caretRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, caretWidth);
            caretRT.offsetMin = new(caretRT.offsetMin.x, 0f);
            caretRT.offsetMax = new(caretRT.offsetMax.x, 0f);


            GameObject selectionGO = new("AndroidSelection");
            selection = selectionGO.AddComponent<RawImage>();
            selectionRT = selectionGO.GetComponent<RectTransform>();
            selection.transform.SetParent(textAreaRT, false);
            selection.transform.SetAsFirstSibling();
            selection.color = selectionColor;

            selectionRT.anchorMin = Vector2.zero;
            selectionRT.anchorMax = new(0, 1);
            selectionRT.pivot = new(0f, 0.5f);
            selectionRT.offsetMin = new(selectionRT.offsetMin.x, 0f);
            selectionRT.offsetMax = new(selectionRT.offsetMax.x, 0f);

            HideSelection();
            StopCaretBLinking();
#endif
        }

        void OnEnable()
        {
            NotificationHub.Default.Subscribe(
               this,
               KeyboardNotificationKeys.AskPreviewFocus,
               null,
               Focus
           );

            pointerDown.pointerClicked += OnPointerDown;
        }

        void OnDisable()
        {
            NotificationHub.Default.Unsubscribe(this, KeyboardNotificationKeys.AskPreviewFocus);

            pointerDown.pointerClicked -= OnPointerDown;
        }

        /// <summary>
        /// Focus the preview bar.<br/>
        /// <br/>
        /// Display the caret.
        /// </summary>
        void Focus()
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            bool onFocusSelectAll = inputField.onFocusSelectAll;
            inputField.onFocusSelectAll = false;
            inputField.Select();
            new Task(async () =>
            {
                await Task.Yield();
                inputField.onFocusSelectAll = onFocusSelectAll;
            }).Start(TaskScheduler.FromCurrentSynchronizationContext());
#else
            
#endif
           StartCaretBlinking();
        }

        /// <summary>
        /// Make a selection.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void Select(int start, int end)
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

#if UNITY_ANDROID // TODO restrict to not UNITY_EDITOR!
            UpdateSelection();
            UpdateCaret();
#endif
        }

        /// <summary>
        /// Deselect and place the caret at <paramref name="newCaretPosition"/>.
        /// </summary>
        /// <param name="newCaretPosition"></param>
        public void Deselect(int newCaretPosition)
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

#if UNITY_ANDROID // TODO restrict to not UNITY_EDITOR!
            UpdateSelection();
            UpdateCaret();
#endif
        }

#if UNITY_ANDROID // TODO restrict to not UNITY_EDITOR!

        PointerDownBehaviour pointerDown;

        RectTransform textAreaRT;
        TMP_Text textTMP;

        RectTransform caretRT;
        RawImage caret;

        int caretWidth => inputField.caretWidth;
        float caretBlinkRate => inputField.caretBlinkRate;
        Color caretColor => inputField.caretColor;

        RectTransform selectionRT;
        RawImage selection;

        Color selectionColor => inputField.selectionColor;

        /// <summary>
        /// Start position of the selection.
        /// </summary>
        int startPos = 0;
        /// <summary>
        /// End position of the selection.
        /// </summary>
        int endPos = 0;
        /// <summary>
        /// Position of the caret.
        /// </summary>
        int caretPos = 0;

        /// <summary>
        /// Whether the selection is displayed.
        /// </summary>
        bool showSelection = false;
        /// <summary>
        /// Whether the caret is displayed.
        /// </summary>
        bool showCaret = false;
        /// <summary>
        /// The blinking caret coroutine.
        /// </summary>
        Coroutine caretCoroutine;

        void OnPointerDown(Notification notification)
        {
            if (!notification.TryGetInfoT(PointerDownBehaviour.NKPointerEvent, out PointerEventData eventData))
            {
                return;
            }

            if (!notification.TryGetInfoT(PointerDownBehaviour.NKCount, out int count))
            {
                return;
            }

            if (!notification.TryGetInfoT(PointerDownBehaviour.NKIsImmediate, out bool isImmediate))
            {
                return;
            }

            if (!isImmediate)
            {
                return;
            }

            if (count == 1)
            {
                Vector2 localPosition = textAreaRT.PointerRelativeToUI(eventData, RectTransformExtensions.Pivot.TopLeft);
                UnityEngine.Debug.Log($"simple {localPosition}");

                //Deselect();
            }
            else if (count == 2)
            {
                Select(0, inputField.text.Length);
            }
        }

        void UpdateSelection()
        {
            if (!isTextSelected)
            {
                HideSelection();
                return;
            }

            string prefix = inputField.text.Substring(0, startPos);
            float prefixWidth = textTMP.GetPreferredValues(prefix).x;
            UnityEngine.Debug.Log($"selection position = {prefixWidth}");
            Vector2 position = selectionRT.anchoredPosition;
            selectionRT.anchoredPosition = new(prefixWidth, position.y);

            UnityEngine.Debug.Log($"selection {startPos} && {endPos - startPos}");
            string selection = inputField.text.Substring(startPos, endPos - startPos);
            float selectionWidth = textTMP.GetPreferredValues(selection).x;
            UnityEngine.Debug.Log($"selection width = {selectionWidth} && {startPos} && {endPos - startPos}");
            selectionRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, selectionWidth);

            ShowSelection();
        }

        void ShowSelection()
        {
            selection.enabled = true;
        }

        void HideSelection()
        {
            selection.enabled = false;
        }

        /// <summary>
        /// Update the caret position.
        /// </summary>
        void UpdateCaret()
        {
            if (isTextSelected)
            {
                StopCaretBLinking();
                return;
            }

            string prefix = inputField.text.Substring(0, caretPos);
            float prefixWidth = textTMP.GetPreferredValues(prefix).x;

            Vector2 position = caretRT.anchoredPosition;
            caretRT.anchoredPosition = new(prefixWidth, position.y);

            StartCaretBlinking();
        }

        /// <summary>
        /// Display and make the caret blink.
        /// </summary>
        void StartCaretBlinking()
        {
            if (caretCoroutine == null)
            {
                showCaret = true;
                caretCoroutine = StartCoroutine(CaretBlinking());
            }
        }

        /// <summary>
        /// Hide the caret.
        /// </summary>
        void StopCaretBLinking()
        {
            showCaret = false;
            if (caretCoroutine != null)
            {
                StopCoroutine(caretCoroutine);
            }
            caret.enabled = false;
            caretCoroutine = null;
        }

        /// <summary>
        /// Make the caret blink.
        /// </summary>
        /// <returns></returns>
        IEnumerator CaretBlinking()
        {
            caret.enabled = true;
            float t = 0;

            while (showCaret)
            {
                yield return null;
                
                t += Time.deltaTime;

                if (t > caretBlinkRate)
                {
                    t = 0;
                    caret.enabled = !caret.enabled;
                }
            }

            caret.enabled = false;
            caretCoroutine = null;
            yield break;
        }
#endif

#if UNITY_EDITOR
        [ContextMenu("TestFocus")]
        void TestFocus()
        {
            UnityEngine.Debug.Log($"test focus = {inputField.stringPosition}");
            Focus();
        }

        [ContextMenu("Test Selection")]
        void TestFieldSelection()
        {
            UnityEngine.Debug.Log($"test focus = {inputField.stringPosition} && {inputField.selectionAnchorPosition} && {inputField.selectionFocusPosition}");
            Select(0, 1);
        }

        [ContextMenu("Test deselection")]
        void TestFieldDeselection()
        {
            UnityEngine.Debug.Log($"test deselection = {inputField.stringPosition} && {inputField.selectionAnchorPosition} && {inputField.selectionFocusPosition}");
            Deselect(1);
        }

        [ContextMenu("Test Android Selection")]
        void TestAndroidSelection()
        {
            UnityEngine.Debug.Log($"test focus = {inputField.stringPosition} && {inputField.selectionAnchorPosition} && {inputField.selectionFocusPosition}");
            Select(1, 2);
        }

        [ContextMenu("Test Android deselection")]
        void TestAndroidDeselection()
        {
            UnityEngine.Debug.Log($"test deselection = {inputField.stringPosition} && {inputField.selectionAnchorPosition} && {inputField.selectionFocusPosition}");
            Deselect(1);
        }

        [ContextMenu("Test character width")]
        void TestCharacterWidth()
        {
            UnityEngine.Debug.Log($"test deselection = {inputField.stringPosition} && {inputField.selectionAnchorPosition} && {inputField.selectionFocusPosition}");
            
            string text = "Hello, world!";
            Vector2 preferredValues = textTMP.GetPreferredValues(text);
            float textWidthInPoints = preferredValues.x;

            Debug.Log("La largeur du mot '" + text + "' est " + textWidthInPoints);
        }
#endif
    }
}