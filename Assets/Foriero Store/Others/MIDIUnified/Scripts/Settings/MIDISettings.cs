using UnityEngine;
using System.Collections;
using ForieroEngine;
using ForieroEngine.MIDIUnified;

#if UNITY_EDITOR
using UnityEditor;
#endif


public partial class MIDISettings : ScriptableObject
{
#if UNITY_EDITOR
    [MenuItem("Foriero/Settings/Midi/Settings")]
    public static void MIDISettingsMenu()
    {
        MIDISettings s = MIDISettings.instance;
        EditorGUIUtility.PingObject(s);
        Selection.objects = new Object[1] { s };
    }
#endif

    static MIDISettings _instance;

    public static MIDISettings instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FResources.Instance<MIDISettings>(typeof(MIDISettings).Name, "Settings");
            }

            return _instance;
        }
    }

    [Tooltip("Log MIDIUnified debug messages!")]
    public bool debug = false;

    [Header("Initialize MIDI?")]
    [Tooltip("If you don't want to initialize MIDIUnified at all set this to FALSE!")]
    public bool initialize = true;

    [Header("Platform Synthesizers")]
    [Tooltip("ANDROID synthetizer settings.")]
    public Synth.SynthSettingsANDROID android;
    [Tooltip("IOS synthetizer settings.")]
    public Synth.SynthSettingsIOS ios;
    [Tooltip("OSX synthetizer settings.")]
    public Synth.SynthSettingsOSX osx;
    [Tooltip("LINUX synthetizer settings.")]
    public Synth.SynthSettingsLINUX linux;
    [Tooltip("WIN synthetizer settings.")]
    public Synth.SynthSettingsWIN win;
    [Tooltip("WSA ( WINDOWS 10 ) synthetizer settings.")]
    public Synth.SynthSettingsWSA wsa;
    [Tooltip("WEBGL synthetizer settings.")]
    public Synth.SynthSettingsWEBGL webgl;

    [Header("Midi IN")]
    [Tooltip("If TRUE code will try to connect 'defaultMidiIn' port.")]
    public bool forceDefaultMidiIn = false;
    public int defaultMidiIn = 1;

    [Header("Midi OUT")]
    [Tooltip("If TRUE code will try to connect 'defaultMidiOut' port.")]
    public bool forceDefaultMidiOut = false;
    public int defaultMidiOut = 1;

    [HideInInspector]
    public int synthChannelMask = -1;
    [HideInInspector]
    public int channelMask = -1;

    [Tooltip("BASS NET Username. Needed for WSA ( WINDOWS 10 ) and ANDROID builds if you are using BASS24 synthetizer.")]
    [HideInInspector]
    public string userName;
    [Tooltip("BASS NET Password. Needed for WSA ( WINDOWS 10 ) and ANDROID builds if you are using BASS24 synthetizer.")]
    [HideInInspector]
    public string password;

    [Header("Midi Instruments")]
    [Tooltip("Ignore 'Program Messages' also called 'Instrument Messages' when midi file starts to play")]
    public bool ignoreProgramMessages = false;

    [SortedEnumPopup]
    public ProgramEnum[] instruments = new ProgramEnum[16] {
        ProgramEnum.None,
        ProgramEnum.None,
        ProgramEnum.None,
        ProgramEnum.None,
        ProgramEnum.None,
        ProgramEnum.None,
        ProgramEnum.None,
        ProgramEnum.None,
        ProgramEnum.None,
        ProgramEnum.None,
        ProgramEnum.None,
        ProgramEnum.None,
        ProgramEnum.None,
        ProgramEnum.None,
        ProgramEnum.None,
        ProgramEnum.None
    };

    public Synth.SynthSettings GetPlatformSettings()
    {
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        return osx;
#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
		return win;
#elif UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX
		return linux;
#elif UNITY_IOS
		return ios;
#elif UNITY_WSA
		return wsa;
#elif UNITY_WEBGL
		return webgl;
#elif UNITY_ANDROID
		return android;
#else
		return null;
#endif
    }

    [System.Serializable]
    public class MidiInputSettings
    {
        public bool initialize = true;

        public bool midiOut = true;
        public bool useCustomVolume = false;
        [Tooltip("This value overrides volume data so you won't be able to hear pressed key dynamics.")]
        [Range(0, 1)]
        public float customVolume = 1f;
        [Tooltip("This value multiplies volume data to make it softer or louder.")]
        [Range(0, 10)]
        public float multiplyVolume = 1f;
        [Tooltip("This will set thread for polling midi messages from your Midi IN devices. If you encounter any instability please set 'through' to false.")]
        public bool midiThrough = false;
        public ChannelEnum midiChannel = ChannelEnum.None;
        public bool cleanBuffer = true;
    }

    [Tooltip("MIDI IN Settings.")]
    public MidiInputSettings inputSettings;

    [System.Serializable]
    public class MidiKeyboardInputSettings
    {
        public bool initialize = true;

        public bool midiOut = true;
        public int keyboardOctave = 4;
        public bool updateKeyboardOctave = false;
        public bool muteTonesWhenChangingOctave = false;
        [Range(0, 1)]
        public float customVolume = 1f;

        public ChannelEnum midiChannel = ChannelEnum.None;

        public enum KeyboardInputType
        {
            ABCDEFG,
            QUERTY
        }

        public KeyboardInputType keyboardInputType = KeyboardInputType.QUERTY;
    }

    [Tooltip("( MIDI ) Keyboard Settings. Please note that this is physical keyaboard on which you type. We map AWSEDFTGYHUJK to mimic real MIDI IN on your normal keyboard.")]
    public MidiKeyboardInputSettings keyboardSettings;

    [System.Serializable]
    public class MidiPlaymakerInputSettings
    {
        public bool initialize = true;

        public bool midiOut = true;
        public ChannelEnum midiChannel = ChannelEnum.None;
        public bool useCustomVolume = false;
        public float customVolume = 1f;
    }

    [Tooltip("MIDI Playmaker Settings needs to be activated if you want to send and receive midi messages from Playmaker.")]
    public MidiPlaymakerInputSettings playmakerSettings;
}
