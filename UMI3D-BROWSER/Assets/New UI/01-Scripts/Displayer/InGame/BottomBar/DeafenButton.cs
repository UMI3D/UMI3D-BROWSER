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

using inetum.unityUtils.audio;
using UnityEngine;
using UnityEngine.UI;

namespace umi3dBrowsers.displayer.ingame
{
    public class DeafenButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image icon;
        [SerializeField] private Sprite undeafenSprite;
        [SerializeField] private Sprite deafenSprite;

        private float _oldConversationVolume;
        private float _oldEnvironmentVolume;

        private void Awake()
        {
            _oldConversationVolume = 0;
            _oldEnvironmentVolume = 0;
            button.onClick.AddListener(Deafen);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(Deafen);
        }

        private void Deafen()
        {
            AudioMixerControl.SetVolume(AudioMixerControl.Group.Conversation, _oldConversationVolume);
            AudioMixerControl.SetVolume(AudioMixerControl.Group.Environment, _oldEnvironmentVolume);
            icon.sprite = _oldEnvironmentVolume > 0 ? undeafenSprite : deafenSprite;
            _oldConversationVolume = _oldConversationVolume == 0 ? 100 : 0;
            _oldEnvironmentVolume = _oldEnvironmentVolume == 0 ? 100 : 0;
        }
    }
}