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

namespace umi3dBrowsers.displayer.ingame
{
    /// <summary>
    /// Notification Key used for the Ui in game (Inside an environment)
    /// </summary>
    public static class UiInGameNotificationKeys
    {
        /// <summary>
        /// Show the tablet with the social panel already opened.
        /// </summary>
        public static readonly string ShowSocialPanel = "show-social-panel";

        /// <summary>
        /// Mute the user
        /// </summary>
        public static readonly string Mute = "own-mute";

        /// <summary>
        /// Unmute the user
        /// </summary>
        public static readonly string Unmute = "own-unmute";

        /// <summary>
        /// Deafen the user
        /// </summary>
        public static readonly string Deafen = "own-deafen";

        /// <summary>
        /// Undeafen the user
        /// </summary>
        public static readonly string Undeafen = "own-undeafen";

        /// <summary>
        /// Show the Emote Panel
        /// </summary>
        public static readonly string ShownEmotePanel = "show-emote-panel";
    }
}