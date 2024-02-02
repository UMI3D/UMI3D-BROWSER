using System.Collections;
using System.Collections.Generic;
using TMPro;
using umi3dBrowsers.keyboard;
using UnityEngine;
using UnityEngine.EventSystems;

namespace umi3dBrowsers.displayer
{
    public class TMP_UMI3DUIInputField : TMP_InputField
    {
        InputFieldToVRKeyboardMiddleware keyboardMiddleware;

        protected override void Awake()
        {
            keyboardMiddleware = GetComponent<InputFieldToVRKeyboardMiddleware>();

        }
        /// <summary>
        /// Setter for <see cref="keyboard"/>.
        /// </summary>
        /// <param name="keyboard"></param>
        public void SetKeyboard(Keyboard keyboard)
        {
            if (keyboardMiddleware != null) { keyboardMiddleware.SetKeyboard(keyboard); }
            else
                Debug.Log($"<color=cyan>Please make sure you've got a key board middleware </color>", this);
        }


        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);

            if (keyboardMiddleware != null)
            {
                keyboardMiddleware.InputSelected(this, res =>
                {
                    text = res;
                });
            }
            else
                Debug.Log($"<color=cyan>Please make sure you've got a key board middleware </color>", this);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);

            if (text.Trim().Length == 0) 
            {
                placeholder.gameObject.SetActive(true);
            }
        }

        public override void OnUpdateSelected(BaseEventData eventData)
        {
            base.OnUpdateSelected(eventData);
        }


    }
}

