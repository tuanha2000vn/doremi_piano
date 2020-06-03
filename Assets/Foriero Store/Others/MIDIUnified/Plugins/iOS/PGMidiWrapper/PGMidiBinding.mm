/*
 *  midiunified.cp
 *  midiunified
 *
 *  Created by Marek Ledvina on 7/27/12.
 *  Copyright (c) 2012 Marek Ledvina. All rights reserved.
 *
 */

#import "PGMidiBinding.h"
#import "PGMidiWrapper.h"

#include <vector>
#include <string>
#include <list>

class MidiHelper{
    public :
    int deviceId = 0;
    PGMidiSource * rtIN = NULL;
    PGMidiDestination * rtOUT = NULL;
    
    MidiHelper() { static int id = 0; deviceId = id++; }
};

std::vector<MidiHelper *> INs;
std::vector<MidiHelper *> OUTs;

int  MidiIn_PortOpen(int i){
    PGMidiSource * source = [[[PGMidiWrapper sharedPGMidiManager] midi].sources objectAtIndex:i];
    
    for(int i = 0; i<INs.size(); i++){
        if(INs[i]->rtIN == source){
            return INs[i]->deviceId;
            break;
        }
    }
    
    [[PGMidiWrapper sharedPGMidiManager] addMidiSourceDelegate:source];

    MidiHelper * hl = new MidiHelper();
    hl->rtIN = source;
    INs.push_back(hl);
    
    return hl->deviceId;
}

void MidiIn_PortClose(int deviceId){
    for (std::vector<MidiHelper*>::iterator hl = INs.begin(); hl != INs.end();)
    {
        if (deviceId == (*hl)->deviceId) {
            [[PGMidiWrapper sharedPGMidiManager] removeMidiSourceDelegate:(*hl)->rtIN];
            delete(*hl);
            hl = INs.erase(hl);
            break;
        } else {
            ++hl;
        }
    }
}

void MidiIn_PortCloseAll(){
    for (std::vector<MidiHelper*>::iterator hl = INs.begin(); hl != INs.end();)
    {
        [[PGMidiWrapper sharedPGMidiManager] removeMidiSourceDelegate:(*hl)->rtIN];
        delete(*hl);
        hl = INs.erase(hl);
    }
}

const char * MidiIn_PortName(int i){
    NSLog(@"Midi In Name : %d",i);
    PGMidiSource * source = [[[PGMidiWrapper sharedPGMidiManager] midi].sources objectAtIndex:i] ;
    return strdup([[source name] UTF8String]);
}

int MidiIn_PortCount(){
    NSLog(@"Midi In Count : %d",[[PGMidiWrapper sharedPGMidiManager] midi].sources.count);
    return [[PGMidiWrapper sharedPGMidiManager] midi].sources.count;
}

int MidiIn_PopMessage (MidiMessage * message){
    if ([[PGMidiWrapper sharedPGMidiManager] midiMessageSize] >0)
    {
        MidiMessageInternal msg = [[PGMidiWrapper sharedPGMidiManager] getMidiMessage];
        
        message->command = msg.command;
        message->data1 = msg.data1;
        message->data2 = msg.data2;
        message->dataSize = msg.dataSize;
        message->data = msg.data;
        message->deviceId = 0;
        
        for (std::vector<MidiHelper*>::iterator hl = INs.begin(); hl != INs.end();hl++)
        {
            if((*hl)->rtIN == msg.source){
                message->deviceId = (*hl)->deviceId;
            }
        }
        
        return 1;
    } else {
        return 0;
    }
    
    return 0;
}

int MidiOut_PortOpen(int i){
    PGMidiDestination * destination = [[[PGMidiWrapper sharedPGMidiManager] midi].destinations objectAtIndex:i];
    
    for(int i = 0; i<OUTs.size(); i++){
        if(OUTs[i]->rtOUT == destination){
            return OUTs[i]->deviceId;
            break;
        }
    }
    
    MidiHelper * hl = new MidiHelper();
    hl->rtOUT = destination;
    OUTs.push_back(hl);
    
    return hl->deviceId;
}

void MidiOut_PortClose(int deviceId){
    for (std::vector<MidiHelper*>::iterator hl = OUTs.begin(); hl != OUTs.end();)
    {
        if (deviceId == (*hl)->deviceId) {
            delete(*hl);
            hl = OUTs.erase(hl);
            break;
        } else {
            ++hl;
        }
    }
}

void MidiOut_PortCloseAll(){
    for (std::vector<MidiHelper*>::iterator hl = OUTs.begin(); hl != OUTs.end();)
    {
        delete((*hl));
        hl = OUTs.erase(hl);
    }
}

const char * MidiOut_PortName(int i){
    NSLog(@"Midi Out Name : %d",i);
    PGMidiDestination *destination = [[[PGMidiWrapper sharedPGMidiManager] midi].destinations objectAtIndex:i];
    return strdup([[destination name] UTF8String]);
}

int MidiOut_PortCount(){
    NSLog(@"Midi Out Count : %d",[[PGMidiWrapper sharedPGMidiManager] midi].destinations.count);
    return [[PGMidiWrapper sharedPGMidiManager] midi].destinations.count;
}

int MidiOut_SendMessage(int Command, int Data1, int Data2){
    for (std::vector<MidiHelper*>::iterator hl = OUTs.begin(); hl != OUTs.end();hl++){
        const UInt8 msg[] = { (Byte)Command, (Byte)Data1, (Byte)Data2 };
        [(*hl)->rtOUT sendBytes:msg size:sizeof(msg)];
        return 1;
    }
    
    return 0;
}

int MidiOut_SendData(UInt8 * data, int dataSize){
    for (std::vector<MidiHelper*>::iterator hl = OUTs.begin(); hl != OUTs.end();hl++){
        [(*hl)->rtOUT sendBytes:data size:dataSize];
        return 1;
    }
    
    return 0;
}

void EnableNetwork(bool enabled){
    //[[PGMidiWrapper sharedManager] midi enableNetwork:enabled];
}

bool MidiIn_PortAdded(){
    bool result = [[PGMidiWrapper sharedPGMidiManager] sourceDeviceAdded];
    return result;
}

bool MidiIn_PortRemoved(){
    bool result = [[PGMidiWrapper sharedPGMidiManager] sourceDeviceRemoved];
    return result;
}

bool MidiOut_PortAdded(){
    bool result = [[PGMidiWrapper sharedPGMidiManager] destinationDeviceAdded];
    return result;
}

bool MidiOut_PortRemoved(){
    bool result = [[PGMidiWrapper sharedPGMidiManager] destinationDeviceRemoved];
    return result;
}







