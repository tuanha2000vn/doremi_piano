#import "BASS24Binding.h"
#import "BASS24Wrapper.h"

int BASS24Start(DWORD freq, DWORD channels){
     NSLog(@"BASS24Start");
    [softsynth sharedSoftsynth].channels = channels;
    [softsynth sharedSoftsynth].frequence = freq;
    return [[softsynth sharedSoftsynth] Start ];
}

int BASS24Stop(){
     NSLog(@"BASS24Stop");
    return [[softsynth sharedSoftsynth] Stop ];
}

int BASS24SendMidiMessage(DWORD Command, DWORD Data1, DWORD Data2){
    //NSLog(@"BASS24SendMidiMessage");
    return [[softsynth sharedSoftsynth] SendMidiMessage:Command:Data1:Data2 ];
}

