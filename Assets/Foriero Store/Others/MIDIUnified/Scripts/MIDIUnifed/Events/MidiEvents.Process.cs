using UnityEngine;
using System.Collections;

namespace ForieroEngine.MIDIUnified{
	
	public partial class MidiEvents {
			
		public void AddShortMessage(int aCommand, int aData1, int aData2)
	    {
			OnMidiRawMessageEvent(aCommand, aData1, aData2);
			ProcessMidiMessage(aCommand, aData1, aData2);
		}
		
		public void AddNoteOn(int midiIndex, int volume, int channel){
			AddShortMessage(channel + (int)CommandEnum.NoteOn, midiIndex, volume);
		}
		
		public void AddNoteOff(int midiIndex, int channel){
			AddShortMessage(channel + (int)CommandEnum.NoteOff, midiIndex, 0);
		}
			
		private void ProcessMidiMessage(int aCommand, int aData1, int aData2){
		    int channel = aCommand.ToMidiChannel();
	        int command = aCommand.ToMidiCommand();
	        switch (command)
	        {
	            case 0x80: //NoteOff
					OnNoteOffEvent(aData1, aData2, channel);
	                break;
	            case 0x90: //NoteOn
	                if(aData2 == 0) OnNoteOffEvent(aData1, aData2, channel);
				    else OnNoteOnEvent(aData1, aData2, channel);
	                break;
	            case 0xA0: //NoteAftertouch
	                break;
	            case 0xB0: //Controller
	                {
	                    OnControllerEvent((ControllerEnum)aData1, aData2, channel);
						switch (aData1)
	                    {
	                        case 0x7B: //Note Off All
	                            	OnAllNotesOffEvent(ControllerEnum.AllNotesOff, aData2, channel);
	                            break;
	                        case 0x07: //Channel Volume
	                            	OnMainVolumeEvent(ControllerEnum.MainVolume, aData2, channel);
	                            break;
	                        case 0x0A: //Pan
	                            	OnPanEvent(ControllerEnum.Pan, aData2, channel);
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
	                          		OnResetControllersEvent(ControllerEnum.ResetControllers, aData2, channel);
	                            break;
							case 0x40:	// Right Pedal
								if(aData2 > 0){
									OnPedalOnEvent(PedalEnum.Right, aData2, channel);
								}
								else {
									OnPedalOffEvent(PedalEnum.Right, aData2, channel);
								}
								break;
							case 0x42:	// Center Pedal
								if(aData2 > 0) {
									OnPedalOnEvent(PedalEnum.Center, aData2, channel);
								}
								else {
									OnPedalOffEvent(PedalEnum.Center, aData2, channel);
								}
								break;
							case 0x43:	// Left Pedal
								if(aData2 > 0) {
									OnPedalOnEvent(PedalEnum.Left, aData2, channel);
								}
								else {
									OnPedalOnEvent(PedalEnum.Left, aData2, channel);
								}
								break;
	                        default:
	                            return;
	                    }
	                }
	                break;
	            case 0xC0: //Program Change - Instrument Change
	               		OnProgramChangedEvent(aData1, aData2, channel);
	                break;
	            case 0xD0: //Channel Aftertouch
						OnChannelAfterTouchEvent(aData1, aData2, channel);
	                break;
	            case 0xE0: //Pitch Bend
						OnPitchBendEnvet(aData1, aData2, channel);
	                break;
				case 0xFF:
					Debug.Log("METAMESSAGE");
					switch(aData1){
					case 0x51:
						Debug.Log("TEMPO CHANGE :" + aData2.ToString()); 
					break;
					}
					break;
	            default:
	                return;
	        }
	    }		
	}
}
