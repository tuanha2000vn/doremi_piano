#if !UNITY_EDITOR && UNITY_IOS

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ForieroEngine.MIDIUnified.Plugins
{
	class MidiINDeviceIOS: IMidiINDevice
	{

#region external

		[DllImport ("__Internal")]
		private static extern int MidiIn_PortOpen (int i);

		[DllImport ("__Internal")]
		private static extern void MidiIn_PortClose (int deviceId);

		[DllImport ("__Internal")]
		private static extern void MidiIn_PortCloseAll ();

		[DllImport ("__Internal")]
		private static extern string MidiIn_PortName (int i);

		[DllImport ("__Internal")]
		private static extern int MidiIn_PortCount ();

		[DllImport ("__Internal")]
		public static extern bool MidiIn_PortAdded ();

		[DllImport ("__Internal")]
		public static extern bool MidiIn_PortRemoved ();

		[DllImport ("__Internal")]
		private static extern int MidiIn_PopMessage (out MidiMessage packet);
				
		//[DllImport ("__Internal")]
		//public static extern void EnableNetwork(bool enabled);

#endregion

#region implementation

		public bool Init(){
			return true;
		}

		public int ConnectDevice (int deviceIndex, bool editor = false)
		{
			return MidiIn_PortOpen (deviceIndex);
		}

		public void DisconnectDevice (int deviceId, bool editor = false)
		{
			MidiIn_PortClose (deviceId);
		}

		public void DisconnectDevices (bool editor = false)
		{
			MidiIn_PortCloseAll ();
		}

		public string GetDeviceName (int deviceIndex)
		{
			return MidiIn_PortName (deviceIndex);
		}

		public int GetDeviceCount ()
		{
			return MidiIn_PortCount ();
		}

		public int PopMessage (out MidiMessage midiMessage, bool editor = false)
		{
			return MidiIn_PopMessage (out midiMessage);
		}

#endregion
	}

	class MidiOUTDeviceIOS: IMidiOUTDevice
	{
#region external

		[DllImport ("__Internal")]
		private static extern int MidiOut_PortOpen (int i);

		[DllImport ("__Internal")]
		private static extern void MidiOut_PortClose (int deviceId);

		[DllImport ("__Internal")]
		private static extern void MidiOut_PortCloseAll ();

		[DllImport ("__Internal")]
		private static extern string MidiOut_PortName (int i);

		[DllImport ("__Internal")]
		private static extern int MidiOut_PortCount ();

		[DllImport ("__Internal")]
		private static extern int MidiOut_SendMessage (int Command, int Data1, int Data2);

		[DllImport ("__Internal")]
		private static extern int MidiOut_SendData (byte[] Data, int DataSize);

		[DllImport ("__Internal")]
		public static extern bool MidiOut_PortAdded ();

		[DllImport ("__Internal")]
		public static extern bool MidiOut_PortRemoved ();

		[DllImport ("__Internal")]
		public static extern void EnableNetwork (bool enabled);

#endregion

#region implementation

		public bool Init(){
			return true;
		}

		public int ConnectDevice (int deviceIndex, bool editor = false)
		{
			return MidiOut_PortOpen (deviceIndex);
		}

		public void DisconnectDevice (int deviceId, bool editor = false)
		{
			MidiOut_PortClose (deviceId);
		}

		public void DisconnectDevices (bool editor = false)
		{
			MidiOut_PortCloseAll ();
		}

		public string GetDeviceName (int deviceIndex)
		{
			return MidiOut_PortName (deviceIndex);
		}

		public int GetDeviceCount ()
		{
			return MidiOut_PortCount ();
		}

		public int SendMessage (byte command, byte data1, byte data2, bool editor = false)
		{
			return MidiOut_SendMessage (command, data1, data2);
		}

		public int SendData (byte[] data, bool editor = false)
		{
			return MidiOut_SendData (data, data.Length);	
		}

#endregion
	}
}

#endif
