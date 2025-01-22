/*
Copyright 2019 - 2025 Inetum

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
using System.Collections;
using System.Collections.Generic;
using umi3d.browserRuntime.ui.inGame.emote;
using umi3d.cdk.collaboration.emotes;
using umi3dVRBrowsersBase.ui.playerMenu;
using UnityEngine;

namespace umi3d.browserRuntime.ui.watch
{
    public class EmoteButton : MonoBehaviour
    {
        [SerializeField] GameObject activeBackground;
        [SerializeField] MonoBehaviour emotePanel;
        [SerializeField] OnOffButton onOffButton;

        ID openID = ID.FromType<EmoteNotificationKeys.OpenMenu>();
        ID closeID = ID.FromType<EmoteNotificationKeys.CloseMenu>();
        Dictionary<string, object> info = new();

        void Awake()
        {
            if (EmoteManager.Exists)
            {
                Setup(EmoteManager.Instance.Emotes);
            } else
            {
                Setup(null);
            }
            EmoteManager.Instance.EmotesLoaded += Setup;

            NotificationHub.Default.Subscribe(
                this,
                openID,
                (Callback)Open
            );
        }

        public void ToggleEmote()
        {
            info[!activeBackground.activeSelf
                ? EmoteNotificationKeys.CloseMenu.Menu
                : EmoteNotificationKeys.OpenMenu.Menu
            ] = emotePanel;

            NotificationHub.Default.Notify(
                this,
                !activeBackground.activeSelf
                ? closeID
                : openID,
                info
            );
        }

        void Setup(IReadOnlyList<Emote> list)
        {
            UnityEngine.Debug.Log($"emote = {list == null}");
            gameObject.SetActive(list != null && list.Count > 0);
        }

        void Open(Notification notification)
        {
            if (notification.TryGetInfoT(EmoteNotificationKeys.OpenMenu.Menu, out MonoBehaviour menu, false))
            {
                if (menu && menu != emotePanel)
                {
                    onOffButton.Toggle(false);
                }
            }
        }
    }
}