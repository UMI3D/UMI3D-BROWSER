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
        Action<PointerEventData> hoverEnterCallBack;
        Action<PointerEventData> hoverExitCallBack;

        public event Action<string> OnTextChanged;

        protected override void OnDisable()
        {
            Keyboard.Instance?.CancelAndClose();
        }

        public void SetCallBacks(Action<PointerEventData> hoverEnterCallBack, Action<PointerEventData> hoverExitCallBack)
        {
            this.hoverEnterCallBack = hoverEnterCallBack;
            this.hoverExitCallBack = hoverExitCallBack;
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            hoverEnterCallBack?.Invoke(eventData);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            hoverExitCallBack?.Invoke(eventData);
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);

            if (Keyboard.Instance != null)
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
            Keyboard.Instance?.OpenKeyboard(this, res =>
            {
                text = res;
                OnTextChanged?.Invoke(res);
            });
        }
    }
}

