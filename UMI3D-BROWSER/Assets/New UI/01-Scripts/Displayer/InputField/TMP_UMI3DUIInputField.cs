using System;
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
        [SerializeField] protected Keyboard keyboard;

        protected override void Awake()
        {
            keyboard = Keyboard.Instance;
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);

            if (keyboard != null)
            {
                InputSelected();
            }
            else
                Debug.Log($"<color=cyan>Please make sure you've got a key board</color>", this);
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

        private void InputSelected()
        {
            keyboard.OpenKeyboard(this, res =>
            {
                text = res;
            });
        }
    }
}

