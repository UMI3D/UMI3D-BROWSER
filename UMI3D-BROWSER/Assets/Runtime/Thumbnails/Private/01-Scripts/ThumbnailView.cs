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
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace umi3d.browserRuntime.thumbnails
{
    [RequireComponent(typeof(Button)), ExecuteInEditMode]
    internal class ThumbnailView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private ThumbnailModelContainer _modelContainer;
        private Button _button;

        private void Awake()
        {
            _modelContainer = GetComponent<ThumbnailModelContainer>();
            _button = GetComponent<Button>();

            _button.onClick.AddListener(_modelContainer.Model.Click);

            NotificationHub.Default.Subscribe(this,
                ID.FromType<ThumbnailNotificationKeys.ThumbnailSet>(),
                (Callback)ThumbnailSet,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _modelContainer.Model));
        }

        private void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
            _button.onClick.RemoveListener(_modelContainer.Model.Click);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _modelContainer.Model.UpdateHover(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _modelContainer.Model.UpdateHover(false);
        }

        private void ThumbnailSet(Notification notification)
        {
            if (notification.TryGetInfoT(ThumbnailNotificationKeys.ThumbnailSet.Position, out Vector3 position, false))
                transform.position = position;
            if (notification.TryGetInfoT(ThumbnailNotificationKeys.ThumbnailSet.Size, out Vector2 size, false))
                ((RectTransform)transform).sizeDelta = size;

            if (notification.TryGetInfoT(ThumbnailNotificationKeys.ThumbnailSet.AnchorMin, out Vector2 anchorMin, false))
                ((RectTransform)transform).anchorMin = anchorMin;
            if (notification.TryGetInfoT(ThumbnailNotificationKeys.ThumbnailSet.AnchorMax, out Vector2 anchorMax, false))
                ((RectTransform)transform).anchorMax = anchorMax;
            if (notification.TryGetInfoT(ThumbnailNotificationKeys.ThumbnailSet.Pivot, out Vector2 pivot, false))
                ((RectTransform)transform).pivot = pivot;
        }
    }
}