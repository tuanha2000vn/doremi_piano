using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;

namespace ForieroEditor.Extensions
{
    public static partial class ForieroEditorExtensions
    {
        public static string FixOSPath(this string s)
        {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
            return s.Replace(@"\", "/");
#elif UNITY_EDITOR_WIN
			s = s.Replace("/", @"\");
			return s.Replace(@"\\", @"\"); 
#else
			return s;
#endif
        }

        public static string FixAssetsPath(this string s)
        {
            s = s.Replace(@"\", "/");
            return s.Replace("//", "/");
        }

        public static string GetAssetPathFromFullPath(this string s)
        {
            return "Assets" + s.FixAssetsPath().Replace(Application.dataPath.FixAssetsPath(), "");
        }

        public static string GetFullPathFromAssetPath(this string s)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), s).FixOSPath();
        }

        public static string RemoveProjectPath(this string s)
        {
            return s.FixOSPath().Replace(Directory.GetCurrentDirectory().FixOSPath(), "").RemoveLeadingSlash().RemoveLeadingBackwardSlash();
        }

        public static string RemovePath(this string s, string path)
        {
            return s.FixOSPath().Replace(path.FixOSPath(), "").RemoveLeadingSlash().RemoveLeadingBackwardSlash();
        }

        public static bool IsInProjectFolder(this string s)
        {
            return s.FixOSPath().Contains(Directory.GetCurrentDirectory().FixOSPath());
        }

        public static string RemoveLeadingSlash(this string s)
        {
            if (s.StartsWith("/"))
            {
                s = s.Substring(1);
            }
            return s;
        }

        public static string RemoveLeadingBackwardSlash(this string s)
        {
            if (s.StartsWith("\\"))
            {
                s = s.Substring(1);
            }
            return s;
        }

        public static string DoubleQuotes(this string s)
        {
            return "\"" + s + "\"";
        }

        public static string SingleQuotes(this string s)
        {
            return "'" + s + "'";
        }
    }
}
