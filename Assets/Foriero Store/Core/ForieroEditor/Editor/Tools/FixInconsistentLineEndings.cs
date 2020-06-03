using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;


namespace ForieroEditor.Tools
{
    /// <summary>
    /// Implements menu items for the Unity Editor to perform
    /// end-of-line conversion and fix issues such as for the
    /// following: "There are inconsistent line endings in the
    /// 'Assets/.../*.cs' script. Some are Mac OS X (UNIX) and
    /// some are Windows. This might lead to incorrect line
    /// numbers in stacktraces and compiler errors."
    /// </summary>
    public static class LineEndingsEditMenu
    {
        [MenuItem("Foriero/Tools/Line Endings/Windows Format (CR,LF)")]
        public static void ConvertLineEndingsToWindowsFormat()
        {
            ConvertLineEndings(LineEndingsEnum.Windows, true);
        }

        [MenuItem("Foriero/Tools/Line Endings/UNIX Format (LF)")]
        public static void ConvertLineEndingsToUnixFormat()
        {
            ConvertLineEndings(LineEndingsEnum.Linux, true);
        }

        enum LineEndingsEnum
        {
            Windows,
            Linux
        }

        static string ToLineEnd(this LineEndingsEnum lineEnd)
        {
            string result = "";

            switch (lineEnd)
            {
                case LineEndingsEnum.Linux:
                    result = "\n";
                    break;
                case LineEndingsEnum.Windows:
                    result = "\r\n";
                    break;
            }

            return result;
        }

        private static void ConvertLineEndings(LineEndingsEnum lineEnd, bool onlyWhenInconsistent)
        {
            string title = string.Format("EOL Conversion to {0} Format", lineEnd.ToString());

            if (!EditorUtility.DisplayDialog(title,
                "This operation may potentially modify " +
                "many files in the current project! " +
                "Hopefully you have backups of everything. " +
                "Are you sure you want to proceed?",
                "Yes",
                "No"))
            {
                Debug.Log("EOL Conversion was not attempted.");
                return;
            }

            var fileTypes = new string[]
            {
                "*.txt",
                "*.cs",
                "*.js",
                "*.boo",
                "*.compute",
                "*.shader",
                "*.cginc",
                "*.glsl",
                "*.xml",
                "*.xaml",
                "*.json",
                "*.inc",
                "*.css",
                "*.htm",
                "*.html",
            };

            string projectAssetsPath = Application.dataPath;

            int totalFileCount = 0;
            var changedFiles = new List<string>();

            var regex = new Regex(@"(?<!\r)\n");

            var comparisonType = System.StringComparison.Ordinal;

            foreach (string fileType in fileTypes)
            {
                string[] filenames = Directory.GetFiles(projectAssetsPath, fileType, SearchOption.AllDirectories);
                totalFileCount += filenames.Length;

                foreach (string filename in filenames)
                {
                    string originalText = File.ReadAllText(filename);

                    int linuxMathces = Regex.Matches(originalText, "\n").Count;
                    int windowsMatches = Regex.Matches(originalText, "\r\n").Count;

                    if (linuxMathces > 0 && windowsMatches > 0)
                    {
                        if (linuxMathces != windowsMatches)
                        {
                            string changedText = "";

                            changedText = regex.Replace(originalText, "\r\n");

                            if (lineEnd == LineEndingsEnum.Linux)
                            {
                                changedText = changedText.Replace("\r\n", "\n");
                            }

                            bool isTextIdentical = string.Equals(changedText, originalText, comparisonType);

                            if (!isTextIdentical)
                            {
                                changedFiles.Add(filename);
                                File.WriteAllText(filename, changedText, System.Text.Encoding.UTF8);
                            }
                        }
                    }
                }
            }

            int changedFileCount = changedFiles.Count;
            int skippedFileCount = (totalFileCount - changedFileCount);

            string message = string.Format(
                "EOL Conversion skipped {0} " +
                "files and changed {1} files",
                skippedFileCount,
                changedFileCount);

            if (changedFileCount <= 0)
            {
                message += ".";
            }
            else
            {
                message += (":" + lineEnd.ToLineEnd());
                message += string.Join(lineEnd.ToLineEnd(), changedFiles.ToArray());
            }

            Debug.Log(message);

            if (changedFileCount > 0)
            {
                AssetDatabase.Refresh();
            }
        }
    }
}
