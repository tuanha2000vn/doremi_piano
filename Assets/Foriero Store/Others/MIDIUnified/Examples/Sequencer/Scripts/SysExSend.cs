using UnityEngine;
using System.Collections;
using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Plugins;

public class SysExSend : MonoBehaviour
{
	void Start ()
	{
		if (MidiInput.singleton)
			MidiInput.singleton.SysExMessageEvent += SysExMessage;
	}

	void OnDestroy ()
	{
		if (MidiInput.singleton)
			MidiInput.singleton.SysExMessageEvent -= SysExMessage;
	}

	#pragma warning disable 436
	void SysExMessage (MidiMessage midiMessage)
	{
		Debug.Log ("SYS EX MESSAGE");
		
		byte[] d = midiMessage.GetData ();
		dataStr = "";
		for (int i = 0; i < d.Length; i++) {
			dataStr += d [i].ToString () + " ";	
		}
	}
	#pragma warning restore 436

	string dataStr = "";

	void OnGUI ()
	{
		if (GUILayout.Button ("Send SYS EX")) {
			byte[] data = new byte[6] { 0xF0, 1, 2, 3, 4, 0xF7 };
			MidiOUTPlugin.SendData (data);
		}
		GUILayout.Label ("DATA :" + dataStr);
		
	}
}
