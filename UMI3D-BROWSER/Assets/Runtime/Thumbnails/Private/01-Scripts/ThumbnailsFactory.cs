/*
Copyright 2019 - 2024 Inetum

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

using inetum.unityUtils;
using System.Collections.Generic;
using UnityEngine;

namespace umi3d.browserRuntime.ui.thumbnails
{
    internal class ThumbnailsFactory : MonoBehaviour
    {
        [SerializeField] GameObject thumbnailPrefab;

        List<ThumbnailModelContainer> activatedThumbnails = new();
        List<ThumbnailModelContainer> deactivatedThumbnails = new();

        ThumbnailsModelContainer model;

        void Awake()
        {
            model = GetComponentInParent<ThumbnailsModelContainer>();

            NotificationHub.Default.Subscribe<ThumbnailsNotificationKeys.Added>(
                this,
                Added
            );
        }

        void OnEnable()
        {
        }

        void OnDisable()
        {
        }

        void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        void Added(Notification notification)
        {
            if (!notification.TryGetInfoT(ThumbnailsNotificationKeys.Added.Thumbnail, out ThumbnailModel thumbnail))
            {
                return;
            }


        }
    }
}