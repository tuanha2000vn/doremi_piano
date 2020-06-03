using UnityEngine;
using System;
using System.Collections;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using ForieroEditor.Extensions;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

public class MIDIUnifiedPostprocessBuildPlayer : ScriptableObject
{

    private static void RunProcess(string aCommand, string anArguments)
    {
        System.Diagnostics.Process p = new System.Diagnostics.Process();
        string buildOutput = "";
        try
        {
            p.StartInfo.FileName = aCommand;
            p.StartInfo.Arguments = anArguments;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            string error = p.StandardError.ReadToEnd();
            p.WaitForExit();
            p.Close();
            if (!string.IsNullOrEmpty(output))
            {
                Debug.Log("OUTPUT : " + output);
            }

            if (!string.IsNullOrEmpty(error))
            {
                Debug.Log("ERROR : " + error);
            }
        }
        catch (System.Exception e)
        {
            buildOutput += "\n\n" + e.Message;
            UnityEngine.Debug.LogWarning(buildOutput);
            return;
        }
        finally
        {
            p.Dispose();
            System.GC.Collect();
        }
    }

    private static bool IsVersion42OrLater()
    {
        string version = Application.unityVersion;
        string[] versionComponents = version.Split('.');

        int majorVersion = 0;
        int minorVersion = 0;

        try
        {
            if (versionComponents != null && versionComponents.Length > 0 && versionComponents[0] != null)
                majorVersion = Convert.ToInt32(versionComponents[0]);
            if (versionComponents != null && versionComponents.Length > 1 && versionComponents[1] != null)
                minorVersion = Convert.ToInt32(versionComponents[1]);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error parsing Unity version number: " + e);
        }

        return (majorVersion > 4 || (majorVersion == 4 && minorVersion >= 2));
    }

    [PostProcessBuild(20)]
    static void OnPostprocessBuildPlayer(BuildTarget target, string buildPath)
    {
        MIDIUnifiedPreprocessBuildPlayer.called = false;

        string sf2PathSource = "";
        string sf2PathDestination = "";

        string[] plugins = Directory.GetFiles(Application.dataPath, "midiunified_plugins.txt", SearchOption.AllDirectories);

        if (plugins == null || plugins.Length == 0)
            return;

        string pluginsFolder = Path.GetDirectoryName(plugins[0]).FixOSPath();

        Debug.Log("MIDIUnified : " + pluginsFolder);

        sf2PathSource = "Assets/Resources/soundfont.sf2".GetFullPathFromAssetPath();

#if UNITY_IOS
						
		if (target == BuildTarget.iOS) {
        	PBXProject project = new PBXProject ();

			string projPath = buildPath + "/Unity-iPhone.xcodeproj/project.pbxproj";

			project.ReadFromFile(projPath);

			string targetGUID = project.TargetGuidByName("Unity-iPhone");

			var files = System.IO.Directory.GetFiles (Application.dataPath, "*.sf2", System.IO.SearchOption.AllDirectories);
        			
            sf2PathSource = "Assets/Resources/soundfont.sf2".GetFullPathFromAssetPath();
            string fileName = "softsynth.sf2";
			
            if(File.Exists(sf2PathSource)){
				File.Copy(sf2PathSource, Path.Combine(buildPath, fileName), true);
				string fileGUID = project.AddFile (fileName, fileName);
				project.AddFileToBuild(targetGUID, fileGUID);
			}
																				
			project.WriteToFile(projPath);
		}
#endif

        #region LINUX

        if (target == BuildTarget.StandaloneLinux || target == BuildTarget.StandaloneLinux64 || target == BuildTarget.StandaloneLinuxUniversal)
        {

            string src_path_x86 = Path.Combine(pluginsFolder, "Linux/x86");
            string dst_path_x86 = Path.Combine(Path.GetDirectoryName(buildPath), "x86");
            if (!Directory.Exists(dst_path_x86))
            {
                Directory.CreateDirectory(dst_path_x86);
            }

            if (File.Exists(Path.Combine(src_path_x86, "libbass.so")))
            {
                File.Copy(Path.Combine(src_path_x86, "libbass.so"), Path.Combine(dst_path_x86, "libbass.so"), true);
            }

            if (File.Exists(Path.Combine(src_path_x86, "libbassmidi.so")))
            {
                File.Copy(Path.Combine(src_path_x86, "libbassmidi.so"), Path.Combine(dst_path_x86, "libbassmidi.so"), true);
            }

            string src_path_x86_64 = Path.Combine(pluginsFolder, "Linux/x86_64");
            string dst_path_x86_64 = Path.Combine(Path.GetDirectoryName(buildPath), "x86_64");
            if (!Directory.Exists(dst_path_x86_64))
            {
                Directory.CreateDirectory(dst_path_x86_64);
            }

            if (File.Exists(Path.Combine(src_path_x86_64, "libbass.so")))
            {
                File.Copy(Path.Combine(src_path_x86_64, "libbass.so"), Path.Combine(dst_path_x86_64, "libbass.so"), true);
            }

            if (File.Exists(Path.Combine(src_path_x86_64, "libbassmidi.so")))
            {
                File.Copy(Path.Combine(src_path_x86_64, "libbassmidi.so"), Path.Combine(dst_path_x86_64, "libbassmidi.so"), true);
            }

            sf2PathSource = MIDIEditorSettings.instance.GetPlatformSoundFontFullPath();
            sf2PathDestination = Path.GetDirectoryName(buildPath) + "/softsynth.sf2";
            if (File.Exists(sf2PathSource))
            {
                File.Copy(sf2PathSource, sf2PathDestination, true);
            }
            else
            {
                Debug.LogError("Can not find file : " + sf2PathSource);
            }
        }

        #endregion

        #region OSX

        if (target == BuildTarget.StandaloneOSXIntel || target == BuildTarget.StandaloneOSXIntel64 || target == BuildTarget.StandaloneOSX)
        {
            sf2PathSource = MIDIEditorSettings.instance.GetPlatformSoundFontFullPath();
            sf2PathDestination = buildPath + "/Contents/Plugins/softsynth.bundle/Contents/Resources/softsynth.sf2";

            if (File.Exists(sf2PathSource))
            {
                File.Copy(sf2PathSource, sf2PathDestination, true);
            }
        }

        #endregion

        #region WIN

        if (target == BuildTarget.StandaloneWindows || target == BuildTarget.StandaloneWindows64)
        {

            string targetPlatform = "";

            if (target == BuildTarget.StandaloneWindows)
                targetPlatform = "x86";
            if (target == BuildTarget.StandaloneWindows64)
                targetPlatform = "x86_64";

            string bassDllPathSource = Path.Combine(pluginsFolder, "WIN/" + targetPlatform + "/bass.dll");
            string bassDllPathDestination = Path.GetDirectoryName(buildPath) + "/bass.dll";
            File.Copy(bassDllPathSource, bassDllPathDestination, true);

            string bassMidiDllPathSource = Path.Combine(pluginsFolder, "WIN/" + targetPlatform + "/bassmidi.dll");
            string bassMidiDllPathDestination = Path.GetDirectoryName(buildPath) + "/bassmidi.dll";
            File.Copy(bassMidiDllPathSource, bassMidiDllPathDestination, true);


            Debug.Log(buildPath);

            sf2PathSource = MIDIEditorSettings.instance.GetPlatformSoundFontFullPath();
            sf2PathDestination = Path.GetDirectoryName(buildPath) + "/" + Path.GetFileNameWithoutExtension(buildPath) + "_Data/Plugins/softsynth.sf2";
            if (File.Exists(sf2PathSource))
            {
                File.Copy(sf2PathSource, sf2PathDestination, true);
            }
        }

        #endregion

        #region ANDROID

        if (target == BuildTarget.Android)
        {

            //			string dest_armeabi_directory = buildPath + "/libs/armeabi";
            //			string dest_armeabi_v7a_directory = buildPath + "/libs/armeabi-v7a";
            //			string dest_sf2_directory = buildPath + "/res/raw";
            //			
            //			string src_armeabi_directory = Application.dataPath + "/Editor/AndroidBASS/armeabi";
            //			string src_armeabi_v7a_directory = Application.dataPath + "/Editor/AndroidBASS/armeabi-v7a";
            //			string src_sf2_directory = Application.dataPath + "/Editor/AndroidBASS/sf2";
            //			
            //			string bassFileName = "libbass.so";
            //			string bassMidiFileName = "libbassmidi.so";
            //			string sf2FileName = "softsynth.sf2";
            //					
            //			if(!Directory.Exists(dest_armeabi_directory)) Directory.CreateDirectory(dest_armeabi_directory);
            //			
            //			File.Copy(src_armeabi_directory + "/" + bassFileName, dest_armeabi_directory + "/" + bassFileName, true);
            //			File.Copy(src_armeabi_directory + "/" + bassMidiFileName, dest_armeabi_directory + "/" + bassMidiFileName, true);
            //			
            //			if(!Directory.Exists(dest_armeabi_v7a_directory)) Directory.CreateDirectory(dest_armeabi_v7a_directory);
            //			
            //			File.Copy(src_armeabi_v7a_directory + "/" + bassFileName, dest_armeabi_v7a_directory + "/" + bassFileName, true);
            //			File.Copy(src_armeabi_v7a_directory + "/" + bassMidiFileName, dest_armeabi_v7a_directory + "/" + bassMidiFileName, true);
            //			
            //			if(!Directory.Exists(dest_sf2_directory)) Directory.CreateDirectory(dest_sf2_directory);
            //			
            //			File.Copy(src_sf2_directory + "/" + sf2FileName, dest_sf2_directory + "/" + sf2FileName, true);

        }

        #endregion
    }

}