using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class PracticePause : MonoBehaviour
{
    public Sprite KeyBlack;
    public Sprite KeyBlackDown;
    public Sprite KeyWhite;
    public Sprite KeyWhiteDowm;
    public Sprite NoteNormal;
    public Sprite NoteHilight;
    public NoteFlowControl noteFlowControl;

    //private GameObject NoteFlow;
    //private DateTime transformBackTime = DateTime.Now;

    void OnEnable()
    {
        Helpers.ListPracticePause.Clear();
        Helpers.IsPracticePlaying = true;
    }

    void OnDisable()
    {
        Helpers.ListPracticePause.Clear();
        Helpers.IsPracticePlaying = true;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (!Helpers.IsPlaying)
        {
            return;
        }

        if (Helpers.PracticeMode == "Practice-No-Hand")
        {
            return;
        }

        var noteName = other.name;

        if (noteName.Contains("V"))
        {
            return;
        }

        if (Helpers.PracticeMode == "Practice-Left-Hand"
            && !noteName.Contains("L"))
        {
            return;
        }

        if (Helpers.PracticeMode == "Practice-Right-Hand"
            && !noteName.Contains("R"))
        {
            return;
        }


        Debug.Log("PracticePause OnTriggerEnter2D " + other.name);

        var noteAnimation = other.GetComponent<NoteAnimation>();
        //Debug.Log("noteAnimation.NoteIndex " + noteAnimation.NoteIndex);
        if (!Helpers.KeyboardTypeInRange(noteAnimation.NoteIndex + Helpers.LearnTranspose))
        {
            return;
        }

        if (!noteAnimation.PracticePlayed)
        {
            if (!Helpers.ListPracticePause.Contains(other.gameObject))
            {
                Helpers.ListPracticePause.Add(other.gameObject);
                noteFlowControl.SetListPracticePauseSprite();
                Helpers.IsPracticePlaying = false;
            }
        }
    }
}