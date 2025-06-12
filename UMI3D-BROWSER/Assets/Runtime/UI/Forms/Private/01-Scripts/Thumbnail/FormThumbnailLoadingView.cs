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

namespace umi3d.browserRuntime.forms
{
    internal class FormThumbnailLoadingView : MonoBehaviour
    {
        [SerializeField] private float _rotationSpeed = 0.5f;

        FormThumbnailModelContainer _modelContainer;

        void Awake()
        {
            _modelContainer = GetComponentInParent<FormThumbnailModelContainer>();

            NotificationHub.Default.Subscribe(this,
                ID.FromType<FormNotificationKeys.ThumbnailSet>(),
                (Callback)ThumbnailSet,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _modelContainer.Model));
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            gameObject.SetActive(false);
        }

        private void Update()
        {
            transform.Rotate(new Vector3(0, 0, 1) * _rotationSpeed);
        }

        void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        private void ThumbnailSet(Notification notification)
        {
            if (notification.TryGetInfoT(FormNotificationKeys.ThumbnailSet.IsLoading, out bool isLoading, false))
                gameObject.SetActive(isLoading);
        }
    }
}