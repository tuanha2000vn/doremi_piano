using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ForieroEngine.MIDIUnified;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class NotePlayStop : MonoBehaviour
{
    public Sprite KeyBlack;
    public Sprite KeyBlackDown;
    public Sprite KeyWhite;
    public Sprite KeyWhiteDown;

    private List<AudioSource> listAudioSource;
    private List<AudioSource> listAudioSourceAboutToFade;
    private List<AudioSource> listAudioSourceFading;
    private int currentSceneIndex;

    //void OnEnable()
    //{
    //    RayCast01.NotePlayMidiEvent += NotePlayMidi;
    //    RayCast01.NoteStopMidiEvent += NoteStopMidi;
    //    RayCast01.ChangeKeyColorEvent += ChangeKeyColor;
    //}

    //void OnDisable()
    //{
    //    RayCast01.NotePlayMidiEvent -= NotePlayMidi;
    //    RayCast01.NoteStopMidiEvent -= NoteStopMidi;
    //    RayCast01.ChangeKeyColorEvent -= ChangeKeyColor;
    //}

    // Use this for initialization
    void Start()
    {
        Helpers.ChannelSetup();
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        listAudioSource = new List<AudioSource>();
        listAudioSourceAboutToFade = new List<AudioSource>();
        listAudioSourceFading = new List<AudioSource>();

        for (int i = 0; i < 32; i++)
        {
            var audiosource = gameObject.AddComponent<AudioSource>();
            audiosource.playOnAwake = false;
            listAudioSource.Add(audiosource);
        }
    }


    void PlayNoteSound(int noteValue, string touchPhase, int channel, float volume)
    {
        //Debug.Log("PlayNoteSound noteValue" + noteValue + " touchPhase " + touchPhase);
        noteValue = noteValue - 1;
        if (currentSceneIndex == 1)
        {
            noteValue = noteValue + Helpers.PlayTranspose;
        }

        noteValue = Mathf.Clamp(noteValue, 0, 87);

        //if (Helpers.NotesClip.Count < noteValue)
        //{
        //    Debug.Log(noteValue + " clips is not ready");
        //    return;
        //}

        //Debug.Log("noteValue " + noteValue);
        //var path = (noteValue + 20).ToString("D3");
        //path = "BrightMp3/KEPSREC" + path + ".mp3";
        //path = Helpers.StreamingAssetsPath(path);
        //Debug.Log("path " + path);

        if (touchPhase == "down")
        {
            //if (Helpers.KeysWhitesDict.ContainsKey(noteValue))
            {
                MidiOut.NoteOn(noteValue + 20, 127, channel);
                //Debug.Log("MidiOut.NoteOn channel " + channel);
                return;
            }
            //else
            //{
            //    //Debug.Log("Create AudioSource");
            //    var audioSource = listAudioSource.FirstOrDefault(obj =>
            //        !listAudioSourceAboutToFade.Contains(obj)
            //        && !listAudioSourceFading.Contains(obj));

            //    if (audioSource == null)
            //    {
            //        audioSource = gameObject.AddComponent<AudioSource>();
            //        audioSource.playOnAwake = false;
            //        listAudioSource.Add(audioSource);
            //        Debug.LogWarning("Create new AudioSource " + listAudioSource.Count);
            //    }
            //    else
            //    {
            //        //Debug.LogWarning("Use existing AudioSource " + listAudioSource.Count);
            //    }

            //    audioSource.clip = Helpers.NotesClip[noteValue];
            //    audioSource.volume = volume * Random.Range(.9f, 1.1f);
            //    audioSource.Play();
            //    //Debug.Log("Play volume " + volume + " audioSource.volume " + audioSource.volume);
            //    listAudioSourceAboutToFade.Add(audioSource);
            //    //Debug.LogWarning("listAudioSourceAboutToFade.Add " + audioSource);
            //}
        }
        else
        {
            //if (Helpers.KeysWhitesDict.ContainsKey(noteValue))
            {
                MidiOut.NoteOff(noteValue + 20, channel);
                //Debug.Log("MidiOut.NoteOff channel " + channel);
                return;
            }
            //else
            //{
            //    MidiOut.NoteOff(noteValue + 20);
            //    var audioSource = listAudioSourceAboutToFade
            //        .LastOrDefault(obj =>
            //            !listAudioSourceFading.Contains(obj)
            //            && obj.clip == Helpers.NotesClip[noteValue]);

            //    if (audioSource != null)
            //    {
            //        //Debug.Log("Found Source to Fade");
            //        listAudioSourceFading.Add(audioSource);
            //        var coroutine = FadeSound(audioSource);
            //        StartCoroutine(coroutine);
            //    }
            //    else
            //    {
            //        Debug.Log("No Source to Fade " + Helpers.NotesClip[noteValue]);
            //        //foreach (var obj in listAudioSourceAboutToFade)
            //        //{
            //        //    Debug.LogWarning(obj.clip.ToString());
            //        //}
            //    }
            //}
        }
    }

    IEnumerator FadeSound(AudioSource audioSource, float fadeTime = 1.5f)
    {
        float startVolume = audioSource.volume;
        float timeEclapsed = 0;
        while (audioSource.volume > 0)
        {
            timeEclapsed = timeEclapsed + Time.deltaTime;
            audioSource.volume = startVolume - (timeEclapsed / fadeTime);
            //audioSource.volume = Mathf.Max(startVolume - (timeEclapsed / fadeTime), 0);
            //Debug.Log("audioSource.volume " + audioSource.volume + " timeEclapsed " + timeEclapsed + " fadeTime " +
            //          fadeTime);
            //if (audioSource.volume < 0)
            //{
            //    audioSource.volume = 0;
            //}
            yield return null;
        }

        audioSource.Stop();
        listAudioSourceAboutToFade.Remove(audioSource);
        listAudioSourceFading.Remove(audioSource);

        ////Debug.Log("Start fading " + audioSource.clip);
        //while (audioSource.volume > 0)
        //{
        //    var newVolume = Mathf.Max(audioSource.volume - 0.2f, 0);
        //    audioSource.volume = newVolume;
        //    yield return new WaitForSeconds(0.2f);
        //}

        //audioSource.Stop();
        //listAudioSourceAboutToFade.Remove(audioSource);
        //listAudioSourceFading.Remove(audioSource);


        //for (int i = 0; i < 100; i++)
        //{
        //    yield return new WaitForSeconds(0.1f);
        //    float currentVolume = audioSource.volume;
        //    if (currentVolume <= 0)
        //    {
        //        listAudioSourceAboutToFade.Remove(audioSource);
        //        listAudioSourceFading.Remove(audioSource);
        //        break;
        //    }

        //    audioSource.volume = currentVolume - 0.1f;
        //    //Debug.Log("FadeSound " + i + " noteAudioSource.volume " + noteAudioSource.volume);
        //}
    }

    public void NotePlayMidi(int noteValue, int volume, int channel)
    {
        try
        {
            var random = Random.Range(.9f, 1.1f);
            if (currentSceneIndex == 1)
            {
                noteValue = noteValue + Helpers.PlayTranspose;
            }

            MidiOut.NoteOn(noteValue + 20, (int) (volume * random), channel);
        }
        catch (Exception e)
        {
            Debug.LogWarning("NotePlayMidi " + e);
        }
    }

    public void NoteStopMidi(int noteValue, int channel)
    {
        try
        {
            if (currentSceneIndex == 1)
            {
                noteValue = noteValue + Helpers.PlayTranspose;
            }

            MidiOut.NoteOff(noteValue + 20, channel);
        }
        catch (Exception e)
        {
            Debug.LogWarning("NoteStopMidi " + e);
        }
    }

    //public void ChangeNoteColor(GameObject targetGameObject, Sprite sprite, Color color)
    //{
    //    var sp = targetGameObject.GetComponent<SpriteRenderer>();
    //    sp.sprite = sprite;
    //    sp.color = color;
    //}

    public void ChangeKeyColor(GameObject targetGameObject, Sprite sprite, Color color)
    {
        //Debug.LogWarning("ChangeKeyColor " + targetGameObject.name +
        //                 " sprite " + sprite.name +
        //                 " color " + color);

        var sp = targetGameObject.GetComponent<SpriteRenderer>();
        sp.sprite = sprite;
        sp.color = color;

        try
        {
            ParticleSystem particle = targetGameObject.GetComponentInChildren<ParticleSystem>();
            if (particle == null)
            {
                return;
            }

            if (sprite == KeyBlackDown
                || sprite == KeyWhiteDown)
            {
                //particle.Clear();
                if (Helpers.Fps > 27)
                {
                    particle.Play();
                }
            }
            else
            {
                //Debug.Log($"color {color}");
                particle.Clear();
                particle.Stop();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("ChangeKeyColor " + e);
        }
    }

    public void ClearAll()
    {
        Helpers.ListPracticePause.Clear();
        Helpers.ListPracticePreAdd.Clear();
        Helpers.IsPracticePlaying = true;

        MidiOut.AllSoundOff();

        foreach (Transform child in transform)
        {
            int keyValue;
            if (int.TryParse(child.name, out keyValue))
            {
                var sr = child.GetComponent<SpriteRenderer>();
                if (sr.sprite == KeyBlackDown)
                {
                    sr.sprite = KeyBlack;
                }

                if (sr.sprite == KeyWhiteDown)
                {
                    sr.sprite = KeyWhite;
                }

                if (sr.color != Helpers.ColorWhite10)
                {
                    sr.color = Helpers.ColorWhite10;
                }

                ParticleSystem particle = child.GetComponentInChildren<ParticleSystem>();
                particle.Clear();
                particle.Stop();
            }
        }
    }
}