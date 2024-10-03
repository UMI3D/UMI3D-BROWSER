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

using umi3d.cdk.collaboration.emotes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace umi3dBrowsers.displayer.ingame
{
    public class EmoteElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Button button;
        [SerializeField] private Image hoverBorder;
        [SerializeField] private Image icon;

        public void Set(Emote emote)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => EmoteManager.Instance.PlayEmote(emote));
            icon.sprite = emote.icon;

            button.interactable = true;
            icon.gameObject.SetActive(true);
        }

        public void Reset()
        {
            button.onClick.RemoveAllListeners();
            button.interactable = false;
            icon.gameObject.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!button.interactable)
                return;

            hoverBorder.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!button.interactable)
                return;

            hoverBorder.gameObject.SetActive(false);
        }
    }
}