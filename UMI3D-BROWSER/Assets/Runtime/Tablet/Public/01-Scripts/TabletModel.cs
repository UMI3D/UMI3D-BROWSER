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

namespace umi3d.browserRuntime.ui.tablet
{
    public class TabletModel 
    {
        public TabletMenu CurrentMenu { get; private set; } = TabletMenu.Social;

        Notifier _setNotifier;
        Notifier _updateNotifier;

        public TabletModel()
        {
            _setNotifier = NotificationHub.Default.GetNotifier(this, 
                ID.FromType<TabletNotificationKeys.TabletSet>());
            _setNotifier[TabletNotificationKeys.TabletSet.Menu] = CurrentMenu;

            _updateNotifier = NotificationHub.Default.GetNotifier(this,
                ID.FromType<TabletNotificationKeys.TabletUpdate>());
        }

        public void SetMenu(TabletMenu menu)
        {
            CurrentMenu = menu;
            _setNotifier[TabletNotificationKeys.TabletSet.Menu] = CurrentMenu;
            _setNotifier.Notify();
        }

        public void UpdateMenu(TabletMenu menu)
        {
            CurrentMenu = menu;
            _updateNotifier[TabletNotificationKeys.TabletUpdate.Menu] = CurrentMenu;
            _updateNotifier.Notify();
        }
    }
}