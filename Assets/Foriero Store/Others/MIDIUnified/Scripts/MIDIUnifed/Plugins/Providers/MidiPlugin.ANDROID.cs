/* 
* 	(c) Copyright Marek Ledvina, Foriero Studo
*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ForieroEngine.MIDIUnified.Plugins
{
#if UNITY_ANDROID && !UNITY_EDITOR
    class MidiINDeviceANDROID : IMidiINDevice
    {
        public bool Init()
        {
            MidiPlugin.Init();
            return true;
        }

        public int ConnectDevice(int deviceIndex, bool editor = false)
        {
            int result = MidiPlugin.MIDIIN_ConnectDevice(deviceIndex);

            return result;
        }

        public void DisconnectDevice(int id, bool editor = false)
        {
            MidiPlugin.MIDIIN_DisconnectDevice(id);
        }

        public void DisconnectDevices(bool editor = false)
        {
            MidiPlugin.MIDIIN_DisconnectDevices();
        }

        public string GetDeviceName(int deviceIndex)
        {
            return MidiPlugin.MIDIIN_DeviceName(deviceIndex);
        }

        public int GetDeviceCount()
        {
            return MidiPlugin.MIDIIN_DeviceCount();
        }

        public int PopMessage(out MidiMessage midiMessage, bool editor = false)
        {
            return MidiPlugin.MIDIIN_PopMessage(out midiMessage);
        }
    }

    class MidiOUTDeviceANDROID : IMidiOUTDevice
    {
        public bool Init()
        {
            MidiPlugin.Init();
            return true;
        }

        public int ConnectDevice(int deviceIndex, bool editor = false)
        {
            return MidiPlugin.MIDIOUT_ConnectDevice(deviceIndex);
        }

        public void DisconnectDevice(int id, bool editor = false)
        {
            MidiPlugin.MIDIOUT_DisconnectDevice(id);
        }

        public void DisconnectDevices(bool editor = false)
        {
            MidiPlugin.MIDIOUT_DisconnectDevices();
        }

        public string GetDeviceName(int deviceIndex)
        {
            return MidiPlugin.MIDIOUT_DeviceName(deviceIndex);
        }

        public int GetDeviceCount()
        {
            return MidiPlugin.MIDIOUT_DeviceCount();
        }

        public int SendMessage(byte command, byte data1, byte data2, bool editor = false)
        {
            int rawData = (int)command + ((int)data1 << 8) + ((int)data2 << 16);

            return MidiPlugin.MIDIOUT_SendMidiMessage(rawData);
        }

        public int SendData(byte[] data, bool editor = false)
        {
            if (data == null)
            {
                return -1;
            }
            else
            {
                return MidiPlugin.MIDIOUT_SendData(data);
            }
        }
    }

    static class MidiPlugin
    {

        private static bool _initCalled = false;

        private static AndroidJavaObject _midiPlugin = null;
        internal static bool _isInitialized = false;

        static Queue<MidiMessage> _midiMessages = new Queue<MidiMessage>(100);

        public static bool Init()
        {
            if (_initCalled)
            {
                return _isInitialized;
            }

            _initCalled = true;

            AndroidJavaClass jc = null;

            jc = new AndroidJavaClass("com.foriero.midiunifiedplugin.MIDIUnifiedFragment");

            if (jc != null)
            {
                _midiPlugin = jc.CallStatic<AndroidJavaObject>("Init");
                _isInitialized = _midiPlugin != null;
            }

            return _isInitialized;
        }

        // MIDI IN //

        public static int MIDIIN_ConnectDevice(int deviceIndex)
        {
            if (!_isInitialized) return -1;
            return _midiPlugin.Call<int>("MIDIIN_ConnectDevice", new object[] { deviceIndex });
        }

        public static void MIDIIN_DisconnectDevice(int id)
        {
            if (!_isInitialized) return;
            _midiPlugin.Call("MIDIIN_DisconnectDevice", new object[] { id });
        }

        public static void MIDIIN_DisconnectDevices()
        {
            if (!_isInitialized) return;

            _midiPlugin.Call("MIDIIN_DisconnectDevices");
        }

        public static string MIDIIN_DeviceName(int deviceIndex)
        {
            if (!_isInitialized) return "";
            return _midiPlugin.Call<String>("MIDIIN_DeviceName", new object[] { deviceIndex });
        }

        public static int MIDIIN_DeviceCount()
        {
            if (!_isInitialized) return 0;
            return _midiPlugin.Call<int>("MIDIIN_DeviceCount");
        }

        public static int MIDIIN_PopMessage(out MidiMessage aMidiMessage)
        {
            aMidiMessage = new MidiMessage();
			if (!_isInitialized)
	            return 0;

            var javaObject = _midiPlugin.Call<AndroidJavaObject>("MIDIIN_PopMidiMessages");
			if (javaObject != null)
            {
	            var javaObjectPtr = javaObject.GetRawObject();
				if (javaObjectPtr.ToInt32() != 0)
                {
	                var queuedMessages = AndroidJNIHelper.ConvertFromJNIArray<int[]>(javaObjectPtr);
                    foreach (var m in queuedMessages)
                    {
						aMidiMessage.command = Convert.ToByte((m>>16) & 0xFF);
                        aMidiMessage.data1 = Convert.ToByte((m >> 8) & 0xFF);
                        aMidiMessage.data2 = Convert.ToByte(m & 0xFF);
                        aMidiMessage.dataSize = 3;
                        _midiMessages.Enqueue(aMidiMessage);
                    }
                }
            }

            if (_midiMessages.Count > 0)
            {
                aMidiMessage = _midiMessages.Dequeue();
                return 1;
            } else {
    			return 0;
            }
		}

        // MIDI OUT //

        public static int MIDIOUT_ConnectDevice(int deviceIndex)
        {
            if (!_isInitialized) return -1;
            return _midiPlugin.Call<int>("MIDIOUT_ConnectDevice", new object[] { deviceIndex });
        }

        public static void MIDIOUT_DisconnectDevice(int id)
        {
            if (!_isInitialized) return;
            _midiPlugin.Call("MIDIOUT_DisconnectDevice", new object[] { id });
        }

        public static void MIDIOUT_DisconnectDevices()
        {
            if (!_isInitialized) return;
            _midiPlugin.Call("MIDIOUT_DisconnectDevices");
        }

        public static string MIDIOUT_DeviceName(int deviceIndex)
        {
            if (!_isInitialized) return "";
            return _midiPlugin.Call<String>("MIDIOUT_DeviceName", new object[] { deviceIndex });
        }

        public static int MIDIOUT_DeviceCount()
        {
            if (!_isInitialized) return 0;
            return _midiPlugin.Call<int>("MIDIOUT_DeviceCount");
        }

        public static int MIDIOUT_SendMidiMessage(int midiMessage)
        {
            if (!_isInitialized) return -1;
            _midiPlugin.Call("MIDIOUT_SendMidiMessage", new object[] { midiMessage });
            return 1;
        }

        public static int MIDIOUT_SendData(byte[] data)
        {
            if (!_isInitialized) return -1;
            _midiPlugin.Call("MIDIOUT_SendData", new object[] { data });
            return 1;
        }
    }
#endif
}
