using UnityEngine;
using System.Collections;
using System.Collections.Generic.Concurrent;

// using System.Timers;

using ForieroEngine.MIDIUnified.Plugins;

namespace ForieroEngine.MIDIUnified
{
    [RequireComponent(typeof(AudioSource))]
    public partial class MIDI : MonoBehaviour
    {
        AudioSource audioSource;

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        void OnEnable()
        {
            if (instance)
            {
                Debug.LogError("Something is trying to add MIDIUnified into scene but it already exists!");
                DestroyImmediate(this.gameObject);
            }
            else
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
                StartCoroutine(Init());
            }
        }

        void OnDisable()
        {

        }

        void OnApplicationPause()
        {
            MidiOut.AllSoundOff();
        }

        void OnDestroy()
        {
            if (initialized)
            {
                CleanUp();
            }

            PercussionDestroy();
        }

        AudioSourcePoolItem audioSourcePoolItem;
        ScheduledPercussion scheduledPercussion;

        //void OnGUI()
        //{
        //    GUILayout.Label(playingPercussionItems.Count.ToString());
        //}

        void Update()
        {
            for (int i = playingPercussionItems.Count - 1; i >= 0; i--)
            {
                if (!playingPercussionItems[i].IsPlaying())
                {
                    playingPercussionItems[i].TryRelease();
                    playingPercussionItems.RemoveAt(i);
                }
            }

            for (int i = scheduledPercussionItems.Count - 1; i >= 0; i--)
            {
                scheduledPercussion = scheduledPercussionItems[i];

                if (audioSourcePool.AvailableCount > 0)
                {
                    audioSourcePoolItem = audioSourcePool.Acquire();

                    if (!audioSourcePoolItem.audioSource)
                    {
                        audioSourcePoolItem.audioSource = gameObject.AddComponent<AudioSource>();
                        audioSourcePoolItem.audioSource.playOnAwake = false;
                    }

                    audioSourcePoolItem.Schedule(scheduledPercussion);
                    playingPercussionItems.Add(audioSourcePoolItem);
                    playingPercussionItems.Sort((a, b) =>
                    {
                        return -1 * a.dspTime.CompareTo(b.dspTime);
                    });

                    scheduledPercussionItems.RemoveAt(i);
                }
            }
        }
    }
}