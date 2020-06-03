using UnityEngine;
using System.Collections;
using ForieroEngine.MIDIUnified;

public class MidiInstruments : MonoBehaviour
{
	
	public bool ignoreProgramMessages = false;

	[SortedEnumPopup]
	public ProgramEnum[] instruments = new ProgramEnum[16] {
		ProgramEnum.None,
		ProgramEnum.None,
		ProgramEnum.None,
		ProgramEnum.None,
		ProgramEnum.None,
		ProgramEnum.None,
		ProgramEnum.None,
		ProgramEnum.None,
		ProgramEnum.None,
		ProgramEnum.None,
		ProgramEnum.None,
		ProgramEnum.None,
		ProgramEnum.None,
		ProgramEnum.None,
		ProgramEnum.None,
		ProgramEnum.None
	};

	void SetInstruments ()
	{
		for (int i = 0; i < instruments.Length; i++) {
			if (instruments [i] != ProgramEnum.None) {
				MidiOut.SetInstrument (instruments [i], (ChannelEnum)i);
			}
		}	
	}

	void Awake ()
	{
		MidiOut.ignoreProgramMessages = ignoreProgramMessages;	
	}

	// Use this for initialization
	IEnumerator Start ()
	{
		yield return new WaitUntil (() =>
			MIDI.initialized
		);

		MidiOut.ignoreProgramMessages = false;
		SetInstruments ();
		MidiOut.ignoreProgramMessages = ignoreProgramMessages;
	}
}
