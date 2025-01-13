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
using umi3d.browserRuntime.ui.inGame;
using UnityEngine;

namespace umi3d.browserRuntime.shortcuts
{
    public class DeafenShortcut : MonoBehaviour
    {
        private float m_BaseVolume = 100.0f;
        private bool IsAudioOn => AudioListener.volume > .0f;

        private void OnEnable()
        {
            KeyboardShortcut.AddDownListener(ShortcutEnum.MuteUnmuteGeneralVolume, ToggleDeafen);
        }

        private void OnDisable()
        {
            KeyboardShortcut.RemoveDownListener(ShortcutEnum.MuteUnmuteGeneralVolume, ToggleDeafen);
        }

        private void ToggleDeafen()
        {
            if (KeyboardShortcut.IsEditingTextField)
                return;

            if (AudioListener.volume > 0)
                m_BaseVolume = AudioListener.volume;

            AudioListener.volume = IsAudioOn ? .0f : m_BaseVolume;

            NotificationHub.Default.Notify(this, InGameNotificationKeys.DeafenChanged);
        }
    }
}
