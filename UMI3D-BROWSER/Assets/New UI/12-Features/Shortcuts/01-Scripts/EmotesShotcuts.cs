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
using umi3d.browserRuntime.ui.inGame.emote;
using UnityEngine;

namespace umi3d.browserRuntime.shortcuts
{
    public class EmotesShotcuts : MonoBehaviour
    {
        private Notifier playEmoteNotifier;

        private void Awake()
        {
            playEmoteNotifier = NotificationHub.Default.GetNotifier(this, ID.FromType<EmoteNotificationKeys.Play>());
        }


        private void OnEnable()
        {
            KeyboardEmote.AddDownListener(0, Play0);
            KeyboardEmote.AddDownListener(1, Play1);
            KeyboardEmote.AddDownListener(2, Play2);
        }

        private void OnDisable()
        {
            KeyboardEmote.RemoveDownListener(0, Play0);
            KeyboardEmote.RemoveDownListener(1, Play1);
            KeyboardEmote.RemoveDownListener(2, Play2);
        }

        private void Play(int index)
        {
            if (KeyboardEmote.IsEditingTextField)
                return;
            playEmoteNotifier[EmoteNotificationKeys.Play.Id] = index;
            playEmoteNotifier.Notify();
        }

        private void Play0() => Play(0);
        private void Play1() => Play(1);
        private void Play2() => Play(2);
    }
}