using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ForieroEngine.MIDIUnified;
using UnityEngine;

public class NoteFlowControl : MonoBehaviour
{
    // Use this for initialization
    //[SerializeField] private float Speed = 14;
    public MainMenu maimMenu;
    public PanelMeasure panelMeasure;
    public NotePlayStop NotePlayStop;
    public GameObject HandFinger;
    private IEnumerator coroutine;
    private IEnumerator coroutineRepeat;

    void Start()
    {
        //transform.localPosition = new Vector3(0, Camera.main.orthographicSize * (1 / Helpers.ScaleMin), 0);
        transform.localPosition = new Vector3(0, Helpers.OffSet, 0);
        Helpers.IsPlaying = false;
        if (Helpers.HandFingerShow)
        {
            StartCoroutine(HandFingerFade(3));
        }
        else
        {
            HandFinger.SetActive(false);
        }
    }

    IEnumerator HandFingerFade(int waitSeconds)
    {
        HandFinger.SetActive(true);
        yield return new WaitForSeconds(waitSeconds);

        var sr = HandFinger.GetComponent<SpriteRenderer>();
        while (sr.color.a > 0)
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a - 0.01f);
            yield return new WaitForEndOfFrame();
        }

        HandFinger.SetActive(false);
        sr.color = new Color(1f, 1f, 1f, 1f);
    }

    IEnumerator StartPlaying(int waitTime)
    {
        maimMenu.StopPlaying();
        MidiOut.AllSoundOff();
        //transform.localPosition = new Vector3(0, Camera.main.orthographicSize * (1 / Helpers.ScaleMin), 0);
        transform.localPosition = new Vector3(0, Helpers.OffSet, 0);
        panelMeasure.UpdateSliderValue(transform.localPosition.y);
        Debug.Log("Wait for " + waitTime + " seconds");
        yield return new WaitForSeconds(waitTime);
        maimMenu.StartPlaying();
    }


    public void NoteFlowToStart(int waitTime)
    {
        Helpers.SpeedValue = Helpers.OffSet;

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        NotePlayStop.ClearAll();
        coroutine = StartPlaying(waitTime);
        StartCoroutine(coroutine);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
//#if (UNITY_EDITOR)
//        if (Input.GetKey(KeyCode.LeftShift))
//        {
//            var mouseScrollWheel = Input.GetAxis("Mouse ScrollWheel");
//            if (Math.Abs(mouseScrollWheel) > 0)
//            {
//                maimMenu.StopPlaying();
//                var scrollPos = new Vector3(0, 0 - mouseScrollWheel * Helpers.QuarterNoteLength, 0);
//                transform.Translate(scrollPos);
//            }
//        }
//#endif

        if (Helpers.IsPlaying
            && Helpers.IsPracticePlaying)
        {
            //transform.Translate(Vector2.down * Time.deltaTime * Speed * (Helpers.SpeedValue / 60f) *
            //                    Helpers.MetronomeSpeed / 10f * Helpers.ScaleMin);
            var currentSpeed = Helpers.QuarterNoteLength * (Helpers.SpeedValue / 60f) * (Helpers.MetronomeSpeed / 10f) *
                               Helpers.ScaleMin;
            //Debug.Log("currentSpeed " + currentSpeed + " Speed " + Speed + " Helpers.SpeedValue " + Helpers.SpeedValue +
            //          " MetronomeSpeed " + Helpers.MetronomeSpeed);
            transform.Translate(Vector2.down * Time.deltaTime * currentSpeed);
            //var offSet = Camera.main.orthographicSize * (1 / Helpers.ScaleMin);
            panelMeasure.UpdateSliderValue(transform.localPosition.y);

            if (Helpers.PracticeRepeatTimeOut.AddSeconds(3) < DateTime.Now
                && Helpers.PracticeAPos != 0
                && Helpers.PracticeBPos != 0
                && Helpers.PracticeAPos - 10 > Helpers.PracticeBPos
                && transform.localPosition.y < Helpers.PracticeBPos)
            {
                transform.localPosition = new Vector3(0, Helpers.PracticeAPos);
                if (coroutineRepeat != null)
                {
                    StopCoroutine(coroutineRepeat);
                }

                coroutineRepeat = Repeat(2);
                StartCoroutine(coroutineRepeat);
            }
        }

        //Debug.Log($"IsInvoking {IsInvoking("SetListPracticePauseSprite")}");

        if (!Helpers.IsPracticePlaying
            && SetListPracticePauseSpriteCheck < DateTime.Now)
        {
            SetListPracticePauseSprite();
            SetListPracticePauseSpriteCheck = DateTime.Now.AddSeconds(1);
        }
    }

    private DateTime SetListPracticePauseSpriteCheck = DateTime.Now;

   public void SetListPracticePauseSprite()
    {
        //Debug.Log($"SetListPracticePauseSprite");

        List<GameObject> listPauseProcessed = new List<GameObject>();
        float listPauseY = float.PositiveInfinity;
        foreach (var rec in Helpers.ListPracticePause)
        {
            var noteValue = rec.GetComponent<NoteAnimation>().NoteIndex + Helpers.LearnTranspose;
            if (Helpers.KeysDict[noteValue].GetComponent<SpriteRenderer>().color != Helpers.ColorAmber10)
            {
                var sprite = Resources.Load("Sprites\\" + "KeyWhiteDown", typeof(Sprite)) as Sprite;
                if (Helpers.KeysBlacksDict.ContainsKey(noteValue))
                {
                    sprite = Resources.Load("Sprites\\" + "KeyBlackDown", typeof(Sprite)) as Sprite;
                }

                Helpers.KeysDict[noteValue].GetComponent<SpriteRenderer>().color = Helpers.ColorAmber10;
                Helpers.KeysDict[noteValue].GetComponent<SpriteRenderer>().sprite = sprite;
                listPauseProcessed.Add(rec);
                listPauseY = rec.transform.localPosition.y;
            }
        }

        if (listPauseY < float.PositiveInfinity)
        {
            foreach (var rec in Helpers.ListPracticePreAdd.Where(obj =>
                !listPauseProcessed.Contains(obj)
                && !obj.GetComponent<NoteAnimation>().PracticePlayed
                && obj.transform.localPosition.y >= listPauseY
                && obj.transform.localPosition.y < listPauseY + 1.3f))
            {
                var noteValue = rec.GetComponent<NoteAnimation>().NoteIndex + Helpers.LearnTranspose;
                if (Helpers.KeysDict[noteValue].GetComponent<SpriteRenderer>().color != Helpers.ColorAmber10)
                {
                    var sprite = Resources.Load("Sprites\\" + "KeyWhiteDown", typeof(Sprite)) as Sprite;
                    if (Helpers.KeysBlacksDict.ContainsKey(noteValue))
                    {
                        sprite = Resources.Load("Sprites\\" + "KeyBlackDown", typeof(Sprite)) as Sprite;
                    }

                    Helpers.KeysDict[noteValue].GetComponent<SpriteRenderer>().color = Helpers.ColorAmber10;
                    Helpers.KeysDict[noteValue].GetComponent<SpriteRenderer>().sprite = sprite;
                }
            }
        }
    }

    IEnumerator Repeat(int waitTime)
    {
        maimMenu.StopPlaying();
        MidiOut.AllSoundOff();
        panelMeasure.UpdateSliderValue(transform.localPosition.y);
        Debug.Log("Repeat Wait for " + waitTime + " seconds");
        yield return new WaitForSeconds(waitTime);
        maimMenu.StartPlaying();
    }
}