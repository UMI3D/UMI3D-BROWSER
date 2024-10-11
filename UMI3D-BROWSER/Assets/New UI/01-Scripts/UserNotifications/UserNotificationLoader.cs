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

using System.Threading.Tasks;
using umi3d.cdk;
using umi3d.common;
using umi3dBrowsers.player;
using UnityEngine;

namespace umi3dBrowsers.displayer
{
    [CreateAssetMenu(fileName = "UserNotificationLoader", menuName = "UMI3D/User Notification Loader")]
    public class UserNotificationLoader : umi3d.cdk.NotificationLoader
    {
        public WatchNotification watchNotificationPrefab;
        public UserNotification3D notification3DPrefab;

        public event System.Action<NotificationDto> Notification2DReceived;

        public override AbstractLoader GetNotificationLoader()
        {
            return new InternalNotificationLoader(this);
        }

        public void Notify(NotificationDto dto) { Notification2DReceived?.Invoke(dto); }
    }

    public class InternalNotificationLoader : umi3d.cdk.InternalNotificationLoader
    {
        UserNotificationLoader loader;

        public InternalNotificationLoader(UserNotificationLoader loader)
        {
            this.loader = loader;
        }

        public override async Task ReadUMI3DExtension(ReadUMI3DExtensionData value)
        {
            if (value.dto is NotificationOnObjectDto notification3DDto)
            {
                var notif3d = GameObject.Instantiate(loader.notification3DPrefab);
                notif3d.SetParent(UMI3DEnvironmentLoader.GetNode(value.environmentId, notification3DDto.objectId)?.GameObject.transform);

                notif3d.Init(notification3DDto);
                UMI3DEnvironmentLoader.RegisterNodeInstance(value.environmentId, notification3DDto.id, notification3DDto, notif3d.gameObject).NotifyLoaded();
                return;
            }
            else
            {
#if UMI3D_PC
                await base.ReadUMI3DExtension(value);
                loader.Notify(value.dto as NotificationDto);
#elif UMI3D_XR
                Debug.Log("TODO : only display notification in one watch");
                var dto = value.dto as NotificationDto;
                AbstractUserNotification notification;
                foreach (WatchMenu watch in WatchMenu.instances)
                {
                    notification = GameObject.Instantiate(loader.watchNotificationPrefab);
                    notification.SetParent(watch.notificationContainer);

                    notification.Init(dto);
                    UMI3DEnvironmentLoader.RegisterNodeInstance(UMI3DGlobalID.EnvironmentId, dto.id, dto, notification.gameObject);
                }
#endif
            }
        }
    }
}
