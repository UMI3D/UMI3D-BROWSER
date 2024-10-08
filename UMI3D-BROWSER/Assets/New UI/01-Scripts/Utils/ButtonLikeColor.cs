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

namespace umi3d.browserRuntime.ui.utils
{
    public class ButtonLikeColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerClickHandler
    {
        [SerializeField] private Image image;
        [SerializeField] private Color normalColor;
        [SerializeField] private Color highlightedColor;
        [SerializeField] private Color pressedColor;

        private bool isClicked;

        public void OnPointerClick(PointerEventData eventData)
        {
            isClicked = false;
            image.color = normalColor;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isClicked = true;
            image.color = pressedColor;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            image.color = highlightedColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isClicked)
                return;

            image.color = normalColor;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            image.color = normalColor;
        }
#endif
    }
}