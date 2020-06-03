/* 
* 	(c) Copyright Marek Ledvina, Foriero Studo
*/

using System;
using UnityEngine;

using ForieroEngine.MIDIUnified.Plugins;

namespace ForieroEngine.MIDIUnified
{

    public static class MidiOut
    {
        //0 = everything//
        //1 = nothing//
        //2,4,8//
        public static int channelMask = -1;
        public static int synthChannelMask = -1;
        public static bool fireMidiOutEvents = true;

        public static bool applyChannelVolumes = false;

        public static float[] volumes = new float[16] {
            1f,
            1f,
            1f,
            1f,
            1f,
            1f,
            1f,
            1f,
            1f,
            1f,
            1f,
            1f,
            1f,
            1f,
            1f,
            1f
        };

        public struct NoteCache
        {
            public int index;
            public bool on;
        }

        public class ChannelCache
        {
            public NoteCache[] notes = new NoteCache[128];
        }

        public static ChannelCache[] channelCache = new ChannelCache[16] {
            new ChannelCache (),
            new ChannelCache (),
            new ChannelCache (),
            new ChannelCache (),
            new ChannelCache (),
            new ChannelCache (),
            new ChannelCache (),
            new ChannelCache (),
            new ChannelCache (),
            new ChannelCache (),
            new ChannelCache (),
            new ChannelCache (),
            new ChannelCache (),
            new ChannelCache (),
            new ChannelCache (),
            new ChannelCache (),
        };

        public static bool muteMessages = false;

        public static bool ignoreProgramMessages = false;
        public static ProgramEnum[] programEnums = new ProgramEnum[16] {
            ProgramEnum.AcousticGrandPiano,
            ProgramEnum.AcousticGrandPiano,
            ProgramEnum.AcousticGrandPiano,
            ProgramEnum.AcousticGrandPiano,
            ProgramEnum.AcousticGrandPiano,
            ProgramEnum.AcousticGrandPiano,
            ProgramEnum.AcousticGrandPiano,
            ProgramEnum.AcousticGrandPiano,
            ProgramEnum.AcousticGrandPiano,
            ProgramEnum.AcousticGrandPiano,
            ProgramEnum.AcousticGrandPiano,
            ProgramEnum.AcousticGrandPiano,
            ProgramEnum.AcousticGrandPiano,
            ProgramEnum.AcousticGrandPiano,
            ProgramEnum.AcousticGrandPiano,
            ProgramEnum.AcousticGrandPiano
        };

        public static event ShortMessageEventHandler ShortMessageEvent;

        public static int SetInstrument(ProgramEnum anInstrument, ChannelEnum aChannel = ChannelEnum.C0)
        {
            if (aChannel == ChannelEnum.None)
                return 0;
            return SetInstrument((int)anInstrument, (int)aChannel);
        }

        public static int SetInstrument(int anInstrument, int aChannel = 0)
        {
            return SendShortMessage(aChannel + (int)CommandEnum.ProgramChange, anInstrument, 0);
        }

        public static int SchedulePercussion(PercussionEnum aPercussion, int aVolume = 80, float scheduleTime = 0)
        {
            return MIDI.SchedulePercussion(aPercussion, aVolume, scheduleTime);
        }

        public static int Percussion(PercussionEnum aPercussion, int aVolume = 80)
        {
            return NoteOn((int)aPercussion, aVolume, 9);
        }

        public static void NoteDispatch(int aNoteIndex, float aDuration, float aDelay = 0f, int aVolume = 80, int aChannel = 0, Action started = null, Action finished = null)
        {
            MidiDispatcher.DispatchNote(aNoteIndex, aVolume, aChannel, aDuration, aDelay, started, finished);
        }

        public static void NoteDispatch(NoteEnum aNote, AccidentalEnum anAccidental, OctaveEnum anOctave, float aDuration, float aDelay = 0f, int aVolume = 80, ChannelEnum aChannel = ChannelEnum.C0)
        {
            if (aChannel == ChannelEnum.None)
                return;
            int noteIndex = ((int)aNote + (int)anAccidental + ((int)(anOctave == OctaveEnum.None ? OctaveEnum.Octave4 : anOctave) * 12) + 24);
            NoteDispatch(noteIndex, aDuration, aDelay, aVolume, (int)aChannel);
        }

        public static int NoteOn(int aNoteIndex, int aVolume = 80, int aChannel = 0)
        {
            return SendShortMessage(aChannel, (int)CommandEnum.NoteOn, aNoteIndex, aVolume);
        }

        public static int NoteOn(NoteEnum aNote, AccidentalEnum anAccidental, OctaveEnum anOctave, int aVolume = 80, ChannelEnum aChannel = ChannelEnum.C0)
        {
            if (aChannel == ChannelEnum.None)
                return 0;
            int noteIndex = ((int)aNote + (int)anAccidental + ((int)(anOctave == OctaveEnum.None ? OctaveEnum.Octave4 : anOctave) * 12) + 24);
            return NoteOn(noteIndex, aVolume, (int)aChannel);
        }

        public static int NoteOff(int aNoteIndex, int aChannel = 0)
        {
            return SendShortMessage((int)aChannel, (int)CommandEnum.NoteOff, aNoteIndex, 0);
        }

        public static int NoteOff(NoteEnum aNote, AccidentalEnum anAccidental, OctaveEnum anOctave, ChannelEnum aChannel = ChannelEnum.C0)
        {
            if (aChannel == ChannelEnum.None)
                return 0;
            int noteIndex = ((int)aNote + (int)anAccidental + ((int)(anOctave == OctaveEnum.None ? OctaveEnum.Octave4 : anOctave) * 12) + 24);
            return NoteOff(noteIndex, (int)aChannel);
        }

        public static int Pedal(int aPedal, int aValue, int aChannel = 0)
        {
            return SendShortMessage(aChannel + (int)CommandEnum.Controller, aPedal, aValue);
        }

        public static int Pedal(PedalEnum aPedal, int aValue, ChannelEnum aChannel = ChannelEnum.C0)
        {
            if (aChannel == ChannelEnum.None)
                return 0;
            return Pedal((int)aPedal, aValue, (int)aChannel);
        }

        public static int SendControl(ControllerEnum aControl, int aValue, ChannelEnum aChannel = ChannelEnum.C0)
        {
            if (aChannel == ChannelEnum.None && aControl != ControllerEnum.None)
                return 0;
            return SendControl((int)aControl, aValue, (int)aChannel);
        }

        public static int SendControl(int aControl, int aValue, int aChannel = 0)
        {
            return SendShortMessage(aChannel + (int)CommandEnum.Controller, aControl, aValue);
        }

        public static void ChannelSoundsOff(int aChannel)
        {
            int index = 0;
            foreach (NoteCache n in channelCache[aChannel].notes)
            {
                if (n.on)
                    NoteOff(index, aChannel);
                index++;
            }
            SendShortMessage(aChannel + (int)CommandEnum.Controller, (int)ControllerEnum.AllSoundOff, 0);
        }

        public static void ChannelPedalsOff(int aChannel)
        {
            Pedal((int)PedalEnum.Right, 0, aChannel);
            Pedal((int)PedalEnum.Center, 0, aChannel);
            Pedal((int)PedalEnum.Left, 0, aChannel);
        }

        public static int AllPedalsOff()
        {
            for (int i = 0; i < 16; i++)
            {
                Pedal((int)PedalEnum.Right, 0, i);
                Pedal((int)PedalEnum.Center, 0, i);
                Pedal((int)PedalEnum.Left, 0, i);
            }

            return 0;
        }

        public static int AllSoundOff()
        {
            int result = 0;

            if (BASS24Synth.active)
            {
                int channel = 0;
                int index = 0;
                foreach (ChannelCache ch in channelCache)
                {
                    index = 0;
                    foreach (NoteCache n in ch.notes)
                    {
                        if (n.on)
                            NoteOff(index, channel);
                        index++;
                    }
                    channel++;
                }
            }
            else
            {
                for (int i = 0; i < 16; i++)
                {
                    result = SendShortMessage(i + (int)CommandEnum.Controller, (int)ControllerEnum.AllSoundOff, 0);
                }
            }

            return result;
        }

        public static int ResetAllControllers()
        {
            int result = 0;
            for (int i = 0; i < 16; i++)
            {
                result = SendShortMessage(i + (int)CommandEnum.Controller, (int)ControllerEnum.ResetControllers, 0);
            }
            return result;
        }

        public static int SendShortMessage(int aCommand, int aChannel, int aData1, int aData2)
        {
            return SendShortMessage(aChannel + aCommand, aData1, aData2);
        }

        public static int SendShortMessage(int aChannelCommand, int aData1, int aData2, bool through = false)
        {
            int channel = (aChannelCommand & 0x0F);
            int command = (aChannelCommand & 0xF0);
            //			Debug.Log ("-----");
            //			Debug.Log (channel.ToString("X2"));
            //			Debug.Log (command.ToString("X2"));
            //			Debug.Log (aData1.ToString("X2"));
            //			Debug.Log (aData2.ToString("X2"));
            //			Debug.Log ("-----");
            if (command == (int)CommandEnum.ProgramChange)
            {
                programEnums[channel] = (ProgramEnum)aData1;
                if (ignoreProgramMessages)
                    return 0;
            }

            if (command.IsNoteON())
            {
                channelCache[channel].notes[aData1].on = true;
                if (applyChannelVolumes)
                {
                    aData2 = (int)(volumes[channel] * aData2);
                }
            }
            else if (command.IsNoteOFF())
            {
                channelCache[channel].notes[aData1].on = false;
            }

            if (ShortMessageEvent != null && fireMidiOutEvents)
                ShortMessageEvent(aChannelCommand, aData1, aData2);

            if (muteMessages || through)
            {
                return 0;
            }
            else
            {
                if (((1 << channel) & synthChannelMask) != 0)
                {
                    Synth.SendMidiMessage(aChannelCommand, aData1, aData2);
                }

                if (((1 << channel) & channelMask) != 0)
                    return MidiOUTPlugin.SendShortMessage((byte)aChannelCommand, (byte)aData1, (byte)aData2);
                else
                    return 0;
            }
        }

        public static int SendData(byte[] aData)
        {
            return MidiOUTPlugin.SendData(aData);
        }
    }
}