/* 
* 	(c) Copyright Marek Ledvina, Foriero Studo
*/

using UnityEngine;
using System;
using System.Collections;
using ForieroEngine.MIDIUnified;

[AddComponentMenu("MIDIUnified/Generators/MidiInput")]
public class MidiInput : MonoBehaviour, IMidiEvents
{
    public static MidiInput singleton;
    public static Action<MidiInput> OnInitialized;

    public delegate void MidiMessageDelegate(MidiMessage aMidiMessage);

    public event MidiMessageDelegate SysExMessageEvent;
    public event ShortMessageEventHandler ShortMessageEvent;

    void Awake()
    {
        if (singleton != null)
        {
            Debug.LogError("GENERATOR : MidiInput already in scene");
            Destroy(this);
            return;
        }

        singleton = this;

        MIDI.through = MIDISettings.instance.inputSettings.midiThrough;

        if (OnInitialized != null)
        {
            OnInitialized(this);
        }
    }

    void OnDestroy()
    {
        singleton = null;
    }

    void Update()
    {
        if (!MIDISettings.instance.inputSettings.midiThrough)
        {
            MIDI.Fetch();
        }
        ProcessMidiInMessages();
    }

    int volume = 0;
    int command = 0;

    public void ProcessMidiMessage(MidiMessage midiMessage)
    {
        if (midiMessage.dataSize == 3 && midiMessage.command != 0xF0)
        {
            if (midiMessage.command.ToMidiCommand() == 0x90 && midiMessage.data2 == 0)
            {
                midiMessage.command = (byte)(midiMessage.command.ToMidiChannel() + 0x80);
            }

            volume = 0;
            if (MIDISettings.instance.inputSettings.useCustomVolume)
            {
                volume = MidiConversion.GetMidiVolume(MIDISettings.instance.inputSettings.customVolume);
            }
            else
            {
                volume = (int)Mathf.Clamp(MIDISettings.instance.inputSettings.multiplyVolume * midiMessage.data2, 0, 127);
            }

            command = 0;
            if (MIDISettings.instance.inputSettings.midiChannel == ChannelEnum.None)
            {
                command = midiMessage.command;
            }
            else
            {
                command = (int)MIDISettings.instance.inputSettings.midiChannel + midiMessage.command.ToMidiCommand();
            }

            if (MIDISettings.instance.inputSettings.midiOut)
            {
                MidiOut.SendShortMessage(command, midiMessage.data1, volume, MIDISettings.instance.inputSettings.midiThrough);
            }

            if (ShortMessageEvent != null)
            {
                ShortMessageEvent(command, midiMessage.data1, volume);
            }
        }
        else
        {
            // SYS EX MESSAGE //
            if (midiMessage.command == 0xF0)
            {
                if (SysExMessageEvent != null)
                    SysExMessageEvent(midiMessage);
            }
        }
    }

    void ProcessMidiInMessages()
    {
        MidiMessage midiMessage = new MidiMessage();

        while (MIDI.midiInMessages.Count > 0)
        {
            if (MIDI.midiInMessages.TryDequeue(out midiMessage))
            {
                if (!MIDISettings.instance.inputSettings.cleanBuffer)
                {
                    ProcessMidiMessage(midiMessage);
                }
            }
        }
        MIDISettings.instance.inputSettings.cleanBuffer = false;
    }
}
