using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

using ForieroEngine.MIDIUnified.Plugins;

namespace ForieroEngine.MIDIUnified
{
	public static class BASS24Synth
	{
		public static string name = "BASS Synth";
		private static bool _use = false;

		public static bool use {
			get	{ return _use; }
			set { 
				_use = value;
				active = _use ? active : false;
			}
		}

		public static bool active = false;

		#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_EDITOR

		public static void Register (string userName, string password)
		{

		}

		[DllImport ("softsynth")]
		public static extern int Start (int freq, int channels);

		[DllImport ("softsynth")]
		public static extern int Stop ();

		[DllImport ("softsynth")]
		public static extern int SendMidiMessage (int Command, int Data1, int Data2);

		#elif UNITY_IOS
		
		public static int Start(int freq, int channels){
			return BASS24Start(freq, channels);
		}

		public static int Stop(){
			return BASS24Stop();
		}

		public static int SendMidiMessage(int Command, int Data1, int Data2){
			return BASS24SendMidiMessage(Command, Data1, Data2);
		}
		
		[DllImport ("__Internal")]
	    private static extern int BASS24Start(int freq, int channels);
		
		[DllImport ("__Internal")]	
       	private static extern int BASS24Stop();
		
		[DllImport ("__Internal")]
		private static extern int BASS24SendMidiMessage(int Command, int Data1, int Data2);



#elif UNITY_ANDROID || UNITY_WSA
		public static void Register(string user, string password){
			BASS24NETSynth.Register(user, password);	
		}

		public static int Start(int freq, int channels){
			return BASS24NETSynth.Start(freq, channels);
		}

		public static int Stop(){
			return BASS24NETSynth.Stop();
		}

		public static int SendMidiMessage(int Command, int Data1, int Data2){
			return BASS24NETSynth.SendMidiMessage(Command, Data1, Data2);
		}

//#elif UNITY_WSA
//		public static void Register(string user, string password){
//			MidiSynthPlugin.Register(user, password);	
//		}
//
//		public static int Start(int freq, int channels){
//			return MidiSynthPlugin.Start(freq, channels);			
//		}
//
//		public static int Stop(){
//			return MidiSynthPlugin.Stop ();
//		}
//
//		public static int SendMidiMessage(int Command, int Data1, int Data2){
//			return MidiSynthPlugin.SendShortMessage ((byte)Command, (byte)Data1, (byte)Data2);
//		}





#else
		public static int Start(int freq, int channels){
			Debug.LogError ("BASS24 Synthesizer not supported!");
			return 0;
		}

		public static int Stop(){
			Debug.LogError ("BASS24 Synthesizer not supported!");
			return 0;
		}

		public static int SendMidiMessage(int Command, int Data1, int Data2){
			Debug.LogError ("BASS24 Synthesizer not supported!");
			return 0;
		}
#endif
	}
}
