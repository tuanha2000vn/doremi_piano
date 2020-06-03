using UnityEngine;
using System.Collections;
using ForieroEngine.MIDIUnified;

public class FlyingObject : MonoBehaviour
{
	
	public int midiIndex = 60;
	public int volume = 80;
	public bool midiOut = true;
	public ChannelEnum channel = ChannelEnum.C0;

	public Rigidbody rigidBody;

	public delegate void MidiNoteEventHandler (int aMidiId, int aValue, int aChannel);

	public event MidiNoteEventHandler NoteOn;
	public event MidiNoteEventHandler NoteOff;

	void OnEnable ()
	{
		rigidBody = GetComponent<Rigidbody> ();
	}

	void OnNoteOn (int aMidiIdx, int aValue)
	{
		if (NoteOn != null) {
			NoteOn (aMidiIdx, aValue, (int)channel);
		}
	}

	void OnNoteOff (int aMidiIdx, int aValue)
	{
		if (NoteOff != null) {
			NoteOff (aMidiIdx, aValue, (int)channel);
		}
	}

	void OnMouseDown ()
	{
		OnNoteOn (midiIndex, volume);
		if (midiOut && channel != ChannelEnum.None) {
			MidiOut.NoteOn (midiIndex, volume, (int)channel);
		}
	}

	void OnMouseUp ()
	{
		OnNoteOff (midiIndex, 0);
		if (midiOut && channel != ChannelEnum.None) {
			MidiOut.NoteOff (midiIndex, (int)channel);
		}
	}
}
