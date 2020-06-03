using System;
using System.Collections;
using System.Collections.Generic;
using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Plugins;
using UnityEngine;
using UnityEngine.UI;

public class PanelSetting : MonoBehaviour
{
    public delegate void ChangeLanguagueEvent();

    public static event ChangeLanguagueEvent OnChangeLanguage;


    public GameObject ButtonMidi;
    public GameObject PanelMidiIn;
    public GameObject PanelMidiOut;
    public GameObject PanelLanguageSelect;
    public GameObject buttonUseMicrophone;
    public GameObject ButtonUseMIDI;
    public GameObject PanelInputButton;
    public GameObject PanelKeyboardType;

    private IEnumerator coroutineGetMidiInIe;
    private IEnumerator coroutineGetMidiOutIe;
    private List<string> listConnectedMidiIn;
    private List<string> listConnectedMidiOut;
    private bool MidiInConnected = false;

    void OnEnable()
    {
        Debug.Log("Start PanelSetting Animation");
        SetInputButtonColor();
        SetLanguageButtonColor();
        GetMidiInOut();
    }

    public void GetMidiInOut()
    {
        try
        {
            GetMidiIn();
            GetMidiOut();
        }
        catch (Exception e)
        {
            Debug.LogWarning("GetMidiInOut " + e);
        }
    }

    public void GetMidiOut()
    {
        if (coroutineGetMidiOutIe != null)
        {
            StopCoroutine(coroutineGetMidiOutIe);
        }

        if (PanelMidiIn.activeSelf)
        {
            coroutineGetMidiOutIe = GetMidiOutIe();
            StartCoroutine(coroutineGetMidiOutIe);
        }
    }

    IEnumerator GetMidiOutIe()
    {
        yield return new WaitUntil(() =>
            MIDI.initialized
        );


        listConnectedMidiOut = new List<string>();
        //Debug.Log("MidiOUTPlugin.connectedDevices " + MidiOUTPlugin.connectedDevices.Count);
        foreach (MidiDevice md in MidiOUTPlugin.connectedDevices)
        {
            listConnectedMidiOut.Add(md.name);
            //Debug.Log("MidiINPlugin.connectedDevices " + md.name);
        }

        //Debug.LogWarning("GetMidiOutIe");
        foreach (Transform child in PanelMidiOut.transform)
        {
            Destroy(child.gameObject);
        }

        var buttonNoMidi = Instantiate(ButtonMidi, Vector3.zero, Quaternion.identity);
        buttonNoMidi.transform.SetParent(PanelMidiOut.transform);
        buttonNoMidi.transform.localScale = Vector3.one;
        buttonNoMidi.name = "OUT No Midi Out Detected";
        buttonNoMidi.GetComponentInChildren<Text>().text = "No Midi Out Detected";
        var color = buttonNoMidi.GetComponent<Image>().color;
        color.a = 0.2f;
        buttonNoMidi.GetComponent<Image>().color = color;


        for (int i = 0; i < MidiOUTPlugin.GetDeviceCount(); i++)
        {
            if (MidiOUTPlugin.GetDeviceName(i).Contains("Session"))
            {
                continue;
            }

            buttonNoMidi.SetActive(false);
            var button = Instantiate(ButtonMidi, Vector3.zero, Quaternion.identity);
            button.transform.SetParent(PanelMidiOut.transform);
            button.transform.localScale = Vector3.one;
            button.GetComponentInChildren<Text>().text = MidiOUTPlugin.GetDeviceName(i).Trim();

            if (listConnectedMidiOut.Contains(MidiOUTPlugin.GetDeviceName(i)))
            {
                button.name = "OUT DISCONNECT " + MidiOUTPlugin.GetDeviceName(i);
                color = button.GetComponent<Image>().color;
                color.a = 1f;
                button.GetComponent<Image>().color = color;
            }
            else
            {
                button.name = "OUT CONNECT " + MidiOUTPlugin.GetDeviceName(i);
                color = button.GetComponent<Image>().color;
                color.a = 0.2f;
                button.GetComponent<Image>().color = color;
            }

            //Debug.LogWarning(i + ". GetMidiOutIe " + MidiOUTPlugin.GetDeviceName(i) + "");
        }
    }

    public void GetMidiIn()
    {
        if (coroutineGetMidiInIe != null)
        {
            StopCoroutine(coroutineGetMidiInIe);
        }

        if (PanelMidiIn.activeSelf)
        {
            coroutineGetMidiInIe = GetMidiInIe();
            StartCoroutine(coroutineGetMidiInIe);
        }
    }

    IEnumerator GetMidiInIe()
    {
        yield return new WaitUntil(() =>
            MIDI.initialized
        );

        listConnectedMidiIn = new List<string>();
        //Debug.Log("MidiINPlugin.connectedDevices " + MidiINPlugin.connectedDevices.Count);
        foreach (MidiDevice md in MidiINPlugin.connectedDevices)
        {
            listConnectedMidiIn.Add(md.name);
            //Debug.Log("MidiINPlugin.connectedDevices " + md.name);
        }

        //Debug.LogWarning("GetMidiInIe");
        foreach (Transform child in PanelMidiIn.transform)
        {
            Destroy(child.gameObject);
        }

        var buttonNoMidi = Instantiate(ButtonMidi, Vector3.zero, Quaternion.identity);
        buttonNoMidi.transform.SetParent(PanelMidiIn.transform);
        buttonNoMidi.transform.localScale = Vector3.one;
        buttonNoMidi.name = "IN No Midi In Detected";
        if (LocalizationManager.instance != null)
        {
            buttonNoMidi.GetComponentInChildren<Text>().text =
                LocalizationManager.instance.GetLocalizedValue("No Midi In Detected");
        }
        else
        {
            buttonNoMidi.GetComponentInChildren<Text>().text = "No Midi In Detected";
        }

        var color = buttonNoMidi.GetComponent<Image>().color;
        color.a = 0.2f;
        buttonNoMidi.GetComponent<Image>().color = color;

        for (int i = 0; i < MidiINPlugin.GetDeviceCount(); i++)
        {
            if (MidiOUTPlugin.GetDeviceName(i).Contains("Session"))
            {
                continue;
            }

            buttonNoMidi.SetActive(false);
            var button = Instantiate(ButtonMidi, Vector3.zero, Quaternion.identity);
            button.transform.SetParent(PanelMidiIn.transform);
            button.transform.localScale = Vector3.one;
            button.GetComponentInChildren<Text>().text = MidiINPlugin.GetDeviceName(i).Trim();

            if (listConnectedMidiIn.Contains(MidiINPlugin.GetDeviceName(i)))
            {
                button.name = "IN DISCONNECT " + MidiINPlugin.GetDeviceName(i);
                color = button.GetComponent<Image>().color;
                color.a = 1f;
                button.GetComponent<Image>().color = color;
            }
            else
            {
                button.name = "IN CONNECT " + MidiINPlugin.GetDeviceName(i);
                color = button.GetComponent<Image>().color;
                color.a = 0.2f;
                button.GetComponent<Image>().color = color;
            }
        }
    }

    private void SetButtonColor(GameObject button, Color color)
    {
        var image = button.GetComponent<Image>();
        image.color = color;
    }

    public void LanguageClicked(string language)
    {
        Debug.Log("LangueClicked " + language);

        if (PlayerPrefs.GetString("language") != language)
        {
            PlayerPrefs.SetString("language", language);
            if (OnChangeLanguage != null)
            {
                OnChangeLanguage();
                //var mainMenu = GetComponentInParent<MainMenu>();
                //mainMenu.HideSubPanel();
                //mainMenu.SetBoxUp(true);
            }

            SetLanguageButtonColor();
        }
    }

    private void SetLanguageButtonColor()
    {
        foreach (Transform button in PanelLanguageSelect.transform)
        {
            if (button.name == PlayerPrefs.GetString("language"))
            {
                SetButtonColor(button.gameObject, Helpers.ColorBlue10);
            }
            else
            {
                SetButtonColor(button.gameObject, Helpers.ColorBlue05);
            }
        }
    }

    private void SetInputButtonColor()
    {
        if (Helpers.UseMicrophone)
        {
            SetButtonColor(buttonUseMicrophone, Helpers.ColorBlue10);
            SetButtonColor(ButtonUseMIDI, Helpers.ColorBlue05);
            PanelInputButton.SetActive(false);
        }
        else
        {
            SetButtonColor(buttonUseMicrophone, Helpers.ColorBlue05);
            SetButtonColor(ButtonUseMIDI, Helpers.ColorBlue10);
            GetMidiIn();
            PanelInputButton.SetActive(true);
        }
    }

    public void SetInput(bool UseMicrophone)
    {
        Debug.Log("SetInput " + UseMicrophone);
        Helpers.UseMicrophone = UseMicrophone;
        SetInputButtonColor();
    }
}