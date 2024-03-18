using System.Collections;
using umi3dBrowsers;
using umi3dBrowsers.container;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LanguageHandler : MonoBehaviour
{
    public static SupportedLanguages currentLanguage { get;  set; }


    private void Awake()
    {
        currentLanguage = SupportedLanguages.English;
    }


    public void ChangeLocale()
    {
        int localeID;
        switch (currentLanguage)
        {
            case SupportedLanguages.English:
                localeID = 0;
                break;
            case SupportedLanguages.French:
                localeID = 1;
                break;
            case SupportedLanguages.German:
                localeID = 2;
                break;
            case SupportedLanguages.Japanese:
                localeID = 3;
                break;
            case SupportedLanguages.Korean:
                localeID = 4;
                break;
            case SupportedLanguages.Chinese:
                localeID = 5;
                break;
            case SupportedLanguages.Italian:
                localeID = 6;
                break;
            case SupportedLanguages.Netherland:
                localeID = 7;
                break;
            case SupportedLanguages.Spanish:
                localeID = 8;
                break;
            default:
                localeID = -1;
                break;
        }

        if (localeID != -1)
        {
            StartCoroutine(SetLocale(localeID));
        }
        else
        {
            Debug.LogError("Unsupported language selected.");
        }
    }

    private IEnumerator SetLocale(int _localeID)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
    }
}

