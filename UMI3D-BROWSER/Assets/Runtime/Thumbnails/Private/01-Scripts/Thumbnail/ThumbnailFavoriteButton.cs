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
    [RequireComponent(typeof(Button))]
    internal class ThumbnailFavoriteButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        Button button;
        ThumbnailSubButtonImage subButtonImage;
        RectTransform rectTransform;

        ThumbnailModelContainer model;

        void Awake()
        {
            button = GetComponent<Button>();
            subButtonImage = GetComponentInChildren<ThumbnailSubButtonImage>();
            rectTransform = GetComponent<RectTransform>();

            model = GetComponentInParent<ThumbnailModelContainer>();

            NotificationHub.Default.Subscribe<ThumbnailsNotificationKeys.FavoriteStatusSet>(
                this,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == model.model),
                FavoriteStatusUpdated
            );

            NotificationHub.Default.Subscribe<ThumbnailsNotificationKeys.ContentModeChanged>(
                this,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == model.model.thumbnailsModel),
                ContentModeChanged
            );

        }

        void Start()
        {
            SetContentMode(model.model.thumbnailsModel.contentMode);
        }

        void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!button.interactable)
            {
                return;
            }

            if (!model.model.isFavorite)
            {
                subButtonImage.OnPointerEnter();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!button.interactable)
            {
                return;
            }

            if (!model.model.isFavorite)
            {
                subButtonImage.OnPointerExit();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!button.interactable)
            {
                return;
            }

            subButtonImage.OnPointerDown();
            model.model.ToggleFavorite();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!button.interactable)
            {
                return;
            }

            if (!model.model.isFavorite)
            {
                subButtonImage.OnPointerUp();
            }
        }

        void FavoriteStatusUpdated(Notification notification)
        {
            if (!notification.TryGetInfoT(ThumbnailsNotificationKeys.FavoriteStatusSet.IsFavorite, out bool isFavorite))
            {
                return;
            }

            if (isFavorite)
            {
                subButtonImage.OnPointerDown();
            }
            else
            {
                subButtonImage.OnPointerExit();
            }
        }

        void ContentModeChanged(Notification notification)
        {
            if (!notification.TryGetInfoT(ThumbnailsNotificationKeys.ContentModeChanged.ContentMode, out ThumbnailContentMode contentMode))
            {
                return;
            }

            SetContentMode(contentMode);
        }

        void SetContentMode(ThumbnailContentMode contentMode)
        {
            switch (contentMode)
            {
                case ThumbnailContentMode.Small:
                    rectTransform.anchoredPosition = new Vector3(rectTransform.anchoredPosition.x, 25f);
                    break;
                case ThumbnailContentMode.Middle:
                case ThumbnailContentMode.Large:
                    rectTransform.anchoredPosition = new Vector3(rectTransform.anchoredPosition.x, 45f);
                    break;
                default:
                    break;
            }
        }
    }
}