using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    public string key;

    void OnEnable()
    {
        //Debug.Log("LocalizedText OnEnable " + key);
        PanelSetting.OnChangeLanguage += ChangeLanguage;
        ChangeLanguage();
    }


    void OnDisable()
    {
        //Debug.Log("LocalizedText OnDisable " + key);
        PanelSetting.OnChangeLanguage -= ChangeLanguage;
    }

    // Use this for initialization
    void Awake()
    {
        ChangeLanguage();
    }

    void ChangeLanguage()
    {
        if (LocalizationManager.instance != null)
        {
            string localizedValue = LocalizationManager.instance.GetLocalizedValue(key);
            if (!string.IsNullOrEmpty(localizedValue))
            {
                Text text = GetComponent<Text>();
                text.text = LocalizationManager.instance.GetLocalizedValue(key);
            }
        }
    }
}