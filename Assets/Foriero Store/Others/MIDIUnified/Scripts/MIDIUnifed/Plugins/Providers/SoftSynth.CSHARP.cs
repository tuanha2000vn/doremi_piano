using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace ForieroEngine.MIDIUnified
{
	public static class CSHARPSynth
	{
		public static string name = "CSHARP Synth";
		private static bool _use = false;

		public static bool use {
			get	{ return _use; }
			set { 
				_use = value;
				active = _use ? active : false;
			}
		}

		public static bool active = false;

		
		public static int Start (int sampleRate, int polyphony)
		{
			return CSharpSynth.Init (sampleRate, polyphony) == true ? 1 : 0;
		}

		
		public static int Stop ()
		{
			if (CSharpSynth.singleton) {
				GameObject.Destroy (CSharpSynth.singleton.gameObject);
			}
			return 1;
		}

		public static int SendMessage (int Command, int Data1, int Data2)
		{
			if (CSharpSynth.singleton) {
				CSharpSynth.ShortMessage ((byte)Command, (byte)Data1, (byte)Data2);
				return 1;
			} else {
				return 0;
			}
		}
	}
}
