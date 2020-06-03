//
//  softsynth.h
//  softsynth
//
//  Created by Marek Ledvina on 11/9/12.
//  Copyright (c) 2012 Marek Ledvina. All rights reserved.
//

#import "bass.h"
#import "bassmidi.h"

@interface softsynth : NSObject
{
@private
    BASS_INFO info;
    DWORD input;		// MIDI input device
    HSTREAM stream;		// output stream
    HSOUNDFONT font;	// soundfont
    DWORD preset;		// current preset
    BOOL drums;		// drums enabled?
    
    DWORD _frequence;
    DWORD _channels;
}
@property (nonatomic, assign) DWORD frequence;
@property (nonatomic, assign) DWORD channels;

+ (softsynth*)sharedSoftsynth;

- (int)Start;
- (int)Stop;
- (int)SendMidiMessage:(DWORD) Command:(DWORD) Data1: (DWORD) Data2;
- (int)LoadSF2;

@end
