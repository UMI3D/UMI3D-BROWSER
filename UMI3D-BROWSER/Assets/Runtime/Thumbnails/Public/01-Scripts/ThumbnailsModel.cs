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

namespace umi3d.browserRuntime.ui.thumbnails
{
    public class ThumbnailsModel
    {
        public ThumbnailsModel()
        {
            contentModeNotifier = NotificationHub.Default
                .GetNotifier<ThumbnailsNotificationKeys.ContentModeChanged>(this);

            horizontalSliderValueNotifier = NotificationHub.Default
                .GetNotifier<ThumbnailsNotificationKeys.SliderValueWillChanged>(this);
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
            contentModeNotifier[ThumbnailsNotificationKeys.ThumbnailsContentModeChanged.ContentMode] = contentMode;
            contentModeNotifier.Notify();
        }

        #endregion

        #region Horizontal slider

        public float horizontalSliderValue;

        Notifier horizontalSliderValueNotifier;

        public void SetHorizontalSliderValue(float value)
        {
            horizontalSliderValueNotifier[ThumbnailsNotificationKeys.SliderValueWillChanged.Value] = value;
            horizontalSliderValueNotifier.Notify();
        }

        #endregion
    }
}