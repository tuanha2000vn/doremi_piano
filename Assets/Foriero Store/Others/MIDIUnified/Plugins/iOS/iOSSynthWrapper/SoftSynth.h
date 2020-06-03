/*
 *  UnityMidi.h
 *  UnityMidi
 *
 *  Created by Marek Ledvina on 3/31/11.
 *  Copyright 2011 Foriero Studio. All rights reserved.
 *
 */

#include <CoreMidi/CoreMidi.h>

extern "C" {
#pragma GCC visibility push(default)

    // mode - dls, sf2, aupreset
    int StartSoftSynth(int freq, int channels, int mode);
    int StopSoftSynth();
    int SendSynthMessage(int Command, int Data1, int Data2);

#pragma GCC visibility pop
}
