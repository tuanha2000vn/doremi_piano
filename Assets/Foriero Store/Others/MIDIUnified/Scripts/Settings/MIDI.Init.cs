using UnityEngine;
using System.Collections;

using ForieroEngine.MIDIUnified.Plugins;

namespace ForieroEngine.MIDIUnified
{
    public partial class MIDI : MonoBehaviour
    {
        public static MIDISettings settings
        {
            get
            {
                return MIDISettings.instance;
            }
        }

        static IEnumerator Init()
        {
            Debug.Log("MIDI Init");
            if (settings.initialize)
            {
                yield return InitMidiIO();
                InitSynth();
                InitMIDIUnfied();
                initialized = true;
            }
            else
            {
                Debug.Log("MIDI was set to be not initialized. Go to Menu->Foriero->Settings->Midi");
            }
        }

        public static bool forceDefaultMidiIn = false;
        public static int defaultMidiIn = 1;
        public static bool forceDefaultMidiOut = false;
        public static int defaultMidiOut = 1;

        public static int channelMask = -1;
        public static int synthChannelMask = -1;

        public static bool initialized = false;

        public static void RefreshMidiIO()
        {
            MidiINPlugin.Refresh();
            MidiOUTPlugin.Refresh();
        }

        public static void RefreshSynth()
        {

        }

        static void InitMIDIUnfied()
        {
            Debug.Log("Initializing MIDIUnified");
            MidiOut.channelMask = channelMask;
            MidiOut.channelMask = PlayerPrefs.GetInt("MIDIOUT_MIDI_MASK", settings.channelMask);
            Debug.Log("Channel Mask : " + MidiOut.channelMask);

            MidiOut.synthChannelMask = synthChannelMask;
            MidiOut.synthChannelMask = PlayerPrefs.GetInt("SYNTH_MIDI_MASK", settings.synthChannelMask);
            Debug.Log("SynthChannel Mask : " + MidiOut.synthChannelMask);

            for (int i = 0; i < settings.instruments.Length; i++)
            {
                if (settings.instruments[i] != ProgramEnum.None)
                {
                    MidiOut.SetInstrument(settings.instruments[i], (ChannelEnum)i);
                }
            }

            MidiOut.ignoreProgramMessages = settings.ignoreProgramMessages;
        }

        static IEnumerator InitMidiIO()
        {
            if (!MidiINPlugin.initialized)
            {
                MidiINPlugin.Init();
            }

            yield return new WaitUntil(() =>
                 MidiINPlugin.Initialized()
            );

            if (!MidiOUTPlugin.initialized)
            {
                MidiOUTPlugin.Init();
            }

            yield return new WaitUntil(() =>
               MidiOUTPlugin.Initialized()
            );

            forceDefaultMidiIn = settings.forceDefaultMidiIn;
            defaultMidiIn = settings.defaultMidiIn;
            forceDefaultMidiOut = settings.forceDefaultMidiOut;
            defaultMidiOut = settings.defaultMidiOut;

            if (forceDefaultMidiIn)
            {
                if (defaultMidiIn >= 0 && defaultMidiIn < MidiINPlugin.GetDeviceCount())
                {
                    MidiINPlugin.ConnectDevice(defaultMidiIn);
                }
            }
            else
            {
                MidiINPlugin.RestoreConnections();
                if (MidiINPlugin.connectedDevices.Count == 0)
                {
                    if (defaultMidiIn >= 0 && defaultMidiIn < MidiINPlugin.GetDeviceCount())
                    {
                        MidiINPlugin.ConnectDevice(defaultMidiIn);
                    }
                }
            }

            if (forceDefaultMidiOut)
            {
                if (defaultMidiOut >= 0 && defaultMidiOut < MidiOUTPlugin.GetDeviceCount())
                {
                    MidiOUTPlugin.ConnectDevice(defaultMidiOut);
                }
            }
            else
            {
                MidiOUTPlugin.RestoreConnections();
                if (MidiOUTPlugin.connectedDevices.Count == 0)
                {
                    if (defaultMidiOut >= 0 && defaultMidiOut < MidiOUTPlugin.GetDeviceCount())
                    {
                        MidiOUTPlugin.ConnectDevice(defaultMidiOut);
                    }
                }
            }
        }

        static void InitSynth()
        {
            if (settings != null)
            {
                Synth.settings = settings.GetPlatformSettings();
                if (Synth.settings.GetSynthEnum() == Synth.SynthEnum.Bass24)
                {
#if UNITY_ANDROID || UNITY_WSA
                    BASS24NETSynth.Register(settings.userName, settings.password);
#endif
                }
                Synth.Start();
            }
            else
            {
                Debug.LogError("InitSynth : MIDISettings NULL");
            }
        }

        static void CleanUp()
        {
            MidiOut.AllPedalsOff();
            MidiOut.AllSoundOff();
            MidiOut.ResetAllControllers();
            MidiINPlugin.DisconnectDevices();
            MidiOUTPlugin.DisconnectDevices();
            Synth.Stop();
        }
    }
}