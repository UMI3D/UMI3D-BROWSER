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
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.thumbnails
{
    [RequireComponent(typeof(Image))]
    internal class ThumbnailSubButtonImage : MonoBehaviour
    {
        [Header("Sprite")]
        [SerializeField] Sprite defaultImage;
        [SerializeField] Sprite hoverImage;
        [SerializeField] Sprite clickImage;

        [Header("Color")]
        [SerializeField] Color defaultColor;
        [SerializeField] Color hoverColor;
        [SerializeField] Color clickColor;

        Image image;

        void Awake()
        {
            image = GetComponent<Image>();
        }

        public void OnPointerEnter()
        {
            image.sprite = hoverImage;
            image.color = hoverColor;
        }

        public void OnPointerExit()
        {
            image.sprite = defaultImage;
            image.color = defaultColor;
        }

        public void OnPointerDown()
        {
            image.sprite = clickImage;
            image.color = clickColor;
        }

        public void OnPointerUp()
        {
            image.sprite = hoverImage;
            image.color = hoverColor;
        }
    }
}