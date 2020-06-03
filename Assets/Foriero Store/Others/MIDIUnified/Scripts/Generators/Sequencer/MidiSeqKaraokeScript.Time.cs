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
	void Update ()
	{
		if (state == State.Playing) {
			cancelUpdate = false;

			midiFinished = true;
			musicFinished = true;
			vocalsFinished = true;

			deltaTime = Time.deltaTime * speed;

			if (musicClip && music) {
				deltaTime = audioMusic.time - lastTime; 
				musicFinished = !audioMusic.isPlaying;
			} else if (vocalsClip && vocals) {
				deltaTime = audioVocals.time - lastTime; 
				vocalsFinished = !audioVocals.isPlaying;
			}

			deltaTimeNumerator = deltaTime + deltaTimeRest;

			deltaTimeIterator = Mathf.FloorToInt (deltaTimeNumerator / deltaTimeResolution);

			for (int k = 0; k < deltaTimeIterator; k++) {

				if (cancelUpdate) {
					break;
				}

				midiFinished = false;

				if (CallEvents ()) {
					UpdateTime ();
				}

				if (midiFinished) {
					break;
				}
			}	

			if (midiFinished && musicFinished && vocalsFinished) {
				state = State.Finished;
				return;
			}

			deltaTimeRest = deltaTimeNumerator % deltaTimeResolution;

		}
	}

	void UpdateTime ()
	{
		deltaTime = deltaTimeResolution;
		
		periodResolution = PPQN * 1000f * deltaTime * MicrosecondsPerMillisecond;
		//ticksPerClock = PPQN / PPQNMinValue;
		deltaTicks = (fractionalTicks + periodResolution) / tempoTicks;
		fractionalTicks += periodResolution - deltaTicks * tempoTicks;
		
		if (repeatBarSelection) {
			barTmp = bar;

			if (beatCount != (int)((ticks + deltaTicks) / PPQN / (4f / timeSignatureDenominator)) + 1)
			if (beatCount % (int)timeSignatureNumerator + 1 == 1) {
				barTmp++;
			}

			if (barTmp > endBar + 1) {

				if (OnRepeat != null) {
					OnRepeat (startBar);
				}

				cancelUpdate = true;
				SetBar (startBar, true, pickUpBarOnRepeat);
				return;
			} 
		}
		
		if (beatCount != (int)(ticks / PPQN / (4f / timeSignatureDenominator)) + 1) {
			beat = beatCount % (int)timeSignatureNumerator + 1;

			if (beat == 1) {
				bar++;
			}

			beatCount = (int)(ticks / PPQN / (4f / timeSignatureDenominator)) + 1;	

			if (metronome) {
				TimeKeeper.BeatEvent (beat, timeSignatureNumerator, timeSignatureDenominator);
			}
		}	
		
		ticks += deltaTicks;
		time += deltaTime;
		
		lastTime = time;
		lastDeltaTime = deltaTime;
		lastDeltaTicks = deltaTicks;
	}

	int GetTrackEventPosFromAbsoluteTicks (int aTrackIndex, float aAbsoluteTicks)
	{
		for (int i = 0; i < tracks [aTrackIndex].Count; i++) {
			if (tracks [aTrackIndex] [i].AbsoluteTime > aAbsoluteTicks)
				return i - 1;
		}
		return tracks [aTrackIndex].Count - 1;
	}

	public float TimeToTicks (float aTime)
	{
		return PPQN * 1000f * aTime * (float)MicrosecondsPerMillisecond / tempoTicks;
	}

	public float TicksToTime (float ticks)
	{
		return ticks / PPQN / 1000f / (float)MicrosecondsPerMillisecond * tempoTicks;
	}
}
