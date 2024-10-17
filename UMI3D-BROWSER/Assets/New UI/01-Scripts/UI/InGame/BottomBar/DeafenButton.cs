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

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.inGame.bottomBar
{
    public class DeafenButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Button button;
        [SerializeField] private Image icon;
        [SerializeField] private Sprite audioOnSprite;
        [SerializeField] private Sprite audioOffSprite;

        private float m_BaseVolume;

        private bool IsAudioOn => AudioListener.volume > .0f;
        private void Awake()
        {
            m_BaseVolume = 100.0f;
            button.onClick.AddListener(Deafen);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(Deafen);
        }

        private void Deafen()
        {
            if (AudioListener.volume > 0)
                m_BaseVolume = AudioListener.volume;

            AudioListener.volume = IsAudioOn ? .0f : m_BaseVolume;
            UpdateIcon();
        }

        private void UpdateIcon()
        {
            icon.sprite = IsAudioOn ? audioOnSprite : audioOffSprite;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            icon.sprite = !IsAudioOn ? audioOnSprite : audioOffSprite;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            EventSystem.current.SetSelectedGameObject(null);

            UpdateIcon();
        }
    }
}