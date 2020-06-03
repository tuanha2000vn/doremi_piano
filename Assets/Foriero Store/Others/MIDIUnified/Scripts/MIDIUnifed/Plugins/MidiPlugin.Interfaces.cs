using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ForieroEngine.MIDIUnified.Plugins
{
    public interface IMidiDevice
    {
        bool Init();

        int ConnectDevice(int deviceIndex, bool editor = false);

        void DisconnectDevice(int deviceId, bool editor = false);

        void DisconnectDevices(bool editor = false);

        string GetDeviceName(int deviceIndex);

        int GetDeviceCount();
    }

    public interface IMidiEditorDevice
    {
        int GetConnectedDeviceCount();

        int GetConnectedDeviceId(int connectedDeviceIndex);

        string GetConnectedDeviceName(int connectedDeviceIndex);

        bool GetConnectedDeviceIsEditor(int connectedDeviceIndex);
    }

    public interface IMidiINDevice : IMidiDevice
    {
        int PopMessage(out MidiMessage midiMessage, bool editor = false);
    }

    public interface IMidiOUTDevice : IMidiDevice
    {
        int SendMessage(byte command, byte data1, byte data2, bool editor = false);

        int SendData(byte[] data, bool editor = false);
    }

    public interface ISynthPlugin
    {

    }
}
