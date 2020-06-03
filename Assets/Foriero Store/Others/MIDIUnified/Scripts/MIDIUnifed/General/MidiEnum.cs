/* 
* 	All rights reserverd © Marek Ledvina
*/

using System;

namespace ForieroEngine.MIDIUnified
{
    public enum CommandEnum
    {
        None = int.MaxValue,
        NoteOn = 0x90,
        NoteOff = 0x80,
        NoteAftertouch = 0xA0,
        Controller = 0xB0,
        ProgramChange = 0xC0,
        ChannelAftertouch = 0xD0,
        PitchBend = 0xE0,
        Sysex = 0xF0,
        // <summary>Eox (comes at end of a sysex message)</summary>
        Eox = 0xF7,
        // <summary>Timing clock (used when synchronization is required)</summary>
        TimingClock = 0xF8,
        // <summary>Start sequence</summary>
        StartSequence = 0xFA,
        // <summary>Continue sequence</summary>
        ContinueSequence = 0xFB,
        // <summary>Stop sequence</summary>
        StopSequence = 0xFC,
        // <summary>Auto-Sensing</summary>
        AutoSensing = 0xFE,
        // <summary>Meta-event</summary>
        MetaEvent = 0xFF,

        Unknown = int.MaxValue - 1
    }

    public enum TimeSignatureEnum
    {
        TwoTwo,
        ThreeFour,
        FourFour,
        SixEight
    }

    public static class TimeSignatureExtensionMethods
    {
        private static string[] TimeSignatureNames = new string[] {
            "2/2",
            "3/4",
            "4/4",
            "6/8"
        };

        public static string Name(this TimeSignatureEnum timeSignature)
        {
            int value = Math.Abs((int)timeSignature);
            if (value >= 0 && value <= 3)
            {
                return TimeSignatureNames[value];
            }
            else
            {
                return String.Format("{0} Not supported time signature.", value);
            }
        }

        public static int BeatsPerMeasure(this TimeSignatureEnum timeSignature)
        {
            int result = 4;
            switch (timeSignature)
            {
                case TimeSignatureEnum.TwoTwo:
                    result = 2;
                    break;
                case TimeSignatureEnum.ThreeFour:
                    result = 3;
                    break;
                case TimeSignatureEnum.FourFour:
                    result = 4;
                    break;
                case TimeSignatureEnum.SixEight:
                    result = 6;
                    break;
                default:
                    result = 4;
                    break;
            }
            return result;
        }
    }

    public enum ControllerEnum
    {
        None = int.MaxValue,
        BankSelect = 0x00,
        Modulation = 0x01,
        BreathController = 0x02,
        FootController = 0x04,
        PortamentoTime = 0x05,
        DataEntry = 0x06,
        MainVolume = 0x07,
        Balance = 0x08,
        Pan = 0x0A,
        ExpressionController = 0x0B,
        EffectControl1 = 0x0C,
        EffectControl2 = 0x0D,
        GeneralPurposeController1 = 0x10,
        GeneralPurposeController2 = 0x11,
        GeneralPurposeController3 = 0x12,
        GeneralPurposeController4 = 0x13,
        BankSelectLSB = 0x20,
        ModulationLSB = 0x21,
        BreathControllerLSB = 0x22,
        FootControllerLSB = 0x24,
        PortamentoTimeLSB = 0x25,
        DataEntryLSB = 0x26,
        MainVolumeLSB = 0x27,
        BalanceLSB = 0x28,
        PanLSB = 0x2A,
        ExpressionControllerLSB = 0x2B,
        EffectControl1LSB = 0x2C,
        EffectControl2LSB = 0x2D,
        DamperPedal = 0x40,
        Portamento = 0x41,
        Sostenuto = 0x42,
        SoftPedal = 0x43,
        LegatoFootswitch = 0x44,
        Hold2 = 0x45,
        SoundController1 = 0x46,
        SoundController2 = 0x47,
        SoundController3 = 0x48,
        SoundController4 = 0x49,
        SoundController5 = 0x4A,
        SoundController6 = 0x4B,
        SoundController7 = 0x4C,
        SoundController8 = 0x4D,
        SoundController9 = 0x4E,
        SoundController10 = 0x4F,
        GeneralPurposeController5 = 0x50,
        GeneralPurposeController6 = 0x51,
        GeneralPurposeController7 = 0x52,
        GeneralPurposeController8 = 0x53,
        PortamentoControl = 0x54,
        Effects1Depth = 0x5B,
        Effects2Depth = 0x5C,
        Effects3Depth = 0x5D,
        Effects4Depth = 0x5E,
        Effects5Depth = 0x5F,
        DataIncrement = 0x60,
        DataDecrement = 0x61,
        NonRegisteredParameteLSB = 0x62,
        NonRegisteredParameteMSB = 0x63,
        RegisteredParameterLSB = 0x64,
        RegisteredParameterMSB = 0x65,
        AllSoundOff = 0x78,
        ResetControllers = 0x79,
        AllNotesOff = 0x7B,
        OmniModeOff = 0x7C,
        OmniModeOn = 0x7D,
        Unknown = int.MaxValue - 1
    }


    public enum OctaveEnum
    {
        None = int.MaxValue,
        Octave0 = 0,
        Octave1 = 1,
        Octave2 = 2,
        Octave3 = 3,
        Octave4 = 4,
        Octave5 = 5,
        Octave6 = 6,
        Octave7 = 7,
        Octave8 = 8,
        Octave9 = 9,
        Octave10 = 10
    }

    public enum PedalEnum
    {
        Right = 64,
        Center = 66,
        Left = 67
    }

    public enum NoteEnum
    {
        None = int.MaxValue,
        A = 9,
        B = 11,
        C = 0,
        D = 2,
        E = 4,
        F = 5,
        G = 7
    }

    public enum AccidentalEnum
    {
        Doubleflat = -2,
        Flat = -1,
        None = 0,
        Sharp = 1,
        Doublesharp = 2
    }

    public enum IntervalEnum
    {
        P1,
        m2,
        M2,
        m3,
        M3,
        P4,
        TT,
        P5,
        m6,
        M6,
        m7,
        M7,
        P8
    }

    public enum KeySignatureEnum
    {
        CFlatMaj = -7,
        GFlatMaj_EFlatMin = -6,
        DFlatMaj_BFlatMin = -5,
        AFlatMaj_FMin = -4,
        EFlatMaj_CMin = -3,
        BFlatMaj_GMin = -2,
        FMaj_DMin = -1,
        CMaj_AMin = 0,
        GMaj_EMin = 1,
        DMaj_BMin = 2,
        AMaj_FSharpMin = 3,
        EMaj_CSharpMin = 4,
        BMaj_GSharpMin = 5,
        FSharpMaj_DSharpMin = 6,
        CSharpMaj = 7
    }

    public enum ScaleEnum
    {
        Pentatonic,
        Lydian,
        Chromatic
    }

    public enum ToneFlatsEnum
    {
        A = 0,
        BFlat = 1,
        B = 2,
        C = 3,
        DFlat = 4,
        D = 5,
        EFlat = 6,
        E = 7,
        F = 8,
        GFlat = 9,
        G = 10,
        AFlat = 11
    }

    public enum ToneSharpsEnum
    {
        A = 0,
        ASharp = 1,
        B = 2,
        C = 3,
        CSharp = 4,
        D = 5,
        DSharp = 6,
        E = 7,
        F = 8,
        FSharp = 9,
        G = 10,
        GSharp = 11
    }

    public enum ToneEnum
    {
        A = 0,
        ASharpBFlat = 1,
        B = 2,
        C = 3,
        CSharpDFlat = 4,
        D = 5,
        DSharpEFlat = 6,
        E = 7,
        F = 8,
        FSharpGFlat = 9,
        G = 10,
        GSharpAFlat = 11
    }

    public enum ChannelEnum
    {

        None = int.MaxValue,
        C0 = 0,
        C1 = 1,
        C2 = 2,
        C3 = 3,
        C4 = 4,
        C5 = 5,
        C6 = 6,
        C7 = 7,
        C8 = 8,
        C9 = 9,
        C10 = 10,
        C11 = 11,
        C12 = 12,
        C13 = 13,
        C14 = 14,
        C15 = 15
    }

    /// <summary>
    /// Defines constants representing the General MIDI instrument set.
    /// </summary>
    public enum ProgramEnum
    {
        None = int.MaxValue,
        AcousticGrandPiano = 0,
        BrightAcousticPiano = 1,
        ElectricGrandPiano = 2,
        HonkyTonkPiano = 3,
        ElectricPiano1 = 4,
        ElectricPiano2 = 5,
        Harpsichord = 6,
        Clavinet = 7,
        Celesta = 8,
        Glockenspiel = 9,
        MusicBox = 10,
        Vibraphone = 11,
        Marimba = 12,
        Xylophone = 13,
        TubularBells = 14,
        Dulcimer = 15,
        DrawbarOrgan = 16,
        PercussiveOrgan = 17,
        RockOrgan = 18,
        ChurchOrgan = 19,
        ReedOrgan = 20,
        Accordion = 21,
        Harmonica = 22,
        TangoAccordion = 23,
        AcousticGuitarNylon = 24,
        AcousticGuitarSteel = 25,
        ElectricGuitarJazz = 26,
        ElectricGuitarClean = 27,
        ElectricGuitarMuted = 28,
        OverdrivenGuitar = 29,
        DistortionGuitar = 30,
        GuitarHarmonics = 31,
        AcousticBass = 32,
        ElectricBassFinger = 33,
        ElectricBassPick = 34,
        FretlessBass = 35,
        SlapBass1 = 36,
        SlapBass2 = 37,
        SynthBass1 = 38,
        SynthBass2 = 39,
        Violin = 40,
        Viola = 41,
        Cello = 42,
        Contrabass = 43,
        TremoloStrings = 44,
        PizzicatoStrings = 45,
        OrchestralHarp = 46,
        Timpani = 47,
        StringEnsemble1 = 48,
        StringEnsemble2 = 49,
        SynthStrings1 = 50,
        SynthStrings2 = 51,
        ChoirAahs = 52,
        VoiceOohs = 53,
        SynthVoice = 54,
        OrchestraHit = 55,
        Trumpet = 56,
        Trombone = 57,
        Tuba = 58,
        MutedTrumpet = 59,
        FrenchHorn = 60,
        BrassSection = 61,
        SynthBrass1 = 62,
        SynthBrass2 = 63,
        SopranoSax = 64,
        AltoSax = 65,
        TenorSax = 66,
        BaritoneSax = 67,
        Oboe = 68,
        EnglishHorn = 69,
        Bassoon = 70,
        Clarinet = 71,
        Piccolo = 72,
        Flute = 73,
        Recorder = 74,
        PanFlute = 75,
        BlownBottle = 76,
        Shakuhachi = 77,
        Whistle = 78,
        Ocarina = 79,
        Lead1Square = 80,
        Lead2Sawtooth = 81,
        Lead3Calliope = 82,
        Lead4Chiff = 83,
        Lead5Charang = 84,
        Lead6Voice = 85,
        Lead7Fifths = 86,
        Lead8BassAndLead = 87,
        Pad1NewAge = 88,
        Pad2Warm = 89,
        Pad3Polysynth = 90,
        Pad4Choir = 91,
        Pad5Bowed = 92,
        Pad6Metallic = 93,
        Pad7Halo = 94,
        Pad8Sweep = 95,
        Fx1Rain = 96,
        Fx2Soundtrack = 97,
        Fx3Crystal = 98,
        Fx4Atmosphere = 99,
        Fx5Brightness = 100,
        Fx6Goblins = 101,
        Fx7Echoes = 102,
        Fx8SciFi = 103,
        Sitar = 104,
        Banjo = 105,
        Shamisen = 106,
        Koto = 107,
        Kalimba = 108,
        BagPipe = 109,
        Fiddle = 110,
        Shanai = 111,
        TinkleBell = 112,
        Agogo = 113,
        SteelDrums = 114,
        Woodblock = 115,
        TaikoDrum = 116,
        MelodicTom = 117,
        SynthDrum = 118,
        ReverseCymbal = 119,
        GuitarFretNoise = 120,
        BreathNoise = 121,
        Seashore = 122,
        BirdTweet = 123,
        TelephoneRing = 124,
        Helicopter = 125,
        Applause = 126,
        Gunshot = 127
    }

    /// <summary>
    /// General MIDI percussion note.
    /// </summary>
    /// <remarks>
    /// <para>
    /// In General MIDI, notes played on <see cref="Channel.Channel10"/> make the following
    /// percussion sounds, regardless of any <see cref="OutputDevice.SendProgramChange">Program
    /// Change</see> messages on that channel.
    /// </para>
    /// <para>
    /// This enum is used with <see cref="OutputDevice.SendPercussion">OutputDevice.SendPercussion
    /// </see> and <see cref="PercussionMessage"/>.  Equivalently, when cast to <see cref="Note"/>
    /// it can be used with <see cref="OutputDevice.SendNoteOn">OutputDevice.SendNoteOn</see> and
    /// <see cref="NoteOnMessage"/> on <see cref="Channel.Channel10"/>.</para>
    /// <para>This enum has extension methods, such as <see cref="PercussionExtensionMethods.Name"/>
    /// and <see cref="PercussionExtensionMethods.IsValid"/>, defined in
    /// <see cref="PercussionExtensionMethods"/>. </para>
    /// </remarks>
    public enum PercussionEnum
    {
        /// <summary>General MIDI percussion 35 ("Acousic Bass Drum").</summary>
        AcousticBassDrum = 35,
        /// <summary>General MIDI percussion 36 ("Bass Drum").</summary>
        BassDrum1 = 36,
        /// <summary>General MIDI percussion 37 ("Side Stick").</summary>
        SideStick = 37,
        /// <summary>General MIDI percussion 38 ("Acoustic Snare").</summary>
        AcousticSnare = 38,
        /// <summary>General MIDI percussion 39 ("Hand Clap").</summary>
        HandClap = 39,
        /// <summary>General MIDI percussion 40 ("Electric Snare").</summary>
        ElectricSnare = 40,
        /// <summary>General MIDI percussion 41 ("Low Floor Tom").</summary>
        LowFloorTom = 41,
        /// <summary>General MIDI percussion 42 ("Closed Hi-hat").</summary>
        ClosedHiHat = 42,
        /// <summary>General MIDI percussion 43 ("High Floor Tom").</summary>
        HighFloorTom = 43,
        /// <summary>General MIDI percussion 44 ("Pedal Hi-hat").</summary>
        PedalHiHat = 44,
        /// <summary>General MIDI percussion 45 ("LowTom").</summary>
        LowTom = 45,
        /// <summary>General MIDI percussion 46 ("Open Hi-hat").</summary>
        OpenHiHat = 46,
        /// <summary>General MIDI percussion 47 ("Low Mid Tom").</summary>
        LowMidTom = 47,
        /// <summary>General MIDI percussion 48 ("High Mid Tom").</summary>
        HighMidTom = 48,
        /// <summary>General MIDI percussion 49 ("Crash Cymbal 1").</summary>
        CrashCymbal1 = 49,
        /// <summary>General MIDI percussion 50 ("High Tom").</summary>
        HighTom = 50,
        /// <summary>General MIDI percussion 51 ("Ride Cymbal 1").</summary>
        RideCymbal1 = 51,
        /// <summary>General MIDI percussion 52 ("Chinese Cymbal").</summary>
        ChineseCymbal = 52,
        /// <summary>General MIDI percussion 53 ("Ride Bell").</summary>
        RideBell = 53,
        /// <summary>General MIDI percussion 54 ("Tambourine").</summary>
        Tambourine = 54,
        /// <summary>General MIDI percussion 55 ("Splash Cymbal").</summary>
        SplashCymbal = 55,
        /// <summary>General MIDI percussion 56 ("Cowbell").</summary>
        Cowbell = 56,
        /// <summary>General MIDI percussion 57 ("Crash Cymbal 2").</summary>
        CrashCymbal2 = 57,
        /// <summary>General MIDI percussion 58 ("Vibra Slap").</summary>
        VibraSlap = 58,
        /// <summary>General MIDI percussion 59 ("Ride Cymbal 2").</summary>
        RideCymbal2 = 59,
        /// <summary>General MIDI percussion 60 ("High Bongo").</summary>
        HighBongo = 60,
        /// <summary>General MIDI percussion 61 ("Low Bongo").</summary>
        LowBongo = 61,
        /// <summary>General MIDI percussion 62 ("Mute High Conga").</summary>
        MuteHighConga = 62,
        /// <summary>General MIDI percussion 63 ("Open High Conga").</summary>
        OpenHighConga = 63,
        /// <summary>General MIDI percussion 64 ("Low Conga").</summary>
        LowConga = 64,
        /// <summary>General MIDI percussion 65 ("High Timbale").</summary>
        HighTimbale = 65,
        /// <summary>General MIDI percussion 66 ("Low Timbale").</summary>
        LowTimbale = 66,
        /// <summary>General MIDI percussion 67 ("High Agogo").</summary>
        HighAgogo = 67,
        /// <summary>General MIDI percussion 68 ("Low Agogo").</summary>
        LowAgogo = 68,
        /// <summary>General MIDI percussion 69 ("Cabasa").</summary>
        Cabasa = 69,
        /// <summary>General MIDI percussion 70 ("Maracas").</summary>
        Maracas = 70,
        /// <summary>General MIDI percussion 71 ("Short Whistle").</summary>
        ShortWhistle = 71,
        /// <summary>General MIDI percussion 72 ("Long Whistle").</summary>
        LongWhistle = 72,
        /// <summary>General MIDI percussion 73 ("Short Guiro").</summary>
        ShortGuiro = 74,
        /// <summary>General MIDI percussion 74 ("Long Guiro").</summary>
        LongGuiro = 74,
        /// <summary>General MIDI percussion 75 ("Claves").</summary>
        Claves = 75,
        /// <summary>General MIDI percussion 76 ("High Wood Block").</summary>
        HighWoodBlock = 76,
        /// <summary>General MIDI percussion 77 ("Low Wood Block").</summary>
        LowWoodBlock = 77,
        /// <summary>General MIDI percussion 78 ("Mute Cuica").</summary>
        MuteCuica = 78,
        /// <summary>General MIDI percussion 79 ("Open Cuica").</summary>
        OpenCuica = 79,
        /// <summary>General MIDI percussion 80 ("Mute Triangle").</summary>
        MuteTriangle = 80,
        /// <summary>General MIDI percussion 81 ("Open Triangle").</summary>
        OpenTriangle = 81
    }

    /// <summary>
    /// Extension methods for the Percussion enum.
    /// </summary>
    /// Be sure to "using midi" if you want to use these as extension methods.
    public static class PercussionExtensionMethods
    {
        /// <summary>
        /// Returns true if the specified percussion is valid.
        /// </summary>
        /// <param name="percussion">The percussion to test.</param>
        public static bool IsValid(this PercussionEnum percussion)
        {
            return (int)percussion >= 35 && (int)percussion <= 81;
        }

        /// <summary>
        /// Throws an exception if percussion is not valid.
        /// </summary>
        /// <param name="percussion">The percussion to validate.</param>
        /// <exception cref="ArgumentOutOfRangeException">The percussion is out-of-range.
        /// </exception>
        public static void Validate(this PercussionEnum percussion)
        {
            if (!percussion.IsValid())
            {
                throw new ArgumentOutOfRangeException("Percussion out of range");
            }
        }

        private static string[] PercussionNames = new string[] {
            "Acoustic Bass Drum",
            "Bass Drum 1",
            "Side Stick",
            "Acoustic Snare",
            "Hand Clap",
            "Electric Snare",
            "Low Floor Tom",
            "Closed Hi-hat",
            "High Floor Tom",
            "Pedal Hi-Hat",
            "Low Tom",
            "Open Hi-Hat",
            "Low-Mid Tom",
            "High-Mid Tom",
            "Crash Cymbal 1",
            "High Tom",
            "Ride Cymbal 1",
            "Chinese Cymbal",
            "Ride Bell",
            "Tambourine",
            "Splash Cymbal",
            "Cowbell",
            "Crash Cymbal 2",
            "Vibra Slap",
            "Ride Cymbal 2",
            "High Bongo",
            "Low Bongo",
            "Mute High Conga",
            "Open High Conga",
            "Low Conga",
            "High Timbale",
            "Low Timbale",
            "High Agogo",
            "Low Agogo",
            "Cabasa",
            "Maracas",
            "Short Whistle",
            "Long Whistle",
            "Short Guiro",
            "Long Guiro",
            "Claves",
            "High Wood Block",
            "Low Wood Block",
            "Mute Cuica",
            "Open Cuica",
            "Mute Triangle",
            "Open Triangle"
        };

        /// <summary>
        /// Returns the human-readable name of a MIDI percussion.
        /// </summary>
        /// <param name="percussion">The percussion.</param>
        public static string Name(this PercussionEnum percussion)
        {
            percussion.Validate();
            return PercussionNames[(int)percussion - 35];
        }
    }
}
