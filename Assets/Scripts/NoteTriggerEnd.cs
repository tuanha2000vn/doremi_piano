using System.Text.RegularExpressions;
using ForieroEngine.MIDIUnified;
using UnityEngine;

public class NoteTriggerEnd : MonoBehaviour
{
    private NotePlayStop notePlayStop;
    private NoteTrigger noteTrigger;

    void Start()
    {
        notePlayStop = GameObject.Find("Keyboard").GetComponent<NotePlayStop>();
        noteTrigger = GameObject.Find("NoteTrigger").GetComponent<NoteTrigger>();
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        var noteName = other.name;
        var noteAnimation = other.GetComponent<NoteAnimation>();
        if (noteAnimation != null)
        {
            var noteValue = noteAnimation.NoteIndex + Helpers.LearnTranspose;

            if (noteTrigger.dictChannel.ContainsKey(other.name))
            {
                notePlayStop.NoteStopMidi(noteValue, noteTrigger.dictChannel[other.name]);
                //Debug.Log("stop " + noteValue + " on outputChannel " + dictChannel[other.name]);
                noteTrigger.dictChannel.Remove(other.name);
            }
            else
            {
                if (Helpers.PracticeMode == "Practice-Left-Hand"
                    && noteName.Contains("L"))
                {
                    return;
                }

                if (Helpers.PracticeMode == "Practice-Right-Hand"
                    && noteName.Contains("R"))
                {
                    return;
                }

                //MidiOut.AllSoundOff();
                for (int i = 0; i < 16; i++)
                {
                    if (MidiOut.channelCache[i].notes[noteValue + 20].on)
                    {
                        Debug.Log("Can not find any valid channel. Stop channel " + i + " for " + noteValue);
                        MidiOut.NoteOff(noteValue + 20, i);
                    }
                }
            }
        }
    }
}