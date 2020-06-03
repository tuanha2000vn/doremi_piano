using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor;
using System.Text.RegularExpressions;
using System.IO;

using ForieroEditor.Extensions;

using Image = UnityEngine.UI.Image;

namespace ForieroEditor
{
	public static class FFMpeg
	{
		public static string cmdFFMpeg {
			get {
				#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
				return "/opt/local/bin/ffmpeg";
				#elif UNITY_EDITOR_WIN
				return "ffmpeg";
				#endif
			}
		}

		public static string cmdSOX {
			get {
				#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
				return "/opt/local/bin/sox";
				#elif UNITY_EDITOR_WIN
				return "sox";
				#endif
			}
		}

		#region ConvertToMP3

		public static void ConvertSelectedAudio (string suffix, string toExtension, string fromExtension = null)
		{
			toExtension = toExtension.ToLower ();

			if (!string.IsNullOrEmpty (fromExtension)) {
				fromExtension = fromExtension.ToLower ();
			}

			foreach (Object o in Selection.GetFiltered(typeof(AudioClip), SelectionMode.DeepAssets)) {
				if (o is AudioClip) {
					string path = Path.Combine (Directory.GetCurrentDirectory (), AssetDatabase.GetAssetPath (o.GetInstanceID ()));
					if (string.IsNullOrEmpty (fromExtension)) {
						ConvertAudio (path, suffix, toExtension);
					} else if (Path.GetExtension (path).ToLower ().Contains (fromExtension)) {
						ConvertAudio (path, suffix, toExtension);
					} 
				}
			}
			AssetDatabase.Refresh ();
		}

		public static void ConvertSelectedAudioToMono (string suffix)
		{
			foreach (Object o in Selection.GetFiltered(typeof(AudioClip), SelectionMode.DeepAssets)) {
				if (o is AudioClip) {
					string path = Path.Combine (Directory.GetCurrentDirectory (), AssetDatabase.GetAssetPath (o.GetInstanceID ()));

					string outputPath = Path.GetDirectoryName (path);
					string fileNameWithoutExtension = Path.GetFileNameWithoutExtension (path);
					string extension = Path.GetExtension (path).ToLower ();
					string outputAudioPath = Path.Combine (outputPath, fileNameWithoutExtension + suffix + extension);

					string args = path.FixOSPath ().DoubleQuotes () + " -c 1 " + outputAudioPath.FixOSPath ().DoubleQuotes ();
					Debug.Log (cmdSOX + " " + args);
					GenerateProcess (cmdSOX, args);
				}
			}
			AssetDatabase.Refresh ();
		}

		public static void ConvertAudio (string inputPngFilePath, string suffix, string toExtension)
		{
			toExtension = toExtension.ToLower ();

			string path = Path.GetDirectoryName (inputPngFilePath);
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension (inputPngFilePath);
			string extension = Path.GetExtension (inputPngFilePath).ToLower ();

			string outputPngFilePath = Path.Combine (path, fileNameWithoutExtension + suffix + "." + toExtension);

			string codec = "";

			if (toExtension == "mp3") {
				codec = "libmp3lame";
			} else if (toExtension == "ogg") {
				codec = "libvorbis";
			} else if (toExtension == "aac") {
				codec = "libfaac";
			} else if (toExtension == "ac3") {
				codec = "ac3";
			} 

			if (string.IsNullOrEmpty (codec)) {
				Debug.LogError ("Codec not found for conversion : " + extension + " => " + toExtension);
				return;
			}
					
			if (extension.Replace (".", "") == toExtension) {
				Debug.LogWarning ("File will not be converted : " + extension + " => " + toExtension);
				return;
			}

			string args = "-i " + inputPngFilePath.FixOSPath ().DoubleQuotes () + " -acodec " + codec + " " + outputPngFilePath.FixOSPath ().DoubleQuotes ();
			Debug.Log (cmdFFMpeg + " " + args);
			GenerateProcess (cmdFFMpeg, args);
		}

		#endregion

		public static void GenerateProcess (string aCommand, string anArguments)
		{
			System.Diagnostics.Process p = new System.Diagnostics.Process ();
			string buildOutput = "";
			try {
				p.StartInfo.FileName = aCommand;
				p.StartInfo.Arguments = anArguments;
				p.StartInfo.CreateNoWindow = true;
				p.StartInfo.UseShellExecute = false;
				p.StartInfo.RedirectStandardOutput = true;
				p.StartInfo.RedirectStandardError = true;
				p.Start ();
				string output = p.StandardOutput.ReadToEnd ();
				string error = p.StandardError.ReadToEnd ();
				p.WaitForExit ();

				p.Close ();

				if (!string.IsNullOrEmpty (output)) {
					Debug.Log ("OUTPUT : " + output);
				}

				if (!string.IsNullOrEmpty (error)) {
					Debug.Log ("ERROR : " + error);	
				}

			} catch (System.Exception e) {
				buildOutput += "\n\n" + e.Message;
				UnityEngine.Debug.LogWarning (buildOutput);
				return;
			} finally {
				p.Dispose ();
				System.GC.Collect ();
			}
		}
	}
}
