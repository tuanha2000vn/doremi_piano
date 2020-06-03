using UnityEngine;
using System.Collections;
using ForieroEngine;
using ForieroEngine.MIDIUnified;

#if UNITY_EDITOR
using UnityEditor;
#endif


public partial class MIDIPercussionSettings : ScriptableObject
{
#if UNITY_EDITOR
    [MenuItem("Foriero/Settings/Midi/Percussions Settings")]
    public static void MIDIPercussionSettingsMenu()
    {
        MIDIPercussionSettings s = MIDIPercussionSettings.instance;
        EditorGUIUtility.PingObject(s);
        Selection.objects = new Object[1] { s };
    }
#endif

    static MIDIPercussionSettings _instance;

    public static MIDIPercussionSettings instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FResources.Instance<MIDIPercussionSettings>(typeof(MIDIPercussionSettings).Name, "Settings");
            }

            return _instance;
        }
    }

    [Tooltip("")]
    public PercussionEnum metronomeHeavy = PercussionEnum.HighWoodBlock;
    [Tooltip("")]
    [Range(0f, 1f)]
    public float metronomeHeavyVolume = 1f;

    [Tooltip("")]
    public PercussionEnum metronomeLight = PercussionEnum.LowWoodBlock;
    [Tooltip("")]
    [Range(0f, 1f)]
    public float metronomeLightVolume = 1f;

    [Tooltip("")]
    public PercussionEnum rhythm = PercussionEnum.HandClap;
    [Tooltip("")]
    [Range(0f, 1f)]
    public float rhythmVolume = 1f;


    [Tooltip("")]
    public AudioClip[] percussionClips;

    public AudioClip GetAudioClip(PercussionEnum percussionEnum)
    {
        int index = (int)percussionEnum - (int)PercussionEnum.AcousticBassDrum;
        if (index >= 0 && index < percussionClips.Length)
        {
            return percussionClips[index];
        }

        return null;
    }
}
