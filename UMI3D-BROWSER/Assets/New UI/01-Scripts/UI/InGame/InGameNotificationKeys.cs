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

namespace umi3d.browserRuntime.ui.inGame
{
    /// <summary>
    /// Notification Key used for the Ui in game (Inside an environment)
    /// </summary>
    public static class InGameNotificationKeys
    {
        public static readonly string PlayHoverSound = "inGame-playSound-hover";
        public static readonly string PlayClickSound = "inGame-playSound-click";

        public static readonly string DeafenChanged = "inGame-deafen-changed";
        public static readonly string IsDeafen = "inGame-deafen-isActive";
    }
}