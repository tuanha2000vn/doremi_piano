using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

using UnityEditor.Audio;

using Debug = UnityEngine.Debug;

using ForieroEngine.Extensions;
using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Plugins;

public partial class MIDIEditor: EditorWindow
{
	//	public static List<AudioMixerController> GetAudioMixerControllers(){
	//		Assembly unityEditorAssembly = typeof(AudioMixerWindow).Assembly;
	//		Type AudioMixerWindowClass = unityEditorAssembly.GetType( "UnityEditor.AudioMixerWindow" );
	//		MethodInfo GetAllControllers = AudioMixerWindowClass.GetMethod( "GetAllControllers", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static, null, new System.Type[] {  }, null );
	//		var returnValue = GetAllControllers.Invoke( [CALLING THIS AudioMixerWindow OBJECT], new System.Object[] { [INSERT THESE PARAMETERS HERE: ] } ) as System.Collections.Generic.List`1[UnityEditor.Audio.AudioMixerController];
	//	}
}