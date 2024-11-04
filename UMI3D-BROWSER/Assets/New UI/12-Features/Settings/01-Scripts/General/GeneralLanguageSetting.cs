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
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace umi3d.browserRuntime.ui.settings
{
    [RequireComponent(typeof(SettingsDropdownControl))]
    internal class GeneralLanguageSetting : MonoBehaviour
    {
        SettingsDropdownControl dropdownControl;

        Locale selectedLanguage;
        List<Locale> languages;
        
        GeneralSettings generalSettings;

        void Awake()
        {
            dropdownControl = GetComponent<SettingsDropdownControl>();

            generalSettings = GetComponentInParent<GeneralSettings>();

            dropdownControl.indexToItem = index => LocalToString(languages[index]);
            dropdownControl.valueChanged += ValueChanged;
        }

        void Start()
        {
            if (!generalSettings.TryGetLocal(out selectedLanguage))
            {
                UnityEngine.Debug.LogError($"[GeneralLanguageSetting] no language saved.");
                selectedLanguage = LocalizationSettings.SelectedLocale;
            }
            LocalizationSettings.SelectedLocale = selectedLanguage;
            languages = LocalizationSettings.AvailableLocales.Locales;

            dropdownControl.selectedIndex = languages.IndexOf(selectedLanguage);
            dropdownControl.optionsCount = languages.Count;
            dropdownControl.SetOptions();
        }

        string LocalToString(Locale locale)
        {
            return Unity.VisualScripting.StringUtility.FirstCharacterToUpper(locale.Identifier.CultureInfo.NativeName);
        }

        void ValueChanged(int index)
        {
            selectedLanguage = languages[index];
            LocalizationSettings.SelectedLocale = selectedLanguage;
            generalSettings.model.selectedLanguage = selectedLanguage.LocaleName;
        }
    }
}