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
	public class Bar
	{
		public float time;
		public float ticks;
		public float timeDuration;
		public float ticksDuration;
		public int timeSignatureNumerator;
		public int timeSignatureDenominator;
		public int majorMinor;
		public int sharpsFlats;
		public float tempo;
		public int[] eventPos;
		public bool[] endOfTrack;
	}

	public List<Bar> bars = new List<Bar> ();

	void UpdateBars ()
	{
					
		for (int i = 0; i < tracks.Count; i++) {
			while (tracks [i] [eventPos [i]].AbsoluteTime <= ticks) {
				
				if (endOfTrack [i])
					break;
				
				midiEvent = tracks [i] [eventPos [i]];
									
				switch (midiEvent.CommandCode) {
				case MidiCommandCode.MetaEvent:
					metaEvent = (midiEvent as MetaEvent);
					switch (metaEvent.MetaEventType) {
					case MetaEventType.KeySignature:
						keyMajorMinor = (metaEvent as KeySignatureEvent).MajorMinor;
						Debug.Log ("MAJOR or MINOR: " + keyMajorMinor);
						keySharpsFlats = (metaEvent as KeySignatureEvent).SharpsFlats;
						Debug.Log ("SIGNATURE : " + keySharpsFlats);
						break;
					case MetaEventType.SequenceTrackName:
						Debug.Log ("TrackName : " + (metaEvent as TextEvent).Text);
						break;
					case MetaEventType.SetTempo:
						TempoEvent tempoEvent = (midiEvent as TempoEvent);
						tempoOriginal = (float)tempoEvent.Tempo;
						if (!tempoCustom)
							tempo = tempoOriginal;								
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
					case MetaEventType.EndTrack:
						
						break;
					}
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
						state = State.Finished;
						ticks = ticks - lastDeltaTicks;
						totalTime = time - lastDeltaTime;
						Bar lastBar = bars.Last ();
						lastBar.timeDuration = time - lastBar.time;
						lastBar.ticksDuration = ticks - lastBar.ticks;
						return;
					}
					break;	
				}
				
				eventPos [i] = eventPos [i] == tracks [i].Count - 1 ? eventPos [i] : eventPos [i] + 1;	
			}			
		}
				
		if (beatCount != (int)(ticks / PPQN / (4f / timeSignatureDenominator)) + 1) {
			beat = beatCount % (int)timeSignatureNumerator + 1;
			beatCount = (int)(ticks / PPQN / (4f / timeSignatureDenominator)) + 1;
			if (beat == 1) {
				Bar bar = new Bar () { 
					time = this.time,
					ticks = this.ticks,
					tempo = this.tempo,
					timeSignatureNumerator = this.timeSignatureNumerator,
					timeSignatureDenominator = this.timeSignatureDenominator,
					majorMinor = this.keyMajorMinor,
					sharpsFlats = this.keySharpsFlats
				};
				
				if (bars.Count > 0) {
					Bar lastBar = bars.Last ();
					lastBar.timeDuration = time - lastDeltaTime - lastBar.time;
					lastBar.ticksDuration = ticks - lastDeltaTicks - lastBar.ticks;
				}
				
				bar.eventPos = new int[eventPos.Length];
				bar.endOfTrack = new bool[endOfTrack.Length];
				
				for (int i = 0; i < bar.eventPos.Length; i++) {
					bar.eventPos [i] = GetTrackEventPosFromAbsoluteTicks (i, ticks - 100);
					bar.endOfTrack [i] = endOfTrack [i];
				}
				bars.Add (bar);
			}
		}
		
		deltaTime = deltaTimeResolution;
																	
		periodResolution = PPQN * 1000f * deltaTime * MicrosecondsPerMillisecond;
		//ticksPerClock = PPQN / PPQNMinValue;
		deltaTicks = (fractionalTicks + periodResolution) / tempoTicks;
		fractionalTicks += periodResolution - deltaTicks * tempoTicks;
		ticks += deltaTicks;
		time += deltaTime;
		
		lastTime = time;
		lastDeltaTime = deltaTime;
		lastDeltaTicks = deltaTicks;
	}

	public void SetBar (int aBarNr, bool play, bool pickUpBar = true)
	{
		if (midiFile != null) {		
			Debug.Log (state.ToString ());
			State stateSetBar = state;

			audioMusic.Pause ();
			audioVocals.Pause ();

			CancelPickUpBarCounting ();
			state = State.None;
			ResetSequencer ();
			//			if(midiOut){
			//				MidiOut.AllPedalsOff();
			//				MidiOut.AllSoundOff();
			//			}


			if (aBarNr <= 0) {
				time = lastTime = 0;
				ticks = 0;
				if (audioMusic.clip)
					audioMusic.time = 0;
				if (audioVocals.clip)
					audioVocals.time = 0;
				if (stateSetBar == State.Playing) {
					Play (pickUpBar);	
				} else {
					if (play) {
						Play (pickUpBar);
					} else {
						Pause ();
					}	
				}
			} else {
				bar = aBarNr;
				time = lastTime = bars [aBarNr].time;
				ticks = bars [aBarNr].ticks;

				if (audioMusic.clip)
					audioMusic.time = time;
				if (audioVocals.clip)
					audioVocals.time = time;

				for (int i = 0; i < bars [aBarNr].eventPos.Length; i++) {
					eventPos [i] = bars [aBarNr].eventPos [i];
					endOfTrack [i] = bars [aBarNr].endOfTrack [i];
				}

				for (int i = 0; i < words.Count; i++) {
					if (words [i].absoluteStartTime >= ticks) {
						wordPos = wordOffsetPos = i - 1;
						break;
					}
				}

				for (int i = 0; i < sentences.Count; i++) {
					if (sentences [i].absoluteStartTime >= ticks) {
						sentencePos = i - 1;
						break;
					}
				}

				for (int i = 0; i < verses.Count; i++) {
					if (verses [i].absoluteStartTime >= ticks) {
						versePos = i - 1;
						break;
					}
				}

				if (stateSetBar == State.Playing) {
					Play (pickUpBar);	
				} else {
					if (play) {
						Play (pickUpBar);
					} else {
						Pause ();
					}
				}
			}
		}
	}
}
