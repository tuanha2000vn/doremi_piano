using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ForieroEngine.MIDIUnified{
	
	public partial class MidiEvents {
		
		public delegate void MidiRawMessageHandler(int aCommand, int aData1, int aData2);
		public delegate void NoteEventHandler(int aMidiId, int aValue, int aChannel);
		public delegate void PedalEventHandler(PedalEnum aPedal, int aValue, int aChannel);
		public delegate void ControllerEventHandler(ControllerEnum aControllerCommand, int aValue, int aChannel);
		
		public event MidiRawMessageHandler MidiRawMessageEvent;
		public event NoteEventHandler NoteOnEvent;
		public event NoteEventHandler NoteOffEvent;
		public event NoteEventHandler NoteAfterTouchEvent;
		public event NoteEventHandler ProgramChangedEvent;
		public event NoteEventHandler ChannelAfterTouchEvent;
		public event NoteEventHandler PitchBendEnvet;
		public event PedalEventHandler PedalOnEvent;
		public event PedalEventHandler PedalOffEvent;
		public event ControllerEventHandler AllNotesOffEvent;
		public event ControllerEventHandler MainVolumeEvent;
		public event ControllerEventHandler PanEvent;
		public event ControllerEventHandler ModulationEvent;
		public event ControllerEventHandler ResetControllersEvent;
		public event ControllerEventHandler ControllerEvent;
		
		
		void OnControllerEvent(ControllerEnum aControllerCommand, int aValue, int aChannel){
			if(ControllerEvent != null){
				ControllerEvent(aControllerCommand, aValue, aChannel);	
			}
		}
		
		void OnMidiRawMessageEvent(int aCommand, int aData1, int aData2){
			if(MidiRawMessageEvent != null){
				MidiRawMessageEvent(aCommand, aData1, aData2);	
			}
		}
		
		void OnResetControllersEvent(ControllerEnum aControllerCommand, int aValue, int aChannel){
			if(ResetControllersEvent != null){
				ResetControllersEvent(aControllerCommand, aValue, aChannel);	
			}
		}
		
		void OnModulationEvent(ControllerEnum aControllerCommand, int aValue, int aChannel){
			if(ModulationEvent != null){
				ModulationEvent(aControllerCommand, aValue, aChannel);	
			}
		}
		
		void OnPanEvent(ControllerEnum aControllerCommand, int aValue, int aChannel){
			if(PanEvent != null){
				PanEvent(aControllerCommand, aValue, aChannel);	
			}
		}
		
		void OnMainVolumeEvent(ControllerEnum aControllerCommand, int aValue, int aChannel){
			if(MainVolumeEvent != null){
				MainVolumeEvent(aControllerCommand, aValue, aChannel);	
			}
		}
		
		void OnAllNotesOffEvent(ControllerEnum aControllerCommand, int aValue, int aChannel){
			if(AllNotesOffEvent != null){
				AllNotesOffEvent(aControllerCommand, aValue, aChannel);	
			}
		}
		
		void OnPitchBendEnvet(int aMidiId, int aValue, int aChannel){
			if(PitchBendEnvet != null){
				PitchBendEnvet(aMidiId, aValue, aChannel);	
			}
		}
		
		void OnChannelAfterTouchEvent(int aMidiId, int aValue, int aChannel){
			if(ChannelAfterTouchEvent != null){
				ChannelAfterTouchEvent(aMidiId, aValue, aChannel);	
			}
		}
		
		void OnProgramChangedEvent(int aMidiId, int aValue, int aChannel){
			if(ProgramChangedEvent != null){
				ProgramChangedEvent(aMidiId, aValue, aChannel);	
			}
		}
		
		void OnNoteAfterTouchEvent(int aMidiId, int aValue, int aChannel){
			if(NoteAfterTouchEvent != null){
				NoteAfterTouchEvent(aMidiId, aValue, aChannel);	
			}
		}
		
		void OnNoteOnEvent(int aMidiId, int aValue, int aChannel){
			if(NoteOnEvent != null){
				NoteOnEvent(aMidiId, aValue, aChannel);
			}
		}
	
		void OnNoteOffEvent(int aMidiId, int aValue, int aChannel){
			if(NoteOffEvent != null){
				NoteOffEvent(aMidiId, aValue, aChannel);
			} 
		}
	
		void OnPedalOnEvent(PedalEnum aPedal, int aValue, int aChannel){
			if(PedalOnEvent != null){
				PedalOnEvent(aPedal, aValue, aChannel);
			}
		}
	
		void OnPedalOffEvent(PedalEnum aPedal, int aValue, int aChannel){
			if(PedalOffEvent != null){
				PedalOffEvent(aPedal, aValue, aChannel);
			}
		}
	}
}
