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
using inetum.unityUtils.ui;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.keyboard
{
    public class UMI3DInputFieldSelection : BaseInputFieldSelection
    {
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
        /// Whether the caret is displayed.
        /// </summary>
        bool showCaret = false;
        /// <summary>
        /// The blinking caret coroutine.
        /// </summary>
        Coroutine caretCoroutine;
        private bool LongPress;

        public override int startPosition { get; set; }

        public override int endPosition { get; set; }

        public override int stringPosition { get; set; }

        public UMI3DInputFieldSelection(MonoBehaviour context) : base(context)
        {
            inputField = context.GetComponentInChildren<TMP_InputField>();

            pointerDown = context.gameObject.AddComponent<PointerDownBehaviour>();
            pointerDown.isSimpleClick = false;

            textAreaRT = inputField.textViewport;
            textTMP = inputField.textComponent;

            GameObject caretGO = new("MobileCaret");
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


            GameObject selectionGO = new("MobileSelection");
            selection = selectionGO.AddComponent<RawImage>();
            selectionRT = selectionGO.GetComponent<RectTransform>();
            selection.transform.SetParent(textAreaRT, false);
            selection.transform.SetAsFirstSibling();
            selection.color = selectionColor;

            selectionRT.anchorMin = new(0, 1);
            selectionRT.anchorMax = new(0, 1);
            selectionRT.pivot = new(0f, 1f);
            selectionRT.offsetMin = new(selectionRT.offsetMin.x, 0f);
            selectionRT.offsetMax = new(selectionRT.offsetMax.x, 0f);
        }

        public override void OnEnable()
        {
            base.OnEnable();
            inputField.interactable = false;
            pointerDown.pointerClicked += OnPointerDown;
            pointerDown.pointerUp += OnPointerUp;
            pointerDown.pointerMoved += OnPointerMoved;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            pointerDown.pointerClicked -= OnPointerDown;
            pointerDown.pointerUp -= OnPointerUp;
            pointerDown.pointerMoved -= OnPointerMoved;
        }

        public override void Focus()
        {
            HideSelection();
            StartCaretBlinking();
        }

        public override void Blur()
        {
            HideSelection();
            StopCaretBLinking();
        }

        private List<RectTransform> _selectionsBox = new();

        public override void UpdateSelection()
        {
            foreach (var selectionBox in _selectionsBox)
            {
                selectionBox.gameObject.SetActive(false);
            }

            if (!isTextSelected)
            {
                HideSelection();
                return;
            }

            var startPos = Mathf.Min(startPosition, endPosition);
            var endPos = Mathf.Max(startPosition, endPosition);

            var textInfo = inputField.textComponent.textInfo;
            int startCharLine = textInfo.characterInfo[startPos].lineNumber;
            int endCharLine = textInfo.characterInfo[endPos].lineNumber;


            for (int i = startCharLine; i <= endCharLine; i++)
            {
                RectTransform selectionBox;
                if (i >= _selectionsBox.Count)
                {
                    selectionBox = GameObject.Instantiate(selectionRT, selectionRT.parent);
                    _selectionsBox.Add(selectionBox);
                }
                else
                {
                    selectionBox = _selectionsBox[i];
                }

                int lineStartCharIndex = textInfo.lineInfo[i].firstCharacterIndex;
                int lineEndCharIndex = textInfo.lineInfo[i].lastCharacterIndex;

                if (i == startCharLine)
                {
                    lineStartCharIndex = startPos;
                }
                if (i == endCharLine)
                {
                    lineEndCharIndex = endPos;
                }

                var size = inputField.textComponent.GetTextSize(inputField.text.Substring(lineStartCharIndex, lineEndCharIndex - lineStartCharIndex));

                var posX = lineStartCharIndex - textInfo.lineInfo[i].firstCharacterIndex <= 0 ? 0 : inputField.textComponent.GetTextSize(inputField.text.Substring(textInfo.lineInfo[i].firstCharacterIndex, lineStartCharIndex - textInfo.lineInfo[i].firstCharacterIndex)).x;
                var posY = -i * inputField.textComponent.GetTextSize("0").y;
                selectionBox.anchoredPosition = new Vector2(posX, posY);
                selectionBox.sizeDelta = size;

                selectionBox.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Update the caret position.
        /// </summary>
        public override void UpdateCaret()
        {
            if (isTextSelected)
            {
                StopCaretBLinking();
                return;
            }

            // Get the width of the portion of the text before the position of the caret.
            string prefix = GetRenderSubstring(0, stringPosition);
            float prefixWidth = textTMP.GetTextSize(prefix).x;

            // Get the offset due to alignment settings.
            float alignOffset = inputField.GetAlignmentOffset();

            Vector2 position = caretRT.anchoredPosition;
            caretRT.anchoredPosition = new(prefixWidth + alignOffset, position.y);

            StartCaretBlinking();
        }

        string GetRenderSubstring(int startIndex, int length)
        {
            if (inputField.contentType == TMP_InputField.ContentType.Password)
            {
                return new string('*', length);
            }

            return inputField.text.Substring(startIndex, length);
        }

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

            if (!notification.TryGetInfoT(PointerDownBehaviour.NKIsLongPress, out bool isLongPress))
            {
                return;
            }

            if (isLongPress)
            {
                LongPress = true;
                var caretPosition = TMP_TextUtilities.FindNearestCharacter(inputField.textComponent, eventData.position, Camera.main, false);
                startPosition = caretPosition;
                endPosition = caretPosition;
                UpdateSelection();
            }

            if (!isImmediate)
            {
                return;
            }

            if (count == 2)
            {
                Select(0, inputField.text.Length);
            }
            else
            {
                Vector2 localPosition = textAreaRT.PointerRelativeToUI(eventData, RectTransformExtensions.Pivot.TopLeft);
                int caretPosition = PointerPositionToCaretPosition(localPosition);

                Deselect(caretPosition);
            }
        }

        void ShowSelection()
        {
            selection.enabled = true;
        }

        void HideSelection()
        {
            //selection.enabled = false;
        }

        int PointerPositionToCaretPosition(Vector2 position)
        {
            int _position = 0;
            float globalWidth = 0f;

            while (_position + 1 <= inputField.text.Length)
            {
                string letter = inputField.text.Substring(_position, 1);
                float letterWidth = textTMP.GetPreferredValues(letter).x;
                if (position.x < globalWidth + letterWidth / 2f)
                {
                    return _position;
                }
                globalWidth += letterWidth;
                _position++;
            }

            return _position;
        }

        /// <summary>
        /// Display and make the caret blink.
        /// </summary>
        void StartCaretBlinking()
        {
            if (caretCoroutine == null)
            {
                showCaret = true;
                caretCoroutine = context.StartCoroutine(CaretBlinking());
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
                context.StopCoroutine(caretCoroutine);
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

        private void OnPointerUp(PointerEventData data)
        {
            LongPress = false;
        }

        private void OnPointerMoved(PointerEventData data)
        {
            if (!LongPress)
                return;

            endPosition = TMP_TextUtilities.FindNearestCharacter(inputField.textComponent, data.position, Camera.main, false);
            
            UpdateSelection();
        }
    }
}