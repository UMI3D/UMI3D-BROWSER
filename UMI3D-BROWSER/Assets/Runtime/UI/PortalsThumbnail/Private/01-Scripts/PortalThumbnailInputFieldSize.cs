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

namespace umi3d
{
    public class PortalThumbnailInputFieldSize : MonoBehaviour
    {
        [SerializeField] private float _maxSize;
        [SerializeField] private float _offset;

        private TMP_InputField _inputField;
        private ThumbnailListModelContainer _modelContainer;

        private void Awake()
        {
            _inputField = GetComponent<TMP_InputField>();
            _modelContainer = GetComponentInParent<ThumbnailListModelContainer>();
        }

        void Start()
        {
            UpdateSize();

            NotificationHub.Default.Subscribe(this,
                ID.FromType<ThumbnailNotificationKeys.ChangeMode>(),
                (Callback)ChangeMode,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _modelContainer.Model));

            _inputField.onValueChanged.AddListener(OnValueChanged);
        }

        private void ChangeMode(Notification notification)
        {
            if (!notification.TryGetInfoT(ThumbnailNotificationKeys.ChangeMode.Mode, out ThumbnailMode mode))
                return;

            var viewSize = ((RectTransform)transform.parent.parent.parent.parent.parent).sizeDelta;
            _maxSize = (viewSize.x - (mode.Spacing * (mode.NbrColumn - 1))) / mode.NbrColumn - 47;
            Debug.Log(_maxSize);

            UpdateSize();
        }

        private void OnValueChanged(string newValue)
        {
            UpdateSize();
        }

        [ContextMenu("Update Size")]
        private void UpdateSize()
        {
            var preferredWidth = _inputField.preferredWidth + _offset;
            var correctWidth = Mathf.Min(preferredWidth, _maxSize);

            var inputFieldRectTransform = _inputField.transform as RectTransform;
            inputFieldRectTransform.sizeDelta = new Vector2(correctWidth, inputFieldRectTransform.sizeDelta.y);
        }
    }
}