using UnityEngine;
using System.Collections;
using System.Collections.Generic.Concurrent;

// using System.Timers;

using ForieroEngine.MIDIUnified.Plugins;

namespace ForieroEngine.MIDIUnified
{
    [RequireComponent(typeof(AudioSource))]
    public partial class MIDI : MonoBehaviour
    {
        public static MIDI instance;
        // static Timer timer;

        public static ConcurrentQueue<MidiMessage> midiInMessages = new ConcurrentQueue<MidiMessage>();
        public static ConcurrentQueue<MidiMessage> midiOutMessages = new ConcurrentQueue<MidiMessage>();

        static bool _through = false;

        public static bool through
        {
            set
            {
                _through = value;
                // if (value) {
                // 	InitTimer ();
                // } else {
                // 	FinalizeTimer ();
                // }
            }
            get
            {
                return _through;
            }
        }

        //static void OnTimer (object sender, System.Timers.ElapsedEventArgs e)
        //{
        //	Fetch ();
        //}

        //static void InitTimer ()
        //{
        //	timer = new System.Timers.Timer (10);
        //	timer.Elapsed += OnTimer;
        //	timer.Start ();
        //}

        //static void FinalizeTimer ()
        //{
        //	if (timer != null) {
        //		timer.Stop ();
        //		timer.Elapsed -= OnTimer;
        //		timer.Dispose ();
        //	}
        //}

        public static void Fetch()
        {
            MidiMessage midiMessage = new MidiMessage();

            while (MidiINPlugin.PopMessage(out midiMessage) != 0)
            {
                midiInMessages.Enqueue(midiMessage);
                midiOutMessages.Enqueue(midiMessage);
                midiMessage = new MidiMessage();
            }

            while (midiOutMessages.Count > 0)
            {
                if (midiOutMessages.TryDequeue(out midiMessage))
                {
                    MidiOUTPlugin.SendShortMessage(midiMessage.command, midiMessage.data1, midiMessage.data2);
                    Synth.SendMidiMessage(midiMessage.command, midiMessage.data1, midiMessage.data2);
                    midiMessage = new MidiMessage();
                }
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void AutoInit()
        {
            Debug.Log("MIDI AutoInit");
            if (instance)
            {
                Debug.Log("MIDIUnified already in scene.");
                return;
            }
            else
            {
                Debug.Log("MIDI AutoInitializing");
                GameObject go = new GameObject("MIDIUnified - AutoInit");
                go.AddComponent<MIDI>();

                if (MIDISettings.instance.inputSettings.initialize && MidiInput.singleton == null)
                {
                    go.AddComponent<MidiInput>();
                }

                if (MIDISettings.instance.keyboardSettings.initialize && MidiKeyboardInput.singleton == null)
                {
                    go.AddComponent<MidiKeyboardInput>();
                }

                if (MIDISettings.instance.playmakerSettings.initialize && MidiPlayMakerInput.singleton == null)
                {
                    go.AddComponent<MidiPlayMakerInput>();
                }
            }
        }
    }
}