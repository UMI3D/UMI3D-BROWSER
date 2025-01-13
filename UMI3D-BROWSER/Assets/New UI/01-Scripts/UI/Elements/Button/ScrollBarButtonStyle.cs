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

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.elements.button
{
    [RequireComponent(typeof(Button))]
    public class ScrollBarButtonStyle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private Color iconColor;
        [SerializeField] private Color iconColorHover;
        [SerializeField] private Color iconColorActive;

        private bool isActive;
        private bool isHover;
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(Click);
        }

        public void Click()
        {
            isActive = false;
            icon.color = isHover ? iconColorHover : iconColor;

            EventSystem.current.SetSelectedGameObject(null);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isActive = true;
            icon.color = iconColorActive;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isHover = true;
            if (isActive)
                return;

            icon.color = iconColorHover;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHover = false;
            if (isActive)
                return;

            icon.color = iconColor;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            icon.color = iconColor;
        }
#endif
    }
}