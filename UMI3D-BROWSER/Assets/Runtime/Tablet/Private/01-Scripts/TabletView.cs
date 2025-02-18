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

using inetum.unityUtils.observation;
using UnityEngine;

namespace umi3d.browserRuntime.ui.tablet
{
    public class TabletView : MonoBehaviour
    {
        GameObject gameObjectToEnable;

        private void Awake()
        {
#if UMI3D_PC
            gameObjectToEnable = gameObject;
#elif UMI3D_XR
            gameObjectToEnable = GetComponentInParent<Canvas>().gameObject;
#endif
            NotificationHub.Default.Subscribe(this, 
                ID.FromType<TabletNotificationKeys.Open>(),
                (Callback)Open
            );
            NotificationHub.Default.Subscribe(this,
                ID.FromType<TabletNotificationKeys.Close>(),
                (Callback)Close
            );
            NotificationHub.Default.Subscribe(this,
                ID.FromType<TabletNotificationKeys.Toggle>(),
                (Callback)Toggle
            );
            gameObjectToEnable.SetActive(false);
        }

        private void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        private void Open()
        {
            if (gameObjectToEnable.activeSelf)
                return;
            gameObjectToEnable.SetActive(true);
            NotificationHub.Default.Notify(this, ID.FromType<TabletNotificationKeys.Opened>());
        }

        private void Close()
        {
            if (!gameObjectToEnable.activeSelf)
                return;
            gameObjectToEnable.SetActive(false);
            NotificationHub.Default.Notify(this, ID.FromType<TabletNotificationKeys.Closed>());
        }

        private void Toggle()
        {
            if (gameObjectToEnable.activeSelf)
            {
                gameObjectToEnable.SetActive(false);
                NotificationHub.Default.Notify(this, ID.FromType<TabletNotificationKeys.Closed>());
            } else
            {
                gameObjectToEnable.SetActive(true);
                NotificationHub.Default.Notify(this, ID.FromType<TabletNotificationKeys.Opened>());
            }
        }
    }
}

