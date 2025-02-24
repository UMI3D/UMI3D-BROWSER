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
using umi3d.browserRuntime.ui.tablet;
using umi3d.cdk.collaboration;
using UnityEngine;

namespace umi3d.browserRuntime.ui.watch
{
    public class TabletOpenToggle : MonoBehaviour
    {
        private void Awake()
        {
            UMI3DEnvironmentClient.EnvironmentLoaded.AddListener(Show);
            UMI3DCollaborationClientServer.Instance.OnLeavingEnvironment.AddListener(Hide);

            Hide();
        }

        private void OnDestroy()
        {
            UMI3DEnvironmentClient.EnvironmentLoaded.RemoveListener(Show);
            UMI3DCollaborationClientServer.Instance.OnLeavingEnvironment.RemoveListener(Hide);
        }

        [ContextMenu("Toggle")]
        public void ToggleOpenTablet()
        {
            NotificationHub.Default.Notify(this, ID.FromType<TabletNotificationKeys.Toggle>());
        }

        void Show()
        {
            gameObject.SetActive(true);
        }

        void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}