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
using TMPro;
using umi3d.browserRuntime.thumbnails;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace umi3d.browserRuntime.portalsThumbnails
{
    internal class PortalThumbnailNameView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private Image _icon;
        [SerializeField] private Image _background;

        private ThumbnailModelContainer _modelContainer;
        private PortalThumbnailModelContainer _portalModelContainer;

        bool _isHovering = false;
        Color _tempColor = Color.white;

        private void Awake()
        {
            _modelContainer = GetComponentInParent<ThumbnailModelContainer>();
            _portalModelContainer = GetComponentInParent<PortalThumbnailModelContainer>();

            _inputField.onEndEdit.AddListener(Submit);

            NotificationHub.Default.Subscribe(this,
                ID.FromType<ThumbnailNotificationKeys.ThumbnailSet>(),
                (Callback)ThumbnailSet,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _modelContainer.Model));
            NotificationHub.Default.Subscribe(this,
                ID.FromType<ThumbnailNotificationKeys.ThumbnailUpdated>(),
                (Callback)ThumbnailUpdated,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _modelContainer.Model));
            NotificationHub.Default.Subscribe(this,
                ID.FromType<PortalThumbnailNotificationKeys.PortalThumbnailSet>(),
                (Callback)PortalThumbnailSet,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _portalModelContainer.Model));

            _icon.gameObject.SetActive(false);
            _background.color = new Color(0, 0, 0, 0);
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            gameObject.SetActive(false);
            _inputField.text = string.Empty;
        }

        private void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
            _inputField.onSubmit.RemoveListener(Submit);
        }

        void ThumbnailSet(Notification notification)
        {
            if (notification.TryGetInfoT(ThumbnailNotificationKeys.ThumbnailSet.Name, out string name, false))
                _inputField.text = name;
            if (notification.TryGetInfoT(ThumbnailNotificationKeys.ThumbnailSet.Color, out Color color, false) && color != new Color(0, 0, 0, 0))
            {
                _tempColor = color;
                if (!_inputField.isFocused)
                    _inputField.textComponent.color = color;
            }
        }

        void ThumbnailUpdated(Notification notification)
        {
            if (notification.TryGetInfoT(ThumbnailNotificationKeys.ThumbnailUpdated.Name, out string name, false))
                _inputField.text = name;
            if (notification.TryGetInfoT(ThumbnailNotificationKeys.ThumbnailUpdated.Color, out Color color, false) && color != new Color(0, 0, 0, 0))
            {
                _tempColor = color;
                if (!_inputField.isFocused)
                    _inputField.textComponent.color = color;
            }
        }

        private void PortalThumbnailSet()
        {
            gameObject.SetActive(_portalModelContainer.Model.Portal != null);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovering = true;

            _icon.gameObject.SetActive(true);
            _background.color = new Color(1, 1, 1, 1);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovering = false;
            if (_inputField.isFocused)
                return;

            _icon.gameObject.SetActive(false);
            _background.color = new Color(0, 0, 0, 0);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _inputField.Select();
        }

        private void Submit(string newName)
        {
            _modelContainer.Model.UpdateName(newName);
            _inputField.textComponent.color = _tempColor;
            if (!_isHovering)
            {
                _icon.gameObject.SetActive(false);
                _background.color = new Color(0, 0, 0, 0);
            }
        }
    }
}