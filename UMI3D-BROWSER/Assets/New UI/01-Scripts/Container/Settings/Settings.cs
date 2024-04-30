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
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using umi3d.cdk.collaboration;
using umi3dBrowsers.containere;
using umi3dBrowsers.displayer;
using UnityEngine;

namespace umi3dBrowsers.container
{
    [Serializable]
    public class settingPanel
    {
        [SerializeField] private GameObject panel;
        public GameObject Panel => panel;
        [SerializeField] private string panelName;
        public string PanelName => panelName;
    }

    public class Settings : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] private UnityEngine.Localization.Locale selectedLanguage;
        [SerializeField] private AvailibleThemes selectedTheme;
        [SerializeField] private GeneralSettingsContainer generalSettingsContainer;

        [Header("Audio")]
        [SerializeField] private float environmentVolume;
        [SerializeField] private float conversationVolume;
        [SerializeField] private bool isMicOn;
        [SerializeField] private AudioSettingsContainer audioSettingsContainer;

        [Header("Graphics")]
        [SerializeField] private QualityLevel quality;
        [SerializeField] private GraphicsSettings graphicsSettingsContainer;

        [Header("Comfort")]
        [SerializeField] private bool isFadingWhenTP;
        [SerializeField] private bool isFadingWhenReorient;
        [SerializeField] private float fadingSpeedWhenReorient;
        [SerializeField] private ComfortSettings comfortSettingsContainer;

        [Header("Navigation")]
        [SerializeField] private TabManager tabManager;
        [SerializeField] private int startPanelIndex;
        [Space]
        [SerializeField] private List<settingPanel> settingPanels = new();
        [SerializeField] private bool useLocalization;

        private void Awake()
        {
            generalSettingsContainer.OnLanguageChanged += (language) => this.selectedLanguage = language;
            generalSettingsContainer.OnThemeChanged += (theme) => this.selectedTheme = theme;

            audioSettingsContainer.OnConversationVolumenChanged += (value) => { this.conversationVolume = value; AudioMixerControl.SetVolume(AudioMixerControl.Group.Conversation, value); };
            audioSettingsContainer.OnEnvironmentVolumeChanged += (value) => { this.environmentVolume = value; AudioMixerControl.SetVolume(AudioMixerControl.Group.Environment, value); };
            audioSettingsContainer.OnIsMicOnChanged += (value) => { this.isMicOn = value; MicrophoneListener.mute = !value; };

            graphicsSettingsContainer.OnQualityLevelChange += (value) => quality = value;

            comfortSettingsContainer.OnTpFadingChanged += (value) => isFadingWhenTP = value;
            comfortSettingsContainer.OnRiorientFadingChanged += (value) => isFadingWhenReorient = value;
            comfortSettingsContainer.OnFadingValueChanged += (value) => fadingSpeedWhenReorient = value;

            SetTabs();
        }

        private void SetTabs()
        {
            for(int i = 0; i<settingPanels.Count; i++)
            {
                tabManager.AddNewTab(settingPanels[i].PanelName, useLocalization, settingPanels[i].Panel);
            }

            tabManager.InitSelectedButtonById();
        }
    }
}

