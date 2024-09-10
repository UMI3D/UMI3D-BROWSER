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
using TMPro;
using umi3d.browserRuntime.NotificationKeys;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.keyboard
{
    public class KeyboardPreviewBarSelection : MonoBehaviour
    {
        TMPro.TMP_InputField inputField;

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
            set
            {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
                if (inputField.selectionAnchorPosition < inputField.selectionFocusPosition)
                {
                    inputField.selectionAnchorPosition = value;
                }
                else
                {
                    inputField.selectionFocusPosition = value;
                }
#else
                startPos = value;
                UpdateSelection();
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
                // Minus one to convert from caret pos to character pos.
                return Mathf.Max(
                    inputField.selectionAnchorPosition -1,
                    inputField.selectionFocusPosition -1
                );
#else
                return endPos;
#endif
            }
            set
            {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
                inputField.selectionFocusPosition = value;
#else
                endPos = value;
                UpdateSelection();
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
            set
            {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
                inputField.stringPosition = value;

                caretPos = value;
                UpdateCaret(); // TODO: remove this call.
#else
                caretPos = value;
                UpdateCaret();
#endif
            }
        }

        void Awake()
        {
            inputField = GetComponentInChildren<TMPro.TMP_InputField>();

#if UNITY_ANDROID // TODO restrict to not UNITY_EDITOR!,
            textArea = inputField.transform.GetChild(0).gameObject;
            textTMP = inputField.textComponent;

            GameObject caretGO = new("AndroidCaret");
            caret = caretGO.AddComponent<RawImage>();
            caretRT = caretGO.GetComponent<RectTransform>();
            caret.transform.SetParent(textArea.transform, false);
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
            selection.transform.SetParent(textArea.transform, false);
            selection.transform.SetAsFirstSibling();
            selection.color = selectionColor;

            selectionRT.anchorMin = Vector2.zero;
            selectionRT.anchorMax = new(0, 1);
            selectionRT.pivot = new(0f, 0.5f);
            selectionRT.offsetMin = new(selectionRT.offsetMin.x, 0f);
            selectionRT.offsetMax = new(selectionRT.offsetMax.x, 0f);
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
        }

        void OnDisable()
        {
            NotificationHub.Default.Unsubscribe(this, KeyboardNotificationKeys.AskPreviewFocus);
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
        }

        /// <summary>
        /// Deselect and place the caret at <paramref name="newCaretPosition"/>.
        /// </summary>
        /// <param name="newCaretPosition"></param>
        public void Deselect(int newCaretPosition)
        {
            if (newCaretPosition < 0)
            {
                UnityEngine.Debug.LogError($"[Keyboard selection] Caret must be superior to 0.");
                return;
            }

            startPosition = newCaretPosition;
            endPosition = newCaretPosition;
            stringPosition = newCaretPosition;
        }

#if UNITY_ANDROID

        GameObject textArea;
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

        void UpdateSelection()
        {
            
        }

        //void GetSelectionPositionAndSize(out float position, out float width)
        //{

        //}

        /// <summary>
        /// Update the caret position.
        /// </summary>
        void UpdateCaret()
        {
            UnityEngine.Debug.Log($"caret pos = {caretPos}");
            string prefix = inputField.text.Substring(0, caretPos);
            UnityEngine.Debug.Log($"prefix '{prefix}'");
            float prefixWidth = textTMP.GetPreferredValues(prefix).x;
            UnityEngine.Debug.Log($"prefix width = {prefixWidth}");

            Vector2 position = caretRT.anchoredPosition;
            caretRT.anchoredPosition = new(prefixWidth, position.y);
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
            Select(1, 2);
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