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

namespace umi3d.browserRuntime.ui.thumbnails
{
    public class ThumbnailModel 
    {
        public ThumbnailModel()
        {
            selectNotifier = NotificationHub.Default
                .GetNotifier<ThumbnailsNotificationKeys.Select>(this);

            imageChangedNotifier = NotificationHub.Default
                .GetNotifier<ThumbnailsNotificationKeys.ImageSpriteWillChange>(this);

            updateFavoriteNotifier = NotificationHub.Default
                .GetNotifier<ThumbnailsNotificationKeys.FavoriteStatusUpdated>(this);

            favoriteChangedNotifier = NotificationHub.Default
               .GetNotifier<ThumbnailsNotificationKeys.FavoriteStatusChanged>(this);

            deleteNotifier = NotificationHub.Default
               .GetNotifier<ThumbnailsNotificationKeys.Delete>(this);

            subInputsVisibilityNotifier = NotificationHub.Default
               .GetNotifier<ThumbnailsNotificationKeys.SubInputsVisibilityWillChange>(this);

            updateNameNotifier = NotificationHub.Default
              .GetNotifier<ThumbnailsNotificationKeys.NameWillChange>(this);
        }

        #region Clicked

        Notifier selectNotifier;

        public void Select()
        {
            selectNotifier.Notify();
        }

        #endregion

        #region Image

        public Sprite image;

        Notifier imageChangedNotifier;

        public void SetImage(Sprite image)
        {
            this.image = image;
            imageChangedNotifier[ThumbnailsNotificationKeys.ImageSpriteWillChange.Sprite] = image;
            imageChangedNotifier.Notify();
        }

        #endregion

        #region Favorite

        public bool isFavorite;

        Notifier updateFavoriteNotifier;
        Notifier favoriteChangedNotifier;

        public void UpdateFavoriteStatus(bool isFavorite)
        {
            this.isFavorite = isFavorite;
            updateFavoriteNotifier[ThumbnailsNotificationKeys.FavoriteStatusUpdated.IsFavorite] = isFavorite;
            updateFavoriteNotifier.Notify();
        }

        public void ToggleFavorite()
        {
            isFavorite = !isFavorite;
            favoriteChangedNotifier[ThumbnailsNotificationKeys.FavoriteStatusChanged.IsFavorite] = isFavorite;
            favoriteChangedNotifier.Notify();
        }

        #endregion

        #region Delete

        Notifier deleteNotifier;

        public void Delete()
        {
            deleteNotifier.Notify();
        }

        #endregion

        #region Sub Inputs

        public bool isDisplaying = false;

        Notifier subInputsVisibilityNotifier;

        public void SetSubInputsVisibility(bool isVisible)
        {
            this.isDisplaying = isVisible;
            subInputsVisibilityNotifier[ThumbnailsNotificationKeys.SubInputsVisibilityWillChange.IsVisible] = isVisible;
            subInputsVisibilityNotifier.Notify();
        }

        #endregion

        #region InputField

        public string name;

        Notifier updateNameNotifier;

        public void UpdateName(string name)
        {
            this.name = name;
            updateNameNotifier[ThumbnailsNotificationKeys.NameWillChange.Name] = name;
            updateNameNotifier.Notify();
        }

        #endregion
    }
}