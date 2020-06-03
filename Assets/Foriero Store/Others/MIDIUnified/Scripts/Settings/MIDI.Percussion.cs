using UnityEngine;
using System.Collections;
using ForieroEngine.MIDIUnified.Plugins;
using System.Collections.Generic;
using ForieroEngine.ObjectPool;

namespace ForieroEngine.MIDIUnified
{
    public partial class MIDI : MonoBehaviour
    {
        public static MIDIPercussionSettings percussionSettings
        {
            get
            {
                return MIDIPercussionSettings.instance;
            }
        }

        AudioClip percussionAudioClip;

        struct ScheduledPercussion
        {
            public AudioClip clip;
            public float volume;
            public double dspTime;
        }

        ConcurrentPool<AudioSourcePoolItem> audioSourcePool = new ConcurrentPool<AudioSourcePoolItem>("Pool", null, 5);
        List<ScheduledPercussion> scheduledPercussionItems = new List<ScheduledPercussion>();
        List<AudioSourcePoolItem> playingPercussionItems = new List<AudioSourcePoolItem>();

        static int Percussion(PercussionEnum percussionEnum, int volume)
        {
            if (!instance) return 0;

            instance.percussionAudioClip = percussionSettings.GetAudioClip(percussionEnum);
            if (instance.percussionAudioClip)
            {
                instance.audioSource.PlayOneShot(instance.percussionAudioClip, volume.ToVolume());
                return 1;
            }

            return 0;
        }

        public static int SchedulePercussion(PercussionEnum percussionEnum, int volume, float scheduleTime = 0)
        {
            if (!instance) return 0;

            if (scheduleTime <= 0)
            {
                return Percussion(percussionEnum, volume);
            }
            else
            {
                instance.percussionAudioClip = percussionSettings.GetAudioClip(percussionEnum);
                if (instance.percussionAudioClip)
                {
                    var item = new ScheduledPercussion();
                    item.clip = instance.percussionAudioClip;
                    item.volume = volume.ToVolume();
                    item.dspTime = AudioSettings.dspTime + scheduleTime;
                    instance.scheduledPercussionItems.Add(item);

                    //Debug.Log("Adding percussion : " + item.dspTime);

                    instance.scheduledPercussionItems.Sort((a, b) =>
                    {
                        return -1 * a.dspTime.CompareTo(b.dspTime);
                    });

                    return 1;
                }
            }
            return 0;
        }

        public static void CancelScheduledPercussion()
        {
            if (!instance) return;

            instance.scheduledPercussionItems.Clear();

            foreach (AudioSourcePoolItem item in instance.playingPercussionItems)
            {
                item.TryRelease();
            }

            instance.playingPercussionItems.Clear();
        }

        void PercussionDestroy()
        {
            CancelScheduledPercussion();
            audioSourcePool.Dispose();
            audioSourcePool = null;
        }

        class AudioSourcePoolItem : RecyclableObject
        {
            public AudioSource audioSource;
            public double dspTime;

            public override void Recycle()
            {
                if (audioSource)
                {
                    audioSource.Stop();
                }
            }

            public bool IsPlaying()
            {
                return audioSource != null && audioSource.isPlaying;
            }

            public void Schedule(ScheduledPercussion scheduledPercussion)
            {
                this.dspTime = scheduledPercussion.dspTime;

                if (audioSource)
                {
                    audioSource.clip = scheduledPercussion.clip;
                    audioSource.volume = scheduledPercussion.volume;
                    audioSource.PlayScheduled(scheduledPercussion.dspTime);
                }
            }
        }
    }
}