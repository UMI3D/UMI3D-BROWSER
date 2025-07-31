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

        public IInputFieldSelectionIndicator StartIndicator { get; set; }

        public IInputFieldSelectionIndicator EndIndicator { get; set; }

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

            caretRT.anchorMin = new(0, 1);
            caretRT.anchorMax = new(0, 1);
            caretRT.pivot = new(0f, 1f);
            caretRT.sizeDelta = new Vector2(caretWidth, inputField.textComponent.fontSize);
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
            HideSelection();
            if (!isTextSelected) return;

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
                    selectionBox = CreateSelectionBox();
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

                var size = inputField.textComponent.GetTextSize(inputField.text.Substring(lineStartCharIndex, lineEndCharIndex - lineStartCharIndex)); // FIXME : Break with margin top and bottom

                float positionX = GetHorizontalPosition(lineStartCharIndex);
                float positionY = GetVerticalPosition(lineStartCharIndex);
                selectionBox.anchoredPosition = new Vector2(positionX, positionY);
                selectionBox.sizeDelta = size;

                selectionBox.gameObject.SetActive(true);
            }

            StartIndicator?.ShowAt(new Vector2(GetHorizontalPosition(startPos), GetVerticalPosition(startPos)));
            EndIndicator?.ShowAt(new Vector2(GetHorizontalPosition(endPos), GetVerticalPosition(endPos) - inputField.textComponent.GetTextSize("0").y));
        }

        private RectTransform CreateSelectionBox()
        {
            GameObject selectionGO = new("MobileSelection");
            var selection = selectionGO.AddComponent<RawImage>();
            var selectionRT = selectionGO.GetComponent<RectTransform>();
            selection.transform.SetParent(textAreaRT, false);
            selection.transform.SetAsFirstSibling();
            selection.color = selectionColor;

            selectionRT.anchorMin = new(0, 1);
            selectionRT.anchorMax = new(0, 1);
            selectionRT.pivot = new(0f, 1f);
            return selectionRT;
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

            float positionX = GetHorizontalPosition(stringPosition);
            float positionY = GetVerticalPosition(stringPosition);
            caretRT.anchoredPosition = new(positionX, positionY);

            StartCaretBlinking();
        }

        private float GetHorizontalPosition(int charIndex)
        {
            int lineIndex = inputField.textComponent.textInfo.characterInfo[charIndex].lineNumber;
            int lineStartCharIndex = inputField.textComponent.textInfo.lineInfo[lineIndex].firstCharacterIndex;

            string textBefore = charIndex - lineStartCharIndex <= 0 ? "" : GetRenderSubstring(lineStartCharIndex, charIndex - lineStartCharIndex);
            float textBeforeWidth = textTMP.GetTextSize(textBefore).x;

            float positionX = textBeforeWidth + inputField.GetAlignmentOffset();

            switch (inputField.textComponent.horizontalAlignment)
            {
                case HorizontalAlignmentOptions.Left:
                    positionX += inputField.textComponent.margin.x * 2; // FIXME : Break with margin right
                    break;
                case HorizontalAlignmentOptions.Center:
                    positionX += inputField.textComponent.margin.x; // FIXME : Break with margin right
                    break;
                case HorizontalAlignmentOptions.Right:
                    positionX -= inputField.textComponent.margin.z;
                    break;
                case HorizontalAlignmentOptions.Justified:
                case HorizontalAlignmentOptions.Geometry:
                case HorizontalAlignmentOptions.Flush:
                    Debug.Log("Justified, Geometry, Flush are not implemented");
                    break;
            }

            return positionX;
        }

        private float GetVerticalPosition(int charIndex)
        {
            int lineIndex = inputField.textComponent.textInfo.characterInfo[charIndex].lineNumber;

            float lineHeight = inputField.textComponent.GetTextSize("0").y;
            float totalHeight = inputField.textViewport.rect.height;
            float positionY = -lineIndex * lineHeight;

            switch (inputField.textComponent.verticalAlignment)
            {
                case VerticalAlignmentOptions.Top:
                    positionY -= inputField.textComponent.margin.y;
                    break;
                case VerticalAlignmentOptions.Middle:
                    float middleOffset = (totalHeight - (inputField.textComponent.textInfo.lineCount * lineHeight)) / 2;
                    positionY -= middleOffset + inputField.textComponent.margin.y;
                    break;
                case VerticalAlignmentOptions.Bottom:
                    float bottomOffset = totalHeight - (inputField.textComponent.textInfo.lineCount * lineHeight);
                    positionY -= bottomOffset + inputField.textComponent.margin.y;
                    break;
                case VerticalAlignmentOptions.Capline:
                case VerticalAlignmentOptions.Baseline:
                case VerticalAlignmentOptions.Geometry:
                    Debug.Log("Capline, Baseline, Geometry are not implemented");
                    break;
            }

            return positionY;
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
                var caretPosition = TMP_TextUtilities.FindNearestCharacter(inputField.textComponent, eventData.position, Camera.main, false);

                Deselect(caretPosition);
            }
        }

        void HideSelection()
        {
            foreach (var selectionBox in _selectionsBox)
            {
                selectionBox.gameObject.SetActive(false);
            }

            StartIndicator?.Hide();
            EndIndicator?.Hide();
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