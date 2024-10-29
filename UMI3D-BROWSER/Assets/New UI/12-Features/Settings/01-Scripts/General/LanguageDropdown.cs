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

using System.Collections.Generic;
using umi3dBrowsers.displayer;
using umi3dBrowsers.services.connection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace umi3d.browserRuntime.ui.settings.general
{
    public class LanguageDropdown : MonoBehaviour
    {
        [SerializeField] private GridDropDown languageDropdown;

        private Locale selectedLanguage;

        private void Start()
        {
            selectedLanguage = LocalizationSettings.SelectedLocale;

            List<GridDropDownItemCell> languages = new();

            var i = 0;
            var indexCurrent = 0;
            LocalizationSettings.AvailableLocales.Locales.ForEach(language => {
                GridDropDownItemCell cell = new(language.Identifier.CultureInfo.NativeName.FirstCharacterToUpper());
                languages.Add(cell);
                if (language == selectedLanguage)
                    indexCurrent = i;

                i++;
            });

            languageDropdown.Init(languages, indexCurrent);

            languageDropdown.OnClick += () => {

                selectedLanguage = LocalizationSettings.AvailableLocales.Locales.Find(language => language.Identifier.CultureInfo.NativeName.FirstCharacterToUpper() == languageDropdown.GetValue());
                LocalizationSettings.SelectedLocale = selectedLanguage;
                PlayerPrefsManager.SaveLocalisationSet(selectedLanguage);
            };
        }
    }
}