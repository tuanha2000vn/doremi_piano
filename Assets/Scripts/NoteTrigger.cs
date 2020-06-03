using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class NoteTrigger : MonoBehaviour
{
    private NotePlayStop notePlayStop;
    public Sprite KeyBlack;
    public Sprite KeyBlackDown;
    public Sprite KeyWhite;
    public Sprite KeyWhiteDown;

    private DateTime NoteSpeedDelay = DateTime.Now - TimeSpan.FromSeconds(2);

    public Dictionary<string, int> dictChannel;

    void Start()
    {
        notePlayStop = GameObject.Find("Keyboard").GetComponent<NotePlayStop>();
        dictChannel = new Dictionary<string, int>();
        //dictCollision = new Dictionary<GameObject, int>();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("OnTriggerEnter2D " + other.name);

        var noteName = other.name;
        if (Helpers.VoiceChannelInstrument == "Mute"
            && noteName.Contains("V"))
        {
            return;
        }

        var noteAnimation = other.GetComponent<NoteAnimation>();

        var noteValue = noteAnimation.NoteIndex + Helpers.LearnTranspose;

        if ((Helpers.LeftChannelInstrument == "Mute"
             || Helpers.PracticeMode == "Practice-Left-Hand"
             || Helpers.PracticeMode == "Practice-Both-Hand")
            && noteName.Contains("L")
            && Helpers.KeyboardTypeInRange(noteValue))
        {
            return;
        }

        if ((Helpers.RightChannelInstrument == "Mute"
             || Helpers.PracticeMode == "Practice-Right-Hand"
             || Helpers.PracticeMode == "Practice-Both-Hand")
            && noteName.Contains("R")
            && Helpers.KeyboardTypeInRange(noteValue))
        {
            return;
        }

        Sprite sprite = KeyWhiteDown;
        if (Helpers.KeysBlacksDict.ContainsKey(noteValue))
        {
            sprite = KeyBlackDown;
        }

        if (noteName.Contains("L"))
        {
            notePlayStop.ChangeKeyColor(Helpers.KeysDict[noteValue], sprite, Helpers.ColorBlue10);
        }

        if (noteName.Contains("R"))
        {
            notePlayStop.ChangeKeyColor(Helpers.KeysDict[noteValue], sprite, Helpers.ColorGreen10);
        }

        int volume = 100;

        int outputChannel = 0;
        if (noteName.Contains("L"))
        {
            //noteColor = Helpers.ColorBlue05;
            outputChannel = Helpers.GetNoteChannel(noteValue, Helpers.InputChannel.LeftHand);
        }
        else if (noteName.Contains("R"))
        {
            //noteColor = Helpers.ColorGreen05;
            outputChannel = Helpers.GetNoteChannel(noteValue, Helpers.InputChannel.RightHand);
        }
        else if (noteName.Contains("V"))
        {
            //noteColor = Helpers.ColorWhite00;
            outputChannel = Helpers.GetNoteChannel(noteValue, Helpers.InputChannel.Voice);
        }

        if (Helpers.IsPlaying)
        {
            notePlayStop.NotePlayMidi(noteValue, volume, outputChannel);
            if (!dictChannel.ContainsKey(other.name))
            {
                dictChannel.Add(other.name, outputChannel);
            }
        }

        other.GetComponent<NoteAnimation>().StartAnimation();
        //notePlayStop.ChangeNoteColor(other.gameObject, NoteHighlight, noteColor);

        if (NoteSpeedDelay < DateTime.Now
            && other.name.Contains("<"))
        {
            var speedValueString = Regex.Match(other.name, @"\<([^)]*)\>").Groups[1].Value;
            int speedValue;
            if (int.TryParse(speedValueString, out speedValue)
                && Helpers.SpeedValue != speedValue)
            {
                Helpers.SpeedValue = speedValue;
                Debug.LogWarning("Helpers.SpeedValue " + Helpers.SpeedValue);
            }

            NoteSpeedDelay = DateTime.Now.AddSeconds(0.1);
        }

        //Debug.Log("OnTriggerEnter2D " + noteValue);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //Debug.Log("OnTriggerExit2D " + other.name);

        var noteAnimation = other.GetComponent<NoteAnimation>();
        //if (noteAnimation != null)
        {
            var noteValue = noteAnimation.NoteIndex + Helpers.LearnTranspose;

            Sprite sprite = KeyWhite;
            if (Helpers.KeysBlacksDict.ContainsKey(noteValue))
            {
                sprite = KeyBlack;
            }

            //Debug.Log($"OnTriggerExit2D ChangeKeyColor {noteValue}");
            notePlayStop.ChangeKeyColor(Helpers.KeysDict[noteValue], sprite, Helpers.ColorWhite10);

            if (Helpers.PracticeMode == "Practice-No-Hand")
            {
                other.GetComponent<NoteAnimation>().NoteNormalReset();
            }
        }
    }
}