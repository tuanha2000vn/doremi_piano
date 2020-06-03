using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using ForieroEngine.MIDIUnified;

public class MidiInstrumentUI : MonoBehaviour
{
	public Dropdown channelDropdown;
	public Dropdown instrumentDropdown;

	void Awake ()
	{
		string[] names = Enum.GetNames (typeof(ProgramEnum));
		Array.Sort (names);
		instrumentDropdown.AddOptions (new List<string> (names));
	}

	public void OnInstrumentChange ()
	{
		Debug.Log (instrumentDropdown.value);

		string channel = channelDropdown.options [channelDropdown.value].text;
		string instrument = instrumentDropdown.options [instrumentDropdown.value].text;

		if (channel == "All") {
			for (int i = 0; i < 16; i++) {
				MidiOut.SetInstrument ((ProgramEnum)Enum.Parse (typeof(ProgramEnum), instrument), (ChannelEnum)i);
			}
		} else {
			MidiOut.SetInstrument ((ProgramEnum)Enum.Parse (typeof(ProgramEnum), instrument), (ChannelEnum)(int.Parse (channel) - 1));
		}
	}

	public void OnChannelChange ()
	{
		Debug.Log (channelDropdown.value);

	}
}

