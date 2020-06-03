using System.Collections;
using ForieroEngine.MIDIUnified;
using UnityEngine;
using UnityEngine.UI;

public class BuildSoundSource : MonoBehaviour
{
    // Use this for initialization
    public Slider LevelLoaderPanelSlider;
    public Text LevelLoaderPanelText;

    void Start()
    {
        MidiOut.AllSoundOff();

        ////turn off all channel
        //for (int i = 0; i < 88; i++)
        //{
        //    for (int j = 0; j < 16; j++)
        //    {
        //        MidiOut.NoteOff(i + 20, j);
        //    }
        //}

        Helpers.IsPlaying = true;

        if (!Helpers.NotesClipReady)
        {
            //var coroutine = BuildSoundCr();
            //StartCoroutine(coroutine);
        }
    }

    //In case we don't init via First Run
    IEnumerator BuildSoundCr()
    {
        LevelLoaderPanelSlider.gameObject.SetActive(true);
        LevelLoaderPanelText.gameObject.SetActive(true);

        for (int i = Helpers.NotesClip.Count; i < 88; i++)
        {
            var path = (i + 20).ToString("D3");
            path = "BrightMp3/KEPSREC" + path;
            var clip = Resources.Load<AudioClip>(path);
            if (!Helpers.NotesClip.Contains(clip))
            {
                Debug.Log("BuildSoundCr " + i);
                Helpers.NotesClip.Add(clip);
                yield return null;
                LevelLoaderPanelSlider.value = i / 87f;
                LevelLoaderPanelText.text = "Building Sound..." + (int) (100 * LevelLoaderPanelSlider.value) + "%";
            }

            yield return null;
        }

        LevelLoaderPanelSlider.gameObject.SetActive(false);
        LevelLoaderPanelText.gameObject.SetActive(false);
        Helpers.NotesClipReady = true;
    }
}