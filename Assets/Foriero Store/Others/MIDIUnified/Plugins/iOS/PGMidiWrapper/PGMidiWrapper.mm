/*
 *  midiunified.cp
 *  midiunified
 *
 *  Created by Marek Ledvina on 7/27/12.
 *  Copyright (c) 2012 Marek Ledvina. All rights reserved.
 *
 */

#import "PGMidiWrapper.h"

#import "PGMidi.h"

@interface PGMidiWrapper () <PGMidiDelegate, PGMidiSourceDelegate>
@end

@implementation PGMidiWrapper

@synthesize midi;
@synthesize lock;
@synthesize sourceDeviceAdded;
@synthesize sourceDeviceRemoved;
@synthesize destinationDeviceAdded;
@synthesize destinationDeviceRemoved;

///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark NSObject

+ (PGMidiWrapper*) sharedPGMidiManager
{
	static PGMidiWrapper *sharedSingleton;
	
	if( !sharedSingleton ) {
		sharedSingleton = [[PGMidiWrapper alloc] init];
        NSLog(@"PGMidiWrapper Initialized");
    }
	
	return sharedSingleton;
}

- (id)init
{
	if( self = [super init] )
	{
        lock = [[NSLock alloc] init];
        midi = [[PGMidi alloc] init];
        midi.networkEnabled = YES;
        midi.delegate = self;
        NSLog(@"PGMidi Initialized");
    }
	return self;
}

- (void) dealloc
{
    if(self.lock){
      self.lock = NULL;
    }
    
    if(self.midi){
      self.midi = NULL;
    }
#if ! PGMIDI_ARC
    [super dealloc];
#endif
}

#pragma mark PGMidiDelegate

- (void) midi:(PGMidi*)midi sourceAdded:(PGMidiSource *)source { sourceDeviceAdded = TRUE; }
- (void) midi:(PGMidi*)midi sourceRemoved:(PGMidiSource *)source { sourceDeviceRemoved = TRUE; }
- (void) midi:(PGMidi*)midi destinationAdded:(PGMidiDestination *)destination { destinationDeviceAdded = TRUE; }
- (void) midi:(PGMidi*)midi destinationRemoved:(PGMidiDestination *)destination { destinationDeviceRemoved = TRUE; }

#pragma mark PGMidiSourceDelegate

- (void) addString:(NSString*)string
{
    NSLog(@"%@", string);
}

NSString *StringFromPacket(const MIDIPacket *packet)
{
    // Note - this is not an example of MIDI parsing. I'm just dumping
    // some bytes for diagnostics.
    // See comments in PGMidiSourceDelegate for an example of how to
    // interpret the MIDIPacket structure.
    return [NSString stringWithFormat:@"  %u bytes: [%02x,%02x,%02x]",
            packet->length,
            (packet->length > 0) ? packet->data[0] : 0,
            (packet->length > 1) ? packet->data[1] : 0,
            (packet->length > 2) ? packet->data[2] : 0
            ];
}

- (void) midiSource:(PGMidiSource*)source midiReceived:(const MIDIPacketList *)packetList
{
    const MIDIPacket *packet = &packetList->packet[0];
    for (int i = 0; i < packetList->numPackets; ++i)
    {
        int byte = 0;
		while (byte < packet->length)
		{
			// Look for the next command byte (Command bytes have the first byte set)
			if (packet->data[byte] & 128)
			{
				// Parse the midi message
				MidiMessageInternal message;
				message.command = packet->data[byte];
                message.data1 = 0;
				message.data2 = 0;
                message.dataSize = 3;
                message.data = NULL;
                message.source = source;
				
				byte++;
                
				// Look if we have a data bit
				if (byte < packet->length && (packet->data[byte] & 128) == 0)
				{
					message.data1 = packet->data[byte];
					byte++;
                    
					// Look if we have a data bit
					if (byte < packet->length && (packet->data[byte] & 128) == 0)
					{
						message.data2 = packet->data[byte];
						byte++;
					}
				}
                
				// Add to the command buffer
                midiMessages.push_back(message);
			}
			else
				byte++;
		}
        packet = MIDIPacketNext(packet);
    }
}

///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark Private



///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark Public

- (void) addMidiSourceDelegate:(PGMidiSource *)source{
    [source addDelegate:self];
}

- (void) removeMidiSourceDelegate:(PGMidiSource *)source{
    [source removeDelegate:self];
}

- (int) midiMessageSize{
    if(midiMessages.empty()) return 0;
    else return midiMessages.size();
    
}

- (MidiMessageInternal) getMidiMessage{
    MidiMessageInternal msg = midiMessages.front();
    midiMessages.pop_front();
    return msg;
}


@end








