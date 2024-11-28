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
        public ThumbnailsModel thumbnailsModel;

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

            subInputsVisibilityNotifier = NotificationHub.Default
               .GetNotifier<ThumbnailsNotificationKeys.SubInputsVisibilityWillChange>(this);

            setNameNotifier = NotificationHub.Default
              .GetNotifier<ThumbnailsNotificationKeys.NameSet>(this);

            updateNameNotifier = NotificationHub.Default
             .GetNotifier<ThumbnailsNotificationKeys.NameUpdated>(this);
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

        public void Delete()
        {
            if (thumbnailsModel == null)
            {
                UnityEngine.Debug.LogError($"Error: Thumbnails model is null when trying to delete thumbnail.");
            }
            thumbnailsModel?.Delete(this);
        }

        #endregion

        #region Sub Inputs

        public bool areSubInputsVisible = false;

        Notifier subInputsVisibilityNotifier;

        public void SetSubInputsVisibility(bool isVisible)
        {
            this.areSubInputsVisible = isVisible;
            subInputsVisibilityNotifier[ThumbnailsNotificationKeys.SubInputsVisibilityWillChange.IsVisible] = isVisible;
            subInputsVisibilityNotifier.Notify();
        }

        #endregion

        #region InputField

        public string name;

        Notifier updateNameNotifier;
        Notifier setNameNotifier;

        public void SetName(string name)
        {
            this.name = name;
            setNameNotifier[ThumbnailsNotificationKeys.NameSet.Name] = name;
            setNameNotifier.Notify();
        }

        public void UpdateName(string name)
        {
            this.name = name;
            updateNameNotifier[ThumbnailsNotificationKeys.NameUpdated.Name] = name;
            updateNameNotifier.Notify();
        }

        #endregion
    }
}