/*
 *  UnityMidi.h
 *  UnityMidi
 *
 *  Created by Marek Ledvina on 3/31/11.
 *  Copyright 2011 Foriero Studio. All rights reserved.
 *
 */
#include <CoreMidi/CoreMidi.h>
#include "PGMidiWrapper.h"


extern "C" {
#pragma GCC visibility push(default)
    
    /* External interface to the midiunified, C-based */
    
    CFStringRef rtmidipluginUUID(void);
    /* MIDI IN */
    
    int MidiIn_PortOpen(int i);
    void MidiIn_PortClose(int deviceId);
    void MidiIn_PortCloseAll();
    const char * MidiIn_PortName(int i);
    int MidiIn_PortCount();
    
    int MidiIn_PopMessage (MidiMessage* message);
    
    bool MidiIn_PortAdded();
    bool MidiIn_PortRemoved();
    
    /* MIDI OUT */
    
    int MidiOut_PortOpen(int i);
    void MidiOut_PortClose(int deviceId);
    void MidiOut_PortCloseAll();
    const char * MidiOut_PortName(int i);
    int MidiOut_PortCount();
    
    int MidiOut_SendMessage(int Command, int Data1, int Data2);
    int MidiOut_SendData(UInt8 * data, int dataSize);

    bool MidiOut_PortAdded();
    bool MidiOut_PortRemoved();
    void EnableNetwork(bool enabled);
#pragma GCC visibility pop
}