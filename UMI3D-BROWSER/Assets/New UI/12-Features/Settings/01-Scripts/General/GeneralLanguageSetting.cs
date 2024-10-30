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

using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace umi3d.browserRuntime.ui.settings
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class GeneralLanguageSetting : MonoBehaviour
    {
        TMP_Dropdown dropdown;
        TMP_Text label;

        Locale selectedLanguage;
        int selectedLanguageIndex;
        List<Locale> languages;
        List<TMP_Dropdown.OptionData> options = new();

        GeneralSettings generalSettings;

        void Awake()
        {
            generalSettings = GetComponentInParent<GeneralSettings>();

            if (!generalSettings.TryGetLocal(out selectedLanguage))
            {
                UnityEngine.Debug.LogError($"[GeneralLanguageSetting] no language saved.");
                selectedLanguage = LocalizationSettings.SelectedLocale;
            }
            
            languages = LocalizationSettings.AvailableLocales.Locales;
            selectedLanguageIndex = languages.IndexOf(selectedLanguage);

            dropdown = GetComponent<TMP_Dropdown>();
            dropdown.onValueChanged.AddListener(ValueChanged);
            label = GetComponentInChildren<TMP_Text>();
            SetOptions();
        }

        string LocalToString(Locale locale)
        {
            return locale.Identifier.CultureInfo.NativeName.FirstCharacterToUpper();
        }

        void SetOptions()
        {
            for (int i = 0; i < languages.Count; i++)
            {
                if (i == selectedLanguageIndex)
                {
                    continue;
                }

                string language = LocalToString(languages[i]);
                options.Add(new(language));
            }
            dropdown.options = options;
            dropdown.SetValueWithoutNotify(-1);
            label.text = LocalToString(selectedLanguage);
        }

        void UpdateOptions()
        {
            int index = 0;
            for (int i = 0; i < options.Count; i++)
            {
                index = i < selectedLanguageIndex ? i : i + 1;

                options[i].text = LocalToString(languages[index]);
            }
            dropdown.SetValueWithoutNotify(-1);
            label.text = LocalToString(selectedLanguage);
        }

        void ValueChanged(int index)
        {
            selectedLanguageIndex = index < selectedLanguageIndex ? index : index + 1;
            selectedLanguage = languages[selectedLanguageIndex];
            LocalizationSettings.SelectedLocale = selectedLanguage;
            generalSettings.model.selectedLanguage = selectedLanguage.Identifier.Code;

            UpdateOptions();
        }
    }
}