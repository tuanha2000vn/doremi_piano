using UnityEngine;
using System.Collections;

namespace ForieroEngine.Extensions
{
	public static partial class ForieroEngineExtensions
	{
		public static bool isLinux ()
		{
#if UNITY_WSA && !UNITY_EDITOR
            return false;
#else
			int p = (int)System.Environment.OSVersion.Platform;
			return (p == 4) || (p == 6) || (p == 128);
#endif
		}

		public static string FixOSPath (this string s)
		{
			if (isLinux ()) {
				return s.Replace (@"\", "/");
			} else {
				s = s.Replace ("/", @"\");
				return s.Replace (@"\\", @"\");
			}
		}

		public static string FixAssetsPath (this string s)
		{
			s = s.Replace (@"\", "/");
			return s.Replace ("//", "/");
		}
	}
}
