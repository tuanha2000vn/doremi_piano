using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelInstrumentSelect : MonoBehaviour
{
    public GameObject PanelGrid;
    public GameObject ButtonInstrument;
    public GameObject TextInstrument;
    public Image ButtonInstrumentPlayImage;
    public Image ButtonInstrumentLeft;
    public Image ButtonInstrumentRight;
    public Image ButtonInstrumentVoice;
    private Dictionary<string, string> dictIntrument;
    private IEnumerator coroutine;

    // Use this for initialization
    void Start()
    {
        gameObject.SetActive(true);
        foreach (Transform button in PanelGrid.transform)
        {
            Destroy(button.transform.gameObject);
        }

        TextInstrument.GetComponent<Text>().text = Helpers.InstrumentSelectMode;

        dictIntrument = new Dictionary<string, string>
        {
            {"AcousticGrandPiano", "Grand Piano"}, //0
            //{"BrightAcousticPiano", "Bright Piano"},//1
            {"ElectricGrandPiano", "Electric Piano"}, //2
            {"Celesta", "Celesta"}, //8
            {"Glockenspiel", "Glockenspiel"}, //9
            {"Accordion", "Accordion"}, //21
            {"Harmonica", "Harmonica"}, //22
            {"AcousticGuitarNylon", "Guitar Nylon"}, //24
            {"AcousticGuitarSteel", "Guitar Steel"}, //25
            //{"ElectricGuitarClean", "Electric Guitar"},//27
            {"Violin", "Violin"}, //40
            //{"Contrabass", "Contrabass"},//43
            {"StringEnsemble1", "String Ensemble"}, //48
            {"Trumpet", "Trumpet"}, //56
            {"Clarinet", "Clarinet"}, //71
            {"Flute", "Flute"}, //73
            {"Whistle", "Whistle"}, //78
            {"SynthVoice", "Synth Voice"}, //85
            {"Mute", "Mute"}
        };
        //dictIntrument.Add("HonkyTonkPiano", "Honky Piano");
        //dictIntrument.Add("Viola", "Viola");

        foreach (var rec in dictIntrument)
        {
            GameObject newButton = Instantiate(ButtonInstrument, Vector3.zero, Quaternion.identity);
            newButton.transform.SetParent(PanelGrid.transform);
            newButton.transform.localScale = Vector3.one;
            newButton.name = rec.Key;
            newButton.GetComponent<ButtonInstrument>().Setup(rec.Key, rec.Value);
        }

        SetButtonInstrumentColor("PanelInstrument : Start");
    }

    public void SetButtonInstrumentColor(string callFrom)
    {
        //Debug.Log("SetButtonInstrumentColor " + callFrom + " Helpers.InstrumentSelectMode " +
        //          Helpers.InstrumentSelectMode);

        if (LocalizationManager.instance != null)
        {
            TextInstrument.GetComponent<Text>().text =
                LocalizationManager.instance.GetLocalizedValue(Helpers.InstrumentSelectMode);
        }

        foreach (Transform child in PanelGrid.transform)
        {
            Image imageBackground = child.transform.GetChild(0).GetComponent<Image>();
            Image imageBorder = child.transform.GetChild(2).GetComponent<Image>();

            var borderColor = new Color(1, 1, 1, 0.5f);
            var backgroundColor = new Color(1, 1, 1, 0.5f);

            if (Helpers.InstrumentSelectMode == "Select Play Instrument")
            {
                //borderColor = Helpers.ColorWhite01;
                backgroundColor = Helpers.ColorWhite25;
                if (child.name == Helpers.PlayChannelInstrument)
                {
                    borderColor = Helpers.ColorWhite10;
                    backgroundColor = Helpers.ColorWhite05;
                }
            }
            else if (Helpers.InstrumentSelectMode == "Select Left Hand Instrument")
            {
                //borderColor = Helpers.ColorWhite01;
                backgroundColor = Helpers.ColorBlue25;
                if (child.name == Helpers.LeftChannelInstrument)
                {
                    borderColor = Helpers.ColorWhite10;
                    backgroundColor = Helpers.ColorBlue05;
                }
            }
            else if (Helpers.InstrumentSelectMode == "Select Right Hand Instrument")
            {
                //borderColor = Helpers.ColorWhite01;
                backgroundColor = Helpers.ColorGreen25;
                if (child.name == Helpers.RightChannelInstrument)
                {
                    borderColor = Helpers.ColorWhite10;
                    backgroundColor = Helpers.ColorGreen05;
                }
            }
            else if (Helpers.InstrumentSelectMode == "Select Melody Instrument")
            {
                //borderColor = Helpers.ColorWhite01;
                backgroundColor = Helpers.ColorWhite25;
                if (child.name == Helpers.VoiceChannelInstrument)
                {
                    borderColor = Helpers.ColorWhite10;
                    backgroundColor = Helpers.ColorWhite05;
                }
            }


            imageBorder.color = borderColor;
            imageBackground.color = backgroundColor;

            //Debug.Log("child.name " + child.name +
            //          " Helpers.PlayChannelInstrument " + Helpers.PlayChannelInstrument +
            //          " Helpers.LeftChannelInstrument " + Helpers.LeftChannelInstrument +
            //          " Helpers.RightChannelInstrument " + Helpers.RightChannelInstrument +
            //          " Helpers.VoiceChannelInstrument " + Helpers.VoiceChannelInstrument);

            if (child.name == "Mute")
            {
                if (Helpers.InstrumentSelectMode == "Select Play Instrument")
                {
                    child.gameObject.SetActive(false);
                }
                else
                {
                    child.gameObject.SetActive(true);
                }
            }
        }

        ChangeChangeButtonSpriteAll();
    }

    public void ChangeChangeButtonSpriteAll()
    {
        ChangeButtonSprite(ButtonInstrumentPlayImage, Helpers.PlayChannelInstrument);
        ChangeButtonSprite(ButtonInstrumentLeft, Helpers.LeftChannelInstrument);
        ChangeButtonSprite(ButtonInstrumentRight, Helpers.RightChannelInstrument);
        ChangeButtonSprite(ButtonInstrumentVoice, Helpers.VoiceChannelInstrument);
    }

    private void ChangeButtonSprite(Image image, string instrumentName)
    {
        image.sprite = Resources.Load("Sprites\\Instruments\\" + instrumentName, typeof(Sprite)) as Sprite;
    }
}