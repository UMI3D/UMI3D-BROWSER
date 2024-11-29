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
using System.Collections.Generic;
using umi3d.browserRuntime.ui.popup;
using UnityEngine;

namespace umi3d.browserRuntime.ui.thumbnails
{
    public class ThumbnailsModel
    {
        public List<ThumbnailModel> thumbnails = new();

        public ThumbnailsModel()
        {
            contentModeNotifier = NotificationHub.Default
                .GetNotifier<ThumbnailsNotificationKeys.ContentModeChanged>(this);

            sliderValueSetNotifier = NotificationHub.Default
                .GetNotifier<ThumbnailsNotificationKeys.SliderValueSet>(this);

            sliderValueSetNotifier = NotificationHub.Default
                .GetNotifier<ThumbnailsNotificationKeys.SliderValueSet>(this);

            gridPropertiesNotifier = NotificationHub.Default
                .GetNotifier<ThumbnailsNotificationKeys.GridPropertiesWillChange>(this);

            SliderButtonVisibilityNotifier = NotificationHub.Default
               .GetNotifier<ThumbnailsNotificationKeys.SliderButtonVisibilityWillChange>(this);

            thumbnailAddedNotifier = NotificationHub.Default
                .GetNotifier<ThumbnailsNotificationKeys.Added>(this);

            thumbnailDeletedNotifier = NotificationHub.Default
                .GetNotifier<ThumbnailsNotificationKeys.Deleted>(this);

            popupNotifier = new(this);
        }

        #region Content mode

        public ThumbnailContentMode contentMode;
        public ThumbnailContentMode primaryContentMode;
        public ThumbnailContentMode secondaryContentMode;

        Notifier contentModeNotifier;

        public void SetContentMode(ThumbnailContentMode contentMode)
        {
            this.contentMode = contentMode;
            contentModeNotifier[ThumbnailsNotificationKeys.ContentModeChanged.ContentMode] = contentMode;
            contentModeNotifier.Notify();
        }

        /// <summary>
        /// Switch between <see cref="ThumbnailContentMode.Large"/> and <see cref="ThumbnailContentMode.Small"/>
        /// </summary>
        public void ToggleContentMode()
        {
            contentMode = contentMode == primaryContentMode
               ? secondaryContentMode
               : primaryContentMode;
            contentModeNotifier[ThumbnailsNotificationKeys.ContentModeChanged.ContentMode] = contentMode;
            contentModeNotifier.Notify();
        }

        #endregion

        #region Horizontal slider

        public const float scrollButtonSpeed = 1.0f;

        public float horizontalSliderValue;
        public bool areSlideButtonVisible;

        Notifier sliderValueSetNotifier;
        Notifier SliderButtonVisibilityNotifier;

        public void SlideTowardLeft()
        {
            //if (vignetteDisplayers.Count > (int)vignetteMode)
            //    scrollbar.value -= scrollButtonSpeed / (vignetteDisplayers.Count - (int)vignetteMode);
        }

        public void SlideTowardRight()
        {
            //if (vignetteDisplayers.Count > (int)vignetteMode)
            //    scrollbar.value += scrollButtonSpeed / (vignetteDisplayers.Count - (int)vignetteMode);
        }

        public void ResetSlider()
        {
            horizontalSliderValue = 0f;
            sliderValueSetNotifier[ThumbnailsNotificationKeys.SliderValueSet.Value] = 0f;
            sliderValueSetNotifier.Notify();
        }

        public void SetHorizontalSliderValue(float value)
        {
            horizontalSliderValue = value;
            sliderValueSetNotifier[ThumbnailsNotificationKeys.SliderValueSet.Value] = value;
            sliderValueSetNotifier.Notify();
        }

        public void DisplaySlideButton(bool display)
        {
            areSlideButtonVisible = display;
            SliderButtonVisibilityNotifier[ThumbnailsNotificationKeys.SliderButtonVisibilityWillChange.IsVisible] = display;
            SliderButtonVisibilityNotifier.Notify();
        }

        #endregion

        #region Layout

        public ThumbnailsGridProperties smallContentModeLayout = 
            new ThumbnailsGridProperties(
                size: new(169f, 111f), 
                rowCount: 2, 
                spacing: new(11f, 30f)
            );
        public ThumbnailsGridProperties middleContentModeLayout =
            new ThumbnailsGridProperties(
                size: new(227f, 265f),
                rowCount: 1,
                spacing: new(24f, 0f)
            );
        public ThumbnailsGridProperties largeContentModeLayout =
            new ThumbnailsGridProperties(
                size: new(345f, 266f),
                rowCount: 1,
                spacing: new(24f, 30f)
            );

        public Vector2 gridSize;
        public int gridRowCount;
        public Vector2 gridSpacing;

        Notifier gridPropertiesNotifier;

        public void SetGridProperties(Vector2 gridSize, int gridRowCount, Vector2 gridSpacing)
        {
            this.gridSize = gridSize;
            this.gridRowCount = gridRowCount;
            this.gridSpacing = gridSpacing;
            gridPropertiesNotifier[ThumbnailsNotificationKeys.GridPropertiesWillChange.Size] = gridSize;
            gridPropertiesNotifier[ThumbnailsNotificationKeys.GridPropertiesWillChange.RowCount] = gridRowCount;
            gridPropertiesNotifier[ThumbnailsNotificationKeys.GridPropertiesWillChange.Spacing] = gridSpacing;
            gridPropertiesNotifier.Notify();
        }

        #endregion

        #region Add Thumbnail

        Notifier thumbnailAddedNotifier;

        public void Add(ThumbnailModel thumbnail)
        {
            if (thumbnails.Contains(thumbnail)) return;
            thumbnails.Add(thumbnail);
            thumbnail.thumbnailsModel = this;
            thumbnailAddedNotifier[ThumbnailsNotificationKeys.Added.Thumbnail] = thumbnail;
            thumbnailAddedNotifier.Notify();
        }

        #endregion

        #region Delete Thumbnails

        const string LOCALIZATION_TABLE = "UMI3D_inetum";

        PopupNotifier popupNotifier;
        Notifier thumbnailDeletedNotifier;

        public void Delete(ThumbnailModel thumbnail)
        {
            if (!thumbnails.Contains(thumbnail)) return;
            thumbnails.Remove(thumbnail);
            thumbnail.thumbnailsModel = null;
            thumbnailDeletedNotifier[ThumbnailsNotificationKeys.Deleted.Thumbnail] = thumbnail;
            thumbnailDeletedNotifier.Notify();
        }

        /// <summary>
        /// Display a Dialogue box before deleting the thumbnail. Let the user choose.
        /// </summary>
        /// <param name="thumbnail"></param>
        public void AskToDelete(ThumbnailModel thumbnail)
        {
            popupNotifier
                .enqueue
                .SetType(PopupType.Warning)
                .SetArguments(("worldName", thumbnail.name))
                .SetDescription(LOCALIZATION_TABLE, "popup_deleteWorld_description")
                .SetButtons((LOCALIZATION_TABLE, "popup_cancel"), (LOCALIZATION_TABLE, "popup_yes"))
                .SetButtonsAction(index =>
                {
                    if (index == 1)
                    {
                        Delete(thumbnail);
                    }
                })
                .Notify();
        }

        #endregion
    }
}