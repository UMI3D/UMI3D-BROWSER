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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace umi3dBrowsers.container
{
    public class Settings : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] private SupportedLanguages selectedLanguage;
        [SerializeField] private AvailibleThemes selectedTheme;
        [SerializeField] private GeneralSettingsContainer generalSettingsContainer;

        [Header("Audio")]
        [SerializeField] private float environmentVolume;
        [SerializeField] private float conversationVolume;
        [SerializeField] private bool isMicOn;
        [SerializeField] private AudioSettingsContainer audioSettingsContainer;

        public List<GameObject> settingPanels = new();

        private void Awake()
        {
            settingPanels.Add(generalSettingsContainer.gameObject);
            generalSettingsContainer.OnLanguageChanged += (language) => this.selectedLanguage = language;
            generalSettingsContainer.OnThemeChanged += (theme) => this.selectedTheme = theme;

            audioSettingsContainer.OnConversationVolumenChanged += (value) => this.conversationVolume = value;
            audioSettingsContainer.OnEnvironmentVolumeChanged += (value) => this.environmentVolume = value;
            audioSettingsContainer.OnIsMicOnChanged += (value) => this.isMicOn = value;
        }

        public void OpenGeneralSettings()
        {
            foreach (GameObject panel in settingPanels)
            {
                panel.SetActive(false);
            }

            generalSettingsContainer.gameObject.SetActive(true);
        }
    }
}

