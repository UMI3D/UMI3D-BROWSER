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
using System.Collections.Generic;
using umi3d.cdk.collaboration.emotes;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.inGame.emote
{
    public class OpenEmoteButton : MonoBehaviour
    {
        [SerializeField] Button button;
        [SerializeField] GameObject activeBackground;

        ID openID = ID.FromType<EmoteNotificationKeys.OpenMenu>();
        ID closeID = ID.FromType<EmoteNotificationKeys.CloseMenu>();

        void Awake()
        {
            button.onClick.AddListener(ToggleEmote);
            NotificationHub.Default.Subscribe(
                this, 
                openID, 
                (Callback)ShowBackground
            );
            NotificationHub.Default.Subscribe(
                this,
                closeID, 
                (Callback)HideBackground
            );
            activeBackground.SetActive(false);

            EmoteManager.Instance.EmotesLoaded += Setup;
            gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            button.onClick.RemoveListener(ToggleEmote);
            NotificationHub.Default.Unsubscribe(this);

            EmoteManager.Instance.EmotesLoaded -= Setup;
        }

        void ToggleEmote()
        {
            NotificationHub.Default.Notify(
                this, 
                activeBackground.activeSelf 
                ? closeID
                : openID
            );
        }

        void ShowBackground()
        {
            activeBackground.SetActive(true);
        }

        void HideBackground()
        {
            activeBackground.SetActive(false);
        }

        void Setup(IReadOnlyList<Emote> list)
        {
            gameObject.SetActive(list != null && list.Count > 0);
        }
    }
}