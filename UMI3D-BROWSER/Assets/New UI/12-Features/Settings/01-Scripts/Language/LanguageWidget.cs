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

using umi3d.browserRuntime.conditionalCompilation;
using umi3dBrowsers.data.ui;
using umi3dBrowsers.linker.ui;
using umi3dBrowsers.services.connection;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace umi3d.browserRuntime.ui.settings
{
    public class LanguageWidget : MonoBehaviour
    {
        [SerializeField] SettingsLanguageControl.LanguageParams[] languageParams;

        [SerializeField] private GameObject languagePrefab;
        [SerializeField] private MenuNavigationLinker menuNavigationLinker;
        [SerializeField] private MultiDeviceReference<PanelData> nextPanel;

        void OnEnable()
        {
            if (PlayerPrefsManager.GetLocalisationLocal() != null && !menuNavigationLinker.ForceLanguage)
                menuNavigationLinker.ShowPanel(nextPanel.Reference);

            LocalizationSettings.InitializationOperation.Completed += (operation) => {
                var local = PlayerPrefsManager.GetLocalisationLocal() ?? LocalizationSettings.ProjectLocale;
                LocalizationSettings.SelectedLocale = local;
            };
        }

        void Start()
        {
            for (int i = 0; i < languageParams.Length; i++)
            {
                GameObject languageGO = Instantiate(languagePrefab);
                languageGO.transform.SetParent(transform, false);
                SettingsLanguageControl languageControl = languageGO.GetComponent<SettingsLanguageControl>();

                var currentAsso = languageParams[i];

                languageControl.Init(currentAsso);
                languageControl.OnClick += () =>
                {
                    menuNavigationLinker.ShowPanel(nextPanel.Reference);
                };
            }
        }
    }
}


