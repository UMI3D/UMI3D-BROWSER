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
using UnityEngine;

namespace umi3d.browserRuntime.thumbnails
{
    [RequireComponent(typeof(TMP_Text)), ExecuteInEditMode]
    internal class ThumbnailNameView : MonoBehaviour
    {
        private ThumbnailModelContainer _modelContainer;
        private TMP_Text _text;

        private void Awake()
        {
            _modelContainer = GetComponentInParent<ThumbnailModelContainer>();
            _text = GetComponent<TMP_Text>();

            NotificationHub.Default.Subscribe(this,
                ID.FromType<ThumbnailNotificationKeys.ThumbnailSet>(),
                (Callback)ThumbnailSet,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _modelContainer.Model));
            NotificationHub.Default.Subscribe(this,
                ID.FromType<ThumbnailNotificationKeys.ThumbnailUpdated>(),
                (Callback)ThumbnailUpdated,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _modelContainer.Model));
        }

        private void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        private void ThumbnailSet(Notification notification)
        {
            if (notification.TryGetInfoT(ThumbnailNotificationKeys.ThumbnailSet.Name, out string name, false))
                _text.text = name;
            if (notification.TryGetInfoT(ThumbnailNotificationKeys.ThumbnailSet.Color, out Color color, false))
                _text.color = color;
        }

        private void ThumbnailUpdated(Notification notification)
        {
            if (notification.TryGetInfoT(ThumbnailNotificationKeys.ThumbnailUpdated.Name, out string name, false))
                _text.text = name;
            if (notification.TryGetInfoT(ThumbnailNotificationKeys.ThumbnailUpdated.Color, out Color color, false))
                _text.color = color;
        }
    }
}