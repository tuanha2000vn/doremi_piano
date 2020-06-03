using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor;
using System.Text.RegularExpressions;
using System.IO;

using Image = UnityEngine.UI.Image;

namespace ForieroEditor
{
	public static partial class Menu
	{

		[MenuItem ("Assets/SOX/ToMono")]
		public static void ConvertAudioToMono ()
		{
			switch (EditorUtility.DisplayDialogComplex ("Convert", "Convert selected audio to MONO?", "Yes - Suffix", "No", "Yes")) {
			case 0:
				FFMpeg.ConvertSelectedAudioToMono ("_mono");
				break;
			case 1:

				break;
			case 2:
				FFMpeg.ConvertSelectedAudioToMono ("");
				break;
			}	
		}

		[MenuItem ("Assets/FFMpeg/MP3/To OGG")]
		public static void ConvertAudioMP3ToOGG ()
		{
			switch (EditorUtility.DisplayDialogComplex ("Convert", "Convert selected to OGG?", "Yes - Suffix", "No", "Yes")) {
			case 0:
				FFMpeg.ConvertSelectedAudio ("_ogg", "ogg", "mp3");
				break;
			case 1:

				break;
			case 2:
				FFMpeg.ConvertSelectedAudio ("", "ogg", "mp3");
				break;
			}	
		}

		[MenuItem ("Assets/FFMpeg/MP3/To WAV")]
		public static void ConvertAudioMP3ToWAV ()
		{
			switch (EditorUtility.DisplayDialogComplex ("Convert", "Convert selected to WAV?", "Yes - Suffix", "No", "Yes")) {
			case 0:
				FFMpeg.ConvertSelectedAudio ("_wav", "wav", "mp3");
				break;
			case 1:

				break;
			case 2:
				FFMpeg.ConvertSelectedAudio ("", "wav", "mp3");
				break;
			}	
		}

		[MenuItem ("Assets/FFMpeg/AIF/To MP3")]
		public static void ConvertAudioAIFToMP3 ()
		{
			switch (EditorUtility.DisplayDialogComplex ("Convert", "Convert selected to MP3?", "Yes - Suffix", "No", "Yes")) {
			case 0:
				FFMpeg.ConvertSelectedAudio ("_mp3", "mp3", "aif");
				break;
			case 1:

				break;
			case 2:
				FFMpeg.ConvertSelectedAudio ("", "mp3", "aif");
				break;
			}	
		}

		[MenuItem ("Assets/FFMpeg/AIF/To OGG")]
		public static void ConvertAudioAIFToOGG ()
		{
			switch (EditorUtility.DisplayDialogComplex ("Convert", "Convert selected to OGG?", "Yes - Suffix", "No", "Yes")) {
			case 0:
				FFMpeg.ConvertSelectedAudio ("_ogg", "ogg", "aif");
				break;
			case 1:

				break;
			case 2:
				FFMpeg.ConvertSelectedAudio ("", "ogg", "aif");
				break;
			}	
		}

		[MenuItem ("Assets/FFMpeg/WAV/To MP3")]
		public static void ConvertAudioWAVToMP3 ()
		{
			switch (EditorUtility.DisplayDialogComplex ("Convert", "Convert selected to MP3?", "Yes - Suffix", "No", "Yes")) {
			case 0:
				FFMpeg.ConvertSelectedAudio ("_mp3", "mp3", "wav");
				break;
			case 1:

				break;
			case 2:
				FFMpeg.ConvertSelectedAudio ("", "mp3", "wav");
				break;
			}	
		}

		[MenuItem ("Assets/FFMpeg/WAV/To OGG")]
		public static void ConvertAudioWAVToOGG ()
		{
			switch (EditorUtility.DisplayDialogComplex ("Convert", "Convert selected to OGG?", "Yes - Suffix", "No", "Yes")) {
			case 0:
				FFMpeg.ConvertSelectedAudio ("_ogg", "ogg", "wav");
				break;
			case 1:

				break;
			case 2:
				FFMpeg.ConvertSelectedAudio ("", "ogg", "wav");
				break;
			}	
		}
	}
}
