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
using umi3d.browserRuntime.thumbnails;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace umi3d.browserRuntime.portalsThumbnails
{
    [RequireComponent(typeof(Button))]
    internal class PortalThumbnailFavoriteButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] Image _icon;
        Sprite _baseSprite;
        [SerializeField] Sprite _inFavoriteSprite;
        Color _iconNormalColor;
        [SerializeField] Color _iconHoverColor;
        [SerializeField] Color _iconPressedColor;
        [SerializeField] Color _iconActiveColor;

        Button _button;
        PortalThumbnailModelContainer _portalModelContainer;
        ThumbnailModelContainer _modelContainer;

        bool _isClicking;
        bool _isHovering;
        bool _isActive;

        bool _canShow => _portalModelContainer.Model.Portal != null;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _portalModelContainer = GetComponentInParent<PortalThumbnailModelContainer>();
            _modelContainer = GetComponentInParent<ThumbnailModelContainer>();

            _baseSprite = _icon.sprite;
            _iconNormalColor = _icon.color;

            _button.onClick.AddListener(Click);

            NotificationHub.Default.Subscribe(this,
                ID.FromType<ThumbnailNotificationKeys.ThumbnailUpdated>(),
                (Callback)ThumbnailUpdated,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _modelContainer.Model));
            NotificationHub.Default.Subscribe(this,
                ID.FromType<PortalThumbnailNotificationKeys.PortalThumbnailSet>(),
                (Callback)PortalThumbnailSet,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _portalModelContainer.Model));
            NotificationHub.Default.Subscribe(this,
                ID.FromType<PortalThumbnailNotificationKeys.PortalThumbnailUpdated>(),
                (Callback)PortalThumbnailUpdated,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _portalModelContainer.Model));

            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(Click);
            NotificationHub.Default.Unsubscribe(this);
        }

        void Click()
        {
            _portalModelContainer.Model.ToggleFavorite();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovering = true;

            if (!_isClicking && !_isActive)
                _icon.color = _iconHoverColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovering = false;

            if (!_isClicking && !_isActive)
                _icon.color = _iconNormalColor;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isClicking = true;

            if (!_isActive)
                _icon.color = _iconPressedColor;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isClicking = false;

            if (!_isActive)
                _icon.color = _isHovering ? _iconHoverColor : _iconNormalColor;
        }

        private void ThumbnailUpdated(Notification notification)
        {
            if (notification.TryGetInfoT(ThumbnailNotificationKeys.ThumbnailUpdated.Hover, out bool hover, false))
                gameObject.SetActive(hover && _canShow);
        }

        private void PortalThumbnailSet(Notification notification)
        {
            if (notification.TryGetInfoT(PortalThumbnailNotificationKeys.PortalThumbnailSet.IsFavorite, out bool isFavorite, false))
            {
                _icon.sprite = isFavorite ? _inFavoriteSprite : _baseSprite;
                _icon.color = isFavorite ? _iconActiveColor : _isHovering ? _iconHoverColor : _iconNormalColor;
                _isActive = isFavorite;
            }
        }

        private void PortalThumbnailUpdated(Notification notification)
        {
            if (notification.TryGetInfoT(PortalThumbnailNotificationKeys.PortalThumbnailUpdated.IsFavorite, out bool isFavorite, false))
            {
                _icon.sprite = isFavorite ? _inFavoriteSprite : _baseSprite;
                _icon.color = isFavorite ? _iconActiveColor : _isHovering ? _iconHoverColor : _iconNormalColor;
                _isActive = isFavorite;
            }
        }
    }
}