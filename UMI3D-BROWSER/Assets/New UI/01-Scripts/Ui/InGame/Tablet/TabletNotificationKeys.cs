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

namespace umi3d.browserRuntime.ui.inGame.tablet
{
    public static class TabletNotificationKeys
    {
        public static readonly string Open = "tablet-open";
        public static readonly string OpenUserNotification = "tablet-open-userNotification";
        public static readonly string OpenSocial = "tablet-open-social";

        public static readonly string Close = "tablet-close";
        public static readonly string CloseScreens = "tablet-close-screens";

        public static readonly string PlayHoverSound = "tablet-playSound-hover";
        public static readonly string PlayClickSound = "tablet-playSound-click";

        public static readonly string NewScreenSelected = "tablet-newScreenSelected";

        public static readonly string UserNotificationReceived = "tablet-userNotification-received";
        public static readonly string ClickButtonSocial = "tablet-click-social";
    }
}
