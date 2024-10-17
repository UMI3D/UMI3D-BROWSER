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
using System;
using System.Collections.Generic;
using umi3d.browserRuntime.conditionalCompilation;
using umi3dBrowsers.data.ui;
using umi3dBrowsers.linker.ui;
using umi3dBrowsers.services.connection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Settings;

namespace umi3dBrowsers.displayer
{
    public class LanguageWidget : MonoBehaviour
    {
        [Serializable]
        public class LanguageAssociation
        {
            [SerializeField] public LanguageSelectionField.LanguageParams languageParams;
            [SerializeField] public LanguageSelectionField languageSelectionField;
        }

        [SerializeField] private List<LanguageAssociation> languageAssociations = new();

        [SerializeField, ReadOnly] private UnityEngine.Localization.Locale selectedLanguage;
        [SerializeField] private GameObject languagePrefab;
        [SerializeField] private MenuNavigationLinker menuNavigationLinker;
        [SerializeField] private MultiDeviceReference<PanelData> nextPanel;

        public UnityEvent<UnityEngine.Localization.Locale> OnSupportedLanguageValidated;

        private void OnEnable()
        {
            if (PlayerPrefsManager.GetLocalisationLocal() != null && !menuNavigationLinker.ForceLanguage)
                menuNavigationLinker.ShowPanel(nextPanel.Reference);

            LocalizationSettings.InitializationOperation.Completed += (operation) => {
                var local = PlayerPrefsManager.GetLocalisationLocal() ?? LocalizationSettings.ProjectLocale;
                LocalizationSettings.SelectedLocale = local;
            };
        }

        private void Start()
        {
            selectedLanguage = LocalizationSettings.SelectedLocale;

            LanguageSelectionField baseField = null;
            for (int i = 0; i < languageAssociations.Count; i++)
            {
                LanguageAssociation currentAsso = languageAssociations[i];

                if (languageAssociations[i].languageSelectionField == null)
                {
                    languageAssociations[i].languageSelectionField = Instantiate(languagePrefab, transform).GetComponent<LanguageSelectionField>();
                }

                LanguageSelectionField currentField = languageAssociations[i].languageSelectionField;

                currentField.Init(currentAsso.languageParams);
                currentField.OnClick += () =>
                {
                    selectedLanguage = currentAsso.languageParams.SupportedLanguages;
                    languageAssociations.ForEach(language =>
                    {
                        if (language.languageSelectionField != currentField)
                            language.languageSelectionField.Disable();
                        OnSupportedLanguageValidated?.Invoke(selectedLanguage);
                        menuNavigationLinker.ShowPanel(nextPanel.Reference);
                    });
                };

                currentField.OnDisabled += () =>
                {
                    if (currentField.Params.SupportedLanguages == selectedLanguage)
                        selectedLanguage = null;
                };

                if (selectedLanguage == currentAsso.languageParams.SupportedLanguages)
                    baseField = currentField;
            }
        }
    }
}


