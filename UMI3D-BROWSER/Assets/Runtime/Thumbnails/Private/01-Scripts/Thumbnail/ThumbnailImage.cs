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

using inetum.unityUtils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.thumbnails
{
    [RequireComponent(typeof(Image))]
    internal class ThumbnailImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] Color imageColor;
        [SerializeField] Color imageHoverColor;
        [SerializeField] Sprite defaultImage;

        Image image;

        ThumbnailModelContainer model;

        void Awake()
        {
            image = GetComponent<Image>();

            model = GetComponentInParent<ThumbnailModelContainer>();

            NotificationHub.Default.Subscribe<ThumbnailsNotificationKeys.ImageSpriteWillChange>(
                this,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == model.model),
                ImageSpriteWillChange
            );
        }

        void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        void ImageSpriteWillChange(Notification notification)
        {
            if (!notification.TryGetInfoT(ThumbnailsNotificationKeys.ImageSpriteWillChange.Sprite, out Sprite sprite))
            {
                return;
            }

            image.sprite = sprite == null
                ? defaultImage
                : sprite;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            image.color = imageHoverColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            image.color = imageColor;
        }
    }
}