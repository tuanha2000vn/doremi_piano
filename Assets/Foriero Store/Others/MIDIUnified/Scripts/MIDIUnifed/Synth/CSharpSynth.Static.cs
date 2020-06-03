using System.Collections.Generic.Concurrent;
using AudioSynthesis.Synthesis;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public partial class CSharpSynth : MonoBehaviour
{
    static volatile bool initialized = false;
    static volatile bool playEnabled = false;
    static volatile bool allSoundsOff = false;

    static ConcurrentQueue<MidiMessage> queue = new ConcurrentQueue<MidiMessage>();

    public static bool Init(int sampleRate = 22050, int polyphony = 64)
    {
        if (singleton)
        {
            Debug.LogError("C# Synth already in scene!");
            return false;
        }

        GameObject go = new GameObject("CSharpSynth");
        singleton = go.AddComponent<CSharpSynth>();

        GameObject.DontDestroyOnLoad(go);

        playEnabled = singleton.InitSynth(sampleRate, polyphony);
        return playEnabled;
    }

    public static void ShortMessage(byte Command, byte Data1, byte Data2)
    {
        if (singleton)
        {
            queue.Enqueue(new MidiMessage() { channel = Command.ToMidiChannel(), command = Command.ToMidiCommand(), data1 = Data1, data2 = Data2 });
        }
    }

    public static void AllSoundOff()
    {
        if (singleton)
        {
            allSoundsOff = true;
        }
    }
}
