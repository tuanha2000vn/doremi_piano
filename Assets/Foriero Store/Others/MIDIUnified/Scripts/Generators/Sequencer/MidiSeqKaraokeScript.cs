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
	
	public static MidiSeqKaraokeScript singleton;

	public enum State
	{
		None,
		PickUpBar,
		Playing,
		Pausing,
		Finished
	}

	public bool playOnStart = false;
	public float delay = 0f;
	private State pState = State.None;

	public State state {
		set {
			if (value != pState) {
				State tmpState = pState;
				pState = value;
				switch (value) {
				case State.PickUpBar:
					if (OnPickUpBar != null)
						OnPickUpBar (bar);
					break;
				case State.Finished:
					if (OnFinished != null)
						OnFinished ();
					break;
				case State.Playing:
					if (tmpState == State.Pausing) {
						if (OnContinue != null)
							OnContinue ();
					} else {
						if (OnPlay != null)
							OnPlay ();
					}
					break;
				case State.Pausing:
					if (OnPause != null)
						OnPause ();
					break;
				}
			}
		}
		get {
			return pState;	
		}
	}

	public TextAsset midiFileTextAsset;

	[HideInInspector]
	public byte[] midiFileBytes;

	public bool midiOut = true;
	public bool midiThrough = false;

	public bool music = false;
	public AudioSource audioMusic;
	public AudioClip musicClip;
	public float musicVolume = 1f;

	public bool vocals = false;
	public AudioSource audioVocals;
	public AudioClip vocalsClip;

	public float vocalsVolume = 1f;
		
	public const int MicrosecondsPerMinute = 60000000;
	public const int MicrosecondsPerSecond = 1000000;
	public const int MicrosecondsPerMillisecond = 1000;

	//	[Tooltip ("This value multiplies volume data to make it softer or louder.")]
	//	[Range (0, 10)]
	//	public float multiplyVolume = 1f;
	public float speed = 1f;
	public float ticks = 0;
	public float time = 0f;
	public float totalTime = 0f;
	public int beat = 0;
	public int beatCount = 0;
	public bool metronome = true;
	public int bar = 0;
	private int barTmp = 0;
	public int barCount = 0;
	public bool pickUpBar = true;
	public bool pickUpBarOnRepeat = true;
	public bool forceTrackAsChannel = true;
		
	public int timeSignatureNumerator = 4;
	private int _timeSignatureDenominator = 2;
	public int timeSignatureDenominator = 4;
	
	public int PPQN = 24;
	public readonly int PPQNMinValue = 24;
	public bool tempoCustom = false;
	float tempoOriginal = 120f;
	public float tempo = 120f;

	public float tempoTicks {
		get {
			return MicrosecondsPerMinute / (musicClip ? tempoOriginal : tempo);	
		}
	}

	public int keyMajorMinor = 0;
	public int keySharpsFlats = 0;
	
	private float fractionalTicks = 0;
	private float lastTime = 0f;
	private float lastDeltaTime = 0f;
	private float lastDeltaTicks = 0f;

	//	float speedTmp = 1f;
	//	float tempoTmp = 120f;
	
	//	BPM – it’s a beats per minute 60 000 000/MPQN
	//	PPQN – Pulses per quater note, resolution found in MIDI file
	//	MPQN – microseconds per quaternote or quaternote duration, range is 0-8355711 microseconds (according to MIDI file specification)
	//	QLS – seconds per quaternote (MPQN/1000000)
	//	TDPS – seconds per tick (QLS/PPQN)
	//  TPS - ticks per second (1000000/QLS/PPQN) = (1000000/1/MPQN/1000000/PPQN)
		
						
	bool initialized = false;
	MidiFile midiFile;
	[HideInInspector]
	public List<IList<MidiEvent>> tracks = new List<IList<MidiEvent>> ();
	public int[] eventPos = new int[0];
	public bool[] endOfTrack = new bool[0];
	public bool[] muteTrack = new bool[0];
			
	readonly float deltaTimeResolution = 0.001f;
	
	float deltaTimeNumerator = 0f;
	float deltaTimeRest = 0f;
			
	public bool repeatBarSelection = false;
	public int startBar = 0;
	public int endBar = 0;

	void Awake ()
	{
		singleton = this;
		Initialize (midiFileTextAsset, vocalsClip, musicClip);
	}

	//System.Timers.Timer timer;

	void Start ()
	{
//		timer = new System.Timers.Timer (20);
//		timer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) => {
//			if (audioVocals) {
//				Debug.Log (audioVocals.time);
//			}
//		};
//		timer.Start ();
		if (initialized) {
			FireAction (0.1f, () => {
				if (OnMidiLoaded != null)
					OnMidiLoaded ();	
				if (playOnStart) {
					this.FireAction (delay, () => {
						Play (pickUpBar);
					});	
				}
			});
		}
	}

	int pickUpBarCounter = 0;
	FireCoroutine pickUpCoroutine;
	bool onPickupBarBeginFired = false;

	void PickUpBar ()
	{
		state = State.PickUpBar;
		if (pickUpBarCounter > 0) {
			if (!onPickupBarBeginFired && OnPickupBarBegin != null) {
				OnPickupBarBegin (bar); 
				onPickupBarBeginFired = true;
			}
			pickUpBarCounter--;	
			TimeKeeper.BeatEvent (timeSignatureNumerator - pickUpBarCounter, timeSignatureNumerator, timeSignatureDenominator);
			pickUpCoroutine = this.FireAction (60f / bars [bar].tempo / (timeSignatureDenominator / 4f) / speed, PickUpBar);
		} else {
			CancelPickUpBarCounting ();
			if (OnPickupBarEnd != null)
				OnPickupBarEnd ();
			if (music && audioMusic)
				audioMusic.Play ();
			if (vocals && audioVocals)
				audioVocals.Play ();
			state = State.Playing;
		}
	}

	public class FireCoroutine
	{
		public readonly MonoBehaviour monoBehaviour;
		
		private bool killed = false;

		public FireCoroutine (MonoBehaviour m, float delay, Action onFire)
		{
			monoBehaviour = m;
			m.StartCoroutine (Fire (delay, onFire));
		}

		public void Kill ()
		{
			killed = true;	
		}

		IEnumerator Fire (float delay, Action onFire)
		{
			while (delay > 0f) {
				if (killed)
					yield break;
				delay -= Time.deltaTime;
				yield return null;				
			}	
			onFire.Invoke ();
		}
	}

	public FireCoroutine FireAction (float delay, Action onFire)
	{
		return new FireCoroutine (this, delay, onFire);
	}

	public void Play (bool aPickUpBar)
	{
		Debug.Log ("KARAOKE PLAY");
		CancelPickUpBarCounting ();
		Debug.Log (state);
		if (midiFile == null)
			return;
		if (state == State.Finished)
			ResetSequencer ();

		pickUpBar = aPickUpBar;

		timeSignatureNumerator = bars [bar].timeSignatureNumerator;
		timeSignatureDenominator = bars [bar].timeSignatureDenominator;
		tempo = bars [bar].tempo;
		if (OnTempoChange != null)
			OnTempoChange (tempo);

		if (pickUpBar) {
			Debug.Log ("MIDIKARAOKE PLAY WITH PICKUP BAR");
			//pickUpBar = false;
			pickUpBarCounter = timeSignatureNumerator;
			state = State.PickUpBar;
			onPickupBarBeginFired = false;
			PickUpBar ();
		} else {
			Debug.Log ("MIDIKARAOKE PLAY WITHOUT PICKUP BAR");
			CancelPickUpBarCounting ();
			if (music)
				audioMusic.Play ();
			if (vocals)
				audioVocals.Play ();
			state = State.Playing;
		}
	}

	public void SetMusicVolume (float v)
	{
		if (music && audioMusic) {
			audioMusic.volume = v;
		}
	}

	public void SetVocalsVolume (float v)
	{
		if (vocals && audioVocals) {
			audioVocals.volume = v;
		}
	}

	public void Continue ()
	{
		Debug.Log ("KARAOKE CONTINUE");
		if (music)
			audioMusic.Play ();
		if (vocals)
			audioVocals.Play ();
		state = State.Playing;
	}

	void CancelPickUpBarCounting ()
	{
		if (pickUpCoroutine != null) { 
			pickUpCoroutine.Kill ();
			pickUpCoroutine = null;
		}
	}

	public void Pause ()
	{
		CancelPickUpBarCounting ();
		if (midiFile == null)
			return;
		audioMusic.Pause ();
		audioVocals.Pause ();
		if (midiOut) {
			MidiOut.AllSoundOff ();
		}
		state = State.Pausing;
	}

	public void SetVocals (bool v)
	{
		vocals = v;	
		if (!vocalsClip)
			return;
		
		if (vocals) {
			if (audioMusic && audioMusic.isPlaying) {
				audioVocals.time = audioMusic.time;
				audioVocals.Play ();
			} else {
				if (state == State.Playing) {
					audioVocals.time = time;
					audioVocals.Play ();	
				}
			}
		} else {
			audioVocals.Pause ();
		}
	}

	public void SetMusic (bool m)
	{
		music = m;
		if (!musicClip)
			return;
			
		if (music) {
			if (audioVocals && audioVocals.isPlaying) {
				audioMusic.time = audioVocals.time;
				audioMusic.Play ();
			} else {
				if (state == State.Playing) {
					audioMusic.time = time;
					audioMusic.Play ();	
				}
			}
		} else {
			audioMusic.Pause ();
		}
	}

	void ResetSequencer ()
	{
		ticks = 0;
		time = 0f;
		fractionalTicks = 0;
		beat = 0;
		beatCount = 0;
		bar = 0;
		lastTime = 0f;
		wordPos = 0;
		wordOffsetPos = 0;
		sentencePos = 0;
		versePos = 0;
		deltaTimeNumerator = 0f;
		deltaTimeRest = 0f;
		lastDeltaTime = 0f;
		lastDeltaTicks = 0f;
		//pickUpBar = true;
		
		if (audioMusic)
			audioMusic.time = 0;
		if (audioVocals)
			audioVocals.time = 0;
		if (midiFile == null) {
			eventPos = new int[0];
			endOfTrack = new bool[0];
			muteTrack = new bool[0];
		} else {
			eventPos = new int[midiFile.Tracks];
			endOfTrack = new bool[midiFile.Tracks];	
			muteTrack = new bool[midiFile.Tracks];
		}
		foreach (WordText wt in words) {
			wt.finishFired = false; 
			wt.finishOffsetFired = false;
		}
	}

	public void Stop ()
	{
		CancelPickUpBarCounting ();
		if (midiFile == null)
			return;
		state = State.None;
		ResetSequencer ();
		if (midiOut) {
			MidiOut.AllPedalsOff ();
			MidiOut.AllSoundOff ();
		}

		if (audioMusic) {
			audioMusic.Stop ();
		}

		if (audioVocals) {
			audioVocals.Stop ();
		}

		if (repeatBarSelection) {
			SetBar (startBar, false);	
		}
		if (OnStop != null)
			OnStop ();
	}

	public void SetSpeed (float s)
	{
		if (audioMusic) {
			audioMusic.pitch = s; 
			audioMusic.pitch *= (tempo / tempoOriginal);
		}
		
		if (audioVocals) {
			audioVocals.pitch = s;
			audioVocals.pitch *= (tempo / tempoOriginal);
		}
		
		speed = s;
	}

	public void SetTempo (float t)
	{
		if (audioMusic) {
			audioMusic.pitch = speed; 
			audioMusic.pitch *= (tempo / tempoOriginal);
		}
		
		if (audioVocals) {	
			audioVocals.pitch = speed; 
			audioVocals.pitch *= (tempo / tempoOriginal);
		}
		
		tempo = t;	
	}

	MidiCommandCode command;
	MidiEvent midiEvent;
	MetaEvent metaEvent;
	//ControlChangeEvent controlEvent;
	bool cancelUpdate = false;
	
	float deltaTime = 0f;
	float periodResolution = 0f;
	//float ticksPerClock = 0f;
	float deltaTicks = 0f;

	bool midiFinished = false;
	bool musicFinished = false;
	bool vocalsFinished = false;

	int deltaTimeIterator = 0;
}
