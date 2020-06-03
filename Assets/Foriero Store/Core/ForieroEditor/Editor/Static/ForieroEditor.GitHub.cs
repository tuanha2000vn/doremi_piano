using UnityEngine;
using System.IO;
using ForieroEditor.Extensions;

namespace ForieroEditor
{
	public static class GitHub
	{

		/// <summary>
		/// Gets the repository files.
		/// </summary>
		/// <param name="gitPath">Git path. https://github.com/EsotericSoftware/spine-runtimes/trunk/spine-unity/Assets/spine-unity/</param>
		/// <param name="assetsPath">Assets path. Assets/Spine</param>
		public static void GetRepositoryFiles (string gitHubPath, string assetsPath)
		{
			string cmd = "svn";

			string path = Path.Combine (Directory.GetCurrentDirectory (), assetsPath).FixOSPath ();

			string args = "export --force \"" + gitHubPath + "\" \"" + path + "\"";

			string directory = Path.GetDirectoryName (path);

			if (!Directory.Exists (directory)) {
				Directory.CreateDirectory (directory);
			}

			Proces (cmd, args);
		}

		static void Proces (string cmd, string args)
		{
			Debug.Log (cmd + " " + args);



			System.Diagnostics.Process p = new System.Diagnostics.Process ();
			try {
				p.StartInfo.FileName = cmd;
				p.StartInfo.Arguments = args;
				p.StartInfo.RedirectStandardError = true;
				p.StartInfo.RedirectStandardOutput = true;
				p.StartInfo.UseShellExecute = false;
				p.Start ();
				string error = p.StandardError.ReadToEnd ();
				string output = p.StandardOutput.ReadToEnd ();
				p.WaitForExit ();

				if (!string.IsNullOrEmpty (error)) {
					Debug.LogError (error);
				}

				if (!string.IsNullOrEmpty (output)) {
					Debug.Log (output);
				}
			} catch (System.Exception e) {
				Debug.LogError (e.Message);            
			} finally {
				p.Close ();
				p.Dispose ();
			}
		}
	}
}
