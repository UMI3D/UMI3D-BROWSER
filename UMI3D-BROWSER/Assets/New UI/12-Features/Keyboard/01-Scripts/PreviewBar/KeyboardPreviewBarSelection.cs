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
using TMPro;
using umi3d.browserRuntime.NotificationKeys;
using UnityEngine;

namespace umi3d.browserRuntime.ui.keyboard
{
    public class KeyboardPreviewBarSelection : MonoBehaviour
    {
        TMP_InputField inputField;

#if UNITY_EDITOR
        public enum SelectionType
        {
            Desktop,
            Mobile
        }
        [SerializeField] SelectionType selectionType = SelectionType.Mobile;
        TMPInputFieldSelection tmpInputFieldSelection;
        UMI3DInputFieldSelection umi3dInputFieldSelection;
#else
        BaseInputFieldSelection previewBarSelection;
#endif

        /// <summary>
        /// Is text selected.
        /// </summary>
        public bool isTextSelected 
        {
            get
            {
#if UNITY_EDITOR
                return selectionType switch
                {
                    SelectionType.Desktop => tmpInputFieldSelection.isTextSelected,
                    SelectionType.Mobile => umi3dInputFieldSelection.isTextSelected
                };
#else
                return previewBarSelection.isTextSelected;
#endif
            }
        }

        /// <summary>
        /// Start position of the selection.
        /// </summary>
        public int startPosition
        {
            get
            {
#if UNITY_EDITOR
                return selectionType switch
                {
                    SelectionType.Desktop => tmpInputFieldSelection.startPosition,
                    SelectionType.Mobile => umi3dInputFieldSelection.startPosition
                };
#else
                return previewBarSelection.startPosition;
#endif
            }
            private set
            {
#if UNITY_EDITOR
                switch (selectionType)
                {
                    case SelectionType.Desktop:
                        tmpInputFieldSelection.startPosition = value;
                        break;
                    case SelectionType.Mobile:
                        umi3dInputFieldSelection.startPosition = value;
                        break;
                    default:
                        break;
                }
#else
                previewBarSelection.startPosition = value;
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
#if UNITY_EDITOR
                return selectionType switch
                {
                    SelectionType.Desktop => tmpInputFieldSelection.endPosition,
                    SelectionType.Mobile => umi3dInputFieldSelection.endPosition
                };
#else
                return previewBarSelection.endPosition;
#endif
            }
            private set
            {
#if UNITY_EDITOR
                switch (selectionType)
                {
                    case SelectionType.Desktop:
                        tmpInputFieldSelection.endPosition = value;
                        break;
                    case SelectionType.Mobile:
                        umi3dInputFieldSelection.endPosition = value;
                        break;
                    default:
                        break;
                }
#else
                previewBarSelection.endPosition = value;
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
#if UNITY_EDITOR
                return selectionType switch
                {
                    SelectionType.Desktop => tmpInputFieldSelection.stringPosition,
                    SelectionType.Mobile => umi3dInputFieldSelection.stringPosition
                };
#else
                return previewBarSelection.stringPosition;
#endif
            }
            private set
            {
#if UNITY_EDITOR
                switch (selectionType)
                {
                    case SelectionType.Desktop:
                        tmpInputFieldSelection.stringPosition = value;
                        break;
                    case SelectionType.Mobile:
                        umi3dInputFieldSelection.stringPosition = value;
                        break;
                    default:
                        break;
                }
#else
                previewBarSelection.stringPosition = value;
#endif
            }
        }

        void Awake()
        {
            inputField = GetComponentInChildren<TMP_InputField>();

#if UNITY_EDITOR
            tmpInputFieldSelection = new(this);
            tmpInputFieldSelection.isPreviewBar = true;
            tmpInputFieldSelection.allowTextModification = true;
            tmpInputFieldSelection.allowSelection = true;
            umi3dInputFieldSelection = new(this);
            umi3dInputFieldSelection.isPreviewBar = true;
            umi3dInputFieldSelection.allowTextModification = true;
            umi3dInputFieldSelection.allowSelection = true;
#elif UNITY_STANDALONE_WIN
            previewBarSelection = new TMPInputFieldSelection(this);
            previewBarSelection.isPreviewBar = true;
            previewBarSelection.allowTextModification = true;
            previewBarSelection.allowSelection = true;
#else
            previewBarSelection = new UMI3DInputFieldSelection(this);
            previewBarSelection.isPreviewBar = true;
            previewBarSelection.allowTextModification = true;
            previewBarSelection.allowSelection = true;
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

#if UNITY_EDITOR
            switch (selectionType)
            {
                case SelectionType.Desktop:
                    tmpInputFieldSelection.OnEnable();
                    break;
                case SelectionType.Mobile:
                    umi3dInputFieldSelection.OnEnable();
                    break;
                default:
                    break;
            }
#else
                previewBarSelection.OnEnable();
#endif
        }

        void OnDisable()
        {
            NotificationHub.Default.Unsubscribe(this, KeyboardNotificationKeys.AskPreviewFocus);

#if UNITY_EDITOR
            switch (selectionType)
            {
                case SelectionType.Desktop:
                    tmpInputFieldSelection.OnDisable();
                    break;
                case SelectionType.Mobile:
                    umi3dInputFieldSelection.OnDisable();
                    break;
                default:
                    break;
            }
#else
                previewBarSelection.OnDisable();
#endif
        }

        /// <summary>
        /// Focus the preview bar.<br/>
        /// <br/>
        /// Display the caret and hide selection.
        /// </summary>
        void Focus()
        {
#if UNITY_EDITOR
            switch (selectionType)
            {
                case SelectionType.Desktop:
                    tmpInputFieldSelection.Focus();
                    break;
                case SelectionType.Mobile:
                    umi3dInputFieldSelection.Focus();
                    break;
                default:
                    break;
            }
#else
            previewBarSelection.Focus();
#endif
        }

        /// <summary>
        /// Unfocus the preview bar.<br/>
        /// <br/>
        /// Hide the caret and hide selection.
        /// </summary>
        public void Blur()
        {
#if UNITY_EDITOR
            switch (selectionType)
            {
                case SelectionType.Desktop:
                    tmpInputFieldSelection.Blur();
                    break;
                case SelectionType.Mobile:
                    umi3dInputFieldSelection.Blur();
                    break;
                default:
                    break;
            }
#else
            previewBarSelection.Blur();
#endif
        }

        /// <summary>
        /// Make a selection.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void Select(int start, int end)
        {
#if UNITY_EDITOR
            switch (selectionType)
            {
                case SelectionType.Desktop:
                    tmpInputFieldSelection.Select(start, end);
                    break;
                case SelectionType.Mobile:
                    umi3dInputFieldSelection.Select(start, end);
                    break;
                default:
                    break;
            }
#else
            previewBarSelection.Select(start, end);
#endif
        }

        /// <summary>
        /// Make a selection without notifying.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void SelectWithoutNotify(int start, int end)
        {
#if UNITY_EDITOR
            switch (selectionType)
            {
                case SelectionType.Desktop:
                    tmpInputFieldSelection.SelectWithoutNotify(start, end);
                    break;
                case SelectionType.Mobile:
                    umi3dInputFieldSelection.SelectWithoutNotify(start, end);
                    break;
                default:
                    break;
            }
#else
            previewBarSelection.SelectWithoutNotify(start, end);
#endif
        }

        /// <summary>
        /// Deselect and place the caret at <paramref name="newCaretPosition"/>.
        /// </summary>
        /// <param name="newCaretPosition"></param>
        public void Deselect(int newCaretPosition)
        {
#if UNITY_EDITOR
            switch (selectionType)
            {
                case SelectionType.Desktop:
                    tmpInputFieldSelection.Deselect(newCaretPosition);
                    break;
                case SelectionType.Mobile:
                    umi3dInputFieldSelection.Deselect(newCaretPosition);
                    break;
                default:
                    break;
            }
#else
            previewBarSelection.Deselect(newCaretPosition);
#endif
        }

        /// <summary>
        /// Deselect and place the caret at <paramref name="newCaretPosition"/> without notifying.
        /// </summary>
        /// <param name="newCaretPosition"></param>
        public void DeselectWithoutNotify(int newCaretPosition)
        {
#if UNITY_EDITOR
            switch (selectionType)
            {
                case SelectionType.Desktop:
                    tmpInputFieldSelection.DeselectWithoutNotify(newCaretPosition);
                    break;
                case SelectionType.Mobile:
                    umi3dInputFieldSelection.DeselectWithoutNotify(newCaretPosition);
                    break;
                default:
                    break;
            }
#else
            previewBarSelection.DeselectWithoutNotify(newCaretPosition);
#endif
        }

#if UNITY_EDITOR
        [ContextMenu("TestFocus")]
        void TestFocus()
        {
            UnityEngine.Debug.Log($"test focus = {stringPosition}");
            Focus();
        }

        [ContextMenu("Test Selection")]
        void TestSelection()
        {
            UnityEngine.Debug.Log($"test selection = {stringPosition} && {startPosition} && {endPosition}");
            Select(0, 1);
        }

        [ContextMenu("Test deselection")]
        void TestDeselection()
        {
            UnityEngine.Debug.Log($"test deselection = {stringPosition} && {startPosition} && {endPosition}");
            Deselect(1);
        }
#endif
    }
}