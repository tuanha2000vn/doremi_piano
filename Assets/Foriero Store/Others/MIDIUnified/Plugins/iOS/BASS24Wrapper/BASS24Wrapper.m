//
//  softsynth.m
//  softsynth
//
//  Created by Marek Ledvina on 11/9/12.
//  Copyright (c) 2012 Marek Ledvina. All rights reserved.
//

#import "BASS24Wrapper.h"
#import "bass.h"
#import "bassmidi.h"

@implementation softsynth

@synthesize
    channels = _channels,
    frequence = _frequence;

#pragma mark NSObject
+ (softsynth*)sharedSoftsynth
{
	static softsynth *sharedSingleton;
	
	if( !sharedSingleton )
		sharedSingleton = [[softsynth alloc] init];
	
	return sharedSingleton;
}

- (id)init
{
	if( self = [super init] )
    {
        preset = 0;
        drums = 0;
        _channels = 16;
        _frequence = 44100;
    }
	return self;
}

#pragma mark Private

- (int) LoadSF2
{
    NSString* sf2file = [[NSBundle mainBundle] pathForResource:@"softsynth" ofType:@"sf2"];
    const char * filename = [ sf2file UTF8String ];
    HSOUNDFONT newfont=BASS_MIDI_FontInit(filename,0);
    if (newfont) {
        BASS_MIDI_FontSetVolume(newfont, 4);
        BASS_MIDI_FONT sf;
        sf.font=newfont;
        sf.preset=-1; // use all presets
        sf.bank=0; // use default bank(s)
        BASS_MIDI_StreamSetFonts(0,&sf,1); // set default soundfont
        BASS_MIDI_StreamSetFonts(stream,&sf,1); // set for output stream too
        BASS_MIDI_FontFree(font); // free old soundfont
        font=newfont;
        {
            BASS_MIDI_FONTINFO i;
            BASS_MIDI_FontGetInfo(font,&i);
        }
    }
    return 1;
}


#pragma mark Public

- (int)Start
{
    if(stream != 0) return 0;
    // check the correct BASS was loaded
    if (HIWORD(BASS_GetVersion())!=BASSVERSION) {
        NSLog(@"An incorrect version of BASS was loaded");
        return 0;
    }
    
    BASS_SetConfig(BASS_CONFIG_VISTA_TRUEPOS,0); // allows lower latency on Vista and newer
    BASS_SetConfig(BASS_CONFIG_UPDATEPERIOD,10); // 10ms update period
    
    // setup output - get latency
    if (!BASS_Init(-1,_frequence,BASS_DEVICE_LATENCY,0,NULL)){
        NSLog(@"Can't initialize device");
        return -1;
    }
    
    BASS_GetInfo(&info);
    BASS_SetConfig(BASS_CONFIG_BUFFER,10+info.minbuf+1); // default buffer size = update period + 'minbuf' + 1ms margin
    
    stream = BASS_MIDI_StreamCreate(_channels, BASS_SAMPLE_FLOAT, _frequence);
    
    if (stream == 0 && BASS_ErrorGetCode() == BASS_ERROR_FORMAT){
        stream = BASS_MIDI_StreamCreate(_channels, 0, _frequence);
    }
    
    if (stream == 0) {
        NSLog(@"Can't initialize stream");
        return -1;
    }
    
    BASS_ChannelSetSync(stream,BASS_SYNC_MIDI_EVENT|BASS_SYNC_MIXTIME,MIDI_EVENT_PROGRAM,0,0);
    BASS_ChannelSetAttribute(stream, BASS_ATTRIB_NOBUFFER, 1);
    
    //BASS_ChannelSetAttribute(stream, BASS_ATTRIB_VOL, 2);
    //BASS_SetVolume(1);
    
    [self LoadSF2];
    BASS_ChannelPlay(stream,0);
    return 1;
    
}

- (int)Stop
{
    if(stream == 0) return 0;
    BASS_StreamFree(stream);
    stream = 0;
    BASS_Free();
    BASS_PluginFree(0);
	return 1;
}

- (int)SendMidiMessage:(DWORD) Command:(DWORD) Data1: (DWORD) Data2
{
    DWORD chan = (Command & 0xF);
    DWORD cmd = (Command >> 4);
    
    //    if(cmd == 0x0B){
    //
    //    } else {
    //        BYTE events[3];
    //        events[0]=Command;
    //        events[1]=Data1;
    //        events[2]=Data2;
    //        BASS_MIDI_StreamEvents(stream, BASS_MIDI_EVENTS_RAW + chan, events, 3);
    //    }
    
    switch (cmd)
    {
        case 0x08: //NoteOff
            //OnNoteOffEvent(aData1, aData2, channel);
            BASS_MIDI_StreamEvent(stream,chan,MIDI_EVENT_NOTE,MAKEWORD(Data1,0));
            break;
        case 0x09: //NoteOn
            BASS_MIDI_StreamEvent(stream,chan,MIDI_EVENT_NOTE,MAKEWORD(Data1,Data2));
            //if(aData2 == 0) OnNoteOffEvent(aData1, aData2, channel);
            //else OnNoteOnEvent(aData1, aData2, channel);
            break;
        case 0x0A: //NoteAftertouch
            break;
        case 0x0B: //Controller
        {
            //OnControllerEvent((ControllerEnum)aData1, aData2, channel);
            switch (Data1)
            {
                case 0x7B: //Note Off All
                    BASS_MIDI_StreamEvent(stream, chan, MIDI_EVENT_SOUNDOFF, Data2);
                    //OnAllNotesOffEvent(ControllerEnum.AllNotesOff, aData2, channel);
                    break;
                case 0x07: //Channel Volume
                    BASS_MIDI_StreamEvent(stream, chan, MIDI_EVENT_VOLUME, Data2);
                    //OnMainVolumeEvent(ControllerEnum.MainVolume, aData2, channel);
                    break;
                case 0x0A: //Pan
                    BASS_MIDI_StreamEvent(stream, chan, MIDI_EVENT_PAN, Data2);
                    //OnPanEvent(ControllerEnum.Pan, aData2, channel);
                    break;
                case 0x01: //Modulation
                    
                    break;
                case 0x64: //Fine Select
                    
                    break;
                case 0x65: //Coarse Select
                    
                    break;
                case 0x06: // DataEntry Coarse
                    
                    break;
                case 0x26: // DataEntry Fine
                    
                    break;
                case 0x79: // Reset All
                    BASS_MIDI_StreamEvent(stream, chan, MIDI_EVENT_RESET, Data2);
                    //OnResetControllersEvent(ControllerEnum.ResetControllers, aData2, channel);
                    break;
                case 0x40:	// Right Pedal
                    BASS_MIDI_StreamEvent(stream, chan, MIDI_EVENT_SUSTAIN, Data2);
                    break;
                case 0x41:	// Center Pedal
                    BASS_MIDI_StreamEvent(stream, chan, MIDI_EVENT_PORTAMENTO, Data2);
                    break;
                case 0x42:	// Center Pedal
                    break;
                case 0x43:	// Left Pedal
                    BASS_MIDI_StreamEvent(stream, chan, MIDI_EVENT_SOFT, Data2);
                    break;
                default:
                    break;
            }
        }
            break;
        case 0x0C: //Program Change - Instrument Change
            BASS_MIDI_StreamEvent(stream,chan,MIDI_EVENT_PROGRAM,MAKEWORD(Data1,Data2));
            //OnProgramChangedEvent(aData1, aData2, channel);
            break;
        case 0x0D: //Channel Aftertouch
            //BASS_MIDI_StreamEvent(stream,chan,MIDI_EVENT_,MAKEWORD(Data1,Data2));
            //OnChannelAfterTouchEvent(aData1, aData2, channel);
            break;
        case 0x0E: //Pitch Bend
            // BASS_MIDI_StreamEvent(stream,chan,MIDI_EVENT_PITCHBEND,MAKEWORD(Data1,Data2));
            //OnPitchBendEnvet(aData1, aData2, channel);
            break;
        default:
            break;
    }
    return 1;
}

@end

