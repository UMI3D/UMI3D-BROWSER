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
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.windowBar
{
    public class MinimizeMaximizeApplicationButton : MonoBehaviour
    {
        [SerializeField] private Button button;

        private Notifier notifier;

        private void Awake()
        {
            notifier = NotificationHub.Default.GetNotifier(this, WindowsManagerNotificationKey.FullScreenModeWillChange);
            notifier[WindowsManagerNotificationKey.FullScreenModeChangedInfo.Mode] = FullScreenMode.Windowed;

            button.onClick.AddListener(MinimizeOrMaximize);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(MinimizeOrMaximize);
        }

        private void MinimizeOrMaximize()
        {
            notifier.Notify();
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
