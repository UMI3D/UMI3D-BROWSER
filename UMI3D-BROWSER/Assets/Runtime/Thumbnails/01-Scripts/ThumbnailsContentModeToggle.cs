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
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.thumbnails
{
    [RequireComponent(typeof(Button))]
    internal class ThumbnailsContentModeToggle : MonoBehaviour
    {
        Button button;

        ThumbnailsModel model;

        Notifier notifier;

        void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);

            model = GetComponentInParent<ThumbnailsModel>();

            notifier = NotificationHub.Default
                .GetNotifier<ThumbnailsNotificationKeys.ThumbnailsContentModeChanged>(this);
        }

        [ContextMenu("Toggle content mode")]
        void OnClick()
        {
            notifier[ThumbnailsNotificationKeys.ThumbnailsContentModeChanged.ContentMode] =
                model.contentMode == ThumbnailContentMode.Large
                ? ThumbnailContentMode.Small
                : ThumbnailContentMode.Large;
            notifier.Notify();
        }
    }
}