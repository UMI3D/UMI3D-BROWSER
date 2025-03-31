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
    [RequireComponent(typeof(Image)), ExecuteInEditMode]
    internal class ThumbnailImageView : MonoBehaviour
    {
        private ThumbnailModelContainer _modelContainer;
        private Image _image;

        private Sprite _defaultSprite;
        private Color _defaultColor;

        private void Awake()
        {
            _modelContainer = GetComponentInParent<ThumbnailModelContainer>();
            _image = GetComponent<Image>();

            _defaultSprite = _image.sprite;
            _defaultColor = _image.color;

            NotificationHub.Default.Subscribe(this,
                ID.FromType<ThumbnailNotificationKeys.ThumbnailSet>(),
                (Callback)ThumbnailSet,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _modelContainer.Model));
            NotificationHub.Default.Subscribe(this,
                ID.FromType<ThumbnailNotificationKeys.ThumbnailUpdated>(),
                (Callback)ThumbnailUpdated,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _modelContainer.Model));
        }

        private void OnDisable()
        {
            _image.sprite = _defaultSprite;
            _image.color = _defaultColor;
        }

        private void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        private void ThumbnailSet(Notification notification)
        {
            if (notification.TryGetInfoT(ThumbnailNotificationKeys.ThumbnailSet.Image, out Sprite sprite, false))
                _image.sprite = sprite;
            if (notification.TryGetInfoT(ThumbnailNotificationKeys.ThumbnailSet.Color, out Color color, false))
                _image.color = color;
        }

        private void ThumbnailUpdated(Notification notification)
        {
            if (notification.TryGetInfoT(ThumbnailNotificationKeys.ThumbnailUpdated.Color, out Color color, false) && color != new Color(0, 0, 0, 0))
                _image.color = color;
        }
    }
}