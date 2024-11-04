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

namespace umi3d.browserRuntime.notificationKeys
{
    public static class SettingsNotificationKeys
    {
        public static readonly string CloseAll = "settings-close-all";

        /// <summary>
        /// Event raised when a setting panel is selected.
        /// </summary>
        public class NewPanelSelected { }

        /// <summary>
        /// Notification sent when the quality settings have changed.
        /// </summary>
        public class QualityChanged 
        {
            /// <summary>
            /// The new quality.<br/>
            /// Value is <see cref="notificationKeys.BrowserQualitySettings"/>.
            /// </summary>
            public const string Quality = "Quality";
        }

        /// <summary>
        /// Notification sent when the microphone mode has changed.
        /// </summary>
        public class MicrophoneModeChanged
        {
            /// <summary>
            /// The new mode.<br/>
            /// Value is <see cref="umi3d.cdk.collaboration.MicrophoneMode"/>.
            /// </summary>
            public const string Mode = "Mode";
        }

        public static readonly string NewToggleCustomSelected = "settings-toggle-custom-";

        public static readonly string SetDeafenIndicator = "settings-deafen-indicator";
        public static readonly string IsDeafenIndicatorEnable = "settings-deafen-indicator-enable";
    }
}