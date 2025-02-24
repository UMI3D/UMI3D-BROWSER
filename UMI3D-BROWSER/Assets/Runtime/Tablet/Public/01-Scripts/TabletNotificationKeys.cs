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

namespace umi3d.browserRuntime.ui.tablet
{
    public static class TabletNotificationKeys
    {
        public static readonly string PlayHoverSound = "tablet-playSound-hover";
        public static readonly string PlayClickSound = "tablet-playSound-click";

        public class TabletSet
        {
            public static readonly string Menu = "Menu";
        }

        public class TabletUpdate
        {
            public static readonly string Menu = "Menu";
        }

        public class Open { }
        public class Opened { }
        public class Close { }
        public class Closed { }
        public class Toggle { }

        public class NewScreenSelected { }
    }
}
