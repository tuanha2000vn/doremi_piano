using UnityEngine;
using System.Collections;
using ForieroEngine.MIDIUnified;

public class MusicCube : MonoBehaviour
{
    public int midiIndex = 60;
    public float impulseForce = 5f;
    public int finger = -1;

    Rigidbody rb;

    void Awake()
    {
        MidiOut.ShortMessageEvent += ShortMessage;
        rb = GetComponent<Rigidbody>();
    }

    void OnDestroy()
    {
        MidiOut.ShortMessageEvent -= ShortMessage;
    }

    public void Play()
    {
        GetComponent<Rigidbody>().AddForce(new Vector3(0, impulseForce, 0), ForceMode.Impulse);
        Debug.Log("Play midiIndex" + midiIndex);
        MidiOut.NoteOn(midiIndex, 127, 0);
    }

    public void Stop()
    {
        MidiOut.NoteOff(midiIndex);
    }

    public void OnMouseDown()
    {
        GetComponent<Rigidbody>().AddForce(new Vector3(0, impulseForce, 0), ForceMode.Impulse);
        Debug.Log("OnMouseDown midiIndex " + midiIndex);
        MidiOut.NoteOn(midiIndex, 127, 0);
    }

    public void OnMouseUp()
    {
        MidiOut.NoteOff(midiIndex);
    }

    void OnEnable()
    {
        MidiOut.ShortMessageEvent += ShortMessage;
    }

    void OnDisable()
    {
        MidiOut.ShortMessageEvent -= ShortMessage;
    }

    void ShortMessage(int Command, int Data1, int Data2)
    {
        if (Command.ToMidiCommand() == 144 && Data1 == midiIndex)
        {
            rb.AddForce(new Vector3(0, impulseForce, 0), ForceMode.Impulse);
        }

        if (Command.ToMidiCommand() == 128 && Data1 == midiIndex)
        {
        }
    }
}