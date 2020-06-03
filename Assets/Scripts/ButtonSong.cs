using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSong : MonoBehaviour
{
    public Button Button;
    public Text TextSongName;
    public Text TextSongDifficulty;
    public Image Stars;
    private bool HandFingerShow = false;
    private PanelIndicator panelIndicator;
    private SongInfo songInfo;

    void OnEnable()
    {
        PanelSetting.OnChangeLanguage += ChangeDifficultyText;
    }


    void OnDisable()
    {
        PanelSetting.OnChangeLanguage -= ChangeDifficultyText;
    }

    // Use this for initialization
    void Start()
    {
        panelIndicator = GameObject.Find("PanelIndicator").GetComponent<PanelIndicator>();
        Button.onClick.AddListener(HandleClick);
    }


    public void Setup(SongInfo currentSongInfo)
    {
        songInfo = currentSongInfo;
        TextSongName.text = songInfo.Name + " - " + songInfo.Composer;
#if UNITY_EDITOR
        TextSongName.text = songInfo.Enabled + ". " + songInfo.XmlId.ToString("000") + ". " + TextSongName.text;
#endif
        TextSongName.text = TextSongName.text.TrimEnd(' ', '-');
        HandFingerShow = currentSongInfo.HandFingerShow;
        Stars.GetComponent<Image>().sprite =
            Resources.Load("Sprites\\Difficulty\\D" + songInfo.Difficulty.ToString("00"), typeof(Sprite)) as Sprite;
        ChangeDifficultyText();
    }

    void ChangeDifficultyText()
    {
        string localizationKey = "";
        if (songInfo.Difficulty > 8)
        {
            localizationKey = "Diff-Hard";
        }
        else if (songInfo.Difficulty > 4)
        {
            localizationKey = "Diff-Medium";
        }
        else
        {
            localizationKey = "Diff-Easy";
        }

        string localizedValue = LocalizationManager.instance.GetLocalizedValue(localizationKey);
        if (!string.IsNullOrEmpty(localizedValue))
        {
            TextSongDifficulty.text = LocalizationManager.instance.GetLocalizedValue(localizationKey);
        }
    }

    public void HandleClick()
    {
        Helpers.songInfoName = songInfo.Name;
        Helpers.songInfoComposer = songInfo.Composer;
        Helpers.songInfoXmlId = songInfo.XmlId;
        Helpers.SongXmlCompressed = null;
        Helpers.HandFingerShow = HandFingerShow;
        panelIndicator.LoadLevel(3, "Loading song " + Helpers.songInfoName + "...");
    }
}