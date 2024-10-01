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
using umi3d.browserRuntime.pc;
using UnityEngine;
using UnityEngine.UI;

namespace umi3dBrowsers.displayer.windowBar
{
    public class MinimizeMaximizeApplicationButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        private void Awake()
        {
            button.onClick.AddListener(MinimizeOrMaximize);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(MinimizeOrMaximize);
        }

        private void MinimizeOrMaximize()
        {
            NotificationHub.Default.Notify(this, WindowsManager.IsWindowZoomed ? WindowsManagerNotificationKey.Minimize : WindowsManagerNotificationKey.Maximize);
        }
    }
}
