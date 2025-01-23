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

namespace umi3d.browserRuntime.ui.inGame.emote
{
    public static class EmoteNotificationKeys
    {
        public class OpenMenu
        {
            /// <summary>
            /// The menu to open.
            /// </summary>
            /// <remarks>Value is <see cref="UnityEngine.MonoBehaviour"/></remarks>
            public const string Menu = "Menu";
        }

        public class CloseMenu
        {
            /// <summary>
            /// The menu to close.
            /// </summary>
            /// <remarks>Value is <see cref="UnityEngine.MonoBehaviour"/></remarks>
            public const string Menu = "Menu";
        }

        /// <summary>
        /// Play an emote.
        /// </summary>
        public class Play
        {
            /// <summary>
            /// Id of the emote to be played.
            /// </summary>
            /// <remarks><see cref="int"/></remarks>
            public static readonly string Id = "emote-play-id";
        }
    }
}