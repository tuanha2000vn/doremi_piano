using UnityEngine;
using System.Collections;
using System.IO;

using ForieroEngine.Extensions;

#if UNITY_EDITOR

public static class BASS24NETSynth
{
    public static void Register(string userName, string password)
    {
    }

    public static int Start(int freq = 44100, int channels = 1)
    {
        return 0;
    }

    public static int Stop()
    {
        return 0;
    }

    public static int SendMidiMessage(int Command, int Data1, int Data2)
    {
        return 0;
    }
}

#elif (UNITY_ANDROID || UNITY_WSA) && !UNITY_EDITOR

using Un4seen.Bass;
using Un4seen.Bass.AddOn.Midi;

public static class BASS24NETSynth
{

	static int soundFont = 0;
	//static BASS_MIDI_FONTINFO soundFontInfo;
	static int stream = 0;

	public static void Register (string userName, string password)
	{
		Debug.Log ("BASS24NETSynth - Register : " + userName);

		if (!string.IsNullOrEmpty (userName) && !string.IsNullOrEmpty (password)) {
			BassNet.Registration (userName, password);
		}
	}

	public static int Start (int freq = 44100, int channels = 1)
	{
		if (stream != 0) {
			return 1;
		}

		Debug.Log ("BASS24NETSynth - Start : " + freq.ToString () + " " + channels.ToString ());

		string soundFontFile = (Application.persistentDataPath + @"\soundfont.sf2").FixOSPath ();
		Debug.Log ("BASS24NETSynth - Soundfont store path : " + soundFontFile);
		
		if (!File.Exists (soundFontFile)) {
			TextAsset textAsset = Resources.Load ("soundfont.sf2") as TextAsset;
			if (textAsset) {
				File.WriteAllBytes (soundFontFile, textAsset.bytes);
				if (File.Exists (soundFontFile)) {
					Debug.Log ("BASS24NETSynth - Soundfont stored at path : " + soundFontFile);
				} else {
					Debug.Log ("BASS24NETSynth - Soundfont can not be found at path : " + soundFontFile);
				}
			} else {
				Debug.LogError ("Resource soundfont.sf2");
			}
		}
		
		Bass.BASS_SetConfig (BASSConfig.BASS_CONFIG_VISTA_TRUEPOS, 0); // allows lower latency on Vista and newer
		Bass.BASS_SetConfig (BASSConfig.BASS_CONFIG_UPDATEPERIOD, 10); // 10ms update period

		if (!Bass.BASS_Init (-1, freq, BASSInit.BASS_DEVICE_LATENCY, System.IntPtr.Zero)) {
			Debug.LogError ("BASS_Init");
			return 0;
		}

		BASS_INFO info = Bass.BASS_GetInfo ();
		Bass.BASS_SetConfig (BASSConfig.BASS_CONFIG_BUFFER, 10 + info.minbuf + 1); // default buffer size = update period + 'minbuf' + 1ms margin		

		stream = BassMidi.BASS_MIDI_StreamCreate (channels, BASSFlag.BASS_SAMPLE_FLOAT, freq);
		if (stream == 0 && Bass.BASS_ErrorGetCode () == BASSError.BASS_ERROR_FORMAT) {
			stream = BassMidi.BASS_MIDI_StreamCreate (channels, 0, freq);
		}

		if (stream == 0) {
			Debug.LogError ("BASS_MIDI_StreamCreate");
			return 0;
		}

		Bass.BASS_ChannelSetSync (stream, BASSSync.BASS_SYNC_MIDI_EVENT | BASSSync.BASS_SYNC_MIXTIME, (long)BASSMIDIEvent.MIDI_EVENT_PROGRAM, null, System.IntPtr.Zero);

		Bass.BASS_ChannelSetAttribute (stream, BASSAttribute.BASS_ATTRIB_NOBUFFER, 1);

		int newSoundFont = BassMidi.BASS_MIDI_FontInit (soundFontFile);
		Debug.Log ("BASS_MIDI_FontInit : " + newSoundFont.ToString ());

		if (!BassMidi.BASS_MIDI_FontSetVolume (newSoundFont, 4f)) {
			Debug.LogError ("BASS_MIDI_FontSetVolume");
			return 0;
		}
		;

		BASS_MIDI_FONT bassMidiFont = new BASS_MIDI_FONT ();
		bassMidiFont.font = newSoundFont;
		bassMidiFont.preset = -1;
		bassMidiFont.bank = 0;

		if (!BassMidi.BASS_MIDI_StreamSetFonts (0, new BASS_MIDI_FONT[]{ bassMidiFont }, 1)) {
			Debug.LogError ("BASS_MIDI_StreamSetFonts");
			return 0;
		}

		if (!BassMidi.BASS_MIDI_StreamSetFonts (stream, new BASS_MIDI_FONT[]{ bassMidiFont }, 1)) {
			Debug.LogError ("BASS_MIDI_StreamSetFonts");
			return 0;
		} 

		if (!BassMidi.BASS_MIDI_FontFree (soundFont)) {
			
		}
		; 

		soundFont = newSoundFont;

		//soundFontInfo = BassMidi.BASS_MIDI_FontGetInfo (soundFont);

		if (!Bass.BASS_ChannelPlay (stream, false)) {
			Debug.LogError ("BASS_ChannelPlay");
			return 0;
		}
		;

		return 1;
	}

	public static int Stop ()
	{
		if (stream == 0)
			return 0;
		Bass.BASS_StreamFree (stream);
		stream = 0;
		Bass.BASS_Free ();
		Bass.BASS_PluginFree (0);
		return 1;
	}

	public static int SendMidiMessage (int Command, int Data1, int Data2)
	{
		if (stream == 0)
			return 0;
		//  BYTE events[3] = {Command, (byte)Data1, (byte)Data2};
		//  BassMidi.BASS_MIDI_StreamEvents(stream, BASSMIDIEventS_RAW, events, ((Command&0xf0)==0xc0 || (Command&0xf0)==0xd0?2:3));

		byte chan = (byte)(Command & 0x0F);
		byte cmd = (byte)(Command & 0xF0);

		//    freopen("debug.log", "a", stdout);
		//    printf("%d;", Command);
		//    printf("%d;", chan);
		//    printf("%d;", cmd);
		//    printf("%d;", (byte)Data1);
		//    printf("%d;", (byte)Data2);
		//    printf("%d;\n", returnValue);
		//    fclose(stdout);

		switch (cmd) {
		case 0x80: //NoteOff
			BassMidi.BASS_MIDI_StreamEvent (stream, chan, BASSMIDIEvent.MIDI_EVENT_NOTE, MAKEWORD ((byte)Data1, 0));
			break;
		case 0x90: //NoteOn
			BassMidi.BASS_MIDI_StreamEvent (stream, chan, BASSMIDIEvent.MIDI_EVENT_NOTE, MAKEWORD ((byte)Data1, (byte)Data2));

			break;
		case 0xA0: //NoteAftertouch
			break;
		case 0xB0: //Controller
			{
				switch ((byte)Data1) {
				case 0x7B: //Note Off All
					BassMidi.BASS_MIDI_StreamEvent (stream, chan, BASSMIDIEvent.MIDI_EVENT_SOUNDOFF, (byte)Data2);
					break;
				case 0x07: //Channel Volume
					BassMidi.BASS_MIDI_StreamEvent (stream, chan, BASSMIDIEvent.MIDI_EVENT_VOLUME, (byte)Data2);
					break;
				case 0x0A: //Pan
					BassMidi.BASS_MIDI_StreamEvent (stream, chan, BASSMIDIEvent.MIDI_EVENT_PAN, (byte)Data2);
					break;
				case 0x01: //Modulation

					break;
				case 0x64: //Fine Select

					break;
				case 0x65: //Coarse Select

					break;
				case 0x06: // DataEntry Coarse

					break;
				case 0x26: // DataEntry Fine

					break;
				case 0x79: // Reset All
					BassMidi.BASS_MIDI_StreamEvent (stream, chan, BASSMIDIEvent.MIDI_EVENT_RESET, (byte)Data2);
					break;
				case 0x40:	// Right Pedal
					BassMidi.BASS_MIDI_StreamEvent (stream, chan, BASSMIDIEvent.MIDI_EVENT_SUSTAIN, (byte)Data2);
					break;
				case 0x41:	// Center Pedal
					BassMidi.BASS_MIDI_StreamEvent (stream, chan, BASSMIDIEvent.MIDI_EVENT_PORTAMENTO, (byte)Data2);
					break;
				case 0x42:	// Center Pedal
					break;
				case 0x43:	// Left Pedal
					BassMidi.BASS_MIDI_StreamEvent (stream, chan, BASSMIDIEvent.MIDI_EVENT_SOFT, (byte)Data2);
					break;
				default:
					break;
				}
			}
			break;
		case 0xC0: //Program Change - Instrument Change
			BassMidi.BASS_MIDI_StreamEvent (stream, chan, BASSMIDIEvent.MIDI_EVENT_PROGRAM, MAKEWORD ((byte)Data1, (byte)Data2));
			break;
		case 0xD0: //Channel Aftertouch
			//BassMidi.BASS_MIDI_StreamEvent(stream,chan,BASSMIDIEvent.MIDI_EVENT_,MAKEWORD((byte)Data1,(byte)Data2));
			break;
		case 0xE0: //Pitch Bend
			// BassMidi.BASS_MIDI_StreamEvent(stream,chan,BASSMIDIEvent.MIDI_EVENT_PITCHBEND,MAKEWORD((byte)Data1,(byte)Data2));
			break;
		default:
			break;
		}


		return 1;
	}

	public static int MAKEWORD (byte low, byte high)
	{
		return ((int)high << 8) | low;
	}
}
#endif
