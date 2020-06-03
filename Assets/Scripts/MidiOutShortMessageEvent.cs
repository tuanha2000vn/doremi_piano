using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ForieroEngine.Extensions;
using ForieroEngine.MIDIUnified;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MidiOutShortMessageEvent : MonoBehaviour
{
    public Sprite NoteHighlight;
    public Sprite KeyBlack;
    public Sprite KeyBlackDown;
    public Sprite KeyWhite;
    public Sprite KeyWhiteDowm;
    private NotePlayStop notePlayStop;
    private GameObject NoteFlow;
    private int buildIndex;

    void Awake()
    {
        notePlayStop = GetComponent<NotePlayStop>();
        buildIndex = SceneManager.GetActiveScene().buildIndex;
    }

    void OnEnable()
    {
        MidiOut.ShortMessageEvent -= ShortMessage;
        MidiOut.ShortMessageEvent += ShortMessage;
    }

    void OnDisable()
    {
        MidiOut.ShortMessageEvent -= ShortMessage;
    }

    void ShortMessage(int command, int data1, int data2)
    {
        //Debug.Log(
        //    $"command.ToMidiChannel() {command.ToMidiChannel()} " +
        //    $"command.ToMidiCommand() {command.ToMidiCommand()} " +
        //    $"data1 {data1} data2 {data2} " +
        //    $"command.IsNoteON() {command.IsNoteON()} " +
        //    $"command.IsNoteOFF() {command.IsNoteOFF()}");

        if (command.ToMidiCommand() == 144)
            //if (command.IsNoteON())
        {
            var noteValue = data1 - 20;
            if (buildIndex == 1
                && command.ToMidiChannel() == (int) Helpers.InputChannel.InGameKeyboard)
            {
                noteValue = noteValue - Helpers.PlayTranspose;
            }

            Sprite sprite = KeyWhiteDowm;
            if (Helpers.KeysBlacksDict.ContainsKey(noteValue))
            {
                sprite = KeyBlackDown;
            }

            if (!Helpers.KeysDict.ContainsKey(noteValue))
            {
                //Debug.LogWarning("!Helpers.KeysDict.ContainsKey " + noteValue);
                return;
            }

            if (command.ToMidiChannel() == (int) Helpers.InputChannel.ExternalKeyboard
                || command.ToMidiChannel() == (int) Helpers.InputChannel.InGameKeyboard
                || command.ToMidiChannel() == (int) Helpers.InputChannel.InGamePiano)
            {
                //Debug.LogWarning(
                //    $"{command.ToMidiChannel()} notePlayStop.ChangeKeyColor {Helpers.KeysDict[noteValue].name} {sprite.name} {Helpers.ColorWhite10.ToHex()}");
                notePlayStop.ChangeKeyColor(Helpers.KeysDict[noteValue], sprite, Helpers.ColorWhite10);
                ListPracticeProcess(noteValue);
            }

            //if (command.ToMidiChannel() == 3
            //    || command.ToMidiChannel() == 4)
            //{
            //    //Debug.LogWarning(
            //    //    $"{command.ToMidiChannel()} notePlayStop.ChangeKeyColor {Helpers.KeysDict[noteValue].name} {sprite.name} {Helpers.ColorWhite10.ToHex()}");
            //    notePlayStop.ChangeKeyColor(Helpers.KeysDict[noteValue], sprite, Helpers.ColorBlue10);
            //}

            //if (command.ToMidiChannel() == 5
            //    || command.ToMidiChannel() == 6)
            //{
            //    //Debug.LogWarning(
            //    //    $"{command.ToMidiChannel()} notePlayStop.ChangeKeyColor {Helpers.KeysDict[noteValue].name} {sprite.name} {Helpers.ColorWhite10.ToHex()}");
            //    notePlayStop.ChangeKeyColor(Helpers.KeysDict[noteValue], sprite, Helpers.ColorGreen10);
            //}
        }

        if (command.ToMidiCommand() == 128
            || command.ToMidiCommand() == 176)
            //&& command.ToMidiChannel() != (int) Helpers.InputChannel.ExternalKeyboard)
        {
            var noteValue = data1 - 20;
            if (buildIndex == 1
                && command.ToMidiChannel() == (int) Helpers.InputChannel.InGameKeyboard)
            {
                noteValue = noteValue - Helpers.PlayTranspose;
            }

            Sprite sprite = KeyWhite;
            if (Helpers.KeysBlacksDict.ContainsKey(noteValue))
            {
                sprite = KeyBlack;
            }

            if (!Helpers.KeysDict.ContainsKey(noteValue))
            {
                //Debug.LogWarning("!Helpers.KeysDict.ContainsKey " + noteValue);
                return;
            }

            if (command.ToMidiChannel() == (int) Helpers.InputChannel.ExternalKeyboard
                || command.ToMidiChannel() == (int) Helpers.InputChannel.InGameKeyboard
                || command.ToMidiChannel() == (int) Helpers.InputChannel.InGamePiano)
            {
                notePlayStop.ChangeKeyColor(Helpers.KeysDict[noteValue], sprite, Helpers.ColorWhite10);
            }
        }
    }

    void ListPracticeProcess(int noteValue)
    {
        //if (!Helpers.IsPlaying)
        //{
        //    return;
        //}

        if (Helpers.PracticeMode == "Practice-No-Hand")
        {
            return;
        }

        if (Helpers.ListPracticePreAdd.Count <= 0
            && Helpers.ListPracticePause.Count <= 0)
        {
            return;
        }

        //highlight all pause note and up key
        var pauseNotes = Helpers.ListPracticePause
            .Where(obj => obj.GetComponent<NoteAnimation>().NoteIndex == noteValue - Helpers.LearnTranspose).ToList();

        int i = 0;
        foreach (var obj in pauseNotes)
        {
            var note2 = obj.GetComponent<NoteAnimation>();
            note2.PracticePlayed = true;
            note2.StartAnimation();
            Helpers.ListPracticePause.Remove(obj);
            i++;
        }

        if (i == 0)
        {
            var preAddNote = Helpers.ListPracticePreAdd
                .FirstOrDefault(obj =>
                    obj.GetComponent<NoteAnimation>().NoteIndex == noteValue - Helpers.LearnTranspose
                    && obj.GetComponent<NoteAnimation>().PracticePlayed == false);

            if (preAddNote != null)
            {
                var note2 = preAddNote.GetComponent<NoteAnimation>();
                note2.PracticePlayed = true;
                note2.StartAnimation();
            }
        }


        if (!Helpers.IsPracticePlaying)
        {
            if (Helpers.ListPracticePause.Count <= 0)
            {
                Helpers.IsPracticePlaying = true;
            }
        }
    }
}