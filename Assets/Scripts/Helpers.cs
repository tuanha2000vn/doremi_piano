using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ForieroEngine.MIDIUnified;
using MusicXml.Domain;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Helpers
{
    public static bool IsPlaying = false;
    public static bool IsPracticePlaying = true;
    public static int MetronomeSpeed = 10;
    public static int LearnTranspose = 0;
    public static int PlayTranspose = 0;
    public static float KeyBlackHeight;
    public static float KeyBlackWidth;
    public static float KeysWhiteHeight;
    public static float KeyWhiteWidth;
    public static Dictionary<int, GameObject> KeysDict;
    public static Dictionary<int, GameObject> KeysWhitesDict;
    public static Dictionary<int, GameObject> KeysBlacksDict;
    public static List<AudioClip> NotesClip = new List<AudioClip>();

    public static Vector2 TopPos
    {
        get { return Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 1f, 0)); }
    }

    public static Vector3 BottomPos
    {
        get { return Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0f, 0)); }
    }

    public static Vector3 LeftPos
    {
        get { return Camera.main.ViewportToWorldPoint(new Vector3(0f, 0.5f, 0)); }
    }

    public static Vector3 RightPos
    {
        get { return Camera.main.ViewportToWorldPoint(new Vector3(1f, 0.5f, 0)); }
    }

    public static float CamWidth
    {
        get { return RightPos.x - LeftPos.x; }
    }

    public static float CamHeight
    {
        get { return TopPos.y - BottomPos.y; }
    }

    public static float KeysWidth
    {
        get { return KeyWhiteWidth * 52; }
    }

    public static float KeysHeights
    {
        get { return KeysWhiteHeight; }
    }

    public static float ScaleMin
    {
        get { return CamWidth / KeysWidth; }
    }

    public static float ScaleMiddle
    {
        get { return ScaleMin + (ScaleMax - ScaleMin) / 2; }
    }

    public static float ScaleMax
    {
        get { return CamHeight / KeysHeights; }
    }

    public static string StreamingAssetsPath(string fileName)
    {
        //Debug.Log("fileName " + fileName);
        if (Application.platform == RuntimePlatform.Android)
        {
            // Android
            string oriPath = Path.Combine(Application.streamingAssetsPath, fileName);

            // Android only use WWW to read file
            WWW reader = new WWW(oriPath);
            while (!reader.isDone)
            {
            }

            var realPath = Application.persistentDataPath + "/db";
            File.WriteAllBytes(realPath, reader.bytes);

            return realPath;
        }
        else
        {
            // iOS
            return Path.Combine(Application.streamingAssetsPath, fileName);
            //return Application.streamingAssetsPath + "/"; //"LocalSongList.xml");
        }
    }

    public static string songInfoName = "";
    public static string songInfoComposer = "";
    public static int songInfoXmlId = -1;
    public static string SongXmlCompressed = string.Empty;
    public static int QuarterNoteLength = 10;
    public static int OffSet = 50;

    public static bool NotesClipReady = false;

    public static Score Score;
    public static int ScoreDivision = 0;
    public static int TotalMeasure = 0;
    public static float LastMeasurePostY = 0;
    public static int SpeedValue = 80;
    public static string SelectedInstrument = "AcousticGrandPiano";

    public static int NoteIndex(Pitch pitch, int LearnTranspose)
    {
        int stepValue = pitch.Octave * 12 - 9;
        switch (pitch.Step.ToString())
        {
            case "C":
                stepValue = stepValue + 1;
                break;
            case "D":
                stepValue = stepValue + 3;
                break;
            case "E":
                stepValue = stepValue + 5;
                break;
            case "F":
                stepValue = stepValue + 6;
                break;
            case "G":
                stepValue = stepValue + 8;
                break;
            case "A":
                stepValue = stepValue + 10;
                break;
            case "B":
                stepValue = stepValue + 12;
                break;
            default:
                break;
        }

        stepValue = stepValue + pitch.Alter;
        stepValue = stepValue + LearnTranspose;


        stepValue = Mathf.Clamp(stepValue, 0, 87);
        return stepValue;
    }

    public static string SongViewMode = "All";
    public static string SearchText = "";

    public static Color ColorAmber10 = new Color(1f, .7607f, 0f, 1f);
    public static Color ColorAmber05 = new Color(1f, 0.7568628f, 0.2509804f, 0.5f);
    public static Color ColorAzur05 = new Color(0f, 1f, 1f, 0.5f);
    public static Color ColorBlue10 = new Color(66 / 255f, 165 / 255f, 245 / 255f, 1f);
    public static Color ColorBlue05 = new Color(33 / 255f, 150 / 255f, 243 / 255f, 0.5f);
    public static Color ColorBlue25 = new Color(33 / 255f, 150 / 255f, 243 / 255f, 0.25f);
    public static Color ColorBlue01 = new Color(33 / 255f, 150 / 255f, 243 / 255f, 0.1f);
    public static Color ColorBlue00 = new Color(33 / 255f, 150 / 255f, 243 / 255f, 0.0f);
    public static Color ColorBlack10 = new Color(96 / 255f, 96 / 255f, 96 / 255f, 1f);
    public static Color ColorBlack05 = new Color(96 / 255f, 96 / 255f, 96 / 255f, 0.5f);
    public static Color ColorBlack00 = new Color(96 / 255f, 96 / 255f, 96 / 255f, 0f);
    public static Color ColorGreen10 = new Color(102 / 255f, 187 / 255f, 106 / 255f, 1f);
    public static Color ColorGreen05 = new Color(76 / 255f, 175 / 255f, 80 / 255f, 0.5f);
    public static Color ColorGreen25 = new Color(76 / 255f, 175 / 255f, 80 / 255f, 0.25f);
    public static Color ColorGreen01 = new Color(76 / 255f, 175 / 255f, 80 / 255f, 0.1f);
    public static Color ColorGreen00 = new Color(76 / 255f, 175 / 255f, 80 / 255f, 0.0f);
    public static Color ColorOrange05 = new Color(1f, .5f, 0f, 0.5f);
    public static Color ColorPurple05 = new Color(1f, 0f, 1f, 0.5f);
    public static Color ColorRed05 = new Color(1f, 0f, 0f, 0.5f);
    public static Color ColorYellow05 = new Color(1f, 1f, 0f, 0.5f);
    public static Color ColorYellow10 = new Color(1f, 1f, 0f, 1f);
    public static Color ColorWhite10 = new Color(1f, 1f, 1f, 1f);
    public static Color ColorWhite75 = new Color(1f, 1f, 1f, 0.75f);
    public static Color ColorWhite05 = new Color(1f, 1f, 1f, 0.5f);
    public static Color ColorWhite25 = new Color(1f, 1f, 1f, 0.25f);
    public static Color ColorWhite01 = new Color(1f, 1f, 1f, 0.1f);
    public static Color ColorWhite00 = new Color(1f, 1f, 1f, 0.0f);

    public static string InstrumentSelectMode = "Select Play Instrument";
    public static string PlayChannelInstrument = "AcousticGrandPiano";
    public static string LeftChannelInstrument = "AcousticGrandPiano";
    public static string RightChannelInstrument = "AcousticGrandPiano";
    public static string VoiceChannelInstrument = "Harmonica";

    //private static bool ChannelSetupInit = true;

    public static void ChannelSetup()
    {
        //if (ChannelSetupInit)
        {
            PlayChannelSetup();
            LeftChannelSetup();
            RightChannelSetup();
            VoiceChannelSetup();
            MidiOut.SetInstrument((ProgramEnum) Enum.Parse(typeof(ProgramEnum), "AcousticGrandPiano"),
                (ChannelEnum) InputChannel.ExternalKeyboard);
            MidiOut.SetInstrument((ProgramEnum) Enum.Parse(typeof(ProgramEnum), "AcousticGrandPiano"),
                (ChannelEnum) InputChannel.InGamePiano);

            //ChannelSetupInit = false;
        }
    }


    private static void PlayChannelSetup()
    {
        if (PlayChannelInstrument == "Mute")
        {
            return;
        }

        MidiOut.SetInstrument((ProgramEnum) Enum.Parse(typeof(ProgramEnum), PlayChannelInstrument), (ChannelEnum) 1);
        Debug.Log("Set PlayChannelSetup " + 1 + " to " + PlayChannelInstrument);
    }

    private static void LeftChannelSetup()
    {
        if (LeftChannelInstrument == "Mute")
        {
            return;
        }

        for (int i = 3; i <= 4; i++)
        {
            MidiOut.SetInstrument((ProgramEnum) Enum.Parse(typeof(ProgramEnum), LeftChannelInstrument),
                (ChannelEnum) i);
            //Debug.Log("Set LeftChannelSetup " + i + " to " + LeftChannelInstrument);
        }
    }

    private static void RightChannelSetup()
    {
        if (RightChannelInstrument == "Mute")
        {
            return;
        }

        for (int i = 5; i <= 6; i++)
        {
            MidiOut.SetInstrument((ProgramEnum) Enum.Parse(typeof(ProgramEnum), RightChannelInstrument),
                (ChannelEnum) i);
            //Debug.Log("Set RightChannelSetup " + i + " to " + RightChannelInstrument);
        }
    }

    private static void VoiceChannelSetup()
    {
        if (VoiceChannelInstrument == "Mute")
        {
            return;
        }

        for (int i = 7; i <= 8; i++)
        {
            MidiOut.SetInstrument((ProgramEnum) Enum.Parse(typeof(ProgramEnum), VoiceChannelInstrument),
                (ChannelEnum) i);
            //Debug.Log("Set VoiceChannelSetup " + i + " to " + VoiceChannelInstrument);
        }
    }

    public enum InputChannel
    {
        ExternalKeyboard = 0,
        InGameKeyboard = 1,
        InGamePiano = 2,
        LeftHand = 3,
        RightHand = 4,
        Voice = 5,
    }

    public static int GetNoteChannel(int note, InputChannel channel)
    {
        if (channel == InputChannel.ExternalKeyboard
            || channel == InputChannel.InGameKeyboard
            || channel == InputChannel.InGamePiano)
        {
            return channel.ToInt();
        }

        if (channel == InputChannel.LeftHand)
        {
            if (DictLeftHand.ContainsKey(note))
            {
                if (DictLeftHand[note])
                {
                    DictLeftHand[note] = false;
                    return 4;
                }
                else
                {
                    DictLeftHand[note] = true;
                    return 3;
                }
            }
            else
            {
                DictLeftHand.Add(note, true);
                return 3;
            }
        }

        if (channel == InputChannel.RightHand)
        {
            if (DictRightHand.ContainsKey(note))
            {
                if (DictRightHand[note])
                {
                    DictRightHand[note] = false;
                    return 6;
                }
                else
                {
                    DictRightHand[note] = true;
                    return 5;
                }
            }
            else
            {
                DictRightHand.Add(note, true);
                return 5;
            }
        }

        if (channel == InputChannel.Voice)
        {
            if (DictVoice.ContainsKey(note))
            {
                if (DictVoice[note])
                {
                    DictVoice[note] = false;
                    return 8;
                }
                else
                {
                    DictVoice[note] = true;
                    return 7;
                }
            }
            else
            {
                DictVoice.Add(note, true);
                return 7;
            }
        }

        Debug.LogWarning("GetNoteChannel note " + note + " channel " + channel);
        return channel.ToInt();
    }

    private static Dictionary<int, bool> DictLeftHand = new Dictionary<int, bool>();
    private static Dictionary<int, bool> DictRightHand = new Dictionary<int, bool>();
    private static Dictionary<int, bool> DictVoice = new Dictionary<int, bool>();
    public static string PracticeMode = "Practice-No-Hand";
    public static bool GetScoreCompleted = false;
    public static List<GameObject> ListPracticePause = new List<GameObject>();
    public static List<GameObject> ListPracticePreAdd = new List<GameObject>();
    public static float PracticeAPos = 0f;
    public static float PracticeBPos = 0f;
    public static DateTime PracticeRepeatTimeOut = DateTime.Now;
    public static bool ConnectMidiOnStart = false;
    public static int Fps = 0;

    public static int KeyboardType
    {
        get
        {
            if (!PlayerPrefs.HasKey("KeyboardType"))
            {
                Debug.LogWarning("PlayerPrefs.HasKey(KeyboardType)");
                PlayerPrefs.SetInt("KeyboardType", 88);
            }

            return PlayerPrefs.GetInt("KeyboardType");
        }
    }

    public static bool KeyboardTypeInRange(int noteIndex)
    {
        //if (SceneManager.GetActiveScene().buildIndex == 1)
        //{
        //    noteIndex = noteIndex + PlayTranspose;
        //}

        //if (SceneManager.GetActiveScene().buildIndex == 3)
        //{
        //    noteIndex = noteIndex + LearnTranspose;
        //}

        if (KeyboardType == 76
            && (noteIndex > 83
                || noteIndex < 8))
        {
            return false;
        }

        if (KeyboardType == 61
            && (noteIndex > 76
                || noteIndex < 16))
        {
            return false;
        }

        if (KeyboardType == 49
            && (noteIndex > 64
                || noteIndex < 16))
        {
            return false;
        }

        return true;
    }

    public static bool HandFingerShow = false;
    public static bool UseMicrophone = true;

}