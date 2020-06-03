using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PracticePreAdd : MonoBehaviour
{
    //public Sprite NoteNormal;
    //private NotePlayStop notePlayStop;

    void OnEnable()
    {
        Helpers.ListPracticePreAdd.Clear();
        //notePlayStop = GetComponentInParent<NotePlayStop>();
    }

    void OnDisable()
    {
        Helpers.ListPracticePreAdd.Clear();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!Helpers.IsPlaying)
        {
            return;
        }

        if (Helpers.PracticeMode == "Practice-No-Hand")
        {
            ////Debug.Log($"Practice-No-Hand");
            return;
        }

        var noteName = other.name;

        if (noteName.Contains("V"))
        {
            ////Debug.Log($"V");
            return;
        }

        if (Helpers.PracticeMode == "Practice-Left-Hand"
            && !noteName.Contains("L"))
        {
            ////Debug.Log($"L");
            return;
        }

        if (Helpers.PracticeMode == "Practice-Right-Hand"
            && !noteName.Contains("R"))
        {
            ////Debug.Log($"R");
            return;
        }

        var noteAnimation = other.GetComponent<NoteAnimation>();
        //Debug.Log("noteAnimation.NoteIndex " + noteAnimation.NoteIndex);
        if (!Helpers.KeyboardTypeInRange(noteAnimation.NoteIndex + Helpers.LearnTranspose))
        {
            return;
        }

        if (!Helpers.ListPracticePreAdd.Contains(other.gameObject))
        {
            Helpers.ListPracticePreAdd.Add(other.gameObject);
            //Debug.Log("OnTriggerEnter2D ListPracticePreAdd.Add " + noteName);
        }
        else
        {
            //Debug.LogWarning($"OnTriggerEnter2D Helpers.ListPracticePreAdd.Contains {noteName}");
        }

        Helpers.ListPracticePreAdd = Helpers.ListPracticePreAdd.OrderBy(obj => obj.transform.localPosition.y).ToList();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var noteName = other.name;
        if (Helpers.ListPracticePreAdd.Contains(other.gameObject))
        {
            Helpers.ListPracticePreAdd.Remove(other.gameObject);
            //Debug.Log("OnTriggerExit2D ListPracticePreAdd.Remove " + noteName);
        }
        else
        {
            //Debug.LogWarning($"OnTriggerExit2D !Helpers.ListPracticePreAdd.Contains {noteName}");
        }

        if (Helpers.PracticeMode != "Practice-No-Hand")
        {
            var note2 = other.GetComponent<NoteAnimation>();
            if (noteName.Contains("L")
                || noteName.Contains("R"))
            {
                note2.NoteNormalReset();
                note2.PracticePlayed = false;
            }

            //Color noteColor = Helpers.ColorWhite10;

            //if (noteName.Contains("L"))
            //{
            //    noteColor = Helpers.ColorBlue10;
            //    notePlayStop.ChangeNoteColor(other.gameObject, NoteNormal, noteColor);
            //}
            //else if (noteName.Contains("R"))
            //{
            //    noteColor = Helpers.ColorGreen10;
            //    notePlayStop.ChangeNoteColor(other.gameObject, NoteNormal, noteColor);
            //}
        }
    }

    //void OnTriggerStay2D(Collider2D other)
    //{
    //    if (!Helpers.IsPlaying)
    //    {
    //        return;
    //    }

    //    if (Helpers.PracticeMode == "Practice-No-Hand")
    //    {
    //        ////Debug.Log($"Practice-No-Hand");
    //        return;
    //    }

    //    var noteName = other.name;

    //    if (noteName.Contains("V"))
    //    {
    //        ////Debug.Log($"V");
    //        return;
    //    }

    //    if (Helpers.PracticeMode == "Practice-Left-Hand"
    //        && !noteName.Contains("L"))
    //    {
    //        ////Debug.Log($"L");
    //        return;
    //    }

    //    if (Helpers.PracticeMode == "Practice-Right-Hand"
    //        && !noteName.Contains("R"))
    //    {
    //        //Debug.Log($"R");
    //        return;
    //    }


    //    if (!Helpers.ListPracticePreAdd.Contains(other.gameObject))
    //    {
    //        Helpers.ListPracticePreAdd.Add(other.gameObject);
    //        //Debug.Log($"ListPracticePreAdd.Add {noteName}");
    //    }
    //}
}