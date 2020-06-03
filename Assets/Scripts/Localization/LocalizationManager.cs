using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager instance;
    public bool LocalizationManagerIsReady = false;
    private Dictionary<string, string> localizedText;
    private Dictionary<string, string> localizedTextAll;

    private readonly List<string> listLocalizedFile = new List<string>
    {
        "localizedText_en.json",
        "localizedText_ja.json",
        "localizedText_vi.json",
        "localizedText_zh-Hans.json",
        "localizedText_zh-Hant.json",
    };

    void Awake()
    {
        if (instance == null)
        {
            Debug.Log("LocalizationManager instance == null");
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("LocalizationManager instance != this");
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    //public void LoadLocalizedText(string fileName)
    //{
    //    localizedText = new Dictionary<string, string>();
    //    string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

    //    if (!File.Exists(filePath))
    //    {
    //        filePath = Path.Combine(Application.streamingAssetsPath, "localizedText_en.json");
    //    }


    //    string dataAsJson = File.ReadAllText(filePath);
    //    Debug.Log("dataAsJson: " + dataAsJson);

    //    LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);
    //    Debug.Log("loadedData.Items.Length: " + loadedData.Items.Length);

    //    for (int i = 0; i < loadedData.items.Length; i++)
    //    {
    //        localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
    //    }

    //    Debug.Log(filePath + " Data loaded, dictionary contains: " + localizedText.Count + " entries");

    //    LocalizationManagerIsReady = true;
    //}

    //public IEnumerator LoadLocalizedText(string fileName)
    //{
    //    localizedText = new Dictionary<string, string>();
    //    var filePath = Path.Combine(Application.streamingAssetsPath + "/", fileName);
    //    string dataAsJson;

    //    if (filePath.Contains("://") || filePath.Contains(":///"))
    //    {
    //        UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(filePath);
    //        yield return www.SendWebRequest();
    //        dataAsJson = www.downloadHandler.text;
    //    }
    //    else
    //    {
    //        dataAsJson = File.ReadAllText(filePath);
    //    }

    //    LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

    //    for (int i = 0; i < loadedData.items.Length; i++)
    //    {
    //        localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
    //    }

    //    LocalizationManagerIsReady = true;
    //}

    public IEnumerator LoadLocalizedText(string fileName)
    {
        localizedTextAll = new Dictionary<string, string>();
        foreach (var file in listLocalizedFile)
        {
            var filePath = Path.Combine(Application.streamingAssetsPath + "/", file);
            string dataAsJson;

            if (filePath.Contains("://") || filePath.Contains(":///"))
            {
                UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(filePath);
                yield return www.SendWebRequest();
                dataAsJson = www.downloadHandler.text;
            }
            else
            {
                dataAsJson = File.ReadAllText(filePath);
            }

            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

            for (int i = 0; i < loadedData.items.Length; i++)
            {
                localizedTextAll.Add(file + "_" + loadedData.items[i].key, loadedData.items[i].value);
                Debug.Log("localizedTextAll.Add " + file + "_" + loadedData.items[i].key + ", " +
                          loadedData.items[i].value);
            }
        }

        LocalizationManagerIsReady = true;
    }

    //public string GetLocalizedValue(string key)
    //{
    //    if (instance == null)
    //    {
    //        return null;
    //    }

    //    if (localizedText.ContainsKey(key))
    //    {
    //        return localizedText[key];
    //    }

    //    Debug.LogWarning("There's not entry for " + key);
    //    return null;
    //}
    public string GetLocalizedValue(string key)
    {
        if (instance == null)
        {
            return null;
        }

        key = PlayerPrefs.GetString("language") + "_" + key;
        if (localizedTextAll.ContainsKey(key))
        {
            return localizedTextAll[key];
        }

        Debug.LogWarning("There's not entry for " + key);
        return null;
    }
}