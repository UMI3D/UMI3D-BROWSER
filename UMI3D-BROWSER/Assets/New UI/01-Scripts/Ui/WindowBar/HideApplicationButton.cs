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
using umi3d.browserRuntime.notificationKeys;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.windowBar
{
    public class HideApplicationButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        private void Awake()
        {
            button.onClick.AddListener(Hide);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(Hide);
        }

        private void Hide()
        {
            NotificationHub.Default.Notify(
                this,
                WindowsManagerNotificationKey.Minimize
            );
        }
    }
}
