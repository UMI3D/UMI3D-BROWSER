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

namespace umi3dBrowsers.container
{
    public class GeneralSettingsContainer : MonoBehaviour
    {
        [Header("Language")]
        [SerializeField] private SupportedLanguages selectedLanguage;
        [SerializeField] private GridDropDown languageDropdown;

        [Header("Theme")]
        [SerializeField] private AvailibleThemes selectedThemes;
        [SerializeField] private GridDropDown themeDropDown;

        private void Awake()
        {
            SetLanguageDropDown();
            SetThemeDropDown();
        }

        private void SetLanguageDropDown()
        {
            List<GridDropDownItemCell> languages = new();
            Enum.GetNames(typeof(SupportedLanguages)).ForEach(language =>
            {
                GridDropDownItemCell cell = new(language);
                languages.Add(cell);
            });

            languageDropdown.Init(languages);

            languageDropdown.OnClick += () =>
            {
                selectedLanguage = Enum.Parse<SupportedLanguages>(languageDropdown.GetValue());
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
