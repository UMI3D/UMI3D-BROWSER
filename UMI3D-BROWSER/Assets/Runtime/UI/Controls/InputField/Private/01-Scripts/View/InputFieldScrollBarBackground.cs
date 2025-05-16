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

namespace umi3d.browserRuntime.ui
{
    [RequireComponent(typeof(Image))]
    public class InputFieldScrollBarBackground : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] Color normalColor = Color.white;
        [SerializeField] Color hoverColor = Color.white;

        Image _image;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _image.color = normalColor;
        }

        private void OnValidate()
        {
            _image = GetComponent<Image>();
            _image.color = normalColor;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _image.color = hoverColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _image.color = normalColor;
        }
    }
}