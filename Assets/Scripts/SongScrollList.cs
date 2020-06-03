using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;
using UnityEngine.UI;

public class SongInfo
{
    public int Id;
    public string Composer;
    public int Difficulty;
    public int Enabled;
    public string Name;
    public string Tag;
    public int XmlId;
    public bool HandFingerShow = false;

    //Composer = snapChildren.Child("Composer").Value.ToString(),
    //Difficulty = (int) (long) snapChildren.Child("Difficulty").Value,
    //Enabled = (int) (long) snapChildren.Child("Enabled").Value,
    //Name = snapChildren.Child("Name").Value.ToString(),
    //Type = snapChildren.Child("Type").Value.ToString(),
    //XmlId = (int) (long) snapChildren.Child("XmlId").Value
}

public class SongScrollList : MonoBehaviour
{
    public InputField InputFieldSearch;
    public Transform content;
    public GameObject ButtonSongPrefab;
    public PanelIndicator panelIndicator;
    private List<SongInfo> listSongInfo = new List<SongInfo>();
    private List<GameObject> listSongButton = new List<GameObject>();
    private bool getDataCompleted = false;
    private IEnumerator addButton;
    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;


    // Use this for initialization
    void Start()
    {
        listSongButton = new List<GameObject>();
        addButton = null;
        //if (Application.internetReachability == NetworkReachability.NotReachable)
        //{
        //    panelIndicator.LoadWarning("Error connecting to server. Please check your internect connection.", 10, 1);
        //    Debug.Log("Error. Check internet connection!");
        //    return;
        //}

        Invoke("StartTimeOut", 15);

        if (LocalizationManager.instance != null)
        {
            panelIndicator.ShowIndicator(LocalizationManager.instance.GetLocalizedValue("Loading song list") + "...",
                "SongScrollList Start");
        }


        DisableButtons();

        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://doremipiano-cee8a.firebaseio.com/");

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Set a flag here indiciating that Firebase is ready to use by your
                // application.
                InitializeFirebase();
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    void StartTimeOut()
    {
        if (listSongInfo.Count <= 0)
        {
            panelIndicator.HideIndicator("StartTimeOut");
            panelIndicator.LoadWarning("Error Loading Song List, Please Check Your Internet Connection.", 10, 1);
        }
    }

    // Initialize the Firebase database:
    void InitializeFirebase()
    {
        //FirebaseApp app = FirebaseApp.DefaultInstance;
        ////NOTE: You'll need to replace this url with your Firebase App's database
        //// path in order for the database connection to work correctly in editor.
        //app.SetEditorDatabaseUrl("https//thpiano05.firebaseio.com/");
        //if (app.Options.DatabaseUrl != null)
        //{
        //    app.SetEditorDatabaseUrl(app.Options.DatabaseUrl);
        //}

        //FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://doremipiano-cee8a.firebaseio.com/");

        StartListener();
    }

    void StartListener()
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("SongInfo")
            .OrderByChild("Name")
            .ValueChanged += HandleValueChanged;
    }

    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        listSongInfo.Clear();

        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            getDataCompleted = true;
            return;
        }


        Debug.Log("Received values for SongInfo.");

        if (args.Snapshot != null && args.Snapshot.ChildrenCount > 0)
        {
            foreach (var childSnapshot in args.Snapshot.Children)
            {
                //if (childSnapshot.Child("Composer").Value == null
                //    //|| childSnapshot.Child("Difficulty").Value == null
                //    //|| childSnapshot.Child("Enabled").Value == null
                //    //|| childSnapshot.Child("Name").Value == null
                //    //|| childSnapshot.Child("Type").Value == null
                //    || childSnapshot.Child("XmlId").Value == null)
                //{
                //    Debug.LogWarning(
                //        "Bad data in sample.  Did you forget to call SetEditorDatabaseUrl with your project id?");
                //    //panelIndicator.LoadWarning("Error loading song list: Bad data structure", 10, 1);
                //}
                //else
                //{
                try
                {
                    var songInfo = new SongInfo
                    {
                        Id = int.Parse(childSnapshot.Key),
                        Composer = childSnapshot.Child("Composer").Value.ToString(),
                        Difficulty = int.Parse(childSnapshot.Child("Difficulty").Value.ToString()),
                        Enabled = int.Parse(childSnapshot.Child("Enabled").Value.ToString()),
                        Name = childSnapshot.Child("Name").Value.ToString(),
                        Tag = childSnapshot.Child("Tag").Value.ToString(),
                        XmlId = int.Parse(childSnapshot.Child("XmlId").Value.ToString()),
                    };

                    if (songInfo.Tag.Contains("Finger"))
                    {
                        songInfo.HandFingerShow = true;
                    }

#if !UNITY_EDITOR
                    if (songInfo.Enabled != 0)
                    {
                        listSongInfo.Add(songInfo);
                    }
#else
                    listSongInfo.Add(songInfo);
#endif

                    //Debug.Log("listSongInfo.Add(songInfo) " + songInfo.Name);
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                }

                //}
            }

            getDataCompleted = true;
        }
    }

    void OnGUI()
    {
        if (getDataCompleted)
        {
            Debug.Log("getDataCompleted " + getDataCompleted);
            RefreshDisplay();
            getDataCompleted = false;
        }
    }

    public void RefreshDisplay()
    {
        if (addButton != null)
        {
            StopCoroutine(addButton);
        }

        if (panelIndicator != null)
        {
            if (getDataCompleted
                && listSongInfo.Count <= 0)
            {
                panelIndicator.LoadWarning("Error loading song list: DatabaseError", 10, 1);
                return;
            }

            if (LocalizationManager.instance != null)
            {
                panelIndicator.ShowIndicator(
                    LocalizationManager.instance.GetLocalizedValue("Loading song list") + "...",
                    "SongScrollList RefreshDisplay");
            }

            DisableButtons();
            addButton = AddButton();
            StartCoroutine(addButton);
            panelIndicator.HideIndicator("RefreshDisplay");
        }
    }

    IEnumerator AddButton()
    {
        if (listSongInfo == null)
        {
            yield break;
        }

        List<SongInfo> songInfoFiltered = new List<SongInfo>();
        var searchText = Helpers.SearchText;

        if (string.IsNullOrEmpty(searchText))
        {
            songInfoFiltered = listSongInfo
                .Where(obj =>
                    Helpers.SongViewMode == "All"
                    || obj.Tag.Contains(Helpers.SongViewMode))
                .OrderBy(obj => obj.Name)
                .ToList();
        }
        else
        {
            searchText = RemoveDiacriticsAndLower(Helpers.SearchText);
            songInfoFiltered = listSongInfo
                .Where(obj =>
                    RemoveDiacriticsAndLower(obj.Name).Contains(searchText)
                    || RemoveDiacriticsAndLower(obj.Composer).Contains(searchText))
                .OrderBy(obj => obj.Name)
                .ToList();
        }

        for (int i = 0; i < songInfoFiltered.Count; i++)
        {
            //Set Cap 100
            //if (i >= 100)
            //{
            //    break;
            //}

            SongInfo songInfo = songInfoFiltered[i];
            GameObject newButton = GetSongButtonPool();
            newButton.transform.SetParent(content);
            newButton.transform.localScale = new Vector3(1, 1, 1);
            newButton.name = songInfo.XmlId + ". " + songInfo.Name + " - " + songInfo.Composer;
            ButtonSong buttonSong = newButton.GetComponent<ButtonSong>();
            if (buttonSong == null)
            {
                Debug.LogWarning("buttonSong == null");
            }
            else
            {
                buttonSong.Setup(songInfo);
            }
        }

        yield return null;
    }

    private void DisableButtons()
    {
        //Debug.Log("DisableButtons ");
        foreach (Transform child in content.transform)
        {
            listSongButton.Add(child.gameObject);
            child.gameObject.SetActive(false);
        }
    }

    GameObject GetSongButtonPool()
    {
        var songButton = listSongButton.FirstOrDefault(obj => !obj.activeSelf);

        if (songButton == null)
        {
            songButton = Instantiate(ButtonSongPrefab, Vector3.zero, Quaternion.identity);
            songButton.transform.SetParent(content);
            listSongButton.Add(songButton);
        }
        else
        {
            songButton.SetActive(true);
        }

        return songButton;
    }

    static string RemoveDiacriticsAndLower(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC).ToLower();
    }
}