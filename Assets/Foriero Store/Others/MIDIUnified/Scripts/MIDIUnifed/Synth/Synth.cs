using UnityEngine;
using System.Collections;
using ForieroEngine.MIDIUnified;

public static class Synth
{
    public enum SynthEnum
    {
        None = 0,
        Native = 1,
        Bass24 = 2,
        CSharp = 3
    }

    public static SynthSettings settings;

    public static int Start()
    {
        if (settings == null)
        {
            if (MIDISettings.instance.debug)
            {
                Debug.LogError("Settings are NULL. Likely unsupported synth platform or you set not to initialize mid!");
            }
            return -1;
        }
        int result = -1;
        switch (settings.GetSynthEnum())
        {
            case SynthEnum.None:
                break;
            case SynthEnum.Native:
                result = StartNATIVESynth();
                break;
            case SynthEnum.Bass24:
                result = StartBASS24Synth();
                PreinitBASS24();
                break;
            case SynthEnum.CSharp:
                result = StartCSHARPSynth();
                break;
        }
        return result;
    }

    public static int Stop()
    {
        if (settings == null)
        {
            if (MIDISettings.instance.debug)
            {
                Debug.LogError("Settings are NULL. Likely unsupported synth platform or you set not to initialize mid!");
            }
            return -1;
        }
        int result = -1;
        switch (settings.GetSynthEnum())
        {
            case SynthEnum.None:
                break;
            case SynthEnum.Native:
                result = NATIVESynth.Stop();
                break;
            case SynthEnum.Bass24:
                result = BASS24Synth.Stop();
                break;
            case SynthEnum.CSharp:
                result = CSHARPSynth.Stop();
                break;
        }
        return result;
    }

    public static int SendMidiMessage(int aChannelCommand, int aData1, int aData2)
    {
        if (settings == null)
        {
            if (MIDISettings.instance.debug)
            {
                Debug.LogError("Settings are NULL. Likely unsupported synth platform or you set not to initialize mid!");
            }
            return -1;
        }
        int result = -1;
        switch (settings.GetSynthEnum())
        {
            case SynthEnum.None:
                break;
            case SynthEnum.Native:
                if (NATIVESynth.use && NATIVESynth.active)
                {
                    result = NATIVESynth.SendMessage(aChannelCommand, aData1, aData2);
                }
                break;
            case SynthEnum.Bass24:
                if (BASS24Synth.use && BASS24Synth.active)
                {
                    result = BASS24Synth.SendMidiMessage(aChannelCommand, aData1, aData2);
                }
                break;
            case SynthEnum.CSharp:
                if (CSHARPSynth.use && CSHARPSynth.active)
                {
                    result = CSHARPSynth.SendMessage(aChannelCommand, aData1, aData2);
                }
                break;
        }

        return result;
    }

    static int StartBASS24Synth()
    {
        BASS24Synth.use = true;
        BASS24Synth.active = true;
        Debug.Log("Starting BASS24Synth");
        return BASS24Synth.Start(settings.frequency, settings.channels);
    }

    static void PreinitBASS24()
    {
        Debug.Log("Preinit BASS24h");
        if (settings.preinit)
        {
            for (int i = 0; i < settings.channels; i++)
            {
                for (int k = 0; k < 128; k++)
                {
                    BASS24Synth.SendMidiMessage((int)CommandEnum.NoteOn + i, k, 1);
                    BASS24Synth.SendMidiMessage((int)CommandEnum.NoteOff + i, k, 0);
                }
            }
        }
    }

    static int StartNATIVESynth()
    {
        NATIVESynth.use = true;
        NATIVESynth.active = true;
#if UNITY_IOS
		NATIVESynth.soundBank = (settings as SynthSettingsIOS).soundBank;
#endif
        Debug.Log("Starting NATIVESynth");
        return NATIVESynth.Start(settings.frequency, settings.channels);
    }

    static int StartCSHARPSynth()
    {
        CSHARPSynth.use = true;
        CSHARPSynth.active = true;
        Debug.Log("Starting C#Synth");
        return CSHARPSynth.Start(settings.frequency, settings.channels);
    }

    [System.Serializable]
    public abstract class SynthSettings
    {
        public enum OutputSampleRateDividerEnum
        {
            One = 1,
            Two = 2,
            Four = 4
        }

        [Tooltip("AudioSettings.outputSampleRate / outputSampleRateDivider")]
        public OutputSampleRateDividerEnum outputSampleRateDivider = OutputSampleRateDividerEnum.One;

        public int frequency
        {
            get
            {
                return AudioSettings.outputSampleRate / (int)outputSampleRateDivider;
            }
        }

        public int channels = 16;
        public bool preinit = false;

        public abstract Synth.SynthEnum GetSynthEnum();
    }

    [System.Serializable]
    public class SynthSettingsWSA : SynthSettings
    {
        public enum SynthEnum
        {
            None = Synth.SynthEnum.None,
            Bass24 = Synth.SynthEnum.Bass24,
            Native = Synth.SynthEnum.Native,
            CSharp = Synth.SynthEnum.CSharp,
        }

        public SynthEnum synthesizer = SynthEnum.CSharp;

        public override Synth.SynthEnum GetSynthEnum()
        {
            return (Synth.SynthEnum)synthesizer;
        }
    }

    [System.Serializable]
    public class SynthSettingsWEBGL : SynthSettings
    {
        public enum SynthEnum
        {
            None = Synth.SynthEnum.None,
            CSharp = Synth.SynthEnum.CSharp,
        }

        public SynthEnum synthesizer = SynthEnum.CSharp;

        public override Synth.SynthEnum GetSynthEnum()
        {
            return (Synth.SynthEnum)synthesizer;
        }
    }

    [System.Serializable]
    public class SynthSettingsOSX : SynthSettings
    {
        public enum SynthEnum
        {
            None = Synth.SynthEnum.None,
            Bass24 = Synth.SynthEnum.Bass24,
            Native = Synth.SynthEnum.Native,
            CSharp = Synth.SynthEnum.CSharp,
        }

        public SynthEnum synthesizer = SynthEnum.CSharp;

        public override Synth.SynthEnum GetSynthEnum()
        {
            return (Synth.SynthEnum)synthesizer;
        }
    }

    [System.Serializable]
    public class SynthSettingsLINUX : SynthSettings
    {
        public enum SynthEnum
        {
            None = Synth.SynthEnum.None,
            Bass24 = Synth.SynthEnum.Bass24,
            Native = Synth.SynthEnum.Native,
            CSharp = Synth.SynthEnum.CSharp,
        }

        public SynthEnum synthesizer = SynthEnum.CSharp;

        public override Synth.SynthEnum GetSynthEnum()
        {
            return (Synth.SynthEnum)synthesizer;
        }
    }

    [System.Serializable]
    public class SynthSettingsWIN : SynthSettings
    {
        public enum SynthEnum
        {
            None = Synth.SynthEnum.None,
            Bass24 = Synth.SynthEnum.Bass24,
            Native = Synth.SynthEnum.Native,
            CSharp = Synth.SynthEnum.CSharp,
        }

        public SynthEnum synthesizer = SynthEnum.CSharp;

        public override Synth.SynthEnum GetSynthEnum()
        {
            return (Synth.SynthEnum)synthesizer;
        }
    }

    [System.Serializable]
    public class SynthSettingsIOS : SynthSettings
    {
        public enum SynthEnum
        {
            None = Synth.SynthEnum.None,
            Bass24 = Synth.SynthEnum.Bass24,
            Native = Synth.SynthEnum.Native,
            CSharp = Synth.SynthEnum.CSharp,
        }

        public SynthEnum synthesizer = SynthEnum.CSharp;

        public override Synth.SynthEnum GetSynthEnum()
        {
            return (Synth.SynthEnum)synthesizer;
        }

        public enum SoundBankEnum
        {
            sf2 = 0,
            dls = 1,
            aupreset = 2
        }

        public SoundBankEnum soundBank = SoundBankEnum.sf2;
    }

    [System.Serializable]
    public class SynthSettingsANDROID : SynthSettings
    {
        public enum SynthEnum
        {
            None = Synth.SynthEnum.None,
            Bass24 = Synth.SynthEnum.Bass24,
            CSharp = Synth.SynthEnum.CSharp,
        }

        public SynthEnum synthesizer = SynthEnum.CSharp;

        public override Synth.SynthEnum GetSynthEnum()
        {
            return (Synth.SynthEnum)synthesizer;
        }
    }
}
