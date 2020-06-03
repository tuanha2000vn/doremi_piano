using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ForieroEngine.MIDIUnified.Plugins
{
    public class MidiOUTPlugin
    {
        public static IMidiOUTDevice midiOUTDevice;

        public static IMidiEditorDevice midiEditorDevice;

#if UNITY_EDITOR
        [InitializeOnLoad]
        class InitEditor
        {
            static InitEditor()
            {
                if (!Application.isPlaying)
                {
                    Init();
                }
            }
        }
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InitPlayer()
        {
            if (Application.isPlaying)
            {
                Init();
            }
        }

        public static void Init()
        {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_WIN || UNITY_EDITOR_LINUX || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
            MidiOUTDeviceSTANDALONE device = new MidiOUTDeviceSTANDALONE();
#elif UNITY_IOS
			MidiOUTDeviceIOS device = new MidiOUTDeviceIOS();
#elif UNITY_WSA
            MidiOUTDeviceWSA device = new MidiOUTDeviceWSA();
#elif UNITY_WEBGL
			MidiOUTDeviceWEBGL device = new MidiOUTDeviceWEBGL();
#elif UNITY_ANDROID
            MidiOUTDeviceANDROID device = new MidiOUTDeviceANDROID();
#else
			MidiOUTDeviceNONE device = new MidiOUTDeviceNONE();
#endif
            midiDevice = device as IMidiDevice;
            midiOUTDevice = device as IMidiOUTDevice;
            midiEditorDevice = device as IMidiEditorDevice;

            initialized = midiDevice.Init();

            if (!initialized)
            {
                Debug.LogError("MidiOUTDevice not initialized");
                return;
            }

            Refresh();
        }

        public static bool initialized = false;

        public static List<MidiDevice> connectedDevices = new List<MidiDevice>();
        public static List<string> deviceNames = new List<string>();
        static IMidiDevice midiDevice;

        public static List<MidiDevice> connectedEditorDevices = new List<MidiDevice>();

#if UNITY_EDITOR
        public static void StoreEditorConnections()
        {
            List<string> connectionNames = new List<string>();
            foreach (MidiDevice device in connectedEditorDevices)
            {
                connectionNames.Add(device.name);
            }
            string s = string.Join(";", connectionNames.ToArray());
            EditorPrefs.SetString("MIDI_OUT_NAMES", s);
        }

        public static void RestoreEditorConnections()
        {
            string names = EditorPrefs.GetString("MIDI_OUT_NAMES", "").Trim();
            string[] midiInNames = string.IsNullOrEmpty(names) ? new string[0] : names.Split(';');

            foreach (string midiInName in midiInNames)
            {
                MidiDevice midiDevice = ConnectDeviceByName(midiInName, true);
                if (midiDevice == null)
                {
                    Debug.LogError("Could not conned midi in device : " + midiInName);
                }
            }
        }
#endif

        public static void StoreConnections()
        {
            List<string> connectionNames = new List<string>();
            foreach (MidiDevice device in connectedDevices)
            {
                connectionNames.Add(device.name);
            }
            string s = string.Join(";", connectionNames.ToArray());
            PlayerPrefs.SetString("MIDI_OUT_NAMES", s);
        }

        public static void RestoreConnections()
        {
            string names = PlayerPrefs.GetString("MIDI_OUT_NAMES", "").Trim();
            string[] midiInNames = string.IsNullOrEmpty(names) ? new string[0] : names.Split(';');

            foreach (string midiInName in midiInNames)
            {
                MidiDevice midiDevice = ConnectDeviceByName(midiInName);
                if (midiDevice == null)
                {
                    Debug.LogError("Could not conned midi in device : " + midiInName);
                }
            }
        }

        public static void Refresh()
        {
            if (midiDevice == null)
                return;

            deviceNames = new List<string>();
            for (int i = 0; i < midiDevice.GetDeviceCount(); i++)
            {
                deviceNames.Add(midiDevice.GetDeviceName(i));
            }

            if (midiEditorDevice != null)
            {
                connectedEditorDevices = new List<MidiDevice>();
                for (int i = 0; i < midiEditorDevice.GetConnectedDeviceCount(); i++)
                {
                    if (midiEditorDevice.GetConnectedDeviceIsEditor(i))
                    {
                        MidiDevice md = new MidiDevice();
                        md.name = midiEditorDevice.GetConnectedDeviceName(i);
                        md.deviceId = midiEditorDevice.GetConnectedDeviceId(i);
                        md.isEditor = true;
                        connectedEditorDevices.Add(md);
                    }
                }
            }
        }

        public static bool Initialized()
        {
            if (midiDevice == null)
                return false;

            return initialized;
        }

        public static MidiDevice ConnectDevice(int deviceIndex, bool editor = false)
        {
            if (midiDevice == null)
                return null;

            foreach (MidiDevice md in MidiINPlugin.connectedDevices)
            {
                if (md.name == midiDevice.GetDeviceName(deviceIndex))
                {
                    Debug.LogError("Preventing infinite loop. To have same device connected as IN and OUT is not allowed.");
                    return null;
                }
            }

            int deviceId = midiDevice.ConnectDevice(deviceIndex, editor);

            if (editor)
            {
                foreach (MidiDevice cd in connectedEditorDevices)
                {
                    if (deviceId == cd.deviceId)
                    {
                        Debug.LogError("Editor device already connected");
                        return cd;
                    }
                }
            }
            else
            {
                foreach (MidiDevice cd in connectedDevices)
                {
                    if (deviceId == cd.deviceId)
                    {
                        Debug.LogError("Device already connected");
                        return cd;
                    }
                }
            }

            MidiDevice connectedDevice = new MidiDevice
            {
                deviceId = deviceId,
                name = GetDeviceName(deviceIndex),
                isEditor = editor
            };

            if (editor)
            {
                connectedEditorDevices.Add(connectedDevice);
            }
            else
            {
                connectedDevices.Add(connectedDevice);
            }

            return connectedDevice;
        }

        public static void DisconnectDevices(bool editor = false)
        {
            if (midiDevice == null)
                return;

            if (editor)
            {
                connectedEditorDevices = new List<MidiDevice>();
            }
            else
            {
                connectedDevices = new List<MidiDevice>();
            }

            midiDevice.DisconnectDevices(editor);
        }


        public static MidiDevice ConnectDeviceByName(string deviceName, bool editor = false)
        {
            if (midiDevice == null)
                return null;

            foreach (MidiDevice md in MidiINPlugin.connectedDevices)
            {
                if (md.name == deviceName)
                {
                    Debug.LogError("Preventing infinite loop. To have same device connected as IN and OUT is not allowed.");
                    return null;
                }
            }

            for (int i = 0; i < GetDeviceCount(); i++)
            {
                if (deviceName == GetDeviceName(i))
                {
                    return ConnectDevice(i, editor);
                }
            }

            return null;
        }

        public static void DisconnectDevice(MidiDevice connectedDevice)
        {
            if (midiDevice == null || connectedDevice == null)
                return;

            if (connectedDevice.isEditor)
            {
                for (int i = connectedEditorDevices.Count - 1; i >= 0; i--)
                {
                    if (connectedDevice.deviceId == connectedEditorDevices[i].deviceId)
                    {
                        connectedEditorDevices.RemoveAt(i);
                        break;
                    }
                }
            }
            else
            {
                for (int i = connectedDevices.Count - 1; i >= 0; i--)
                {
                    if (connectedDevice.deviceId == connectedDevices[i].deviceId)
                    {
                        connectedDevices.RemoveAt(i);
                        break;
                    }
                }
            }

            midiDevice.DisconnectDevice(connectedDevice.deviceId, connectedDevice.isEditor);
        }

        public static void DisconnectDeviceByName(string deviceName, bool editor = false)
        {
            if (midiDevice == null)
                return;

            MidiDevice disconnectDevice = null;

            if (editor)
            {
                for (int i = connectedEditorDevices.Count - 1; i >= 0; i--)
                {
                    if (deviceName == connectedEditorDevices[i].name)
                    {
                        disconnectDevice = connectedEditorDevices[i];
                        break;
                    }
                }
            }
            else
            {
                for (int i = connectedDevices.Count - 1; i >= 0; i--)
                {
                    if (deviceName == connectedDevices[i].name)
                    {
                        disconnectDevice = connectedDevices[i];
                        break;
                    }
                }
            }

            if (disconnectDevice != null)
            {
                DisconnectDevice(disconnectDevice);
            }
        }

        public static string GetDeviceName(int deviceIndex)
        {
            if (midiDevice == null)
                return "";

            return midiDevice.GetDeviceName(deviceIndex);
        }

        public static int GetDeviceCount()
        {
            if (midiDevice == null)
                return 0;

            return midiDevice.GetDeviceCount();
        }

        public static int SendShortMessage(byte command, byte data1, byte data2, bool editor = false)
        {
            if (midiOUTDevice == null)
                return -1;

            //Debug.Log (command + " " + DateTime.Now.Millisecond.ToString ());

            return midiOUTDevice.SendMessage(command, data1, data2, editor);
        }

        public static int SendData(byte[] data, bool editor = false)
        {
            if (midiOUTDevice == null)
                return -1;

            return midiOUTDevice.SendData(data, editor);
        }

        public static int GetConnectedDeviceCount()
        {
            if (midiEditorDevice == null)
                return 0;

            return midiEditorDevice.GetConnectedDeviceCount();
        }

    }
}