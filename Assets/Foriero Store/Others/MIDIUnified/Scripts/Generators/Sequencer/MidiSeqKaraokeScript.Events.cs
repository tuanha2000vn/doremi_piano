using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Midi;
using System.Text.RegularExpressions;

public partial class MidiSeqKaraokeScript : MonoBehaviour, IMidiEvents
{
	public event ShortMessageEventHandler ShortMessageEvent;
	public event Action OnInitialized;
	public event Action<WordText> OnWord;
	public event Action OnWordFinished;
	public event Action<WordText> OnWordOffset;
	public event Action OnWordOffsetFinished;
	public event Action<SentenceText> OnSentence;
	//public event Action OnSentenceFinished;
	public event Action<MidiText> OnVerse;
	//public event Action OnVerseFinished;
	public event Action OnFinished;
	public event Action OnPlay;
	public event Action OnContinue;
	public event Action OnStop;
	public event Action OnPause;
	public event Action<int> OnRepeat;
	public event Action<int> OnPickUpBar;
	public event Action<int> OnPickupBarBegin;
	public event Action OnPickupBarEnd;
	
	public event Action OnMidiLoaded;
	
	public event Action<float> OnTempoChange;

	bool CallEvents ()
	{
		if (wordPos < words.Count) {
			if (wordPos > 0) {
				if (!words [wordPos].finishFired) {
					if (words [wordPos].absoluteStartTime <= ticks) {
						if (OnWordFinished != null)
							OnWordFinished ();
						words [wordPos].finishFired = true;
					}
				}
			}

			if (words [wordPos < 0 ? 0 : wordPos].absoluteStartTime <= ticks) {
				if (OnWord != null)
					OnWord (words [wordPos < 0 ? 0 : wordPos]);
				wordPos++;	
			}
		}

		if (wordOffsetPos < words.Count) {
			if (wordOffsetPos > 0) {
				if (!words [wordOffsetPos].finishFired) {
					if (words [wordOffsetPos].absoluteStartTime + TimeToTicks (wordTimeOffset) + TimeToTicks (wordTimeFinishedOffset) <= ticks) {
						if (OnWordOffsetFinished != null)
							OnWordOffsetFinished ();
						words [wordOffsetPos].finishOffsetFired = true;
					}
				}
			}

			if (words [wordOffsetPos < 0 ? 0 : wordOffsetPos].absoluteStartTime + TimeToTicks (wordTimeOffset) <= ticks) {
				if (OnWordOffset != null)
					OnWordOffset (words [wordOffsetPos < 0 ? 0 : wordOffsetPos]);
				wordOffsetPos++;	
			}
		}

		if (sentencePos < sentences.Count) {
			if (sentences [sentencePos < 0 ? 0 : sentencePos].absoluteStartTime + TimeToTicks (senteceTimeOffset) <= ticks) {
				if (OnSentence != null)
					OnSentence (sentences [sentencePos < 0 ? 0 : sentencePos]);
				sentencePos++;	
			}
		}

		if (versePos < verses.Count) {
			if (verses [versePos < 0 ? 0 : versePos].absoluteStartTime + TimeToTicks (versetTimeOffset) <= ticks) {

				if (OnVerse != null) {
					OnVerse (verses [versePos < 0 ? 0 : versePos]);
				}

				versePos++;	
			}
		}

		for (int i = 0; i < tracks.Count; i++) {
			while (tracks [i] [eventPos [i]].AbsoluteTime <= ticks) {

				if (endOfTrack [i])
					break;

				midiEvent = tracks [i] [eventPos [i]];

				command = midiEvent.CommandCode;
				if (command == MidiCommandCode.NoteOn && midiEvent.Data2 == 0) {
					command = MidiCommandCode.NoteOff;
				}

				if (midiOut) {
					if (!muteTrack [i]) {
						MidiOut.SendShortMessage ((forceTrackAsChannel ? i : (midiEvent.Channel - 1)) + (int)command, midiEvent.Data1, /*command == MidiCommandCode.NoteOn ? (int)Mathf.Clamp(midiEvent.Data2 * volume, 0, 127) :*/midiEvent.Data2);
					}
				}

				if (ShortMessageEvent != null) {
					ShortMessageEvent ((forceTrackAsChannel ? i : (midiEvent.Channel - 1)) + (int)command, midiEvent.Data1, midiEvent.Data2);
				}

				switch (midiEvent.CommandCode) {
				case MidiCommandCode.AutoSensing:

					break;
				case MidiCommandCode.ChannelAfterTouch:

					break;
				case MidiCommandCode.ContinueSequence:

					break;
				case MidiCommandCode.ControlChange:
					//controlEvent = (midiEvent as ControlChangeEvent);

					break;
				case MidiCommandCode.Eox:

					break;
				case MidiCommandCode.KeyAfterTouch:

					break;
				case MidiCommandCode.MetaEvent:
					metaEvent = (midiEvent as MetaEvent);
					switch (metaEvent.MetaEventType) {

					case MetaEventType.Copyright:
						Debug.Log ("Copyright : " + (metaEvent as TextEvent).Text);
						break;
					case MetaEventType.CuePoint:

						break;
					case MetaEventType.DeviceName:

						break;
					case MetaEventType.EndTrack:

						break;
					case MetaEventType.KeySignature:
						keyMajorMinor = (metaEvent as KeySignatureEvent).MajorMinor;
						Debug.Log ("MAJOR or MINOR: " + keyMajorMinor);
						keySharpsFlats = (metaEvent as KeySignatureEvent).SharpsFlats;
						Debug.Log ("SIGNATURE : " + keySharpsFlats);
						break;
					case MetaEventType.Lyric:

						break;	
					case MetaEventType.Marker:

						break;
					case MetaEventType.MidiChannel:

						break;
					case MetaEventType.MidiPort:

						break;
					case MetaEventType.ProgramName:
						Debug.Log ("Program Name : " + (metaEvent as TextEvent).Text);
						break;
					case MetaEventType.SequencerSpecific:
						//SequencerSpecificEvent sequencerEvent = midiEvent as SequencerSpecificEvent;

						break;
					case MetaEventType.SequenceTrackName:
						Debug.Log ("TrackName : " + (metaEvent as TextEvent).Text);
						break;
					case MetaEventType.SetTempo:
						TempoEvent tempoEvent = (midiEvent as TempoEvent);
						tempoOriginal = (float)tempoEvent.Tempo;
						if (!tempoCustom)
							tempo = tempoOriginal;
						if (OnTempoChange != null)
							OnTempoChange (tempo);
						break;
					case MetaEventType.SmpteOffset:

						break;
					case MetaEventType.TextEvent:

						break;
					case MetaEventType.TimeSignature:
						TimeSignatureEvent signatureEvent = (midiEvent as TimeSignatureEvent);
						timeSignatureNumerator = signatureEvent.Numerator;
						_timeSignatureDenominator = signatureEvent.Denominator;
						timeSignatureDenominator = (int)Mathf.Pow (2, _timeSignatureDenominator);
						break;
					case MetaEventType.TrackInstrumentName:
						Debug.Log ("Instrument Name : " + (metaEvent as TextEvent).Text);
						break;
					case MetaEventType.TrackSequenceNumber:

						break;
					default:

						break;
					}
					break;

				case MidiCommandCode.NoteOn:

					break;
				case MidiCommandCode.NoteOff:

					break;
				case MidiCommandCode.PatchChange:

					break;
				case MidiCommandCode.PitchWheelChange:

					break;
				case MidiCommandCode.StartSequence:

					break;
				case MidiCommandCode.StopSequence:

					break;
				case MidiCommandCode.Sysex:

					break;
				case MidiCommandCode.TimingClock:

					break;
				}

				if (eventPos [i] >= tracks [i].Count - 1) {
					endOfTrack [i] = true;
					bool endOfFile = true;
					for (int k = 0; k < tracks.Count; k++) {
						if (!endOfTrack [k]) {
							endOfFile = false;
							break;	
						}
					}

					if (endOfFile) {
						ticks = ticks - lastDeltaTicks;
						time = time - lastDeltaTime;
						if (repeatBarSelection) {
							cancelUpdate = true;
							SetBar (startBar, true);
							return false;
						} else {
							cancelUpdate = true;
							midiFinished = true;
							return false;
						}
					}
					break;	
				} 

				eventPos [i] = eventPos [i] == tracks [i].Count - 1 ? eventPos [i] : eventPos [i] + 1;
			}
		}
		return true;
	}
}
