using System;
using System.Collections;
using System.Collections.Generic;
using umi3dBrowsers.displayer;
using UnityEngine;

namespace umi3dBrowsers.keyboard
{
    public class InputFieldToVRKeyboardMiddleware : MonoBehaviour
    {
        /// <summary>
        /// Keyboard to edit this inputfield.
        /// </summary>
        [SerializeField] protected Keyboard keyboard;

        private void Awake()
        {
            keyboard = Keyboard.Instance;
        }

        internal void InputSelected(TMP_UMI3DUIInputField inputField, Action<string> onEditFinished)
        {
            keyboard.OpenKeyboard(inputField, onEditFinished);
        }
    }
}

