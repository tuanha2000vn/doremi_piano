using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Plugins;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject ButtonPlay;
    public GameObject ButtonLearn;
    public GameObject ButtonPractice;
    public GameObject ButtonPracticeA;
    public Sprite A;
    public Sprite AFill;
    public Sprite B;
    public Sprite BFill;
    public GameObject ButtonPracticeB;
    public GameObject ButtonInstrumentPlay;
    public GameObject ButtonInstrumentLeft;
    public GameObject ButtonToStart;
    public GameObject ButtonPlayPause;
    public Sprite PlayIcon;
    public Sprite PauseIcon;
    public GameObject ButtonInstrumentRight;
    public GameObject ButtonInstrumentVoice;
    public GameObject ButtonZoomCircle;
    public GameObject ButtonPlayTransposeCircle;
    public GameObject ButtonLearnMetronomeCircle;
    public GameObject ButtonLearnTransposeCircle;
    public GameObject ButtonSetting;
    public GameObject PanelZoom;
    public GameObject PanelPlayTranspose;
    public GameObject PanelLearnMetronome;
    public GameObject PanelLearnTranspose;
    public GameObject PanelMeasure;
    public GameObject PanelPractice;
    public GameObject PanelInstrument;
    public GameObject PanelSetting;
    public GameObject KeyboardType88;
    public GameObject KeyboardType76;
    public GameObject KeyboardType61;
    public GameObject KeyboardType49;

    private List<GameObject> SubPanelList;
    private List<GameObject> SubBarList;
    private GameObject MainContainer;
    private GameObject noteHolder;

    private IEnumerator animatePanelCoroutine;
    private IEnumerator animatePanelMeasureCoroutine;

    //private IEnumerator AdjustNotePositionCoroutine;

    // Use this for initialization
    void Start()
    {
        if (LocalizationManager.instance == null)
        {
            Debug.Log("LocalizationManager.instance == null");
            SceneManager.LoadScene(0);
            return;
        }

        if (!Helpers.ConnectMidiOnStart)
        {
            StartCoroutine(ConnectMidiOnStart());
            Helpers.ConnectMidiOnStart = true;
        }

        //Move from PanelSetting here because it disabled on start
        var notePlayStop = GameObject.Find("Keyboard");
        if (notePlayStop != null)
        {
            notePlayStop.GetComponent<NotePlayStop>().ClearAll();
        }
        //else
        //{
        //    Debug.LogWarning($"notePlayStop == null");
        //}

        noteHolder = GameObject.Find("NoteHolder");
        MainContainer = GameObject.Find("MainContainer");
        SubPanelList = new List<GameObject>
        {
            PanelInstrument,
            PanelPractice,
            PanelSetting,
        };

        SubBarList = new List<GameObject>
        {
            PanelZoom,
            PanelPlayTranspose,
            PanelLearnMetronome,
            PanelLearnTranspose,
        };

        //Helpers.PlayTranspose
        Helpers.PlayTranspose = 0;
        PanelPlayTranspose.GetComponentInChildren<Slider>().value = Helpers.PlayTranspose;
        PanelPlayTranspose.transform.GetChild(0).GetChild(0).GetChild(2).GetChild(1).GetComponent<Text>().text =
            " " + PlusNumber(Helpers.PlayTranspose);
        //SetCircleRotation(PanelPlayTranspose.GetComponentInChildren<Slider>(), ButtonPlayTransposeCircle);

        //Helpers.MetronomeSpeed
        Helpers.MetronomeSpeed = 10;
        PanelLearnMetronome.GetComponentInChildren<Slider>().value = Helpers.MetronomeSpeed;
        PanelLearnMetronome.transform.GetChild(0).GetChild(0).GetChild(2).GetChild(1).GetComponent<Text>().text =
            " " + Helpers.MetronomeSpeed * 10 + "%";
        //SetCircleRotation(PanelLearnMetronome.GetComponentInChildren<Slider>(), ButtonLearnMetronomeCircle);

        //Helpers.LearnTranspose
        Helpers.LearnTranspose = 0;
        PanelLearnTranspose.GetComponentInChildren<Slider>().value = Helpers.LearnTranspose;
        PanelLearnTranspose.transform.GetChild(0).GetChild(0).GetChild(2).GetChild(1).GetComponent<Text>().text =
            " " + PlusNumber(Helpers.PlayTranspose);
        //SetCircleRotation(PanelLearnTranspose.GetComponentInChildren<Slider>(), ButtonPlayTransposeCircle);
        ButtonPlay.transform.GetChild(1).GetChild(1).GetComponent<Image>().enabled = false;
        ButtonLearn.transform.GetChild(1).GetChild(1).GetComponent<Image>().enabled = false;
        ButtonPractice.transform.GetChild(1).GetComponent<Image>().sprite =
            Resources.Load("Sprites\\" + Helpers.PracticeMode, typeof(Sprite)) as Sprite;

        ButtonInstrumentPlay.SetActive(false);
        ButtonInstrumentLeft.SetActive(false);
        ButtonPractice.SetActive(false);
        ButtonPracticeA.SetActive(false);
        Helpers.PracticeAPos = 0f;
        ButtonPracticeB.SetActive(false);
        Helpers.PracticeBPos = 0f;
        ButtonToStart.SetActive(false);
        ButtonPlayPause.SetActive(false);
        ButtonInstrumentRight.SetActive(false);
        ButtonInstrumentVoice.SetActive(false);

        ButtonZoomCircle.SetActive(false);
        ButtonPlayTransposeCircle.SetActive(false);
        ButtonLearnMetronomeCircle.SetActive(false);
        ButtonLearnTransposeCircle.SetActive(false);

        PanelZoom.SetActive(false);
        PanelPlayTranspose.SetActive(false);
        PanelLearnMetronome.SetActive(false);
        PanelLearnTranspose.SetActive(false);
        PanelInstrument.SetActive(false);
        PanelMeasure.SetActive(false);
        PanelPractice.SetActive(false);
        PanelSetting.SetActive(false);

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            ButtonPlay.transform.GetChild(1).GetChild(1).GetComponent<Image>().enabled = true;
            ButtonInstrumentPlay.SetActive(true);
            ButtonZoomCircle.SetActive(true);
            //var slider = SliderZoom.GetComponent<Slider>();
            //slider.minValue = Helpers.ScaleMin;
            //slider.maxValue = Helpers.ScaleMax;
            //slider.value = Helpers.ScaleMiddle;
            var slider = PanelZoom.GetComponentInChildren<Slider>();
            slider.minValue = Helpers.ScaleMin;
            slider.maxValue = Helpers.ScaleMax;
            slider.value = Helpers.ScaleMiddle;

            ButtonPlayTransposeCircle.SetActive(true);
        }

        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            ButtonLearn.transform.GetChild(1).GetChild(1).GetComponent<Image>().enabled = true;
        }

        if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            ButtonLearn.transform.GetChild(1).GetChild(1).GetComponent<Image>().enabled = true;
            ButtonInstrumentLeft.SetActive(true);
            ButtonPractice.SetActive(true);
            ButtonPracticeA.SetActive(true);
            ButtonPracticeB.SetActive(true);
            ButtonToStart.SetActive(true);
            ButtonPlayPause.SetActive(true);
            ButtonInstrumentRight.SetActive(true);
            //ButtonInstrumentVoice.SetActive(true);
            ButtonLearnMetronomeCircle.SetActive(true);
            ButtonLearnTransposeCircle.SetActive(true);
            PanelMeasure.SetActive(true);
        }

        PanelInstrument.GetComponent<PanelInstrumentSelect>().SetButtonInstrumentColor("MainMenu : Start");

        KeyboardTypeSet();
    }

    public void PlayModeClicked()
    {
        Debug.Log("PlayModeClicked");
        HideSubPanel();

        if (SceneManager.GetActiveScene().buildIndex != 1)
        {
            var panelLoadingIndicator = GameObject.Find("PanelIndicator").GetComponent<PanelIndicator>();
            if (LocalizationManager.instance != null)
            {
                panelLoadingIndicator.LoadLevel(1,
                    LocalizationManager.instance.GetLocalizedValue("Loading") + " " +
                    LocalizationManager.instance.GetLocalizedValue("Play"));
            }

            //SceneManager.LoadScene("01-Play");
        }
    }

    public void LearnModeClicked()
    {
        Debug.Log("LearnModeClicked");
        HideSubPanel();

        if (SceneManager.GetActiveScene().buildIndex != 2)
        {
            var panelLoadingIndicator = GameObject.Find("PanelIndicator").GetComponent<PanelIndicator>();
            if (LocalizationManager.instance != null)
            {
                panelLoadingIndicator.LoadLevel(2,
                    LocalizationManager.instance.GetLocalizedValue("Loading song list") + "...");
            }
        }
    }

    public void ToStartClicked()
    {
        Debug.Log("ToStartClicked");
        var noteFlow = GameObject.Find("NoteFlow");
        noteFlow.GetComponent<NoteFlowControl>().NoteFlowToStart(1);
        Helpers.ListPracticePause.Clear();
        Helpers.ListPracticePreAdd.Clear();
        var notePlayStop = GameObject.Find("Keyboard");
        if (notePlayStop != null)
        {
            notePlayStop.GetComponent<NotePlayStop>().ClearAll();
        }
        else
        {
            //Debug.LogWarning($"notePlayStop == null");
        }

        //HideSubPanel();
    }

    public void PlayPauseClicked()
    {
        Debug.Log("PlayPauseClicked Helpers.IsPlaying " + Helpers.IsPlaying);
        if (Helpers.IsPlaying)
        {
            StopPlaying();
        }
        else
        {
            StartPlaying();
        }

        //HideSubPanel();
    }

    public void StopPlaying()
    {
        if (ButtonPlayPause != null)
        {
            ButtonPlayPause.GetComponent<Image>().sprite = PlayIcon;
        }

        Helpers.IsPlaying = false;
        MidiOut.AllSoundOff();
    }

    public void StartPlaying()
    {
        ButtonPlayPause.GetComponent<Image>().sprite = PauseIcon;
        Helpers.IsPlaying = true;
    }

    public void PlayTransposeClicked()
    {
        Debug.Log("PlayTransposeClicked");
        HideSubPanel();

        if (PanelPlayTranspose.activeSelf)
        {
            HideBar();
            Debug.Log("PanelPlayTranspose.activeSelf " + PanelPlayTranspose.activeSelf);
            PanelPlayTranspose.SetActive(false);
        }
        else
        {
            HideBar();
            PanelPlayTranspose.SetActive(true);
            if (animatePanelCoroutine != null)
            {
                StopCoroutine(animatePanelCoroutine);
            }

            animatePanelCoroutine = AnimatePanel(PanelPlayTranspose);
            StartCoroutine(animatePanelCoroutine);
        }
    }

    public void PlayTransposeChanged()
    {
        Debug.Log("PlayTransposeChanged " + PanelPlayTranspose.GetComponentInChildren<Slider>().value);
        Helpers.PlayTranspose = (int) PanelPlayTranspose.GetComponentInChildren<Slider>().value;
        PanelPlayTranspose.transform.GetChild(0).GetChild(0).GetChild(2).GetChild(1).GetComponent<Text>().text =
            " " + PlusNumber(Helpers.PlayTranspose);
        SetCircleRotation(PanelPlayTranspose.GetComponentInChildren<Slider>(), ButtonPlayTransposeCircle);
    }

    private string PlusNumber(int number)
    {
        if (number > 0)
        {
            return "+" + number;
        }

        return number.ToString();
    }

    public void ZoomClicked()
    {
        Debug.Log("ZoomClicked");
        HideSubPanel();

        if (PanelZoom.activeSelf)
        {
            HideBar();
            Debug.Log("PanelZoom.activeSelf " + PanelZoom.activeSelf);
            PanelZoom.SetActive(false);
            Debug.Log("PanelZoom.activeSelf " + PanelZoom.activeSelf);
        }
        else
        {
            HideBar();
            PanelZoom.SetActive(true);
            if (animatePanelCoroutine != null)
            {
                StopCoroutine(animatePanelCoroutine);
            }

            animatePanelCoroutine = AnimatePanel(PanelZoom);
            StartCoroutine(animatePanelCoroutine);
        }
    }

    public void ZoomChanged()
    {
        Debug.Log("ZoomChanged " + PanelZoom.GetComponentInChildren<Slider>().value);
        var value = PanelZoom.GetComponentInChildren<Slider>().value;
        MainContainer.transform.localScale = new Vector3(value, value, 1);
        MainContainer.GetComponent<MainContainer>().KeyboardClamEdge();
        SetCircleRotation(PanelZoom.GetComponentInChildren<Slider>(), ButtonZoomCircle);
    }

    public void LearnMetronomeClicked()
    {
        Debug.Log("LearnMetronomeClicked");
        HideSubPanel();

        if (PanelLearnMetronome.activeSelf)
        {
            HideBar();
            PanelLearnMetronome.SetActive(false);
            PanelMeasureSpace(true);
        }
        else
        {
            HideBar();
            PanelLearnMetronome.SetActive(true);
            PanelMeasureSpace(false);

            if (animatePanelCoroutine != null)
            {
                StopCoroutine(animatePanelCoroutine);
            }

            animatePanelCoroutine = AnimatePanel(PanelLearnMetronome);
            StartCoroutine(animatePanelCoroutine);
        }
    }

    public void LearnMetronomeChanged()
    {
        Debug.Log("LearnMetronomeChanged " + PanelLearnMetronome.GetComponentInChildren<Slider>().value);
        Helpers.MetronomeSpeed = (int) PanelLearnMetronome.GetComponentInChildren<Slider>().value;
        PanelLearnMetronome.transform.GetChild(0).GetChild(0).GetChild(2).GetChild(1).GetComponent<Text>().text =
            " " + Helpers.MetronomeSpeed * 10 + "%";
        SetCircleRotation(PanelLearnMetronome.GetComponentInChildren<Slider>(), ButtonLearnMetronomeCircle);
    }

    public void LearnTransposeClicked()
    {
        Debug.Log("LearnTransposeClicked");
        HideSubPanel();

        if (PanelLearnTranspose.activeSelf)
        {
            HideBar();
            PanelLearnTranspose.SetActive(false);
            PanelMeasureSpace(true);
        }
        else
        {
            HideBar();
            PanelLearnTranspose.SetActive(true);
            PanelMeasureSpace(false);

            if (animatePanelCoroutine != null)
            {
                StopCoroutine(animatePanelCoroutine);
            }

            animatePanelCoroutine = AnimatePanel(PanelLearnTranspose);
            StartCoroutine(animatePanelCoroutine);
        }
    }

    public void LearnTransposeChanged()
    {
        Debug.Log("LearnTransposeChanged " + PanelLearnTranspose.GetComponentInChildren<Slider>().value);
        Helpers.LearnTranspose = (int) PanelLearnTranspose.GetComponentInChildren<Slider>().value;
        SetCircleRotation(PanelLearnTranspose.GetComponentInChildren<Slider>(), ButtonLearnTransposeCircle);
        PanelLearnTranspose.transform.GetChild(0).GetChild(0).GetChild(2).GetChild(1).GetComponent<Text>().text =
            " " + PlusNumber(Helpers.LearnTranspose);

        StopPlaying();
        AdjustNotePosition();
        Helpers.ListPracticePause.Clear();
        Helpers.ListPracticePreAdd.Clear();
        //if (AdjustNotePositionCoroutine != null)
        //{
        //    StopCoroutine(AdjustNotePositionCoroutine);
        //}

        //AdjustNotePositionCoroutine = AdjustNotePosition();
        //StartCoroutine(AdjustNotePositionCoroutine);
        var notePlayStop = GameObject.Find("Keyboard");
        if (notePlayStop != null)
        {
            notePlayStop.GetComponent<NotePlayStop>().ClearAll();
        }

        //else
        //{
        //    //Debug.LogWarning($"notePlayStop == null");
        //}
    }

    private void AdjustNotePosition()
    {
        foreach (Transform child in noteHolder.transform)
        {
            var noteAnimation = child.GetComponent<NoteAnimation>();
            if (noteAnimation != null)
            {
                int noteValue = noteAnimation.NoteIndex + Helpers.LearnTranspose;
                noteValue = Mathf.Clamp(noteValue, 0, 87);
                var postX = Helpers.KeysDict[noteValue].transform.localPosition.x;
                var postY = child.transform.localPosition.y;
                //Debug.Log("Last value for " + childName + " is " + noteValue +
                //          " Old X " + child.transform.localPosition.x +
                //          " New X " + postX);
                child.transform.localPosition = new Vector3(postX, postY, 0);
                var duration = child.GetComponent<NoteAnimation>().height;
                child.GetComponent<NoteAnimation>().SetNoteSize(noteValue, duration);
            }
        }
    }

    public void HideBar()
    {
        foreach (var bar in SubBarList)
        {
            bar.SetActive(false);
        }
    }

    public void HideSubPanel()
    {
        foreach (var panel in SubPanelList)
        {
            var anim = panel.GetComponent<Animator>();
            if (anim != null
                && panel.activeSelf)
            {
                anim.SetBool("Show", false);
            }
        }
    }

    public void SetBoxUp(bool up)
    {
        var animButtonPractice = ButtonPractice.GetComponent<Animator>();
        var animButtonInstrumentPlay = ButtonInstrumentPlay.GetComponent<Animator>();
        var animButtonInstrumentLeft = ButtonInstrumentLeft.GetComponent<Animator>();
        var animButtonInstrumentRight = ButtonInstrumentRight.GetComponent<Animator>();
        var animButtonInstrumentVoice = ButtonInstrumentVoice.GetComponent<Animator>();
        var animButtonSetting = ButtonSetting.GetComponent<Animator>();

        if (up)
        {
            if (ButtonPractice.activeSelf) animButtonPractice.SetBool("BoxUp", true);
            if (ButtonInstrumentPlay.activeSelf) animButtonInstrumentPlay.SetBool("BoxUp", true);
            if (ButtonInstrumentLeft.activeSelf) animButtonInstrumentLeft.SetBool("BoxUp", true);
            if (ButtonInstrumentRight.activeSelf) animButtonInstrumentRight.SetBool("BoxUp", true);
            if (ButtonInstrumentVoice.activeSelf) animButtonInstrumentVoice.SetBool("BoxUp", true);
            if (ButtonSetting.activeSelf) animButtonSetting.SetBool("BoxUp", true);
        }
        else
        {
            if (ButtonPractice.activeSelf) animButtonPractice.SetBool("BoxUp", true);
            if (ButtonInstrumentPlay.activeSelf) animButtonInstrumentPlay.SetBool("BoxUp", false);
            if (ButtonInstrumentLeft.activeSelf) animButtonInstrumentLeft.SetBool("BoxUp", false);
            if (ButtonInstrumentRight.activeSelf) animButtonInstrumentRight.SetBool("BoxUp", false);
            if (ButtonInstrumentVoice.activeSelf) animButtonInstrumentVoice.SetBool("BoxUp", false);
            if (ButtonSetting.activeSelf) animButtonSetting.SetBool("BoxUp", false);
        }
    }

    public void PointerUp()
    {
        Debug.Log("PointerUp");
        GameObject myEventSystem = GameObject.Find("EventSystem");
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
    }

    IEnumerator AnimatePanel(GameObject panel)
    {
        //var panelRect = panel.GetComponent<RectTransform>();
        //Debug.Log("panel.offsetMin.x " + panelRect.offsetMin.x + " " + panelRect.offsetMin.y);
        //Debug.Log("panel.offsetMax.x " + panelRect.offsetMax.x + " " + panelRect.offsetMax.y);

        panel.transform.localScale = new Vector3(0, 1, 1);
        while (panel.transform.localScale.x < 1)
        {
            var newValue = panel.transform.localScale.x + 0.1f;
            newValue = Mathf.Min(newValue, 1);
            panel.transform.localScale = new Vector3(newValue, 1, 1);
            yield return new WaitForSeconds(0.01f);
        }
    }

    private void PanelMeasureSpace(bool big)
    {
        if (PanelMeasure != null)
        {
            if (animatePanelMeasureCoroutine != null)
            {
                StopCoroutine(animatePanelMeasureCoroutine);
            }

            animatePanelMeasureCoroutine = AnimatePanelMeasure(big);
            StartCoroutine(animatePanelMeasureCoroutine);

            //var PanelMeasureRect = PanelMeasure.GetComponent<RectTransform>();
            //if (big)
            //{
            //    PanelMeasureRect.offsetMax = new Vector2(-30, -100); //right-top
            //}
            //else
            //{
            //    PanelMeasureRect.offsetMax = new Vector2(-590, -100); //right-top
            //}
        }
    }

    IEnumerator AnimatePanelMeasure(bool big)
    {
        var panelMeasureRect = PanelMeasure.GetComponent<RectTransform>();
        //Debug.Log("PanelMeasureRect.offsetMax.x " + PanelMeasureRect.offsetMax.x + " " + PanelMeasureRect.offsetMax.y);
        if (big)
        {
            while (panelMeasureRect.offsetMax.x < -30)
            {
                var newValue = Mathf.Min(panelMeasureRect.offsetMax.x + 50f, -30);
                panelMeasureRect.offsetMax = new Vector2(newValue, -100);
                yield return new WaitForSeconds(0.01f);
            }
        }
        else
        {
            while (panelMeasureRect.offsetMax.x > -610)
            {
                var newValue = Mathf.Max(panelMeasureRect.offsetMax.x - 100f, -610);
                panelMeasureRect.offsetMax = new Vector2(newValue, -100);
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    public void ButtonInstrumentSelectModeClick(string instrumentSelectMode)
    {
        if (!PanelInstrument.activeSelf)
        {
            PanelInstrument.SetActive(true);
        }

        var animPanelInstrument = PanelInstrument.GetComponent<Animator>();
        //Debug.Log($"ButtonInstrumentSelectModeClick {instrumentSelectMode} {animPanelInstrument.GetBool("Show")}");

        if ((instrumentSelectMode == "Close"
             || Helpers.InstrumentSelectMode == instrumentSelectMode)
            && animPanelInstrument.GetBool("Show"))
        {
            HideSubPanel();
            SetBoxUp(true);
        }
        else
        {
            Helpers.InstrumentSelectMode = instrumentSelectMode;
            HideSubPanel();
            SetBoxUp(true);
            animPanelInstrument.SetBool("Show", true);

            var animButtonInstrumentPlay = ButtonInstrumentPlay.GetComponent<Animator>();
            var animButtonInstrumentLeft = ButtonInstrumentLeft.GetComponent<Animator>();
            var animButtonInstrumentRight = ButtonInstrumentRight.GetComponent<Animator>();
            var animButtonInstrumentVoice = ButtonInstrumentVoice.GetComponent<Animator>();

            if (instrumentSelectMode == "Select Play Instrument")
            {
                animButtonInstrumentPlay.SetBool("BoxUp", false);
            }

            if (instrumentSelectMode == "Select Left Hand Instrument")
            {
                animButtonInstrumentLeft.SetBool("BoxUp", false);
            }

            if (instrumentSelectMode == "Select Right Hand Instrument")
            {
                animButtonInstrumentRight.SetBool("BoxUp", false);
            }

            if (instrumentSelectMode == "Select Melody Instrument")
            {
                animButtonInstrumentVoice.SetBool("BoxUp", false);
            }
        }


        PanelInstrument.GetComponent<PanelInstrumentSelect>()
            .SetButtonInstrumentColor("MainMenu : ButtonInstrumentSelectModeClick");
    }

    public void PracticeClicked()
    {
        if (!PanelPractice.activeSelf)
        {
            PanelPractice.SetActive(true);
        }

        var animButtonPractice = ButtonPractice.GetComponent<Animator>();
        var animPanelPractice = PanelPractice.GetComponent<Animator>();

        //Debug.Log($"PracticeClicked {animButtonPractice.GetBool("BoxUp")}");

        if (animPanelPractice.GetBool("Show"))
        {
            HideSubPanel();
            SetBoxUp(true);
        }
        else
        {
            HideSubPanel();
            SetBoxUp(true);
            animPanelPractice.SetBool("Show", true);
            animButtonPractice.SetBool("BoxUp", false);
        }
    }

    public void SettingClicked()
    {
        Debug.Log("SettingClicked");

        if (!PanelSetting.activeSelf)
        {
            PanelSetting.SetActive(true);
        }

        var animButtonSetting = ButtonSetting.GetComponent<Animator>();
        var animPanelSetting = PanelSetting.GetComponent<Animator>();

        if (animPanelSetting.GetBool("Show"))
        {
            HideSubPanel();
            SetBoxUp(true);
        }
        else
        {
            HideSubPanel();
            SetBoxUp(true);
            animPanelSetting.SetBool("Show", true);
            animButtonSetting.SetBool("BoxUp", false);
        }
    }


    public void PracticeAClicked()
    {
        var aPoint = GameObject.Find("A-Point").GetComponent<SpriteRenderer>();
        var bPoint = GameObject.Find("B-Point").GetComponent<SpriteRenderer>();
        var aPointR = GameObject.Find("A-PointR").GetComponent<SpriteRenderer>();
        var bPointR = GameObject.Find("B-PointR").GetComponent<SpriteRenderer>();

        if (ButtonPracticeA.GetComponent<Image>().sprite == AFill)
        {
            Helpers.PracticeAPos = 0f;
            ButtonPracticeA.GetComponent<Image>().sprite = A;
            aPoint.color = Helpers.ColorWhite00;
            aPoint.transform.localPosition = new Vector3(-42, 0);
            aPointR.color = Helpers.ColorWhite00;
            aPointR.transform.localPosition = new Vector3(42, 0);
            ABLinkSetup(aPoint, bPoint);
            return;
        }

        var noteFlow = GameObject.Find("NoteFlow");
        Helpers.PracticeAPos = noteFlow.transform.localPosition.y;
        ButtonPracticeA.GetComponent<Image>().sprite = AFill;
        aPoint.color = Helpers.ColorWhite05;
        aPoint.transform.localPosition = new Vector3(-42, 0 - Helpers.PracticeAPos + 10);
        aPointR.color = Helpers.ColorWhite05;
        aPointR.transform.localPosition = new Vector3(42, 0 - Helpers.PracticeAPos + 10);

        if (Helpers.PracticeAPos - 15 < Helpers.PracticeBPos)
        {
            ButtonPracticeB.GetComponent<Image>().sprite = B;
            bPoint.color = Helpers.ColorWhite00;
            bPointR.color = Helpers.ColorWhite00;
            Helpers.PracticeBPos = 0f;
            bPoint.transform.localPosition = new Vector3(-42, 0);
            bPointR.transform.localPosition = new Vector3(42, 0);
            Helpers.PracticeRepeatTimeOut = DateTime.Now;
        }
        else
        {
            Helpers.PracticeRepeatTimeOut = DateTime.Now - TimeSpan.FromSeconds(3);
        }

        //Debug.Log($"PracticeAClicked {Helpers.PracticeAPos} {Helpers.PracticeBPos}");
        ABLinkSetup(aPoint, bPoint);
        HideSubPanel();
    }

    public void PracticeBClicked()
    {
        var aPoint = GameObject.Find("A-Point").GetComponent<SpriteRenderer>();
        var bPoint = GameObject.Find("B-Point").GetComponent<SpriteRenderer>();

        var aPointR = GameObject.Find("A-PointR").GetComponent<SpriteRenderer>();
        var bPointR = GameObject.Find("B-PointR").GetComponent<SpriteRenderer>();

        if (ButtonPracticeB.GetComponent<Image>().sprite == BFill)
        {
            Helpers.PracticeBPos = 0f;
            ButtonPracticeB.GetComponent<Image>().sprite = B;
            bPoint.color = Helpers.ColorWhite00;
            bPoint.transform.localPosition = new Vector3(-42, 0);
            bPointR.color = Helpers.ColorWhite00;
            bPointR.transform.localPosition = new Vector3(42, 0);
            ABLinkSetup(aPoint, bPoint);
            return;
        }

        var noteFlow = GameObject.Find("NoteFlow");
        Helpers.PracticeBPos = noteFlow.transform.localPosition.y;
        ButtonPracticeB.GetComponent<Image>().sprite = BFill;
        bPoint.color = Helpers.ColorWhite05;
        bPoint.transform.localPosition = new Vector3(-42, 0 - Helpers.PracticeBPos + 10);
        bPointR.color = Helpers.ColorWhite05;
        bPointR.transform.localPosition = new Vector3(42, 0 - Helpers.PracticeBPos + 10);

        if (Helpers.PracticeAPos - 15 < Helpers.PracticeBPos)
        {
            ButtonPracticeA.GetComponent<Image>().sprite = A;
            aPoint.color = Helpers.ColorWhite00;
            aPointR.color = Helpers.ColorWhite00;
            Helpers.PracticeAPos = 0f;
            aPoint.transform.localPosition = new Vector3(-42, 0);
            aPointR.transform.localPosition = new Vector3(42, 0);
            Helpers.PracticeRepeatTimeOut = DateTime.Now;
        }
        else
        {
            Helpers.PracticeRepeatTimeOut = DateTime.Now - TimeSpan.FromSeconds(3);
        }

        //Debug.Log($"PracticeBClicked {Helpers.PracticeAPos} {Helpers.PracticeBPos}");
        ABLinkSetup(aPoint, bPoint);
        HideSubPanel();
    }

    void ABLinkSetup(SpriteRenderer aPoint, SpriteRenderer bPoint)
    {
        var aBLink = GameObject.Find("A-B-Link").GetComponent<SpriteRenderer>();
        var aBLinkR = GameObject.Find("A-B-LinkR").GetComponent<SpriteRenderer>();
        if (aPoint.color == Helpers.ColorWhite05
            && bPoint.color == Helpers.ColorWhite05)
        {
            aBLink.transform.localPosition =
                new Vector3(aPoint.transform.localPosition.x, aPoint.transform.localPosition.y + 1.5f);
            aBLinkR.transform.localPosition =
                new Vector3(Mathf.Abs(aPoint.transform.localPosition.x), aPoint.transform.localPosition.y + 1.5f);
            var sizeY = bPoint.transform.localPosition.y - aPoint.transform.localPosition.y;
            //aBLink.size = new Vector2(0.5f, aPoint.transform.localPosition.y - bPoint.transform.localPosition.y);
            aBLink.size = new Vector2(0.2f, sizeY - 3f);
            aBLink.color = Helpers.ColorWhite05;
            aBLinkR.size = new Vector2(0.2f, sizeY - 3f);
            aBLinkR.color = Helpers.ColorWhite05;
            //Debug.Log($"ABLinkSetup {sizeY} {aBLink.size}");
        }
        else
        {
            aBLink.color = Helpers.ColorWhite00;
            aBLinkR.color = Helpers.ColorWhite00;
        }
    }

    IEnumerator ConnectMidiOnStart()
    {
        yield return new WaitUntil(() =>
            MIDI.initialized
        );

        try
        {
            for (int i = 0; i < MidiOUTPlugin.GetDeviceCount(); i++)
            {
                var deviceName = MidiOUTPlugin.GetDeviceName(i);
                MidiOUTPlugin.DisconnectDeviceByName(deviceName);
                //Debug.Log($"DisconnectMidiOut {deviceName}");
            }

            for (int i = 0; i < MidiINPlugin.GetDeviceCount(); i++)
            {
                var deviceName = MidiINPlugin.GetDeviceName(i);
                if (!deviceName.Contains("Session"))
                {
                    MidiINPlugin.ConnectDeviceByName(deviceName);
                    //Debug.Log($"ConnectMidiIn {deviceName}");
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("ConnectMidiOnStart " + e);
        }
    }

    public void KeyboardTypeClicked(int keys)
    {
        Debug.Log("KeyboardTypeClicked " + keys);

        if (keys != PlayerPrefs.GetInt("KeyboardType"))
        {
            PlayerPrefs.SetInt("KeyboardType", keys);
            KeyboardTypeSet();
            //HideSubPanel();
            //SetBoxUp(true);
        }
    }

    void KeyboardTypeSet()
    {
        if (Helpers.KeyboardType == 88)
        {
            KeyboardType88.GetComponent<Image>().color = Helpers.ColorWhite10;
        }
        else
        {
            KeyboardType88.GetComponent<Image>().color = Helpers.ColorWhite05;
        }

        if (Helpers.KeyboardType == 76)
        {
            KeyboardType76.GetComponent<Image>().color = Helpers.ColorWhite10;
        }
        else
        {
            KeyboardType76.GetComponent<Image>().color = Helpers.ColorWhite05;
        }

        if (Helpers.KeyboardType == 61)
        {
            KeyboardType61.GetComponent<Image>().color = Helpers.ColorWhite10;
        }
        else
        {
            KeyboardType61.GetComponent<Image>().color = Helpers.ColorWhite05;
        }

        if (Helpers.KeyboardType == 49)
        {
            KeyboardType49.GetComponent<Image>().color = Helpers.ColorWhite10;
        }
        else
        {
            KeyboardType49.GetComponent<Image>().color = Helpers.ColorWhite05;
        }

        if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            var keyboardAlpha = GameObject.Find("KeyboardAlpha");
            if (keyboardAlpha != null)
            {
                var spriteRenderer = keyboardAlpha.GetComponent<SpriteRenderer>();
                if (Helpers.KeyboardType == 49)
                {
                    spriteRenderer.sprite = Resources.Load("Sprites\\KeyboardSize\\4a", typeof(Sprite)) as Sprite;
                }
                else if (Helpers.KeyboardType == 61)
                {
                    spriteRenderer.sprite = Resources.Load("Sprites\\KeyboardSize\\3a", typeof(Sprite)) as Sprite;
                }
                else if (Helpers.KeyboardType == 76)
                {
                    spriteRenderer.sprite = Resources.Load("Sprites\\KeyboardSize\\2a", typeof(Sprite)) as Sprite;
                }
                else
                {
                    spriteRenderer.sprite = Resources.Load("Sprites\\KeyboardSize\\1a", typeof(Sprite)) as Sprite;
                }
            }
        }
    }

    void SetCircleRotation(float min, float max, float value, GameObject imageIndicator)
    {
        var valuePercent = (value - min) / (float) (max - min);
        Debug.Log("min " + min + " max " + max + " value " + value);
        Debug.Log("valuePercent " + valuePercent);
        var rotationZ = 270 * valuePercent - 135;
        rotationZ = -rotationZ;
        Debug.Log("rotationZ " + rotationZ);
        imageIndicator.transform.eulerAngles = new Vector3(0, 0, rotationZ);
    }

    void SetCircleRotation(Slider slider, GameObject parentIndicator)
    {
        var valuePercent = (slider.value - slider.minValue) / (float) (slider.maxValue - slider.minValue);
        Debug.Log("valuePercent " + valuePercent);
        var rotationZ = 270 * valuePercent - 135;
        rotationZ = -rotationZ;
        Debug.Log("rotationZ " + rotationZ);
        parentIndicator.transform.GetChild(2).transform.eulerAngles = new Vector3(0, 0, rotationZ);
    }

    public void OpenUrl(string url)
    {
        Application.OpenURL(url);
    }
}