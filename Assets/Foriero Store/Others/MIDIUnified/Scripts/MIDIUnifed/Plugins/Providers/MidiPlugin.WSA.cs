using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using ForieroEngine.Extensions;

#if !UNITY_EDITOR && UNITY_WSA
using ForieroEngine.MIDIUnified.Plugins.Internal;

namespace ForieroEngine.MIDIUnified.Plugins
{
	class MidiINDeviceWSA : IMidiINDevice
	{

		public bool Init (){
			return MidiInPluginWSA.Initialized();
		}

		public int ConnectDevice (int i, bool editor = false)
		{
			return MidiInPluginWSA.ConnectDevice (i);
		}

		public void DisconnectDevice (int deviceId, bool editor = false)
		{
			MidiInPluginWSA.DisconnectDevice (deviceId);
		}

		public void DisconnectDevices (bool editor = false)
		{
			MidiInPluginWSA.DisconnectDevices ();
		}

		public string GetDeviceName (int deviceIndex)
		{
			return MidiInPluginWSA.GetDeviceName (deviceIndex);
		}

		public int GetDeviceCount ()
		{
			return MidiInPluginWSA.GetDeviceCount ();
		}

		public int PopMessage (out MidiMessage midiMessage, bool editor = false)
		{
			return MidiInPluginWSA.PopMessage (out midiMessage);
		}

	}

	class MidiOUTDeviceWSA : IMidiOUTDevice
	{
		
		public bool Init (){
			return MidiOutPluginWSA.Initialized();
		}

		public int ConnectDevice (int i, bool editor = false)
		{
			return MidiOutPluginWSA.ConnectDevice (i);
		}

		public void DisconnectDevice (int deviceId, bool editor = false)
		{
			MidiOutPluginWSA.DisconnectDevice (deviceId);
		}

		public void DisconnectDevices (bool editor = false)
		{
			MidiOutPluginWSA.DisconnectDevices ();
		}

		public string GetDeviceName (int deviceIndex)
		{
			return MidiOutPluginWSA.GetDeviceName (deviceIndex);
		}

		public int GetDeviceCount ()
		{
			return MidiOutPluginWSA.GetDeviceCount ();
		}

		public int SendMessage (byte command, byte data1, byte data2, bool editor = false)
		{
			return MidiOutPluginWSA.SendMessage (command, data1, data2);
		}

		public int SendData (byte[] data, bool editor = false)
		{
			return MidiOutPluginWSA.SendData (data);	
		}

	}
}

#endif
