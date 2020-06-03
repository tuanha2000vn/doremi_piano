/*
 *  UnityMidi.cp
 *  UnityMidi
 *
 *  Created by Marek Ledvina on 3/31/11.
 *  Copyright 2011 Foriero Studio. All rights reserved.
 *
 */

#import "SoftSynth.h"

#include <CoreAudioKit/CoreAudioKit.h>
#include <AudioToolbox/AudioToolbox.h>
#include <AVFoundation/AVFoundation.h>

#import <vector>
#import <string>
#import <list>
#import <iostream>

enum SoftMode{ sf2 = 0, dls = 1, aupreset = 2 };

int channelCount = 1;
double sampleRate = 44100.0;
int softMode = sf2;

int const maximumFramesPerSlice = 4096;

AUGraph   processingGraph;
AudioUnit ioUnit;
AUNode mixerNode;
AudioUnit mixerUnit;
std::vector<AudioUnit> samplerUnits(0);
std::vector<AUNode> samplerNodes(0);

AudioStreamBasicDescription monoStreamFormat;

bool synthInitialized = false;

NSString* statToString(UInt32 source) {
    char buf[4];
    char* sourceView = (char*) &source;
    buf[0] = sourceView[3];
    buf[1] = sourceView[2];
    buf[2] = sourceView[1];
    buf[3] = sourceView[0];
    return [[NSString alloc] initWithBytes:buf length:4 encoding:NSUTF8StringEncoding];
}

OSStatus SetBankAndPresetLoadFile(int presetNumber, int channel){
    OSStatus result = noErr;
    AudioUnit au = samplerUnits[channel];
   
    NSURL *presetURL;
    switch(softMode){
        case sf2:
            presetURL = [[NSURL alloc] initFileURLWithPath:[[NSBundle mainBundle] pathForResource:@"gmsf2" ofType:@"sf2"]]; 
            break;
        case dls:
            presetURL = [[NSURL alloc] initFileURLWithPath:[[NSBundle mainBundle] pathForResource:@"gmdls" ofType:@"dls"]];
            break;
        case aupreset:
            presetURL = [[NSURL alloc] initFileURLWithPath:[[NSBundle mainBundle] pathForResource:@"preset" ofType:@"aupreset"]];
            break;
    }
    
    // fill out a bank preset data structure
    AUSamplerBankPresetData bpdata;
    bpdata.bankURL  = (__bridge CFURLRef) presetURL;
    bpdata.bankMSB  = kAUSampler_DefaultMelodicBankMSB;
    bpdata.bankLSB  = kAUSampler_DefaultBankLSB;
    bpdata.presetID = (UInt8) presetNumber;

    // set the kAUSamplerProperty_LoadPresetFromBank property
    result = AudioUnitSetProperty(au,
                                  kAUSamplerProperty_LoadPresetFromBank,
                                  kAudioUnitScope_Global,
                                  0,
                                  &bpdata,
                                  sizeof(bpdata));

    // check for errors
    if(result != noErr){
          NSLog(@"Unable to set the preset property on the Sampler. Preset Number %d Channel %d Error code:%d %@",
               presetNumber,
               channel,
               (int) result,
               statToString(result));
    }
    NSLog(@"LoadSF2 OK");
    return result;
}

OSStatus SetBankAndPreset(int presetNumber, int channel){
    OSStatus result = noErr;
    AudioUnit au = samplerUnits[channel];
    
    NSURL *presetURL;
    switch(softMode){
        case dls:
            presetURL = [[NSURL alloc] initFileURLWithPath:[[NSBundle mainBundle] pathForResource:@"gmsf2" ofType:@"sf2"]];
            break;
        case sf2:
            presetURL = [[NSURL alloc] initFileURLWithPath:[[NSBundle mainBundle] pathForResource:@"gmdls" ofType:@"dls"]];
            break;
        case aupreset:
            presetURL = [[NSURL alloc] initFileURLWithPath:[[NSBundle mainBundle] pathForResource:@"preset" ofType:@"aupreset"]];
            break;
    }
    
    AUSamplerBankPresetData bpdata;
    bpdata.bankURL = (__bridge CFURLRef) presetURL;
    bpdata.bankMSB  = kAUSampler_DefaultMelodicBankMSB;
    bpdata.bankLSB  = kAUSampler_DefaultBankLSB;
    bpdata.presetID = (UInt8) presetNumber;
    
    // set the kAUSamplerProperty_LoadPresetFromBank property
    result = AudioUnitSetProperty(au,
                                  kAUSamplerProperty_BankAndPreset,
                                  kAudioUnitScope_Global,
                                  0,
                                  &bpdata,
                                  sizeof(bpdata));
    
    // check for errors
    if(result != noErr){
        NSLog(@"Unable to set the preset property on the Sampler. Preset Number %d Channel %d Error code:%d %@",
              presetNumber,
              channel,
              (int) result,
              statToString(result));
    }

    return YES;
}

OSStatus CreateAUGraph () {
    
	OSStatus result = noErr;
	
    AUNode samplerNode;
    AUNode ioNode;
    AudioUnit samplerUnit;
       
    AudioComponentDescription MixerUnitDescription;
    MixerUnitDescription.componentType          = kAudioUnitType_Mixer;
    MixerUnitDescription.componentSubType       = kAudioUnitSubType_MultiChannelMixer;
    MixerUnitDescription.componentManufacturer  = kAudioUnitManufacturer_Apple;
    MixerUnitDescription.componentFlags         = 0;
    MixerUnitDescription.componentFlagsMask     = 0;
    
	AudioComponentDescription cd = {};
	cd.componentManufacturer     = kAudioUnitManufacturer_Apple;
	cd.componentFlags            = 0;
	cd.componentFlagsMask        = 0;
   
    result = NewAUGraph (&processingGraph);
    if(result != noErr) NSLog(@"Unable to create an AUGraph object. Error code: %d %@", (int) result, statToString(result));
    result = AUGraphAddNode(processingGraph, &MixerUnitDescription, &mixerNode);
    
    for(int i = 0; i< channelCount; i++){
        //Specify the Sampler unit, to be used as the first node of the graph
        
        cd.componentType = kAudioUnitType_MusicDevice;
        cd.componentSubType = kAudioUnitSubType_Sampler;
        

        result = AUGraphAddNode (processingGraph, &cd, &samplerNode);
        if(result != noErr) NSLog(@"Unable to add the Sampler unit to the audio processing graph. Error code: %d %@", (int) result, statToString(result));
        samplerNodes.push_back(samplerNode);
    }
    
    cd.componentType = kAudioUnitType_Output;
    cd.componentSubType = kAudioUnitSubType_RemoteIO;
    
   
    result = AUGraphAddNode (processingGraph, &cd, &ioNode);
    if(result != noErr) NSLog(@"Unable to add the Output unit to the audio processing graph. Error code: %d %@", (int) result, statToString(result));

    result = AUGraphOpen (processingGraph);
    if(result != noErr) NSLog(@"Unable to open the audio processing graph. Error code: %d %@", (int) result, statToString(result));
    
    result =    AUGraphNodeInfo (
                                 processingGraph,
                                 mixerNode,
                                 NULL,
                                 &mixerUnit
                                 );
    
    result = AudioUnitSetProperty (
                                   mixerUnit,
                                   kAudioUnitProperty_ElementCount,
                                   kAudioUnitScope_Input,
                                   0,
                                   &channelCount,
                                   sizeof (channelCount)
                                   );
        
    result = AudioUnitSetProperty (
                                   mixerUnit,
                                   kAudioUnitProperty_MaximumFramesPerSlice,
                                   kAudioUnitScope_Global,
                                   0,
                                   &maximumFramesPerSlice,
                                   sizeof (maximumFramesPerSlice)
                                   );
    for(int i = 0; i< channelCount; i++){
        result = AudioUnitSetProperty (
                                   mixerUnit,
                                   kAudioUnitProperty_StreamFormat,
                                   kAudioUnitScope_Input,
                                   i,
                                   &monoStreamFormat,
                                   sizeof (monoStreamFormat)
                                   );
    }
    
    result = AudioUnitSetProperty (
                                   mixerUnit,
                                   kAudioUnitProperty_SampleRate,
                                   kAudioUnitScope_Output,
                                   0,
                                   &sampleRate,
                                   sizeof (sampleRate)
                                   );
       
    result = AUGraphConnectNodeInput (processingGraph, mixerNode, 0, ioNode, 0);
    if(result != noErr) NSLog(@"Unable to interconnect the nodes in the audio processing graph. Error code: %d %@", (int) result, statToString(result));
   
    for(int i = 0; i< channelCount; i++){
        result = AUGraphConnectNodeInput (processingGraph, samplerNodes[i], 0, mixerNode, i);
        result = AUGraphNodeInfo (processingGraph, samplerNodes[i], 0, &samplerUnit);
        if(result != noErr) NSLog(@"Unable to obtain a reference to the Sampler unit. Error code: %d %@", (int) result, statToString(result));
        samplerUnits.push_back(samplerUnit);
    }
    
    result = AUGraphNodeInfo (processingGraph, ioNode, 0, &ioUnit);
    
    if(result != noErr) NSLog(@"Unable to obtain a reference to the I/O unit. Error code: %d %@", (int) result, statToString(result));
    result = AudioUnitSetProperty (
                                   ioUnit,
                                   kAudioUnitProperty_StreamFormat,
                                   kAudioUnitScope_Input,
                                   0,
                                   &monoStreamFormat,
                                   sizeof (monoStreamFormat)
                                   );
    
	return result;
}

// Starting with instantiated audio processing graph, configure its
// audio units, initialize it, and start it.
OSStatus ConfigureAndStartAudioProcessingGraph() {
    
    OSStatus result = noErr;
       
        result = AudioUnitInitialize (ioUnit);
        if(result != noErr) NSLog(@"Unable to initialize the I/O unit. Error code: %d %@", (int) result, statToString(result));
        
        // Set the I/O unit's output sample rate.
        result =    AudioUnitSetProperty (
                                         ioUnit,
                                         kAudioUnitProperty_SampleRate,
                                         kAudioUnitScope_Output,
                                         0,
                                         &sampleRate,
                                         sizeof(sampleRate)
                                         );
        
        //if(result != noErr) NSLog(@"AudioUnitSetProperty (set Sampler unit output stream sample rate). Error code: %d %@", (int) result, statToString(result));
        
        // Obtain the value of the maximum-frames-per-slice from the I/O unit.
       // result =    AudioUnitGetProperty (
       //                                   ioUnit,
        //                                  kAudioUnitProperty_MaximumFramesPerSlice,
        //                                  kAudioUnitScope_Global,
         //                                 0,
         //                                 &maximumFramesPerSlice,
         //                                 sizeof(maximumFramesPerSlice)
         //                                 );
        
        if(result != noErr) NSLog(@"Unable to retrieve the maximum frames per slice property from the I/O unit. Error code: %d %@", (int) result, statToString(result));
    
    for(int i = 0; i<channelCount; i++){
        // Set the Sampler unit's maximum frames-per-slice.
        result =    AudioUnitSetProperty (
                                          samplerUnits[i],
                                          kAudioUnitProperty_MaximumFramesPerSlice,
                                          kAudioUnitScope_Global,
                                          0,
                                          &maximumFramesPerSlice,
                                          sizeof(maximumFramesPerSlice)
                                          );
        result = AudioUnitSetProperty (
                                       samplerUnits[i],
                                       kAudioUnitProperty_StreamFormat,
                                       kAudioUnitScope_Input,
                                       0,
                                       &monoStreamFormat,
                                       sizeof (monoStreamFormat)
                                       );
        // Set the Sampler unit's output sample rate.
        result =    AudioUnitSetProperty (
                                          samplerUnits[i],
                                          kAudioUnitProperty_SampleRate,
                                          kAudioUnitScope_Output,
                                          0,
                                          &sampleRate,
                                          sizeof (sampleRate)
                                          );
        
        if(result != noErr) NSLog(@"AudioUnitSetProperty (set Sampler unit output stream sample rate). Error code: %d %@", (int) result, statToString(result));
        
        if(result != noErr) NSLog(@"AudioUnitSetProperty (set Sampler unit maximum frames per slice). Error code: %d %@", (int) result, statToString(result));

        }
    
    if (processingGraph) {
        // Initialize the audio processing graph.
        result = AUGraphInitialize (processingGraph);
        if(result != noErr) NSLog(@"Unable to initialze AUGraph object. Error code: %d %@", (int) result, statToString(result));
        
        // Start the graph
        result = AUGraphStart (processingGraph);
        if(result != noErr) NSLog(@"Unable to start audio processing graph. Error code: %d %@", (int) result, statToString(result));
        
        // Print out the graph to the console
        CAShow (processingGraph);
    }
    NSLog(@"ConfigureAndStartProcessingGraph OK");
   
    return result;
}

// Stop the audio processing graph
OSStatus StopAudioProcessingGraph () {
    Boolean isRunning = false;
    OSStatus result = AUGraphIsRunning (processingGraph, &isRunning);
    if (isRunning) {
        result = AUGraphStop (processingGraph);
    }
    return result;
}

// Restart the audio processing graph
OSStatus RestartAudioProcessingGraph() {
    
    OSStatus result = noErr;
    if (processingGraph) result = AUGraphStart (processingGraph);

    if(result != noErr) NSLog(@"Unable to restart the audio processing graph. Error code: %d %@", (int) result, statToString(result));
    return result;
}

// Set up the audio session for this app.
OSStatus SetupAudioSession () {
    
    OSStatus result = noErr;
    AVAudioSession *mySession = [AVAudioSession sharedInstance];
    
    // Specify that this object is the delegate of the audio session, so that
    //    this object's endInterruption method will be invoked when needed.
    //[mySession setDelegate: self];
    
    // Assign the Playback category to the audio session. This category supports
    //    audio output with the Ring/Silent switch in the Silent position.
    NSError *audioSessionError = nil;
    [mySession setCategory: AVAudioSessionCategoryPlayback error: &audioSessionError];
    if (audioSessionError != nil) {NSLog (@"Error setting audio session category."); return NO;}    
    
    [mySession setPreferredSampleRate: sampleRate error: &audioSessionError];
    if (audioSessionError != nil) {NSLog (@"Error setting preferred hardware sample rate."); return NO;}
    
    // Activate the audio session
    [mySession setActive: YES error: &audioSessionError];
    if (audioSessionError != nil) {NSLog (@"Error activating the audio session."); return NO;}
    
    //[mySession setPreferredIOBufferDuration: 1.0 error: &audioSessionError ];
    //if (audioSessionError != nil) {NSLog (@"Error setPreferredIOBufferDuration."); return NO;}
    
    // Obtain the actual hardware sample rate and store it for later use in the audio processing graph.
    // sampleRate = [mySession sampleRate];
    NSLog(@"SetupAudioSession OK");
    return result;
}

extern "C" {
int StartSoftSynth(int freq, int channels, int mode){
    if(synthInitialized) return 0;
    sampleRate = (double)freq;
    channelCount = channels;
    softMode = mode;
    
    size_t bytesPerSample = sizeof (AudioUnitSampleType);
    
    monoStreamFormat.mFormatID          = kAudioFormatLinearPCM;
    monoStreamFormat.mFormatFlags       = kAudioFormatFlagsAudioUnitCanonical;
    monoStreamFormat.mBytesPerPacket    = bytesPerSample;
    monoStreamFormat.mFramesPerPacket   = 1;
    monoStreamFormat.mBytesPerFrame     = bytesPerSample;
    monoStreamFormat.mChannelsPerFrame  = 1;                  // 1 indicates mono
    monoStreamFormat.mBitsPerChannel    = 8 * bytesPerSample;
    monoStreamFormat.mSampleRate        = sampleRate;
    
    OSStatus result;
    SetupAudioSession();
    result = CreateAUGraph ();
    result = ConfigureAndStartAudioProcessingGraph ();
    
    for(int i = 0;i < channelCount; i++) SetBankAndPresetLoadFile(0,i);
    
    synthInitialized = true;
    NSLog(@"StartSoftSynth OK");
    home:
    if (result != noErr) NSLog (@"CreateSoftSynth(), Error code: %d %@\n", (int) result, statToString(result)); 	
    return result;
}

int StopSoftSynth(){
    OSStatus result = noErr;
    if(!synthInitialized) return 0;
    result = StopAudioProcessingGraph();
    ioUnit = NULL;
    result = AUGraphClose(processingGraph);
    processingGraph = NULL;
    samplerUnits.clear();
    samplerNodes.clear();
    synthInitialized = false;
    NSLog(@"StopSoftSynth OK");
home:
    if (result != noErr) NSLog (@"DisposeSoftSynth(), Error code: %d %@\n", (int) result, statToString(result)); 
	return result;
}
    
int SendSynthMessage(int Command, int Data1, int Data2){
    OSStatus result = noErr;
    if(synthInitialized){
       if((Command & 15) < channelCount){
           if((Command >> 4) == 0xC){
               SetBankAndPreset(Data1, Command & 15);
           } else {
               result = MusicDeviceMIDIEvent(samplerUnits[Command & 15], Command >> 4 << 4 | 0, Data1, Data2, 0);
               if (result != noErr) NSLog (@"SendMessage(). Error code: %d %@", (int) result, statToString(result));

           }
       }
    } else {
        NSLog(@"Synthesiser not initialized!");
    }
    return 1;
}
}
