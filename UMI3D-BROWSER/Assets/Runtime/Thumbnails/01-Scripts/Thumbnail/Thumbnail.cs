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

namespace umi3d.browserRuntime.ui.thumbnails
{
    [RequireComponent(typeof(Button), typeof(ThumbnailModelContainer))]
    internal class Thumbnail : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        Button button;
        ThumbnailModelContainer model;

        void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);

            model = GetComponent<ThumbnailModelContainer>();
        }

        void Start()
        {
            model.model.SetSubButtonVisibility(false);
        }

        void OnClick()
        {
            model.model.Select();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            model.model.SetSubButtonVisibility(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            model.model.SetSubButtonVisibility(false);
        }
    }
}