using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

using Debug = UnityEngine.Debug;

using ForieroEngine.Extensions;
using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Plugins;
using ForieroEditor;

public partial class MIDIEditor : EditorWindow
{
	public static MIDIEditor window;

	//bool save = false;

	[MenuItem ("Foriero/Tools/Midi &m")]
	static void Init ()
	{
		window = EditorWindow.GetWindow (typeof (MIDIEditor)) as MIDIEditor;
		window.titleContent = new GUIContent ("MIDI");
	}

	static string messages = "";

	[InitializeOnLoadMethod]
	static void AutoInit ()
	{
		if (!MidiINPlugin.initialized) {
			EditorDispatcher.StartThread ((a) => {
				MidiINPlugin.Init ();
				MidiINPlugin.Refresh ();
				EditorDispatcher.Dispatch ((b) => {
					MidiINPlugin.RestoreEditorConnections ();
					midiINInitialized = true;
				}, new { a = "" });
			}, new { a = "" });
		} else {
			midiINInitialized = true;
		}

		if (!MidiOUTPlugin.initialized) {
			EditorDispatcher.StartThread ((a) => {
				MidiOUTPlugin.Init ();
				MidiOUTPlugin.Refresh ();
				EditorDispatcher.Dispatch ((b) => {
					MidiOUTPlugin.RestoreEditorConnections ();
					midiOUTInitialized = true;
				}, new { a = "" });
			}, new { a = "" });
		} else {
			midiOUTInitialized = true;
		}

		EditorApplication.update -= EditorUpdate;
		EditorApplication.update += EditorUpdate;
	}

	~MIDIEditor ()
	{
		EditorApplication.update -= EditorUpdate;
	}

	static bool midiOUTInitialized = false;
	static bool midiINInitialized = false;

	static void EditorUpdate ()
	{
		//return;
		MidiMessage midiMessage = new MidiMessage ();
		bool received = false;
		while (MidiINPlugin.PopMessage (out midiMessage, true) != 0) {
			messages = midiMessage.ToString () + System.Environment.NewLine + messages;
			received = true;
		}

		if (window != null && received && oneFrameDelay) {
			window.Repaint ();
		}
	}

	Vector2 scrollMessages = Vector2.zero;

	bool debug = false;

	static bool oneFrameDelay = false;

	void OnGUI ()
	{
		if (window == null) {
			window = EditorWindow.GetWindow (typeof (MIDIEditor)) as MIDIEditor;
			oneFrameDelay = false;
			return;
		}

		if (!midiINInitialized || !midiOUTInitialized) {
			EditorGUILayout.HelpBox ("Initializing MIDI. Please wait....", MessageType.Info);
			oneFrameDelay = false;
			return;
		}

		if (!oneFrameDelay) {
			oneFrameDelay = true;
			return;
		}

		if (!oneFrameDelay) {
			oneFrameDelay = true;
			EditorGUILayout.HelpBox ("Waiting one frame...", MessageType.Info);
			return;
		}

		DrawINOUT ();

		if (GUILayout.Button ("Clear")) {
			messages = "";
		}

		scrollMessages = GUILayout.BeginScrollView (scrollMessages);
		messages = GUILayout.TextArea (messages, GUILayout.ExpandWidth (true), GUILayout.ExpandHeight (true));
		GUILayout.EndScrollView ();

		if (debug) {
			GUILayout.Label ("Connected IN Devices : " + MidiINPlugin.GetConnectedDeviceCount ().ToString ());
			GUILayout.Label ("Connected OUT Devices : " + MidiOUTPlugin.GetConnectedDeviceCount ().ToString ());
		}

		debug = GUILayout.Toggle (debug, "Debug");
	}

	float lineHeight = 25f;
	Color backgroundColor;

	void DrawINOUT ()
	{
		backgroundColor = GUI.backgroundColor;

		int count = MidiINPlugin.deviceNames.Count > MidiOUTPlugin.deviceNames.Count ? MidiINPlugin.deviceNames.Count : MidiOUTPlugin.deviceNames.Count;

		if (count == 0) {
			EditorGUILayout.HelpBox ("No MIDI connection found!!!", MessageType.Info);
		}

		float selectionHeight = count * (lineHeight + 5);
		float dialogHeight = selectionHeight + 65;

		Rect defaultRect = new Rect (0, 0, window.position.width, dialogHeight);

		GUI.BeginGroup (defaultRect);

		float width = defaultRect.width;
		float halfWidth = width / 2f;

		GUI.Box (new Rect (-5f, -5f, defaultRect.width + 10, defaultRect.height + 10), "");

		GUILayout.BeginHorizontal ();
		GUILayout.Box ("Midi IN", GUILayout.Width (halfWidth));
		GUILayout.Box ("Midi OUT", GUILayout.Width (halfWidth));
		GUILayout.EndHorizontal ();

		GUILayout.Box ("", GUILayout.ExpandWidth (true), GUILayout.Height (5));

		bool exists = false;

		for (int i = 0; i < count; i++) {
			GUILayout.BeginHorizontal ();
			if (i < MidiINPlugin.deviceNames.Count) {
				exists = false;
				foreach (MidiDevice inDevice in MidiINPlugin.connectedEditorDevices) {
					if (MidiINPlugin.deviceNames [i] == inDevice.name) {
						exists = true;
						break;
					}
				}
				GUI.backgroundColor = exists ? Color.green : backgroundColor;
				if (GUILayout.Button (MidiINPlugin.deviceNames [i], GUILayout.Width (halfWidth), GUILayout.Height (lineHeight))) {
					if (exists) {
						MidiINPlugin.DisconnectDeviceByName (MidiINPlugin.deviceNames [i], true);
					} else {
						MidiINPlugin.ConnectDevice (i, true);
					}
					MidiINPlugin.StoreEditorConnections ();
				}
				GUI.backgroundColor = backgroundColor;
			} else {
				GUILayout.Label ("", GUILayout.Width (halfWidth), GUILayout.Height (lineHeight));
			}

			if (i < MidiOUTPlugin.deviceNames.Count) {
				exists = false;
				foreach (MidiDevice outDevice in MidiOUTPlugin.connectedEditorDevices) {
					if (MidiOUTPlugin.deviceNames [i] == outDevice.name) {
						exists = true;
						break;
					}
				}
				GUI.backgroundColor = exists ? Color.green : backgroundColor;
				if (GUILayout.Button (MidiOUTPlugin.deviceNames [i], GUILayout.Width (halfWidth), GUILayout.Height (lineHeight))) {
					if (exists) {
						MidiOUTPlugin.DisconnectDeviceByName (MidiOUTPlugin.deviceNames [i], true);
					} else {
						MidiOUTPlugin.ConnectDevice (i, true);
					}
					MidiOUTPlugin.StoreEditorConnections ();
				}
				GUI.backgroundColor = backgroundColor;
			} else {
				GUILayout.Label ("", GUILayout.Width (halfWidth), GUILayout.Height (lineHeight));
			}

			GUILayout.EndHorizontal ();
		}

		GUILayout.Box ("", GUILayout.ExpandWidth (true), GUILayout.Height (5));

		GUILayout.BeginHorizontal ();

		if (GUILayout.Button ("Refresh", GUILayout.Width (halfWidth))) {
			MidiINPlugin.Refresh ();
			MidiOUTPlugin.Refresh ();
		}

		if (GUILayout.Button ("Reset", GUILayout.Width (halfWidth))) {
			MidiOut.AllSoundOff ();
			MidiOut.ResetAllControllers ();
		}

		GUILayout.EndHorizontal ();

		GUI.EndGroup ();
	}
}