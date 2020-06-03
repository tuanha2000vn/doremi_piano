using System.IO;
using AudioSynthesis.Synthesis;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public partial class CSharpSynth : MonoBehaviour
{
    public static CSharpSynth singleton;

    [Range(0, 5)]
    public float gain = 2f;

    AudioSource audioSource = null;

    int channels = 0;
    int bufferLength = 0;
    int numBuffers = 0;
    readonly string soundBank = "soundfont.sf2";

    void OnDestroy()
    {
        playEnabled = false;
        initialized = false;

        singleton = null;
    }

    void OnDisable()
    {
        playEnabled = false;
    }

    void OnEnable()
    {
        if (midiSynthesizer != null)
        {
            playEnabled = true;
        }
    }

    byte[] LoadBank(string filename)
    {
        TextAsset asset = Resources.Load<TextAsset>(filename);

        if (asset == null)
        {
            Debug.LogError("NOT FOUND : Resources/" + filename);
            return null;
        }
        else
        {
            return asset.bytes;
        }
    }

    bool InitSynth(int sampleRate = 22050, int polyphony = 64)
    {
        // Get peer AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            return false;
        }

        // Create synthesizer and load bank
        if (midiSynthesizer == null)
        {
            // Get number of channels
            if (AudioSettings.driverCapabilities.ToString() == "Mono")
            {
                channels = 1;
            }
            else
            {
                channels = 2;
            }

            // Create synth
            AudioSettings.GetDSPBufferSize(out bufferLength, out numBuffers);
            sampleRateDivider = AudioSettings.outputSampleRate / sampleRate;
            midiSynthesizer = new Synthesizer(sampleRate, channels, bufferLength / numBuffers, numBuffers, polyphony);

            // Load bank data
            byte[] bankData = LoadBank(soundBank);
            if (bankData == null)
                return false;
            else
            {
                midiSynthesizer.LoadBank(new MFile(bankData, soundBank));
                midiSynthesizer.ResetSynthControls(); // Need to do this for bank to load properly, don't know why
            }

            initialized = true;
        }

        return true;
    }

    #region thread variables

    int bufferHead;
    float[] currentBuffer;
    float[] tempBuffer;
    MidiMessage midiMessage;
    Synthesizer midiSynthesizer;
    int sampleRateDivider = 1;

    #endregion

    // Called when audio filter needs more sound data
    void OnAudioFilterRead(float[] data, int channels)
    {
        // Do nothing if play not enabled
        // This flag is raised/lowered when user starts/stops play
        // Helps avoids thread finding synth in state of shutting down
        if (!playEnabled)
        {
            return;
        }

        if (initialized)
        {
            if (allSoundsOff)
            {
                midiSynthesizer.NoteOffAll(true);
                midiSynthesizer.ResetSynthControls();
                midiSynthesizer.ResetPrograms();

                allSoundsOff = false;
            }

            while (queue.TryDequeue(out midiMessage))
            {
                midiSynthesizer.ProcessMidiMessage(midiMessage.channel, midiMessage.command, midiMessage.data1, midiMessage.data2);
            }
        }

        int count = 0;
        while (count < data.Length)
        {
            if (currentBuffer == null || bufferHead >= currentBuffer.Length)
            {
                midiSynthesizer.GetNext();

                if (sampleRateDivider == 1)
                {
                    currentBuffer = midiSynthesizer.WorkingBuffer;

                    for (int i = 0; i < currentBuffer.Length; i++)
                    {
                        currentBuffer[i] *= gain * 10f;
                    }
                }
                else
                {
                    tempBuffer = midiSynthesizer.WorkingBuffer;

                    currentBuffer = new float[tempBuffer.Length * sampleRateDivider];

                    for (int i = 0; i < tempBuffer.Length; i += channels)
                    {
                        for (int ch = 0; ch < channels; ch++)
                        {
                            for (int d = 0; d < sampleRateDivider; d++)
                            {
                                currentBuffer[i * sampleRateDivider + ch + d] = tempBuffer[i + ch] * gain * 10f;
                            }
                        }
                    }
                }

                bufferHead = 0;
            }

            var length = Mathf.Min(currentBuffer.Length - bufferHead, data.Length - count);
            System.Array.Copy(currentBuffer, bufferHead, data, count, length);
            bufferHead += length;
            count += length;
        }
    }
}
