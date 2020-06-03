using UnityEngine;
using System.Collections;
using ForieroEngine.MIDIUnified;

public static class MidiExtensions
{
    #region int	
    public static int ToInt(this System.Enum enumValue)
    {
        return System.Convert.ToInt32(enumValue);
    }

    public static int ToRawMidiCommand(int command, int channel)
    {
        return channel + command;
    }

    public static int ToMidiCommand(this int i)
    {
        return i & 0xF0;
    }

    public static int ToMidiChannel(this int i)
    {
        return i & 0x0F;
    }

    public static int ToVolume(this float volume)
    {
        return (int)(Mathf.Clamp01(volume) * 127);
    }

    public static float ToVolume(this int attack)
    {
        return attack / 127f;
    }

    public static Color ToMidiColor(this int i)
    {
        return MidiConversion.GetToneColorFromMidiIndex(i);
    }

    public static bool IsNoteON(this int i)
    {
        return (ToMidiCommand(i) == (int)CommandEnum.NoteOn);
    }

    public static bool IsNoteOFF(this int i)
    {
        return (ToMidiCommand(i) == (int)CommandEnum.NoteOff);
    }

    public static int ShiftL(this int i, int bits)
    {
        return i << bits;
    }

    public static int ShiftR(this int i, int bits)
    {
        return i >> bits;
    }

    public static int WriteBit(this int i, byte bit, bool bitValue)
    {
        if (bitValue) return (1 << bit) | i;
        else return i & ~(1 << bit);
    }

    public static bool ReadBit(this int i, byte bit)
    {
        return ((1 << bit) & i) == (1 << bit);
    }

    public static bool IsInByteRange(this int i)
    {
        return (i >= 0 && i <= byte.MaxValue);
    }

    public static bool IsInMidiRange(this int pitch)
    {
        return pitch >= 0 && pitch <= 127;
    }


    public static int Octave(this int i)
    {
        return (i < 0 ? (i - 11) / 12 : i / 12) - 1;
    }

    public static int PositionInOctave(this int i)
    {
        return i < 0 ? 11 - ((-i - 1) % 12) : i % 12;
    }

    public static bool IsInChannelRange(this int i)
    {
        return i >= 0 && i <= 15;
    }


    public static bool IsWhiteKey(this int i)
    {
        switch (i.BaseMidiIndex())
        {
            case 0: return true;
            case 1: return false;
            case 2: return true;
            case 3: return false;
            case 4: return true;
            case 5: return true;
            case 6: return false;
            case 7: return true;
            case 8: return false;
            case 9: return true;
            case 10: return false;
            case 11: return true;
            default: return true;
        }
    }

    public static bool IsBlackKey(this int i)
    {
        switch (i.BaseMidiIndex())
        {
            case 0: return false;
            case 1: return true;
            case 2: return false;
            case 3: return true;
            case 4: return false;
            case 5: return false;
            case 6: return true;
            case 7: return false;
            case 8: return true;
            case 9: return false;
            case 10: return true;
            case 11: return false;
            default: return true;
        }
    }

    public static int BaseMidiIndex(this int i)
    {
        return MidiConversion.BaseMidiIndex(i);
    }

    public static int PrevWhiteKey(this int i)
    {
        i--;
        while (!i.IsWhiteKey())
        {
            i--;
        }
        return i;
    }

    public static int NextWhiteKey(this int i)
    {
        i++;
        while (!i.IsWhiteKey())
        {
            i++;
        }
        return i;
    }

    public static int PrevBlackKey(this int i)
    {
        i--;
        while (!i.IsBlackKey())
        {
            i--;
        }
        return i;
    }

    public static int NextBlackKey(this int i)
    {
        i++;
        while (!i.IsBlackKey())
        {
            i++;
        }
        return i;
    }
    #endregion

    #region byte	
    public static byte ToRawMidiCommand(byte command, byte channel)
    {
        return (byte)(channel + command);
    }

    public static byte ToMidiCommand(this byte i)
    {
        return (byte)(i & 0xF0);
    }

    public static byte ToMidiChannel(this byte i)
    {
        return (byte)(i & 0x0F);
    }

    public static Color ToMidiColor(this byte i)
    {
        return MidiConversion.GetToneColorFromMidiIndex(i);
    }

    public static bool IsNoteON(this byte i)
    {
        return (ToMidiCommand(i) == (byte)CommandEnum.NoteOn);
    }

    public static bool IsNoteOFF(this byte i)
    {
        return (ToMidiCommand(i) == (byte)CommandEnum.NoteOff);
    }

    public static byte ShiftL(this byte i, int bits)
    {
        return (byte)(i << bits);
    }

    public static byte ShiftR(this byte i, int bits)
    {
        return (byte)(i >> bits);
    }

    public static bool IsInByteRange(this byte i)
    {
        return (i >= 0 && i <= byte.MaxValue);
    }

    public static bool IsInMidiRange(this byte pitch)
    {
        return pitch >= 0 && pitch <= 127;
    }


    public static byte Octave(this byte i)
    {
        return (byte)((i < 0 ? (i - 11) / 12 : i / 12) - 1);
    }

    public static byte PositionInOctave(this byte i)
    {
        return (byte)(i < 0 ? 11 - ((-i - 1) % 12) : i % 12);
    }

    public static bool IsInChannelRange(this byte i)
    {
        return i >= 0 && i <= 15;
    }


    public static bool IsWhiteKey(this byte i)
    {
        switch (i.BaseMidiIndex())
        {
            case 0: return true;
            case 1: return false;
            case 2: return true;
            case 3: return false;
            case 4: return true;
            case 5: return true;
            case 6: return false;
            case 7: return true;
            case 8: return false;
            case 9: return true;
            case 10: return false;
            case 11: return true;
            default: return true;
        }
    }

    public static bool IsBlackKey(this byte i)
    {
        switch (i.BaseMidiIndex())
        {
            case 0: return false;
            case 1: return true;
            case 2: return false;
            case 3: return true;
            case 4: return false;
            case 5: return false;
            case 6: return true;
            case 7: return false;
            case 8: return true;
            case 9: return false;
            case 10: return true;
            case 11: return false;
            default: return true;
        }
    }

    public static byte BaseMidiIndex(this byte i)
    {
        return (byte)MidiConversion.BaseMidiIndex(i);
    }

    public static byte PrevWhiteKey(this byte i)
    {
        i--;
        while (!i.IsWhiteKey())
        {
            i--;
        }
        return i;
    }

    public static byte NextWhiteKey(this byte i)
    {
        i++;
        while (!i.IsWhiteKey())
        {
            i++;
        }
        return i;
    }

    public static byte PrevBlackKey(this byte i)
    {
        i--;
        while (!i.IsBlackKey())
        {
            i--;
        }
        return i;
    }

    public static byte NextBlackKey(this byte i)
    {
        i++;
        while (!i.IsBlackKey())
        {
            i++;
        }
        return i;
    }
    #endregion
}