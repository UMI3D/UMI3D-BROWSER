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
    [RequireComponent(typeof(TMP_InputField))]
    internal class PortalThumbnailNameView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image _icon;

        private ThumbnailModelContainer _modelContainer;
        private PortalThumbnailModelContainer _portalModelContainer;
        private TMP_InputField _inputField;
        private TMP_Text _text;

        private void Awake()
        {
            _modelContainer = GetComponentInParent<ThumbnailModelContainer>();
            _portalModelContainer = GetComponentInParent<PortalThumbnailModelContainer>();
            _inputField = GetComponent<TMP_InputField>();

            _inputField.onSubmit.AddListener(_modelContainer.Model.UpdateName);

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
            _inputField.onSubmit.RemoveListener(_modelContainer.Model.UpdateName);
        }

        void ThumbnailSet(Notification notification)
        {
            if (notification.TryGetInfoT(ThumbnailNotificationKeys.ThumbnailSet.Name, out string name, false))
                _inputField.text = name;
            if (notification.TryGetInfoT(ThumbnailNotificationKeys.ThumbnailSet.Color, out Color color, false) && color != new Color(0, 0, 0, 0))
                _inputField.textComponent.color = color;
        }

        void ThumbnailUpdated(Notification notification)
        {
            if (notification.TryGetInfoT(ThumbnailNotificationKeys.ThumbnailSet.Name, out string name, false))
                _inputField.text = name;
            if (notification.TryGetInfoT(ThumbnailNotificationKeys.ThumbnailSet.Color, out Color color, false) && color != new Color(0, 0, 0, 0))
                _inputField.textComponent.color = color;
        }

        private void PortalThumbnailSet()
        {
            gameObject.SetActive(_portalModelContainer.Model.Portal != null);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _icon.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _icon.gameObject.SetActive(false);
        }
    }
}