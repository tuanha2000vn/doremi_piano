using System;
using ForieroEngine.MIDIUnified.Plugins;
using UnityEngine;
using UnityEngine.UI;

public class ButtonMidi : MonoBehaviour
{
    public Button Button;
    public Text Text;

    // Use this for initialization
    void Start()
    {
        Button.onClick.AddListener(HandleClick);
    }

    public void HandleClick()
    {
        try
        {
            //Debug.Log("HandleClick " + Button.name);
            if (Button.name.Contains("IN CONNECT"))
            {
                Debug.Log("MidiINPlugin.ConnectDeviceByName(" + Text.text + ")");
                MidiINPlugin.ConnectDeviceByName(Text.text);
            }

            if (Button.name.Contains("IN DISCONNECT"))
            {
                Debug.Log("MidiINPlugin.DisconnectDeviceByName(" + Text.text + ")");
                MidiINPlugin.DisconnectDeviceByName(Text.text);
            }

            if (Button.name.Contains("OUT CONNECT"))
            {
                Debug.Log("MidiOUTPlugin.ConnectDeviceByName(" + Text.text + ")");
                MidiOUTPlugin.ConnectDeviceByName(Text.text);
            }

            if (Button.name.Contains("OUT DISCONNECT"))
            {
                Debug.Log("MidiOUTPlugin.DisconnectDeviceByName(" + Text.text + ")");
                MidiOUTPlugin.DisconnectDeviceByName(Text.text);
            }

            GameObject.Find("PanelSetting").GetComponent<PanelSetting>().GetMidiInOut();
        }
        catch (Exception e)
        {
            Debug.LogWarning("HandleClick " + e);
        }
    }
}