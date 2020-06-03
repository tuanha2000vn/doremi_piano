using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ForieroEngine.MIDIUnified;

public delegate void TimeKeeperBeatDelegate (int beat, int beatNumerator, int beatDenominator);

public class TimeKeeper : MonoBehaviour, IMidiEvents
{

	#pragma warning disable 67
	public event ShortMessageEventHandler ShortMessageEvent;
	#pragma warning restore 67

	float msElapsed = 0f;

	void Update ()
	{
		msElapsed += Time.deltaTime;
		if (msElapsed * 1000f >= ms) {
			OnInternalTimer ();
			msElapsed = (msElapsed * 1000f % (float)ms) / 1000f;
		}
	}

	static TimeKeeper singleton;

	public static int beat = 0;
	static int beatsNumerator = 4;
	static int beatsDenominator = 4;
	public static int beatsPerMinute = 60;
	public static int upBeatVolume = 80;
	public static int downBeatVolume = 60;
	public static int ms = 1000;

	public static event TimeKeeperBeatDelegate OnBeat;
	public static event TimeKeeperBeatDelegate OnStart;
	public static event TimeKeeperBeatDelegate OnStop;
	
	public static event TimeKeeperBeatDelegate OnChange;

	public static int BeatsPerMinuteToMS (int beatsPerMinute)
	{
		return Mathf.RoundToInt (60000 / beatsPerMinute);	
	}

	public static int MSToBeatsPerMinute (int ms)
	{
		return Mathf.RoundToInt (60000 / ms);	
	}

	public static float GetTimeInterval ()
	{
		return (float)ms / 1000f;	
	}

	public static void SetBeats (int aBeatsNumerator)
	{
		beatsNumerator = aBeatsNumerator;
		if (OnChange != null) {
			OnChange (beat, beatsNumerator, beatsDenominator);
		}
	}

	public static bool IsRunning ()
	{
		return singleton != null;	
	}

	public static void SetBeatsPerMinute (int aBeatsPerMinute)
	{
		beatsPerMinute = aBeatsPerMinute;
		ms = BeatsPerMinuteToMS (beatsPerMinute);

		if (OnChange != null) {
			OnChange (beat, beatsNumerator, beatsDenominator);
		}
	}

	public static void Start (int aBeatsNumerator, int aBeatsDenominator, int aBeatsPerMinute)
	{
		Stop ();
		
		if (!singleton) {
			singleton = new GameObject ("TimeKeeper").AddComponent<TimeKeeper> ();	
		}
			
		beat = 0;
		beatsNumerator = aBeatsNumerator;
		beatsDenominator = aBeatsDenominator;
		beatsPerMinute = aBeatsPerMinute;
		ms = BeatsPerMinuteToMS (beatsPerMinute);

		Debug.Log (ms);
		
		if (OnStart != null) {
			OnStart (beat, beatsNumerator, beatsDenominator);
		}
	}

	public static void Stop ()
	{
		if (singleton) {
			Destroy (singleton.gameObject);
			singleton = null;
		}

		if (OnStop != null) {
			OnStop (beat, beatsNumerator, beatsDenominator);
		}
	}

	static void OnInternalTimer ()
	{
		beat++;

		if (beat > beatsNumerator) {
			beat = 1;
		}

		if (OnBeat != null) {
			OnBeat (beat, beatsNumerator, beatsDenominator);
		}
	}

	public static void BeatEvent (int aBeat, int aBeatsNumerator, int aBeatsDenominator)
	{
		if (OnBeat != null) {
			OnBeat (aBeat, aBeatsNumerator, aBeatsDenominator);
		}
	}

	public static void PlayBeat (int aBeat)
	{
		MidiOut.fireMidiOutEvents = false;

		if (aBeat == 1) {
			MidiOut.NoteOn ((int)PercussionEnum.OpenHiHat, downBeatVolume, 9);
		} else {
			MidiOut.NoteOn ((int)PercussionEnum.SideStick, upBeatVolume, 9);
		}

		MidiOut.fireMidiOutEvents = true;		
	}
}
