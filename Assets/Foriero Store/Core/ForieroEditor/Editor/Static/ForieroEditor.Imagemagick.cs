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
    public static class Imagemagick
    {
        public static string cmdConvert
        {
            get
            {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
                return "/opt/local/bin/convert";
#elif UNITY_EDITOR_WIN
				return "convert";
#endif
            }
        }

        public static string cmdComposite
        {
            get
            {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
                return "/opt/local/bin/composite";
#elif UNITY_EDITOR_WIN
				return "composite";
#endif
            }
        }

        static string _cmdPngQuant = "";

        public static string cmdPngQuant
        {
            get
            {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
                if (string.IsNullOrEmpty(_cmdPngQuant))
                {
                    string[] files = Directory.GetFiles(Application.dataPath, "pngquant", SearchOption.AllDirectories);
                    if (files.Length > 0)
                    {
                        _cmdPngQuant = files[0];
                    }
                }
                return _cmdPngQuant;
#elif UNITY_EDITOR_WIN
				if (string.IsNullOrEmpty (_cmdPngQuant)) {
					string[] files = Directory.GetFiles (Application.dataPath, "pngquant.exe", SearchOption.AllDirectories);
					if (files.Length > 0) {
						_cmdPngQuant = files [0];
					}
				}
				return _cmdPngQuant;
#endif
            }
        }

        #region Softedges

        public static void JpgSelected(string suffix, int quality = 70)
        {
            foreach (Object o in Selection.objects)
            {
                if (o is Texture2D)
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), AssetDatabase.GetAssetPath(o.GetInstanceID()));
                    Jpg(path, suffix, quality);
                }
            }
            AssetDatabase.Refresh();
        }

        public static void Jpg(string inputPngFilePath, string suffix, int quality = 70)
        {
            string path = Path.GetDirectoryName(inputPngFilePath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(inputPngFilePath);
            //string extension = Path.GetExtension (inputPngFilePath);
            string outputPngFilePath = Path.Combine(path, fileNameWithoutExtension + suffix + ".jpg");

            string args = "-quality " + quality.ToString() + " " + inputPngFilePath.FixOSPath().DoubleQuotes() + " " + outputPngFilePath.FixOSPath().DoubleQuotes();
            Debug.Log(cmdConvert + " " + args);
            GenerateProcess(cmdConvert, args);
        }

        #endregion

        #region PngQuant

        public static void PngQuantSelected(string suffix)
        {
            foreach (Object o in Selection.objects)
            {
                if (o is Texture2D)
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), AssetDatabase.GetAssetPath(o.GetInstanceID()));
                    PngQuant(path, suffix);
                }
            }
            AssetDatabase.Refresh();
        }

        public static void PngQuant(string inputPngFilePath, string suffix)
        {
            string path = Path.GetDirectoryName(inputPngFilePath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(inputPngFilePath);
            string extension = Path.GetExtension(inputPngFilePath);
            string outputPngFilePath = Path.Combine(path, fileNameWithoutExtension + suffix + extension);


            string args = "";
            if (outputPngFilePath.FixOSPath().DoubleQuotes() == inputPngFilePath.FixOSPath().DoubleQuotes())
            {
                args = "--ext=.png --force " + inputPngFilePath.FixOSPath().DoubleQuotes();
            }
            else
            {
                args = "--output " + outputPngFilePath.FixOSPath().DoubleQuotes() + " " + inputPngFilePath.FixOSPath().DoubleQuotes();
            }
            Debug.Log(cmdPngQuant + " " + args);
            GenerateProcess(cmdPngQuant, args);
        }

        #endregion

        #region Softedges

        public static void SoftEdgesSelected(string suffix)
        {
            foreach (Object o in Selection.objects)
            {
                if (o is Texture2D)
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), AssetDatabase.GetAssetPath(o.GetInstanceID()));
                    SoftEdges(path, suffix);
                }
            }
            AssetDatabase.Refresh();
        }

        public static void SoftEdges(string inputPngFilePath, string suffix)
        {
            string path = Path.GetDirectoryName(inputPngFilePath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(inputPngFilePath);
            string extension = Path.GetExtension(inputPngFilePath);
            string outputPngFilePath = Path.Combine(path, fileNameWithoutExtension + suffix + extension);

            string args = inputPngFilePath.FixOSPath().DoubleQuotes() + " -alpha set -virtual-pixel transparent -channel A -blur 0x8  -level 50%,100% +channel " + outputPngFilePath.FixOSPath().DoubleQuotes();
            Debug.Log(cmdConvert + " " + args);
            GenerateProcess(cmdConvert, args);
        }

        #endregion

        #region Dropshadow

        public static void DropShadowSelected(string suffix)
        {
            foreach (Object o in Selection.objects)
            {
                if (o is Texture2D)
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), AssetDatabase.GetAssetPath(o.GetInstanceID()));
                    DropShadow(path, suffix);
                }
            }
            AssetDatabase.Refresh();
        }

        public static void DropShadow(string inputPngFilePath, string suffix)
        {
            string path = Path.GetDirectoryName(inputPngFilePath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(inputPngFilePath);
            string extension = Path.GetExtension(inputPngFilePath);
            string outputPngFilePath = Path.Combine(path, fileNameWithoutExtension + suffix + extension);


            string args = inputPngFilePath.FixOSPath().DoubleQuotes() + " \\( +clone -background black -shadow 80x3+2+2 \\) -compose DstOver -composite " + outputPngFilePath.FixOSPath().DoubleQuotes();
            Debug.Log(cmdConvert + " " + args);
            GenerateProcess(cmdConvert, args);
        }

        #endregion

        #region Dropshadow

        public static void DropShadowOutlineSelected(string suffix)
        {
            foreach (Object o in Selection.objects)
            {
                if (o is Texture2D)
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), AssetDatabase.GetAssetPath(o.GetInstanceID()));
                    DropShadowOutline(path, suffix);
                }
            }
            AssetDatabase.Refresh();
        }

        public static void DropShadowOutline(string inputPngFilePath, string suffix)
        {
            string path = Path.GetDirectoryName(inputPngFilePath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(inputPngFilePath);
            string extension = Path.GetExtension(inputPngFilePath);
            string outputPngFilePath = Path.Combine(path, fileNameWithoutExtension + suffix + extension);

            string args = inputPngFilePath.FixOSPath().DoubleQuotes() + @" -bordercolor none -border 20 -background black -alpha background -channel A -blur 0x10 -level 0,0% " + outputPngFilePath.FixOSPath().DoubleQuotes();
            Debug.Log(cmdConvert + " " + args);
            GenerateProcess(cmdConvert, args);
        }

        #endregion

        #region Trim

        public static void TrimSelected(string suffix)
        {
            foreach (Object o in Selection.objects)
            {
                if (o is Texture2D)
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), AssetDatabase.GetAssetPath(o.GetInstanceID()));
                    Trim(path, suffix);
                }
            }
            AssetDatabase.Refresh();
        }

        public static void Trim(string inputPngFilePath, string suffix)
        {
            string path = Path.GetDirectoryName(inputPngFilePath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(inputPngFilePath);
            string extension = Path.GetExtension(inputPngFilePath);
            string outputPngFilePath = Path.Combine(path, fileNameWithoutExtension + suffix + extension);

            string args = inputPngFilePath.FixOSPath().DoubleQuotes() + " -trim " + outputPngFilePath.FixOSPath().DoubleQuotes();
            Debug.Log(cmdConvert + " " + args);
            GenerateProcess(cmdConvert, args);
        }

        #endregion

        #region Gray

        public static void GraySelected(string suffix)
        {
            foreach (Object o in Selection.objects)
            {
                if (o is Texture2D)
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), AssetDatabase.GetAssetPath(o.GetInstanceID()));
                    Gray(path, suffix);
                }
            }
            AssetDatabase.Refresh();
        }

        public static void Gray(string inputPngFilePath, string suffix)
        {
            string path = Path.GetDirectoryName(inputPngFilePath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(inputPngFilePath);
            string extension = Path.GetExtension(inputPngFilePath);
            string outputPngFilePath = Path.Combine(path, fileNameWithoutExtension + suffix + extension);

            string args = inputPngFilePath.FixOSPath().DoubleQuotes() + " -colorspace gray " + outputPngFilePath.FixOSPath().DoubleQuotes();
            Debug.Log(cmdConvert + " " + args);
            GenerateProcess(cmdConvert, args);
        }

        #endregion

        #region Watermark

        public static void WatermarkSelected(string watermarkFile, string suffix)
        {
            foreach (Object o in Selection.objects)
            {
                if (o is Texture2D)
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), AssetDatabase.GetAssetPath(o.GetInstanceID()));
                    Watermark(watermarkFile, path, suffix);
                }
            }
            AssetDatabase.Refresh();
        }

        public static void Watermark(string watemarkFilePath, string inputPngFilePath, string suffix)
        {
            string path = Path.GetDirectoryName(inputPngFilePath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(inputPngFilePath);
            string extension = Path.GetExtension(inputPngFilePath);
            string outputPngFilePath = Path.Combine(path, fileNameWithoutExtension + suffix + extension);

            string args = "-dissolve 15 -tile " + watemarkFilePath.FixOSPath().DoubleQuotes() + " " + inputPngFilePath.FixOSPath().DoubleQuotes() + " " + outputPngFilePath.FixOSPath().DoubleQuotes();
            Debug.Log(cmdComposite + " " + args);
            GenerateProcess(cmdComposite, args);
        }

        #endregion

        //convert yellow_circle.png  -channel RGBA  -blur 0x8  yellow_blurred_RGBA.png

        #region Dropshadow

        public static void BlurSelected(string suffix)
        {
            foreach (Object o in Selection.objects)
            {
                if (o is Texture2D)
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), AssetDatabase.GetAssetPath(o.GetInstanceID()));
                    Blur(path, suffix);
                }
            }
            AssetDatabase.Refresh();
        }

        public static void Blur(string inputPngFilePath, string suffix)
        {
            string path = Path.GetDirectoryName(inputPngFilePath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(inputPngFilePath);
            string extension = Path.GetExtension(inputPngFilePath);
            string outputPngFilePath = Path.Combine(path, fileNameWithoutExtension + suffix + extension);


            string args = inputPngFilePath.FixOSPath().DoubleQuotes() + " -channel RGBA -blur 0x8 " + outputPngFilePath.FixOSPath().DoubleQuotes();
            Debug.Log(cmdConvert + " " + args);
            GenerateProcess(cmdConvert, args);
        }

        #endregion

        public static void GenerateProcess(string aCommand, string anArguments)
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
    }
}
