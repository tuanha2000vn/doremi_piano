using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace ForieroEngine.MIDIUnified.Plugins
{
    class MidiINDeviceNONE : IMidiINDevice
    {
        #region implementation

        public bool Init()
        {
            return true;
        }

        public int ConnectDevice(int deviceIndex, bool editor = false)
        {
            return -1;
        }

        public void DisconnectDevice(int deviceId, bool editor = false)
        {

        }

        public void DisconnectDevices(bool editor = false)
        {

        }

        public string GetDeviceName(int deviceIndex)
        {
            return "";
        }

        public int GetDeviceCount()
        {
            return 0;
        }

        public int PopMessage(out MidiMessage midiMessage, bool editor = false)
        {
            midiMessage = new MidiMessage();
            return 0;
        }

        #endregion
    }

    class MidiOUTDeviceNONE : IMidiOUTDevice
    {

        #region implementation

        public bool Init()
        {
            return true;
        }

        public int ConnectDevice(int deviceIndex, bool editor = false)
        {
            return -1;
        }

        public void DisconnectDevice(int deviceId, bool editor = false)
        {

        }

        public void DisconnectDevices(bool editor = false)
        {

        }

        public string GetDeviceName(int deviceIndex)
        {
            return "";
        }

        public int GetDeviceCount()
        {
            return 0;
        }

        public int SendMessage(byte command, byte data1, byte data2, bool editor = false)
        {
            return 0;
        }

        public int SendData(byte[] data, bool editor = false)
        {
            return 0;
        }

        #endregion
    }
}