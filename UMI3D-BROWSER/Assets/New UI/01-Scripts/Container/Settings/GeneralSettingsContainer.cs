/*
Copyright 2019 - 2023 Inetum

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

using System;
using System.Collections.Generic;
using umi3dBrowsers.displayer;
using UnityEngine;
using System.Linq;
using inetum.unityUtils;
using UnityEngine.Localization.Settings;
using Unity.VisualScripting;
using System.Collections;
using UnityEngine.SocialPlatforms;
using umi3dBrowsers.services.connection;
using utils.tweens;

namespace umi3dBrowsers.container
{
    public class GeneralSettingsContainer : MonoBehaviour
    {
        [Header("Language")]
        [SerializeField] private UnityEngine.Localization.Locale selectedLanguage;
        [SerializeField] private GridDropDown languageDropdown;
        public event Action<UnityEngine.Localization.Locale> OnLanguageChanged;

        [Header("Theme")]
        [SerializeField] private AvailibleThemes selectedThemes;
        [SerializeField] private GridDropDown themeDropDown;
        public event Action<AvailibleThemes> OnThemeChanged;

        [Header("Animations")]
        [SerializeField] private RadioButtonGroup animationsRadio;

        private void Awake()
        {
            animationsRadio.OnSelectedButtonChanged += (btn, val) =>
            {
                UITweens.ToggleAnimation(val != 0);
            };
        }

        private void Start()
        {
            SetLanguageDropDown();
            SetThemeDropDown();
        }

        private void SetLanguageDropDown()
        {
            selectedLanguage = LocalizationSettings.SelectedLocale;

            List<GridDropDownItemCell> languages = new();

            var i = 0;
            var indexCurrent = 0;
            LocalizationSettings.AvailableLocales.Locales.ForEach(language =>
            {
                GridDropDownItemCell cell = new(language.Identifier.CultureInfo.NativeName.FirstCharacterToUpper());
                languages.Add(cell);
                if (language == selectedLanguage)
                    indexCurrent = i;

                i++;
            });

            languageDropdown.Init(languages, indexCurrent);

            languageDropdown.OnClick += () =>
            {
                
                selectedLanguage = LocalizationSettings.AvailableLocales.Locales.Find(language => language.Identifier.CultureInfo.NativeName.FirstCharacterToUpper() == languageDropdown.GetValue());
                LocalizationSettings.SelectedLocale = selectedLanguage;
                PlayerPrefsManager.SaveLocalisationSet(selectedLanguage);

                OnLanguageChanged?.Invoke(selectedLanguage);
            };
        }

        private void SetThemeDropDown()
        {
            List<GridDropDownItemCell> themes = new();
            Enum.GetNames(typeof(AvailibleThemes)).ForEach(theme =>
            {
                GridDropDownItemCell cell = new(theme);
                themes.Add(cell);
            });

            themeDropDown.Init(themes);

            themeDropDown.OnClick += () =>
            {
                selectedThemes = Enum.Parse<AvailibleThemes>(themeDropDown.GetValue());
            };
        }
    }
}
