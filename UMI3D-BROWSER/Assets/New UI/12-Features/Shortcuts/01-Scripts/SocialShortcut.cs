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
using umi3d.baseBrowser.inputs.interactions;
using umi3d.browserRuntime.ui.tablet;
using UnityEngine;

namespace umi3d.browserRuntime.shortcuts
{
    public class SocialShortcup : MonoBehaviour
    {
        Notifier _openSocialNotifier;

        private void Awake()
        {
            _openSocialNotifier = NotificationHub.Default.GetNotifier(this, ID.FromType<TabletNotificationKeys.TabletUpdate>());
            _openSocialNotifier[TabletNotificationKeys.TabletUpdate.Menu] = TabletMenu.Social;
        }

        private void OnEnable()
        {
            KeyboardShortcut.AddDownListener(ShortcutEnum.DisplayHideUsersList, OpenUserList);
        }

        private void OnDisable()
        {
            KeyboardShortcut.RemoveDownListener(ShortcutEnum.DisplayHideUsersList, OpenUserList);
        }
        private void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        private void OpenUserList()
        {
            if (KeyboardShortcut.IsEditingTextField)
                return;
            NotificationHub.Default.Notify(this, ID.FromType<TabletNotificationKeys.Open>());
            _openSocialNotifier.Notify();
        }
    }
}
