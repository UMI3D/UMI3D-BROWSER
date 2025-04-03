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
using UnityEngine.UI;

namespace umi3d.browserRuntime.portalsThumbnails
{
    [RequireComponent(typeof(TMP_Text))]
    public class PortalThumbnailUrlView : MonoBehaviour
    {
        private PortalThumbnailModelContainer _modelContainer;
        private TMP_Text _text;

        private void Awake()
        {
            _modelContainer = GetComponentInParent<PortalThumbnailModelContainer>();
            _text = GetComponent<TMP_Text>();

            NotificationHub.Default.Subscribe(this,
                ID.FromType<PortalThumbnailNotificationKeys.PortalThumbnailSet>(),
                (Callback)ThumbnailSet,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _modelContainer.Model));

            transform.parent.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            _text.text = string.Empty;
            transform.parent.gameObject.SetActive(false);
        }

        private void ThumbnailSet(Notification notification)
        {
            transform.parent.gameObject.SetActive(_modelContainer.Model.Portal != null);
            if (notification.TryGetInfoT(PortalThumbnailNotificationKeys.PortalThumbnailSet.Url, out string url, false))
                _text.text = url;
        }
    }
}