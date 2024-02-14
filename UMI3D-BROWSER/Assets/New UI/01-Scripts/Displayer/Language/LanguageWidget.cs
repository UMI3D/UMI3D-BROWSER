using inetum.unityUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

        [SerializeField, ReadOnly] private SupportedLanguages selectedLanguage = SupportedLanguages.French;
        [SerializeField] private GameObject languagePrefab;

        public UnityEvent<SupportedLanguages> OnSupportedLanguageValidated;

        private void Awake()
        {
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
                    });
                };

                currentField.OnDisabled += () =>
                {
                    if (currentField.Params.SupportedLanguages == selectedLanguage)
                        selectedLanguage = SupportedLanguages.None;
                };
            }
        }

        public void ValidateSelection()
        {
            if (selectedLanguage == SupportedLanguages.None)
                return;
            OnSupportedLanguageValidated?.Invoke(selectedLanguage);
        }
    }
}


