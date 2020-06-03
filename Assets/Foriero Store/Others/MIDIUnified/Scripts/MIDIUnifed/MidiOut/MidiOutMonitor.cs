using UnityEngine;
using System.Collections;
using ForieroEngine.MIDIUnified;

[AddComponentMenu("MIDIUnified/Listeners/MidiOutMonitor")]
public class MidiOutMonitor : MonoBehaviour, IMidiEvents {
	
	public static MidiOutMonitor singleton;
	
	public bool rawMessage = true;
	public bool command = true;
	public bool channel = true;
	public bool noteOn = true;
	public bool noteOff = true;
	public bool pedalOn = true;
	public bool pedalOff = true;
	
	public event ShortMessageEventHandler ShortMessageEvent;
	
	
	MidiEvents midiEvents = new MidiEvents();
	ShortMessageEventHandler shortMessageEventHandler;
	
	void Awake(){
		if(singleton != null) {
			Debug.Log("MIDI OUT MONITOR already in scene.");
			Destroy(this);
			return;
		}
		singleton = this;
		shortMessageEventHandler = new ShortMessageEventHandler(ShortMessage);
		MidiOut.ShortMessageEvent += shortMessageEventHandler;
		if(noteOn) midiEvents.NoteOnEvent += NoteOn;
		if(noteOff) midiEvents.NoteOffEvent += NoteOff;
		if(pedalOn) midiEvents.PedalOnEvent += PedalOn;
		if(pedalOff) midiEvents.PedalOffEvent += PedalOff;
	}
	
	void OnDestroy(){
		MidiOut.ShortMessageEvent -= shortMessageEventHandler;	
	}
	
	void ShortMessage(int aCommand, int aData1, int aData2){
		if(rawMessage) Debug.Log(string.Format("RawComand : {0}\tCommand : {1}\tChannel : {2}\tData1 : {3}\tData2 : {4}",aCommand, aCommand.ToMidiCommand(), aCommand.ToMidiChannel(), aData1, aData2));
		if(command)  Debug.Log("Command : " +  aCommand.ToMidiCommand().ToString());
		if(channel) Debug.Log("Channel : " + aCommand.ToMidiChannel().ToString());
		midiEvents.AddShortMessage(aCommand, aData1, aData2);
		if(ShortMessageEvent !=  null) ShortMessageEvent(aCommand, aData1, aData2);
	}
	
	void NoteOn(int aNote, int aVolume, int aChannel){
		Debug.Log(string.Format("Note ON : {0} Volume : {1} Channel : {2}", aNote, aVolume, aChannel));	
	}
	
	void NoteOff(int aNote, int aVolume, int aChannel){
		Debug.Log(string.Format("Note OFF : {0}  Channel : {1}", aNote, aChannel));	
	}
	
	void PedalOn(PedalEnum aPedal, int aValue, int aChannel){
		Debug.Log("Pedal ON : " + aPedal.ToString() + " Channel : " + aChannel);
	}
	
	void PedalOff(PedalEnum aPedal, int aValue, int aChannel){
		Debug.Log("Pedal OFF : " + aPedal.ToString() + " Channel : " + aChannel);
	}
	
	
}
