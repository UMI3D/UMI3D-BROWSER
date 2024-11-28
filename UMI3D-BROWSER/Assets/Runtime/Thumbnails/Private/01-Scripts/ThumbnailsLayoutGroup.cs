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
    [RequireComponent(typeof(GridLayoutGroup))]
    internal class ThumbnailsLayoutGroup : MonoBehaviour
    {
        GridLayoutGroup gridLayoutGroup;

        ThumbnailsModelContainer model;

        void Awake()
        {
            gridLayoutGroup = GetComponent<GridLayoutGroup>();

            model = GetComponentInParent<ThumbnailsModelContainer>();

            NotificationHub.Default.Subscribe<ThumbnailsNotificationKeys.GridPropertiesWillChange>(
                this,
                GridPropertiesWillChanged
            );
        }

        void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        void GridPropertiesWillChanged(Notification notification)
        {
            if (!notification.TryGetInfoT(ThumbnailsNotificationKeys.GridPropertiesWillChange.Size, out Vector2 size))
            {
                return;
            }

            if (!notification.TryGetInfoT(ThumbnailsNotificationKeys.GridPropertiesWillChange.RowCount, out int rowCount))
            {
                return;
            }

            if (!notification.TryGetInfoT(ThumbnailsNotificationKeys.GridPropertiesWillChange.Spacing, out Vector2 spacing))
            {
                return;
            }

            gridLayoutGroup.cellSize = spacing;
            gridLayoutGroup.constraintCount = rowCount;
            gridLayoutGroup.spacing = spacing;

            model.model.ResetSlider();
        }
    }
}