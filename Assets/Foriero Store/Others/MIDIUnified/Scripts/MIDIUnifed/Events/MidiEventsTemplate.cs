using UnityEngine;
using System.Collections;
using ForieroEngine.MIDIUnified;

[AddComponentMenu ("MIDIUnified/Listeners/MidiEventsTemplate")]
public class MidiEventsTemplate : MonoBehaviour
{

	// if you have draged your objects here, you can still enable or disable the hook //
	public bool hookMidiOut = true;
	public bool hookSequencer = false;
	public bool hookKeyboard = false;
	public bool hookInput = false;
	public bool hookPlaymaker = false;
	public bool hookSynthSequencer = false;
	
	// this is on what you can hook up //
	public MidiOutMonitor midiOutMonitor;
	public MidiSeqKaraokeScript midiSequencerScript;
	public MidiKeyboardInput midiKeyboardInput;
	public MidiInput midiInput;
	public MidiPlayMakerInput midiPlayMakerInput;
	
	public bool rawMidi = true;
	public bool noteOn = true;
	public bool noteOff = true;
	public bool pedalOn = true;
	public bool pedalOff = true;
	
	// This class helps us to parse raw midi messages //
	MidiEvents midiEvents = new MidiEvents ();
	
	
	// here we are establishing hooks //
	void Awake ()
	{
		if (hookMidiOut)
			midiEvents.AddGenerator (midiOutMonitor);
		if (hookSequencer)
			midiEvents.AddGenerator (midiSequencerScript);
		if (hookInput)
			midiEvents.AddGenerator (midiInput);
		if (hookKeyboard)
			midiEvents.AddGenerator (midiKeyboardInput);
		if (hookPlaymaker)
			midiEvents.AddGenerator (midiPlayMakerInput);
		
		// this is optional depending if you want to hook up on notes //
		midiEvents.NoteOnEvent += NoteOn;
		midiEvents.NoteOffEvent += NoteOff;
		midiEvents.PedalOnEvent += PedalOn;
		midiEvents.PedalOffEvent += PedalOff;
		midiEvents.MidiRawMessageEvent += RawMidi;
		midiEvents.ControllerEvent += ControllerEventHandler;
	
	}
	
	// here destroying all the hooks //
	void OnDestroy ()
	{
		midiEvents.NoteOnEvent -= NoteOn;
		midiEvents.NoteOffEvent -= NoteOff;
		midiEvents.PedalOnEvent -= PedalOn;
		midiEvents.PedalOffEvent -= PedalOff;
		midiEvents.MidiRawMessageEvent -= RawMidi;
		midiEvents.ControllerEvent -= ControllerEventHandler;
	}

	
	void RawMidi (int aCommand, int aData1, int aData2)
	{
		if (rawMidi) {
			Debug.Log (string.Format ("Command : {0} Data1 : {1} Data2 : {2}", aCommand, aData1, aData2));
			// how to get channel? //
			// how to get command? //
			int channel = (aCommand & 0xF);
			int command = (aCommand >> 4);
			Debug.Log (channel);
			Debug.Log (command);
			//MidiOut.NoteOn(60,AccidentalEnum.Flat, OctaveEnum.Octave4);		
		}
	}
	
	// event implementation //
	void NoteOn (int aNote, int aVolume, int aChannel)
	{
		if (noteOn)
			Debug.Log (string.Format ("Note ON : {0} Volume : {1} Channel : {2}", aNote, aVolume, aChannel));	
	}

	void NoteOff (int aNote, int aVolume, int aChannel)
	{
		if (noteOff)
			Debug.Log (string.Format ("Note OFF : {0}  Channel : {1}", aNote, aChannel));	
	}

	void PedalOn (PedalEnum aPedal, int aValue, int aChannel)
	{
		if (pedalOn)
			Debug.Log ("Pedal ON : " + aPedal.ToString () + " Channel : " + aChannel);
	}

	void PedalOff (PedalEnum aPedal, int aValue, int aChannel)
	{
		if (pedalOff)
			Debug.Log ("Pedal ON : " + aPedal.ToString () + " Channel : " + aChannel);
	}

	void ControllerEventHandler (ControllerEnum aControllerCommand, int aValue, int aChannel)
	{
		switch ((int)aControllerCommand) {
		case 21:
			
			break;
		}
	}
}
