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
using umi3d.browserRuntime.ui.popup;

namespace umi3d.browserRuntime.portalsThumbnails
{
    public class PortalThumbnailModel 
    {
        public VirtualWorldData Portal { get; set; }
        public VirtualWorlds Portals { get; set; }

        const string LOCALIZATION_TABLE = "UMI3D_inetum";
        private readonly PopupNotifier _popupNotifier;

        private readonly Notifier _setNotifier;
        private readonly Notifier _updateNotifier;

        public PortalThumbnailModel()
        {
            _popupNotifier = new(this);
            _setNotifier = NotificationHub.Default.GetNotifier(this,
                ID.FromType<PortalThumbnailNotificationKeys.PortalThumbnailSet>());
            _updateNotifier = NotificationHub.Default.GetNotifier(this,
                ID.FromType<PortalThumbnailNotificationKeys.PortalThumbnailUpdated>());
        }

        public void SetPortal(VirtualWorldData portal, VirtualWorlds portals)
        {
            Portal = portal;
            Portals = portals;
            _setNotifier[PortalThumbnailNotificationKeys.PortalThumbnailSet.IsFavorite] = Portal.isFavorite;
            _setNotifier.Notify();
        }

        public void ToggleFavorite()
        {
            if (Portal == null)
                return;

            Portals.ToggleWorldFavorite(Portal);
            NotificationHub.Default.Notify(this,
                ID.FromType<PortalThumbnailNotificationKeys.Reset>());

            _updateNotifier[PortalThumbnailNotificationKeys.PortalThumbnailUpdated.IsFavorite] = Portal.isFavorite;
            _updateNotifier.Notify();
        }

        public void Delete(bool showPopup = true)
        {
            if (Portal == null)
                return;

            if (showPopup)
            {
                _popupNotifier
                    .enqueue
                    .SetType(PopupType.Warning)
                    .SetArguments(("worldName", Portal.worldName))
                    .SetDescription(LOCALIZATION_TABLE, "popup_deleteWorld_description")
                    .SetButtons((LOCALIZATION_TABLE, "popup_cancel"), (LOCALIZATION_TABLE, "popup_yes"))
                    .SetButtonsAction(index => {
                        if (index == 1)
                        {
                            RemovePortal();
                        }
                    })
                    .Notify();
            }
            else
            {
                RemovePortal();
            }
        }

        private void RemovePortal()
        {
            Portals.RemoveWorld(Portal);
            Portal = null;
            NotificationHub.Default.Notify(this,
                ID.FromType<PortalThumbnailNotificationKeys.Reset>());
        }
    }
}