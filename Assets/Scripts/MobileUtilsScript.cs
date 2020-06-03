using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MobileUtilsScript : MonoBehaviour
{
    private int FramesPerSec;
    private float frequency = 1.0f;
    public Text fpsText;
    private GameObject _noteHolder;
    private GameObject _songScrollView;
    private GameObject _backgroundParticle;
    private DateTime fpsCheck;

    void Start()
    {
        StartCoroutine(FPS());
        fpsText = GetComponentInChildren<Text>();
        fpsText.text = "";
        _noteHolder = GameObject.Find("NoteHolder");
        _songScrollView = GameObject.Find("SongScrollView");
        _backgroundParticle = GameObject.Find("BackgroundParticle");
    }

    private IEnumerator FPS()
    {
        for (;;)
        {
            // Capture frame-per-second
            int lastFrameCount = Time.frameCount;
            float lastTime = Time.realtimeSinceStartup;
            yield return new WaitForSeconds(frequency);
            float timeSpan = Time.realtimeSinceStartup - lastTime;
            int frameCount = Time.frameCount - lastFrameCount;

            // Display it

            Helpers.Fps = Mathf.RoundToInt(frameCount / timeSpan);
        }
    }


    void OnGUI()
    {
        if (!Application.isEditor)
        {
            return;
        }

        if (fpsCheck < DateTime.Now)
        {
            if (Helpers.Fps < 24
                && _backgroundParticle != null
                && _backgroundParticle.activeSelf)
            {
                _backgroundParticle.SetActive(false);
            }

            if (Helpers.Fps > 27
                && _backgroundParticle != null
                && !_backgroundParticle.activeSelf)
            {
                _backgroundParticle.SetActive(true);
            }

            fpsCheck = DateTime.Now.AddSeconds(5);
        }

        if (_noteHolder != null)
        {
            fpsText.text = "FPS: " + Helpers.Fps + " NoteHolder: " + _noteHolder.transform.childCount;
        }
        else if (_songScrollView != null)
        {
            fpsText.text = "FPS: " + Helpers.Fps + " SongScrollView: " +
                           _songScrollView.transform.GetChild(0).GetChild(0).childCount;
        }
        else
        {
            fpsText.text = "FPS: " + Helpers.Fps;
        }
    }
}