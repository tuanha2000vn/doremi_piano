#if UNITY_EDITOR_OSX || UNITY_EDITOR_WIN || UNITY_EDITOR_LINUX || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX

using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace ForieroEngine.MIDIUnified.Plugins
{

    class MidiINDeviceSTANDALONE : IMidiINDevice, IMidiEditorDevice
    {
        #region external

        [DllImport("rtmidi")]
        private static extern int MidiIn_PortOpen(int deviceIndex, bool editor);

        [DllImport("rtmidi")]
        private static extern void MidiIn_PortClose(int deviceId, bool editor);

        [DllImport("rtmidi")]
        private static extern void MidiIn_PortCloseAll(bool editor);

        [DllImport("rtmidi")]
        private static extern string MidiIn_PortName(int i);

        [DllImport("rtmidi")]
        private static extern int MidiIn_PortCount();

        [DllImport("rtmidi")]
        private static extern int MidiIn_PopMessage(out MidiMessage midiMessage, bool editor);

        [DllImport("rtmidi")]
        public static extern int MidiIn_GetConnectedDeviceCount();

        [DllImport("rtmidi")]
        public static extern int MidiIn_GetConnectedDeviceId(int connectedDeviceIndex);

        [DllImport("rtmidi")]
        public static extern string MidiIn_GetConnectedDeviceName(int connectedDeviceIndex);

        [DllImport("rtmidi")]
        public static extern bool MidiIn_GetConnectedDeviceIsEditor(int connectedDeviceIndex);

        #endregion

        #region implementation


        public bool Init()
        {
            return true;
        }

        public int ConnectDevice(int deviceIndex, bool editor = false)
        {
            return MidiIn_PortOpen(deviceIndex, editor);
        }

        public void DisconnectDevice(int deviceId, bool editor = false)
        {
            MidiIn_PortClose(deviceId, editor);
        }

        public void DisconnectDevices(bool editor = false)
        {
            MidiIn_PortCloseAll(editor);
        }

        public string GetDeviceName(int deviceIndex)
        {
            return MidiIn_PortName(deviceIndex);
        }

        public int GetDeviceCount()
        {
            return MidiIn_PortCount();
        }

        public int PopMessage(out MidiMessage midiMessage, bool editor = false)
        {
            return MidiIn_PopMessage(out midiMessage, editor);
        }

        public int GetConnectedDeviceCount()
        {
            return MidiIn_GetConnectedDeviceCount();
        }

        public int GetConnectedDeviceId(int connectedDeviceIndex)
        {
            return MidiIn_GetConnectedDeviceId(connectedDeviceIndex);
        }

        public string GetConnectedDeviceName(int connectedDeviceIndex)
        {
            return MidiIn_GetConnectedDeviceName(connectedDeviceIndex);
        }

        public bool GetConnectedDeviceIsEditor(int connectedDeviceIndex)
        {
            return MidiIn_GetConnectedDeviceIsEditor(connectedDeviceIndex);
        }

        #endregion
    }

    class MidiOUTDeviceSTANDALONE : IMidiOUTDevice, IMidiEditorDevice
    {
        #region external

        [DllImport("rtmidi")]
        private static extern int MidiOut_PortOpen(int deviceIndex, bool editor);

        [DllImport("rtmidi")]
        private static extern void MidiOut_PortClose(int deviceId, bool editor);

        [DllImport("rtmidi")]
        private static extern void MidiOut_PortCloseAll(bool editor);

        [DllImport("rtmidi")]
        private static extern string MidiOut_PortName(int deviceIndex);

        [DllImport("rtmidi")]
        private static extern int MidiOut_PortCount();

        [DllImport("rtmidi")]
        private static extern int MidiOut_SendMessage(int command, int data1, int data2, bool editor);

        [DllImport("rtmidi")]
        private static extern int MidiOut_SendData(byte[] Data, int dataSize, bool editor);

        [DllImport("rtmidi")]
        public static extern int MidiOut_GetConnectedDeviceCount();

        [DllImport("rtmidi")]
        public static extern int MidiOut_GetConnectedDeviceId(int connectedDeviceIndex);

        [DllImport("rtmidi")]
        public static extern string MidiOut_GetConnectedDeviceName(int connectedDeviceIndex);

        [DllImport("rtmidi")]
        public static extern bool MidiOut_GetConnectedDeviceIsEditor(int connectedDeviceIndex);

        #endregion

        #region implementation

        public bool Init()
        {
            return true;
        }

        public int ConnectDevice(int deviceIndex, bool editor = false)
        {
            return MidiOut_PortOpen(deviceIndex, editor);
        }

        public void DisconnectDevice(int deviceId, bool editor = false)
        {
            MidiOut_PortClose(deviceId, editor);
        }

        public void DisconnectDevices(bool editor = false)
        {
            MidiOut_PortCloseAll(editor);
        }

        public string GetDeviceName(int deviceIndex)
        {
            return MidiOut_PortName(deviceIndex);
        }

        public int GetDeviceCount()
        {
            return MidiOut_PortCount();
        }

        public int SendMessage(byte command, byte data1, byte data2, bool editor = false)
        {
            return MidiOut_SendMessage(command, data1, data2, editor);
        }

        public int SendData(byte[] data, bool editor = false)
        {
            return MidiOut_SendData(data, data.Length, editor);
        }

        public int GetConnectedDeviceCount()
        {
            return MidiOut_GetConnectedDeviceCount();
        }

        public int GetConnectedDeviceId(int connectedDeviceIndex)
        {
            return MidiOut_GetConnectedDeviceId(connectedDeviceIndex);
        }

        public string GetConnectedDeviceName(int connectedDeviceIndex)
        {
            return MidiOut_GetConnectedDeviceName(connectedDeviceIndex);
        }

        public bool GetConnectedDeviceIsEditor(int connectedDeviceIndex)
        {
            return MidiOut_GetConnectedDeviceIsEditor(connectedDeviceIndex);
        }

        #endregion
    }
}

#endif
