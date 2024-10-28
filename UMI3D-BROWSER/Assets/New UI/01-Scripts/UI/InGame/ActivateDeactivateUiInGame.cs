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
using UnityEngine;

namespace umi3d.browserRuntime.ui.inGame
{
    public class ActivateDeactivateUiInGame : MonoBehaviour
    {
        private void Awake()
        {
            NotificationHub.Default.Subscribe(this, InGameNotificationKeys.EnableInGameUi, SetActive);
            gameObject.SetActive(false);
        }

        private void SetActive(Notification notification)
        {
            if (notification.TryGetInfoT<bool>(InGameNotificationKeys.IsInGameUiActive, out var active))
                gameObject.SetActive(active);
        }
    }
}
