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

namespace umi3d.browserRuntime.ui.settings
{
    public static class SettingsPlayerPrefsKeys
    {
        // Audio
        internal static readonly string Microphone = "settings-microphone";
        internal static readonly string EnvironmentVolume = "settings-volume-environement";
        internal static readonly string ConversationVolume = "settings-volume-conversation";
        internal static readonly string UseNoiseReduction = "settings-useNoiseReduction";
        internal static readonly string AudioMode = "settings-audioMode";
        internal static readonly string NoiseThreshold = "settings-noiseThreshold";
        internal static readonly string DelayBeforeMuteMic = "settings-delayBeforeMuteMic";
    }
}