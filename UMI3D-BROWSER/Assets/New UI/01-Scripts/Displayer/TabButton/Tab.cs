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
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace umi3dBrowsers.displayer
{
    public class Tab : Selectable
    {
        public UnityEvent OnClick;

        public Image hoverBar;
        public TextMeshProUGUI label;
        public Color selectedLabelColor;
        public float animationTime = 0.3f;

        public Color labelBaseColor;
        public bool isSelected;

        protected override void Awake()
        {
            base.Awake();
            labelBaseColor = label.color;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            isSelected = true;

            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            if (IsInteractable() && navigation.mode != Navigation.Mode.None && EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(gameObject, eventData);
            OnClick.Invoke();

        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            EnableHoverBarFX();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            DisableHoverBarFX();
        }

        public void EnableHoverBarFX()
        {
            if (!isSelected)
            {
                StartCoroutine(HoverBarAnimation(0, 1));
            }
        }

        public void DisableHoverBarFX()
        {
            if (!isSelected)
            {
                StartCoroutine(HoverBarAnimation(1, 0));
            }
        }

        private IEnumerator HoverBarAnimation(float to, float at)
        {
            float timer = 0f;
            while (timer < animationTime)
            {
                timer += Time.deltaTime;
                float alpha = Mathf.Lerp(to, at, timer / animationTime);
                hoverBar.color = new Color(hoverBar.color.r, hoverBar.color.g, hoverBar.color.b, alpha);
                yield return null;
            }
        }

        internal void SetLabel(string label)
        {
            this.label.text = label.Trim();
        }
    }
}

