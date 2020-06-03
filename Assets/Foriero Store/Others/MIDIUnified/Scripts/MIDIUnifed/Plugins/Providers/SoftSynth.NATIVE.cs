using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace ForieroEngine.MIDIUnified
{
	public static class NATIVESynth
	{
		public static string name = "NATIVE Synth";
		private static bool _use = false;

		public static bool use {
			get	{ return _use; }
			set { 
				_use = value;
				active = _use ? active : false;
			}
		}

		public static bool active = false;
						
		public static Synth.SynthSettingsIOS.SoundBankEnum soundBank = Synth.SynthSettingsIOS.SoundBankEnum.sf2;

		#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		public static int Start (int freq, int channels)
		{
			return Start ();
		}

		[DllImport ("osxsynth")]
		private static extern int Start ();

		[DllImport ("osxsynth")]
		public static extern int Stop ();

		[DllImport ("osxsynth")]
		public static extern int SendMessage (int Command, int Data1, int Data2);

		#elif UNITY_IOS
		public static int Start(int freq, int channels){
			return StartSoftSynth(freq, channels, (int)soundBank);			
		}
		
		public static int Stop(){
			return StopSoftSynth();
		}
		
		public static int SendMessage(int Command, int Data1, int Data2){
			return SendSynthMessage(Command, Data1, Data2);
		}
		
		[DllImport ("__Internal")]
		private static extern int StartSoftSynth(int sampleRate, int channelCount, int mode);
		
		[DllImport ("__Internal")]
		private static extern int StopSoftSynth();
		
		[DllImport ("__Internal")]
		private static extern int SendSynthMessage(int Command, int Data1, int Data2);


		


#elif UNITY_ANDROID
		public static int Start(int freq, int channels){
			return AndroidSynth.Start(freq, channels);
		}
		
		public static int Stop(){
			return AndroidSynth.Stop();
		}
		
		public static int SendMessage(int Command, int Data1, int Data2){
			return AndroidSynth.SendMidiMessage(Command, Data1, Data2);
		}
		
		public static class AndroidSynth
		{

			[DllImport ("androidsynth")]
			public static extern int Start (int x, int y);

			[DllImport ("androidsynth")]
			public static extern int SendMidiMessage (int command, int data1, int data2);

			[DllImport ("androidsynth")]
			public static extern int Stop ();

		}

		


#else
		public static int Start(int freq, int channels){
			Debug.LogError ("NATIVE Synthesizer not supported!");
			return 0;
		}

		public static int Stop(){
			Debug.LogError ("NATIVE Synthesizer not supported!");
			return 0;
		}

		public static int SendMessage(int Command, int Data1, int Data2){
			Debug.LogError ("NATIVE Synthesizer not supported!");
			return 0;
		}
#endif
	}
}
