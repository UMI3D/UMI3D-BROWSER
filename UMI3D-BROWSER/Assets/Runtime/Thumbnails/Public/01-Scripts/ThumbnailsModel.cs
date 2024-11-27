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
    public class ThumbnailsModel
    {
        public ThumbnailsModel()
        {
            contentModeNotifier = NotificationHub.Default
                .GetNotifier<ThumbnailsNotificationKeys.ContentModeChanged>(this);

            horizontalSliderValueNotifier = NotificationHub.Default
                .GetNotifier<ThumbnailsNotificationKeys.SliderValueWillChange>(this);

            horizontalSliderValueNotifier = NotificationHub.Default
                .GetNotifier<ThumbnailsNotificationKeys.SliderValueWillChange>(this);
        }

        #region Content mode

        public ThumbnailContentMode contentMode;

        Notifier contentModeNotifier;

        /// <summary>
        /// Switch between <see cref="ThumbnailContentMode.Large"/> and <see cref="ThumbnailContentMode.Small"/>
        /// </summary>
        public void ToggleContentMode()
        {
            contentMode = contentMode == ThumbnailContentMode.Large
               ? ThumbnailContentMode.Small
               : ThumbnailContentMode.Large;
            contentModeNotifier[ThumbnailsNotificationKeys.ContentModeChanged.ContentMode] = contentMode;
            contentModeNotifier.Notify();
        }

        #endregion

        #region Horizontal slider

        public float scrollButtonSpeed = 1.0f;

        public float horizontalSliderValue;

        Notifier horizontalSliderValueNotifier;
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
            horizontalSliderValueNotifier[ThumbnailsNotificationKeys.SliderValueWillChange.Value] = 0;
            horizontalSliderValueNotifier.Notify();
        }

        void SetHorizontalSliderValue(float value)
        {
            horizontalSliderValueNotifier[ThumbnailsNotificationKeys.SliderValueWillChange.Value] = value;
            horizontalSliderValueNotifier.Notify();
        }

        public void DisplaySlideButton(bool display)
        {
            SliderButtonVisibilityNotifier[ThumbnailsNotificationKeys.SliderButtonVisibilityWillChange.IsVisible] = display;
            SliderButtonVisibilityNotifier.Notify();
        }

        #endregion

        #region Layout

        public Vector2 gridSize;
        public float gridRowCount;
        public Vector2 gridSpacing;

        Notifier gridPropertiesNotifier;

        public void SetGridProperties(Vector2 gridSize, float gridRowCount, Vector2 gridSpacing)
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

    }
}