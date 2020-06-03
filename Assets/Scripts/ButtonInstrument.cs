using UnityEngine;
using UnityEngine.UI;

public class ButtonInstrument : MonoBehaviour
{
    private GameObject PanelInstrument;
    private string handdleClickName;

    // Use this for initialization
    void Start()
    {
        PanelInstrument = GameObject.Find("PanelInstrument");
        gameObject.GetComponent<Button>().onClick.AddListener(HandleClick);
    }

    //Select Instrument
    //Select Left Hand Instrument
    //Select Right Hand Instrument
    //Select Melody Instrument

    public void Setup(string instrumentName, string instrumentDisplayname)
    {
        handdleClickName = instrumentName;
        gameObject.name = instrumentName;
        gameObject.GetComponentInChildren<Text>().text = instrumentDisplayname;
        var instrumentSprite = Resources.Load("Sprites\\Instruments\\" + instrumentName, typeof(Sprite)) as Sprite;
        gameObject.transform.GetChild(1).GetComponent<Image>().sprite = instrumentSprite;
    }

    public void HandleClick()
    {
        Debug.Log("HandleClick " + handdleClickName + " InstrumentSelectMode " + Helpers.InstrumentSelectMode);
        if (Helpers.InstrumentSelectMode == "Select Left Hand Instrument")
        {
            Helpers.LeftChannelInstrument = handdleClickName;
        }
        else if (Helpers.InstrumentSelectMode == "Select Right Hand Instrument")
        {
            Helpers.RightChannelInstrument = handdleClickName;
        }
        else if (Helpers.InstrumentSelectMode == "Select Melody Instrument")
        {
            Helpers.VoiceChannelInstrument = handdleClickName;
        }
        else
        {
            Helpers.PlayChannelInstrument = handdleClickName;
        }

        Helpers.ChannelSetup();

        PanelInstrument.GetComponent<PanelInstrumentSelect>()
            .SetButtonInstrumentColor("ButtonInstrument : HandleClick");
        GameObject.Find("Menu").GetComponent<MainMenu>().ButtonInstrumentSelectModeClick("Close");
    }


    //private void OnEnable()
    //{
    //    Debug.Log("OnEnable");
    //}
}