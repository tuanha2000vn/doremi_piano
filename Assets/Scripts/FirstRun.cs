using System.Collections;
using UnityEngine;

public class FirstRun : MonoBehaviour
{
    public PanelIndicator panelIndicator;

    IEnumerator Start()
    {
        Screen.sleepTimeout = (int) SleepTimeout.NeverSleep;
        Application.targetFrameRate = 60;

        if (string.IsNullOrEmpty(PlayerPrefs.GetString("language")))
        {
            if (Application.systemLanguage == SystemLanguage.ChineseSimplified)
            {
                PlayerPrefs.SetString("language", "localizedText_zh-Hans.json");
            }
            else if (Application.systemLanguage == SystemLanguage.ChineseTraditional
                     || Application.systemLanguage == SystemLanguage.Chinese)
            {
                PlayerPrefs.SetString("language", "localizedText_zh-Hant.json");
            }
            else if (Application.systemLanguage == SystemLanguage.Japanese)
            {
                PlayerPrefs.SetString("language", "localizedText_ja.json");
            }
            else if (Application.systemLanguage == SystemLanguage.Vietnamese)
            {
                PlayerPrefs.SetString("language", "localizedText_vi.json");
            }
            else
            {
                PlayerPrefs.SetString("language", "localizedText_en.json");
            }
        }

        LocalizationManager.instance.LocalizationManagerIsReady = false;
        yield return LocalizationManager.instance.LoadLocalizedText(PlayerPrefs.GetString("language"));

        while (!LocalizationManager.instance.LocalizationManagerIsReady)
        {
            yield return null;
        }

        panelIndicator.LoadLevel(1, "Loading Play Mode...");
    }
}