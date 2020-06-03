using System;
using UnityEngine;
using UnityEngine.UI;

public class PanelMeasure : MonoBehaviour
{
    private Slider PanelMeasureSlider;
    private MainMenu mainMenu;
    private GameObject noteFlow;
    private GameObject notePlayStop;

    // Use this for initialization
    void Start()
    {
        PanelMeasureSlider = GameObject.Find("PanelMeasureSlider").GetComponent<Slider>();
        mainMenu = GameObject.Find("Menu").GetComponent<MainMenu>();
        noteFlow = GameObject.Find("NoteFlow");
        notePlayStop = GameObject.Find("Keyboard");
    }

    //// Update is called once per frame
    //void Update () {

    //}

    public void PointerDown()
    {
        Debug.Log("PointerDown");
        CancelInvoke("MainMenuStartPlaying");
        mainMenu.StopPlaying();
    }

    public void PointerUp()
    {
        Debug.Log("PointerUp");
        CancelInvoke("MainMenuStartPlaying");
        Invoke("MainMenuStartPlaying", 0.5f);
        GameObject myEventSystem = GameObject.Find("EventSystem");
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
    }

    void MainMenuStartPlaying()
    {
        mainMenu.StartPlaying();
    }

    public void PanelMeasureChanged()
    {
        if (!Helpers.IsPlaying)
        {
            //Debug.Log("LearnTransposeChanged " + PanelMeasureSlider.value);
            if (noteFlow != null)
            {
                noteFlow.transform.localPosition = new Vector3(0, PanelMeasureSlider.value, 0);
                if (notePlayStop != null)
                {
                    notePlayStop.GetComponent<NotePlayStop>().ClearAll();
                }
            }

            Helpers.PracticeRepeatTimeOut = DateTime.Now;
        }
    }

    public void UpdateSliderValue(float value)
    {
        if (PanelMeasureSlider != null)
        {
            PanelMeasureSlider.value = value;
        }
    }
}