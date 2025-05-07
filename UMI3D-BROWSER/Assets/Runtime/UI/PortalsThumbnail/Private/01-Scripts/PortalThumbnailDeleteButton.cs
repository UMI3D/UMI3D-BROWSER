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
    internal class PortalThumbnailDeleteButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] Image _icon;
        Sprite _baseSprite;
        [SerializeField] Sprite _hoverSprite;

        Button _button;
        PortalThumbnailModelContainer _portalModelContainer;
        ThumbnailModelContainer _modelContainer;

        bool _canShow => _portalModelContainer.Model.Portal != null;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _portalModelContainer = GetComponentInParent<PortalThumbnailModelContainer>();
            _modelContainer = GetComponentInParent<ThumbnailModelContainer>();

            _baseSprite = _icon.sprite;

            _button.onClick.AddListener(Click);

            NotificationHub.Default.Subscribe(this,
                ID.FromType<ThumbnailNotificationKeys.ThumbnailUpdated>(),
                (Callback)ThumbnailUpdated,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _modelContainer.Model));

            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(Click);
            NotificationHub.Default.Unsubscribe(this);
        }

        void Click()
        {
            _portalModelContainer.Model.Delete();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _icon.sprite = _hoverSprite;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _icon.sprite = _baseSprite;
        }

        private void ThumbnailUpdated(Notification notification)
        {
            if (notification.TryGetInfoT(ThumbnailNotificationKeys.ThumbnailUpdated.Hover, out bool hover, false))
                gameObject.SetActive(hover && _canShow);
        }
    }
}