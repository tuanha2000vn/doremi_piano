using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Utils;
using System;

public class MidiDispatcher : MonoBehaviour, IMidiEvents
{

	public static MidiDispatcher singleton;

	public event ShortMessageEventHandler ShortMessageEvent;

	class MidiMessage
	{
		public int midiIndex;
		public int channel;
		public int volume;
		public float duration;
		public float delay;
		public bool played;
		public Action started;
		public Action finished;
	}

	
	static List<MidiMessage> dispatchedNotes = new List<MidiMessage> (20);

	void OnEnable ()
	{
		singleton = this;
	}

	void Awake ()
	{
		singleton = this;
	}

	void OnDestroy ()
	{
		singleton = null;
	}

	public static void DispatchNote (int midiIndex, int volume, int channel, float duration, float delay = 0f, Action started = null, Action finished = null)
	{
		SetupDispatcher ();
		if (delay <= 0f) {
			MidiOut.NoteOn (midiIndex, volume, channel);
			if (singleton.ShortMessageEvent != null) {
				singleton.ShortMessageEvent (channel + CommandEnum.NoteOn.ToInt (), midiIndex, volume);
			}

			if (started != null) {
				started ();
			}
		}

		dispatchedNotes.Add (
			new MidiMessage () {
				midiIndex = midiIndex,
				channel = channel,
				volume = volume,
				duration = duration,
				delay = delay,
				played = delay <= 0f ? true : false,
				started = started,
				finished = finished
			}
		);	
	}

	public static void SetupDispatcher ()
	{
		if (singleton == null) {
			GameObject go = new GameObject ();
			go.transform.name = "MIDIUnified Dispatcher";
			singleton = go.AddComponent<MidiDispatcher> ();
		}
	}

	void Update ()
	{
		for (int i = 0; i < dispatchedNotes.Count; i++) {
			MidiMessage m = dispatchedNotes [i];
			if (!m.played) {
				m.delay -= Time.deltaTime;
				if (m.delay <= 0) {
					MidiOut.NoteOn (m.midiIndex, m.volume, m.channel);

					if (ShortMessageEvent != null) {
						ShortMessageEvent (m.channel + CommandEnum.NoteOn.ToInt (), m.midiIndex, m.volume);
					}

					m.played = true;

					if (m.started != null) {
						m.started ();
					}
				}
			} else {
				m.duration -= Time.deltaTime;
			}
			
			if (m.duration <= 0f) {
				MidiOut.NoteOff (m.midiIndex, m.channel);

				if (ShortMessageEvent != null) {
					ShortMessageEvent (m.channel + CommandEnum.NoteOff.ToInt (), m.midiIndex, 0);
				}

				if (m.finished != null) {
					m.finished ();
				}
			}
		}

		for (int i = dispatchedNotes.Count - 1; i >= 0; i--) {
			if (dispatchedNotes [i].duration <= 0f) {
				dispatchedNotes.RemoveAt (i);
			}
		}
	}
}
