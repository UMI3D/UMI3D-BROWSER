/*
Copyright 2019 - 2025 Inetum

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

using inetum.unityUtils.observation;
using UnityEngine;

namespace umi3d.browserRuntime.forms
{
    internal class FormThumbnailModel 
    {
        public string HeaderText { get; private set; } = string.Empty;
        public Color IndicatorColor { get; private set; } = Color.white;
        public bool IsLoading { get; private set; } = false;

        private readonly Notifier _setNotifier;

        public FormThumbnailModel()
        {
            _setNotifier = NotificationHub.Default.GetNotifier(this,
                ID.FromType<FormNotificationKeys.ThumbnailSet>());
        }

        public void SetHeader(string headerText, Color? indicatorColor)
        {
            HeaderText = headerText ?? string.Empty;
            IndicatorColor = indicatorColor ?? Color.white;
            _setNotifier[FormNotificationKeys.ThumbnailSet.HeaderText] = HeaderText;
            _setNotifier[FormNotificationKeys.ThumbnailSet.IndicatorColor] = IndicatorColor;
            _setNotifier.Notify();
        }

        public void SetIsLoading(bool isLoading)
        {
            IsLoading = isLoading;
            _setNotifier[FormNotificationKeys.ThumbnailSet.IsLoading] = IsLoading;
            _setNotifier.Notify();
        }
    }
}