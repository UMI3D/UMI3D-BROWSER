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
        DesktopPreviewBarSelection desktopPreviewBarSelection;
        MobilePreviewBarSelection mobilePreviewBarSelection;
#else
        BasePreviewBarSelection previewBarSelection;
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
                    SelectionType.Desktop => desktopPreviewBarSelection.isTextSelected,
                    SelectionType.Mobile => mobilePreviewBarSelection.isTextSelected
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
                    SelectionType.Desktop => desktopPreviewBarSelection.startPosition,
                    SelectionType.Mobile => mobilePreviewBarSelection.startPosition
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
                        desktopPreviewBarSelection.startPosition = value;
                        break;
                    case SelectionType.Mobile:
                        mobilePreviewBarSelection.startPosition = value;
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
                    SelectionType.Desktop => desktopPreviewBarSelection.endPosition,
                    SelectionType.Mobile => mobilePreviewBarSelection.endPosition
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
                        desktopPreviewBarSelection.endPosition = value;
                        break;
                    case SelectionType.Mobile:
                        mobilePreviewBarSelection.endPosition = value;
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
                    SelectionType.Desktop => desktopPreviewBarSelection.stringPosition,
                    SelectionType.Mobile => mobilePreviewBarSelection.stringPosition
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
                        desktopPreviewBarSelection.stringPosition = value;
                        break;
                    case SelectionType.Mobile:
                        mobilePreviewBarSelection.stringPosition = value;
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
            desktopPreviewBarSelection = new(this);
            mobilePreviewBarSelection = new(this);
#elif UNITY_STANDALONE_WIN
            previewBarSelection = new DesktopPreviewBarSelection(this);
#else
            previewBarSelection = new MobilePreviewBarSelection(this);
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
                    desktopPreviewBarSelection.OnEnable();
                    break;
                case SelectionType.Mobile:
                    mobilePreviewBarSelection.OnEnable();
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
                    desktopPreviewBarSelection.OnDisable();
                    break;
                case SelectionType.Mobile:
                    mobilePreviewBarSelection.OnDisable();
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
                    desktopPreviewBarSelection.Focus();
                    break;
                case SelectionType.Mobile:
                    mobilePreviewBarSelection.Focus();
                    break;
                default:
                    break;
            }
#else
            previewBarSelection.Focus();
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
                    desktopPreviewBarSelection.Select(start, end);
                    break;
                case SelectionType.Mobile:
                    mobilePreviewBarSelection.Select(start, end);
                    break;
                default:
                    break;
            }
#else
            previewBarSelection.Select(start, end);
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
                    desktopPreviewBarSelection.Deselect(newCaretPosition);
                    break;
                case SelectionType.Mobile:
                    mobilePreviewBarSelection.Deselect(newCaretPosition);
                    break;
                default:
                    break;
            }
#else
            previewBarSelection.Deselect(newCaretPosition);
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

        //[ContextMenu("Test character width")]
        //void TestCharacterWidth()
        //{
        //    UnityEngine.Debug.Log($"test deselection = {inputField.stringPosition} && {inputField.selectionAnchorPosition} && {inputField.selectionFocusPosition}");
            
        //    string text = "Hello, world!";
        //    Vector2 preferredValues = textTMP.GetPreferredValues(text);
        //    float textWidthInPoints = preferredValues.x;

        //    Debug.Log("La largeur du mot '" + text + "' est " + textWidthInPoints);
        //}
#endif
    }
}