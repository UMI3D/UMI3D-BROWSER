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
using System;
using System.Linq;
using umi3d.browserRuntime.thumbnails;
using UnityEngine;

namespace umi3d.browserRuntime.portalsThumbnails
{
    [RequireComponent(typeof(ThumbnailListModelContainer))]
    internal class PortalThumbnailManager : MonoBehaviour
    {
        [SerializeField] private bool _showOnlyFavorite = false;
        [SerializeField] private Sprite _defaultSprite;

        private ThumbnailListModelContainer _modelContainer;

        private Notifier _tryToConnectNotifier;

        private void Awake()
        {
            _modelContainer = GetComponent<ThumbnailListModelContainer>();

            _tryToConnectNotifier = NotificationHub.Default.GetNotifier(this,
                ID.FromType<PortalThumbnailNotificationKeys.TryToConnect>());
            NotificationHub.Default.Subscribe(this,
                ID.FromType<PortalThumbnailNotificationKeys.Reset>(),
                (Callback)ResetThumbnails);
        }

        private void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        private void OnEnable()
        {
            ResetThumbnails();
        }

        private void ResetThumbnails()
        {
            _modelContainer.Model.ClearThumbnails();
            var portals = PortalThumbnailPlayerPref.GetVirtualWorlds();

            var portalsDatas = _showOnlyFavorite ? portals.FavoriteWorlds : portals.worlds;
            portalsDatas = portalsDatas.OrderBy(portalData => new DateTime(portalData.dateLastConnection)).Reverse().ToList();
            foreach (var portalData in portalsDatas)
            {
                _modelContainer.Model.AddThumbnail(
                    portalData.worldName, 
                    _defaultSprite, 
                    () => {
                        _tryToConnectNotifier[PortalThumbnailNotificationKeys.TryToConnect.Url] = portalData.worldUrl;
                        _tryToConnectNotifier.Notify();
                        portalData.dateLastConnection = DateTime.UtcNow.ToFileTime();
                        portals.UpdateWorld(portalData);
                    }, new() {
                        NormalColor = new Color(0.44f, 0.44f, 0.44f, 1),
                        HoverColor = Color.white,
                    });
                var portalModelContainer = _modelContainer.gameObject.GetComponentsInChildren<PortalThumbnailModelContainer>().Last();
                portalModelContainer.Model.SetPortal(portalData, portals);
            }
        }
    }
}