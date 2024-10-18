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

using inetum.unityUtils;
using System.Collections.Generic;
using umi3d.cdk.collaboration.emotes;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.inGame.emote
{
    public class EmotePanel : MonoBehaviour
    {
        [SerializeField] private List<EmoteElement> elements;
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;

        /// <summary>
        /// List of page containing emotes. <see cref="buttonCount"/> is the number of emote in one page.
        /// </summary>
        private List<List<Emote>> lstEmotes;
        private int currentEmotePage;

        private int buttonCount => elements.Count;
        private int pageCount => lstEmotes?.Count ?? 0;

        private void Awake()
        {
            lstEmotes = new List<List<Emote>>();
            UpdateEmotePage();

            leftButton.onClick.AddListener(() => {
                if (currentEmotePage <= 0)
                    return;
                currentEmotePage--;
                UpdateEmotePage();
            });

            leftButton.onClick.AddListener(() => {
                if (currentEmotePage >= lstEmotes.Count - 1)
                    return;
                currentEmotePage++;
                UpdateEmotePage();
            });

            EmoteManager.Instance.EmotesLoaded += Setup;
            EmoteManager.Instance.EmoteEnded += CloseEmoteMenu;

            NotificationHub.Default.Subscribe(this, EmoteNotificationKeys.Open, Open);
            NotificationHub.Default.Subscribe(this, EmoteNotificationKeys.Close, Close);

            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            EmoteManager.Instance.EmotesLoaded -= Setup;
            EmoteManager.Instance.EmoteEnded -= CloseEmoteMenu;

            NotificationHub.Default.Unsubscribe(this);
        }

        private void CloseEmoteMenu(umi3d.cdk.collaboration.emotes.Emote emote)
        {
            NotificationHub.Default.Notify(this, EmoteNotificationKeys.Close);
        }

        private void Open()
        {
            gameObject.SetActive(true);
        }

        private void Close()
        {
            gameObject.SetActive(false);
        }

        private void Setup(IReadOnlyList<Emote> emotes)
        {
            PrepareListOfEmote(emotes);
            currentEmotePage = 0;
            UpdateEmotePage();
        }

        private void PrepareListOfEmote(IReadOnlyList<Emote> emotes)
        {
            if (emotes == null || emotes.Count == 0)
                return;

            lstEmotes ??= new();
            lstEmotes.Clear();
            for (int i = 0; i < emotes.Count; i++)
            {
                var lstIndex = i / buttonCount;
                if (lstEmotes.Count <= lstIndex)
                    lstEmotes.Add(new());

                lstEmotes[lstIndex].Add(emotes[i]);
            }
        }

        private void UpdateNavigation()
        {
            leftButton.gameObject.SetActive(currentEmotePage > 0);
            rightButton.gameObject.SetActive(currentEmotePage < pageCount - 1);
        }

        private void UpdateEmotePage()
        {
            UpdateNavigation();
            if (lstEmotes.Count <= 0)
            {
                foreach (var element in elements)
                    element.Reset();
                return;
            }

            var emotes = lstEmotes[currentEmotePage];
            for (int i = 0; i < buttonCount; i++)
            {
                if (i < emotes.Count)
                    elements[i].Set(emotes[i]);
                else
                    elements[i].Reset();
            }
        }
    }
}