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
using UnityEngine.UI;

namespace umi3d.browserRuntime.thumbnails
{
    [RequireComponent(typeof(GridLayoutGroup)), ExecuteInEditMode]
    internal class ThumbnailModeView : MonoBehaviour
    {
        private GridLayoutGroup _group;
        private ThumbnailListModelContainer _modelContainer;

        private void Awake()
        {
            _group = GetComponent<GridLayoutGroup>();
            _modelContainer = GetComponentInParent<ThumbnailListModelContainer>();

            NotificationHub.Default.Subscribe(this,
                ID.FromType<ThumbnailNotificationKeys.ChangeMode>(),
                (Callback)ChangeMode,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _modelContainer.Model));
        }

        private void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        private void ChangeMode(Notification notification)
        {
            if (!notification.TryGetInfoT(ThumbnailNotificationKeys.ChangeMode.Mode, out ThumbnailMode mode))
                return;

            var viewSize = ((RectTransform)transform.parent.parent).sizeDelta;
            var width = (viewSize.x - (mode.Spacing * (mode.NbrColumn - 1))) / mode.NbrColumn;
            var height = (viewSize.y - (mode.Spacing * (mode.NbrRow - 1))) / mode.NbrRow;

            _group.cellSize = new Vector2(width, height);
            _group.spacing = new Vector2(mode.Spacing, mode.Spacing);
            _group.constraintCount = mode.NbrRow;
        }
    }
}