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
using System.Globalization;
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

        private IInputFieldSelectionIndicator _startIndicator;
        public IInputFieldSelectionIndicator StartIndicator
        {
            get => _startIndicator;
            set {
                if (_startIndicator != null)
                    _startIndicator.OnMove -= OnStartIndicatorMove;
                _startIndicator = value;
                _startIndicator.OnMove += OnStartIndicatorMove;
            }
        }

        private IInputFieldSelectionIndicator _endIndicator;
        public IInputFieldSelectionIndicator EndIndicator
        {
            get => _endIndicator;
            set {
                if (_endIndicator != null)
                    _endIndicator.OnMove -= OnEndIndicatorMove;
                _endIndicator = value;
                _endIndicator.OnMove += OnEndIndicatorMove;
            }
        }

        private ScrollRect _scrollRect;

        bool _hasVerticalScroll = false;

        public UMI3DInputFieldSelection(MonoBehaviour context, ScrollRect scrollRect, bool hasVerticalScroll) : base(context)
        {
            _hasVerticalScroll = hasVerticalScroll;
            _scrollRect = scrollRect;
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
            _hasVerticalScroll = hasVerticalScroll;
        }

        private void AdjustScrollPosition()
        {
            if (!_scrollRect)
                return;

            var caretPosition = startPosition;
            var textInfo = inputField.textComponent.textInfo;

            if (caretPosition > 0 && caretPosition <= textInfo.characterCount)
            {

                var textRectTransform = inputField.textComponent.rectTransform;
                var inputFieldRectTransform = ((RectTransform)inputField.transform);
                var textAreaRectTransform = ((RectTransform)inputFieldRectTransform.GetChild(0));
                var distance = inputFieldRectTransform.rect.height - _scrollRect.viewport.rect.height;

                // Get char positions
                var charInfo = textInfo.characterInfo[caretPosition - 1];
                var bottomLeft = charInfo.bottomLeft;
                var topLeft = charInfo.topLeft;

                // Calculate correction
                var correction = 0.0f;

                var worldPosTop = textRectTransform.TransformPoint(topLeft);
                var relativePosTop = _scrollRect.viewport.InverseTransformPoint(worldPosTop);
                var textOffsetTop = textAreaRectTransform.offsetMax.y;
                if (relativePosTop.y > 0)
                    correction = (relativePosTop.y - textOffsetTop) / distance;

                var worldPosBottom = textRectTransform.TransformPoint(bottomLeft);
                var relativePosBottom = _scrollRect.viewport.InverseTransformPoint(worldPosBottom);
                var textOffsetBottom = textAreaRectTransform.offsetMin.y;
                if (relativePosBottom.y < -_scrollRect.viewport.rect.height)
                    correction = (relativePosBottom.y + _scrollRect.viewport.rect.height - textOffsetBottom) / distance;

                // Apply correction
                _scrollRect.verticalScrollbar.value += correction;
            }
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
            if (!isTextSelected)
                return;

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

                var size = inputField.textComponent.GetTextSize(inputField.text.Substring(lineStartCharIndex, lineEndCharIndex - lineStartCharIndex)); // FIXME : Break with margin textOffsetTop and textOffsetBottom

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
            AdjustScrollPosition();
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
            _wasOnlyVerticalPointerMovement = true;
        }

        bool _wasOnlyVerticalPointerMovement = true;
        float _verticalMovementThreshold = 50f;

        private void OnPointerMoved(PointerEventData data)
        {
            if (!LongPress)
                return;
            
            Vector2 delta = data.delta;
            float angleInDegrees = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
            Debug.Log($"Angle {angleInDegrees} Delta {delta}");
            if (_hasVerticalScroll && _wasOnlyVerticalPointerMovement)
            {
                if (Mathf.Abs(angleInDegrees) > 90 - _verticalMovementThreshold && Mathf.Abs(angleInDegrees) < 90 + _verticalMovementThreshold)
                {
                    var inputFieldRectTransform = ((RectTransform)inputField.transform);

                    // Calculate correction
                    var correction = -delta.y / inputFieldRectTransform.sizeDelta.y;

                    // Apply correction
                    _scrollRect.verticalScrollbar.value += correction;

                    return;
                } else
                {
                    _wasOnlyVerticalPointerMovement = false;
                }
            }

            endPosition = TMP_TextUtilities.FindNearestCharacter(inputField.textComponent, data.position, Camera.main, false);

            UpdateSelection();
        }

        private void OnStartIndicatorMove(Vector2 position)
        {
            startPosition = TMP_TextUtilities.FindNearestCharacter(inputField.textComponent, position - new Vector2(0, inputField.textComponent.fontSize), Camera.main, false);
            startPosition = Mathf.Min(startPosition, endPosition);
            UpdateSelection();
        }

        private void OnEndIndicatorMove(Vector2 position)
        {
            endPosition = TMP_TextUtilities.FindNearestCharacter(inputField.textComponent, position + new Vector2(0, inputField.textComponent.fontSize), Camera.main, false);
            endPosition = Mathf.Max(startPosition, endPosition);
            UpdateSelection();
        }
    }
}