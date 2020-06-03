/*
 *  UnityMidi.h
 *  UnityMidi
 *
 *  Created by Marek Ledvina on 3/31/11.
 *  Copyright 2011 Foriero Studio. All rights reserved.
 *
 */
#import <Foundation/Foundation.h>
#import <Foundation/NSLock.h>
#include <CoreMidi/CoreMidi.h>
#include "PGMidi.h"
#import <list>

struct MidiMessageInternal
{
    char command;
    char data1;
    char data2;
    int dataSize;
    int * data;
    PGMidiSource * source;
};

struct MidiMessage
{
	char command;
	char data1;
	char data2;
    int dataSize;
    int * data;
    int deviceId;
};

@class PGMidi;
@protocol PGMidiSourceDelegate;

@interface PGMidiWrapper : NSObject
{
    std::list<MidiMessageInternal> midiMessages;
    __unsafe_unretained PGMidi *midi;
    __unsafe_unretained NSLock *lock;
    bool sourceDeviceAdded;
    bool sourceDeviceRemoved;
    bool destinationDeviceAdded;
    bool destinationDeviceRemoved;
}

@property (nonatomic,unsafe_unretained) PGMidi *midi;
@property (nonatomic,unsafe_unretained) NSLock *lock;
@property (nonatomic,assign) bool sourceDeviceAdded;
@property (nonatomic,assign) bool sourceDeviceRemoved;
@property (nonatomic,assign) bool destinationDeviceAdded;
@property (nonatomic,assign) bool destinationDeviceRemoved;

+ (PGMidiWrapper*) sharedPGMidiManager;

- (void) addMidiSourceDelegate:(PGMidiSource *)source;
- (void) removeMidiSourceDelegate:(PGMidiSource *)source;
- (int) midiMessageSize;
- (MidiMessageInternal) getMidiMessage;
@end