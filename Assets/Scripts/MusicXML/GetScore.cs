using System;
using System.Collections;
using Assets.SimpleZip;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;
using UnityEngine.UI;

public class GetScore : MonoBehaviour
{
    // Use this for initialization
    public Slider PanelMeasureSlider;
    public GameObject MeasureHolder;
    public PanelIndicator panelIndicator;
    public NoteFlowControl noteFlowControl;
    public GameObject ButtonInstrumentVoice;
    private bool getDataCompleted = false;
    private string songPath;
    private NoteCreator noteCreator;

    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;

    void Start()
    {
        if (string.IsNullOrEmpty(Helpers.songInfoName)
            || Helpers.songInfoXmlId == -1)
        {
            if (LocalizationManager.instance != null)
            {
                panelIndicator.LoadLevel(2,
                    LocalizationManager.instance.GetLocalizedValue("Loading song list") + "...");
            }
            return;
        }

        ButtonInstrumentVoice.SetActive(false);

        if (LocalizationManager.instance != null)
        {
            panelIndicator.ShowIndicator(LocalizationManager.instance.GetLocalizedValue("Generating") +
                                         " " + Helpers.songInfoName + " - " + Helpers.songInfoComposer + "...",
                "GetScore Start");
        }

        noteCreator = GetComponent<NoteCreator>();
        Debug.Log("songInfoName " + Helpers.songInfoName + " songInfoComposer " + Helpers.songInfoComposer +
                  " songInfoXmlId " + Helpers.songInfoXmlId);
        Helpers.Score = null;


        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://doremipiano-cee8a.firebaseio.com/");

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Set a flag here indiciating that Firebase is ready to use by your
                // application.
                InitializeFirebase();
            }
            else
            {
                Debug.LogError(String.Format(
                    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });

        Invoke("GetSongDataTimeOut", 15);
    }

    void GetSongDataTimeOut()
    {
        if (!getDataCompleted)
        {
            panelIndicator.HideIndicator("GetSongDataTimeOut");
            panelIndicator.LoadWarning(
                "Error Getting Song " + Helpers.songInfoName + ", Please Check Your Internet Connection.", 10, 1);
        }
    }

    // Initialize the Firebase database:
    protected virtual void InitializeFirebase()
    {
        //FirebaseApp app = FirebaseApp.DefaultInstance;
        //// NOTE: You'll need to replace this url with your Firebase App's database
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
            .GetReference("MusicScore/" + Helpers.songInfoXmlId)
            .ValueChanged += HandleValueChanged;
    }

    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        Helpers.SongXmlCompressed = null;

        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            getDataCompleted = true;
            return;
        }


        Debug.Log("Received values for MusicScore/" + Helpers.songInfoXmlId);

        if (args.Snapshot != null && args.Snapshot.Value != null)
        {
            Helpers.SongXmlCompressed = args.Snapshot.Value.ToString();
        }

        getDataCompleted = true;
    }

    void Update()
    {
        if (getDataCompleted)
        {
            CancelInvoke("GetSongDataTimeOut");
            StartCoroutine(ProcessSongXmlCompressed());
            //ProcessSongXmlCompressed();
            getDataCompleted = false;
        }
    }

    IEnumerator ProcessSongXmlCompressed()
    {
        if (string.IsNullOrEmpty(Helpers.SongXmlCompressed))
        {
            panelIndicator.LoadWarning("Error loading " + Helpers.songInfoName + " - Can not get song data.", 10,
                2);
            Debug.LogWarning("string.IsNullOrEmpty(Helpers.SongXmlCompressed)");
            //return;
            yield break;
        }

        var songXmlUnCompressed = "";
        try
        {
            songXmlUnCompressed = Zip.Decompress(Helpers.SongXmlCompressed);
        }
        catch (Exception e)
        {
            panelIndicator.LoadWarning("Error loading " + Helpers.songInfoName + " - Can not get song data..", 10, 2);
            Debug.LogWarning(e);
            //return;
            yield break;
        }

        if (string.IsNullOrEmpty(songXmlUnCompressed))
        {
            panelIndicator.LoadWarning("Error loading " + Helpers.songInfoName + " - Can not get song data...",
                10, 2);
            Debug.LogWarning("string.IsNullOrEmpty(songXmlUnCompressed)");
            //return;
            yield break;
        }

        StartCoroutine(MusicXmlParserIe.GetScore(songXmlUnCompressed));

        while (!Helpers.GetScoreCompleted)
        {
            yield return null;
        }

        //Helpers.Score = MusicXmlParser.GetScore(songXmlUnCompressed);

        //clean up memory
        songXmlUnCompressed = "";
        bool hideIndicatorInvoked = false;

        if (Helpers.Score == null)
        {
            Debug.LogWarning("score == null");
            panelIndicator.LoadWarning("Error loading " + Helpers.songInfoName + " - Parsing Score", 10, 2);
            //return;
            yield break;
        }
        else if (Helpers.Score.Parts.Count <= 0)
        {
            Debug.LogWarning("Helpers.Score.Parts.Count <= 0");
            panelIndicator.LoadWarning("Error loading " + Helpers.songInfoName + " - No Part Found", 10, 2);
            //return;
            yield break;
        }
        else
        {
            foreach (var part in Helpers.Score.Parts)
            {
                if (part.Name == "Voice")
                {
                    ButtonInstrumentVoice.SetActive(true);
                    break;
                }
            }


            Helpers.LastMeasurePostY = 0;

            foreach (Transform child in MeasureHolder.transform)
            {
                Destroy(child.gameObject);
            }

            noteFlowControl.NoteFlowToStart(1);
            foreach (var measure in Helpers.Score.Parts[0].Measures)
            {
                //Debug.Log(measure.Number + " StartPos " + measure.StartPos + " Duration " + measure.Duration);
                noteCreator.CreateMeasure(measure);
                if (!hideIndicatorInvoked)
                {
                    Invoke("HideIndicator", 1);
                    hideIndicatorInvoked = true;
                }

                Helpers.TotalMeasure = measure.Number;
            }

            yield return null;
        }


        var lastMeasure = MeasureHolder.transform.GetChild(MeasureHolder.transform.childCount - 1);
        var measureLength =
            lastMeasure.transform.localPosition.y; // + lastMeasure.GetComponent<BoxCollider2D>().size.y;

        //var offSet = Camera.main.orthographicSize * (1 / Helpers.ScaleMin);
        PanelMeasureSlider.maxValue = Helpers.OffSet;
        PanelMeasureSlider.minValue = 0 - measureLength - Helpers.OffSet;
        PanelMeasureSlider.value = PanelMeasureSlider.maxValue;

        //Debug.LogWarning("PanelMeasureSlider.maxValue " + PanelMeasureSlider.maxValue + " minValue " +
        //                 PanelMeasureSlider.minValue);

        //var songName = GameObject.Find("SongName");
        //songName.GetComponent<Renderer>().sortingOrder = 100;
        //songName.GetComponent<TextMesh>().text = "\n" + Helpers.songInfoName + " - " + Helpers.songInfoComposer;
        var songNameTop = GameObject.Find("SongNameTop");
        songNameTop.GetComponent<Text>().text = Helpers.songInfoName + " - " + Helpers.songInfoComposer;
        songNameTop.GetComponent<Text>().text = songNameTop.GetComponent<Text>().text.TrimEnd(' ', '-');
        if (!hideIndicatorInvoked)
        {
            HideIndicator();
        }
    }

    void HideIndicator()
    {
        panelIndicator.HideIndicator("GetScore Start");
    }
}