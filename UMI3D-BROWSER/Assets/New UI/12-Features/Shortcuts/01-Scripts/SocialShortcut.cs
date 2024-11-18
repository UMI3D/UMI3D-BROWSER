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
using umi3d.baseBrowser.inputs.interactions;
using umi3d.browserRuntime.ui.inGame;
using umi3d.browserRuntime.ui.inGame.emote;
using umi3d.browserRuntime.ui.inGame.tablet;
using umi3d.cdk.collaboration;
using UnityEngine;

namespace umi3d.browserRuntime.shortcuts
{
    public class SocialShortcup : MonoBehaviour
    {
        private void OnEnable()
        {
            KeyboardShortcut.AddDownListener(ShortcutEnum.DisplayHideUsersList, OpenUserList);
        }

        private void OnDisable()
        {
            KeyboardShortcut.RemoveDownListener(ShortcutEnum.DisplayHideUsersList, OpenUserList);
        }

        private void OpenUserList()
        {
            NotificationHub.Default.Notify(this, TabletNotificationKeys.Open);
            NotificationHub.Default.Notify(this, TabletNotificationKeys.ClickButtonSocial);
        }
    }
}
