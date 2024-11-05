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
using umi3dBrowsers.services.connection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.settings
{
    [RequireComponent(typeof(Button))]
    public class SettingsLanguageControl : MonoBehaviour
    {
        [Serializable]
        public struct LanguageParams
        {
            [SerializeField] public UnityEngine.Localization.Locale SupportedLanguages;
            [SerializeField] public Sprite languageFlag;
        }

        public event Action OnClick;
        public LanguageParams Params;

        Button button;
        Image background;
        Image flag;
        TMPro.TMP_Text isoCode;
        TMPro.TMP_Text language;

        void Awake()
        {
            button = GetComponent<Button>();
            Image[] images = GetComponentsInChildren<Image>();
            background = images[0];
            flag = images[1];
            TMPro.TMP_Text[] texts = GetComponentsInChildren<TMPro.TMP_Text>();
            isoCode = texts[0];
            language = texts[1];

            button.onClick.AddListener(Click);
        }

        public void Init(LanguageParams param) 
        {
            Params = param;
            flag.sprite = param.languageFlag;
            isoCode.text = param.SupportedLanguages.Identifier.Code.ToUpper();
            language.text = param.SupportedLanguages.Identifier.CultureInfo.NativeName.FirstCharacterToUpper();
        }

        public void Click()
        {
            LocalizationSettings.SelectedLocale = Params.SupportedLanguages;
            PlayerPrefsManager.SaveLocalisationSet(Params.SupportedLanguages);

            OnClick?.Invoke();
        }
    }
}

