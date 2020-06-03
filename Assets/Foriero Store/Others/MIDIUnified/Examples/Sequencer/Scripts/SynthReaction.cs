using UnityEngine;
using System.Collections;
using ForieroEngine.MIDIUnified;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SynthReaction : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
	public NoteEnum note = NoteEnum.C;
	public AccidentalEnum accidental = AccidentalEnum.None;
	public OctaveEnum octave = OctaveEnum.Octave4;
	public ChannelEnum channel = ChannelEnum.C0;

	public int value = 80;

	#region IPointerUpHandler implementation

	public void OnPointerUp (PointerEventData eventData)
	{
		MidiOut.NoteOff (note, accidental, octave, channel);	
	}

	#endregion

	#region IPointerDownHandler implementation

	public void OnPointerDown (PointerEventData eventData)
	{
		MidiOut.NoteOn (note, accidental, octave, value, channel);	
	}

	#endregion
}
